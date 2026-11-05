using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using FatturazioneContratti.Common;
using FatturazioneContratti.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace FatturazioneContratti.Invoicing
{
    public static class AggiornamentoFattureService
    {
        /// <summary>
        /// Allinea date/nr canoni delle righe fattura da quelle della riga ordine.
        /// Se <see cref="FiltriAllineamentoFatture.SincronizzaFatturaElettronica"/> è true,
        /// esegue anche l'upsert dei dati fattura elettronica (AllordCliFattEle →
        /// MaEiItdocAdditionalData).
        /// Il messaggio di esito viene loggato qui dentro (filtri ed esito come informativo,
        /// eccezione con stack come errore): il caller passa solo il delegato.
        /// </summary>
        public static bool AdeguaFattureDaOrdini(
            SqlConnection connection,
            SqlTransaction transaction,
            FiltriAllineamentoFatture filtri,
            int loginId,
            Action<string, LogTarget>? logMessage = null,
            CancellationToken token = default)
        {
            // I filtri della run vengono registrati su UI/file come negli altri workflow.
            LogDiagnostica.LogFiltri(logMessage, filtri);

            // Timestamp unico dell'operazione (convenzione degli altri workflow):
            // tutte le righe aggiornate portano lo stesso valore.
            DateTime now = DateTime.Now;

            string sFromDate = filtri.DataFattDa.ToString("yyyyMMdd");
            string sToDate = filtri.DataFattA.ToString("yyyyMMdd");

            // --- OPERAZIONE A: date/nr canoni ---
            var sb = new StringBuilder();
            sb.Append("UPDATE MA_SaleDocDetail SET ");
            sb.Append("ALL_CanoniDataI = Ord.ALL_CanoniDataI, ");
            sb.Append("ALL_CanoniDataF = Ord.ALL_CanoniDataF, ");
            sb.Append("ALL_NrCanoni = Ord.ALL_NrCanoni, ");
            sb.Append("TBModified = @DataMod, ");
            sb.Append("TBModifiedID = @User ");
            sb.Append("FROM MA_SaleDoc Testa ");
            sb.Append("INNER JOIN MA_SaleDocDetail Doc ON Testa.SaleDocId = Doc.SaleDocId ");
            sb.Append("INNER JOIN MA_SaleOrdDetails Ord ON Ord.SaleOrdId = Doc.SaleOrdId AND Ord.SubId = Doc.SaleOrdSubId AND Ord.Position = Doc.SaleOrdPos ");
            // Tipi riga Merce/Servizio e tipi documento, presi dagli enum Mago
            // (Common/MagoEnum.cs) invece che dai valori numerici hardcoded.
            sb.Append($"WHERE (Doc.LineType = {(int)LineType.Merce} OR Doc.LineType = {(int)LineType.Servizio}) ");
            sb.Append($"AND (Testa.DocumentType = {(int)DocumentType.Fattura} OR Testa.DocumentType = {(int)DocumentType.FatturaAccompagnatoria} OR Testa.DocumentType = {(int)DocumentType.NotaCredito}) ");
            sb.Append("AND (Testa.DocumentDate >= @FromDate AND Testa.DocumentDate <= @ToDate) ");
            sb.Append("AND (@AllNumbers = 1 OR (@AllNumbers = 0 AND Testa.DocNo >= @NrFirst AND Testa.DocNo <= @NrLast)) ");
            sb.Append("AND Doc.ALL_CanoniDataI = '17991231'");

            try
            {
                using var cmd = new SqlCommand(sb.ToString(), connection, transaction);
                // Timeout alto anche per il comando raw: il default ADO è 30s e la UPDATE
                // con JOIN a tre tabelle su VPN poteva morire ben prima del timeout del context EF.
                cmd.CommandTimeout = SqlTools.CommandTimeoutSeconds;
                cmd.Parameters.Add("@FromDate", SqlDbType.VarChar, 8).Value = sFromDate;
                cmd.Parameters.Add("@ToDate", SqlDbType.VarChar, 8).Value = sToDate;
                cmd.Parameters.Add("@AllNumbers", SqlDbType.Bit).Value = filtri.AllNumbers;
                cmd.Parameters.Add("@NrFirst", SqlDbType.VarChar, 10).Value = filtri.NrFirst ?? string.Empty;
                cmd.Parameters.Add("@NrLast", SqlDbType.VarChar, 10).Value = filtri.NrLast ?? string.Empty;
                cmd.Parameters.Add("@DataMod", SqlDbType.VarChar, 8).Value = now.ToString("yyyyMMdd");
                cmd.Parameters.Add("@User", SqlDbType.SmallInt, 32).Value = loginId;

                // Interruzione richiesta prima della UPDATE: si lancia OperationCanceledException
                // e la transazione del chiamante viene rollbackata dal dispose.
                token.ThrowIfCancellationRequested();

                int irows = cmd.ExecuteNonQuery();
                LogDiagnostica.Informativo(logMessage, irows <= 0
                    ? "Nessun documento di vendita da aggiornare"
                    : $"Adeguate {irows} righe canoni.");

                // --- OPERAZIONE B: upsert dati fattura elettronica ---
                if (filtri.SincronizzaFatturaElettronica)
                {
                    token.ThrowIfCancellationRequested();
                    int feRows = UpsertDatiFatturaElettronica(connection, transaction, filtri, now, loginId, logMessage, token);
                    LogDiagnostica.Informativo(logMessage, feRows <= 0
                        ? "Nessun dato fattura elettronica da sincronizzare"
                        : $"Sincronizzati {feRows} dati fattura elettronica.");
                }

                return true;
            }
            catch (OperationCanceledException)
            {
                // Interruzione utente: risale al chiamante, che rollbacka la transazione.
                throw;
            }
            catch (Exception ex)
            {
                // Errore: messaggio su UI e file, stack sul file.
                LogDiagnostica.Errore(logMessage, ex);
                return false;
            }
        }

        private static int UpsertDatiFatturaElettronica(
            SqlConnection connection,
            SqlTransaction transaction,
            FiltriAllineamentoFatture filtri,
            DateTime now,
            int loginId,
            Action<string, LogTarget>? logMessage,
            CancellationToken token)
        {
            // Chiavi delle righe fattura target: stessi filtri dell'operazione A, senza il
            // vincolo ALL_CanoniDataI (si allineano anche righe già processate per i dati FE).
            string sFromDate = filtri.DataFattDa.ToString("yyyyMMdd");
            string sToDate = filtri.DataFattA.ToString("yyyyMMdd");

            var righe = new List<(int SaleDocId, int SaleOrdId, int SaleOrdSubId, int SaleOrdPos)>();
            using (var cmdChiavi = new SqlCommand(
                @"SELECT DISTINCT Doc.SaleDocId, Doc.SaleOrdId, Doc.SaleOrdSubId, Doc.SaleOrdPos
                  FROM MA_SaleDoc Testa
                  INNER JOIN MA_SaleDocDetail Doc ON Testa.SaleDocId = Doc.SaleDocId
                  WHERE (Doc.LineType = @LineTypeMerce OR Doc.LineType = @LineTypeServizio)
                    AND (Testa.DocumentType = @DocFattura OR Testa.DocumentType = @DocFatturaAcc OR Testa.DocumentType = @DocNotaCredito)
                    AND (Testa.DocumentDate >= @FromDate AND Testa.DocumentDate <= @ToDate)
                    AND (@AllNumbers = 1 OR (@AllNumbers = 0 AND Testa.DocNo >= @NrFirst AND Testa.DocNo <= @NrLast))
                    AND Doc.SaleOrdId IS NOT NULL",
                connection, transaction))
            {
                cmdChiavi.CommandTimeout = SqlTools.CommandTimeoutSeconds;
                cmdChiavi.Parameters.Add("@LineTypeMerce", SqlDbType.Int).Value = (int)LineType.Merce;
                cmdChiavi.Parameters.Add("@LineTypeServizio", SqlDbType.Int).Value = (int)LineType.Servizio;
                cmdChiavi.Parameters.Add("@DocFattura", SqlDbType.Int).Value = (int)DocumentType.Fattura;
                cmdChiavi.Parameters.Add("@DocFatturaAcc", SqlDbType.Int).Value = (int)DocumentType.FatturaAccompagnatoria;
                cmdChiavi.Parameters.Add("@DocNotaCredito", SqlDbType.Int).Value = (int)DocumentType.NotaCredito;
                cmdChiavi.Parameters.Add("@FromDate", SqlDbType.VarChar, 8).Value = sFromDate;
                cmdChiavi.Parameters.Add("@ToDate", SqlDbType.VarChar, 8).Value = sToDate;
                cmdChiavi.Parameters.Add("@AllNumbers", SqlDbType.Bit).Value = filtri.AllNumbers;
                cmdChiavi.Parameters.Add("@NrFirst", SqlDbType.VarChar, 10).Value = filtri.NrFirst ?? string.Empty;
                cmdChiavi.Parameters.Add("@NrLast", SqlDbType.VarChar, 10).Value = filtri.NrLast ?? string.Empty;

                using var reader = cmdChiavi.ExecuteReader();
                while (reader.Read())
                {
                    righe.Add((Convert.ToInt32(reader.GetValue(0)), Convert.ToInt32(reader.GetValue(1)),
                        reader.IsDBNull(2) ? 0 : Convert.ToInt32(reader.GetValue(2)),
                        reader.IsDBNull(3) ? 0 : Convert.ToInt32(reader.GetValue(3))));
                }
            }

            if (righe.Count == 0) return 0;

            // Recupero AllordCliFattEle per gli ordini delle righe fattura (1 per ordine).
            var saleOrdIds = righe.Select(r => r.SaleOrdId).Distinct().ToList();
            var fattElePerOrdine = new Dictionary<int, EFMago.Models.AllordCliFattEle>();
            var sbFe = new StringBuilder("SELECT * FROM ALLOrdCliFattEle WHERE IdOrdCli IN (");
            for (int i = 0; i < saleOrdIds.Count; i++)
            {
                if (i > 0) sbFe.Append(", ");
                sbFe.Append($"@fe{i}");
            }
            sbFe.Append(")");
            using (var cmdFe = new SqlCommand(sbFe.ToString(), connection, transaction))
            {
                cmdFe.CommandTimeout = SqlTools.CommandTimeoutSeconds;
                for (int i = 0; i < saleOrdIds.Count; i++)
                    cmdFe.Parameters.Add($"@fe{i}", SqlDbType.Int).Value = saleOrdIds[i];
                using var reader = cmdFe.ExecuteReader();
                while (reader.Read())
                {
                    var fe = new EFMago.Models.AllordCliFattEle
                    {
                        IdOrdCli = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("IdOrdCli"))),
                        F2_1_1_5_4 = GetStringOrNull(reader, "F2_1_1_5_4"),
                        F2_1_1_11 = GetStringOrNull(reader, "F2_1_1_11"),
                        F2_1_2_6 = GetStringOrNull(reader, "F2_1_2_6"),
                        F2_1_2_7 = GetStringOrNull(reader, "F2_1_2_7"),
                        F2_1_3_6 = GetStringOrNull(reader, "F2_1_3_6"),
                        F2_1_3_7 = GetStringOrNull(reader, "F2_1_3_7"),
                        F2_1_4_6 = GetStringOrNull(reader, "F2_1_4_6"),
                        F2_1_4_7 = GetStringOrNull(reader, "F2_1_4_7"),
                        F2_1_5_6 = GetStringOrNull(reader, "F2_1_5_6"),
                        F2_1_5_7 = GetStringOrNull(reader, "F2_1_5_7"),
                    };
                    fattElePerOrdine[fe.IdOrdCli] = fe;
                }
            }

            // Costruisco le righe da upsert: per ogni riga fattura e per ogni campo FE valorizzato.
            // DocSubId = 0, SubLine = 0, Line = 1 (o MAX(Line)+1 per quel DocId se esistono già righe).
            var docIds = righe.Select(r => r.SaleDocId).Distinct().ToList();
            var lineEsistenti = new Dictionary<int, int>(); // DocId -> MAX(Line)
            using (var cmdLine = new SqlCommand(
                $"SELECT DocID, ISNULL(MAX(Line), 0) FROM MA_EI_ITDocAdditionalData WHERE DocID IN ({string.Join(",", docIds.Select((_, i) => $"@d{i}"))}) GROUP BY DocID",
                connection, transaction))
            {
                cmdLine.CommandTimeout = SqlTools.CommandTimeoutSeconds;
                for (int i = 0; i < docIds.Count; i++)
                    cmdLine.Parameters.Add($"@d{i}", SqlDbType.Int).Value = docIds[i];
                using var reader = cmdLine.ExecuteReader();
                while (reader.Read())
                    lineEsistenti[Convert.ToInt32(reader.GetValue(0))] = Convert.ToInt32(reader.GetValue(1));
            }

            var prossimaLine = new Dictionary<int, int>(lineEsistenti);
            var righeDaScrivere = new List<(int DocId, int Line, string FieldName, string FieldValue)>();

            // Mappa completa dei campi FE: tutti i campi di ALLOrdCliFattEle (tranne IdOrdCli
            // e i campi di audit) vengono sincronizzati, con il FieldName XML corrispondente.
            var sezioniFe = new Dictionary<string, string>
            {
                { "2", "DatiOrdineAcquisto" },
                { "3", "DatiContratto" },
                { "4", "DatiConvenzione" },
                { "5", "DatiRicezione" },
            };
            var suffissiFe = new Dictionary<string, string>
            {
                { "1", "RiferimentoNumeroLinea" },
                { "2", "IdDocumento" },
                { "3", "Data" },
                { "4", "NumItem" },
                { "5", "CodiceCommessaConvenzione" },
                { "6", "CodiceCUP" },
                { "7", "CodiceCIG" },
            };
            var campiFe = new List<(string PropName, string FieldName)>();
            campiFe.Add(("F2_1_1_5_4", "FatturaElettronica.FatturaElettronicaBody.DatiGenerali.DatiGeneraliDocumento.DatiRitenuta.CausalePagamento"));
            campiFe.Add(("F2_1_1_11", "FatturaElettronica.FatturaElettronicaBody.DatiGenerali.DatiGeneraliDocumento.Causale"));
            foreach (var sec in sezioniFe)
                foreach (var suf in suffissiFe)
                    campiFe.Add(($"F2_1_{sec.Key}_{suf.Key}", $"FatturaElettronica.FatturaElettronicaBody.DatiGenerali.{sec.Value}.{suf.Value}"));

            var visti = new HashSet<(int DocId, string FieldName, string FieldValue)>();
            foreach (var riga in righe)
            {
                if (!fattElePerOrdine.TryGetValue(riga.SaleOrdId, out var fe)) continue;

                int line = prossimaLine.TryGetValue(riga.SaleDocId, out int l) ? l : 0;
                if (line == 0) line = 1;
                prossimaLine[riga.SaleDocId] = line + 1;

                foreach (var campo in campiFe)
                {
                    var prop = fe.GetType().GetProperty(campo.PropName);
                    var valore = ConvertFeValue(campo.PropName, prop?.GetValue(fe));
                    if (string.IsNullOrEmpty(valore)) continue;
                    // Evita righe duplicate con lo stesso valore per la stessa fattura.
                    if (!visti.Add((riga.SaleDocId, campo.FieldName, valore))) continue;
                    righeDaScrivere.Add((riga.SaleDocId, line, campo.FieldName, valore));
                }
            }

            if (righeDaScrivere.Count == 0) return 0;

            // Upsert: per ogni (DocId, 0, Line, 0, FieldName) esiste già → UPDATE, altrimenti INSERT.
            int scritti = 0;
            foreach (var r in righeDaScrivere)
            {
                token.ThrowIfCancellationRequested();

                using var cmdCheck = new SqlCommand(
                    "SELECT COUNT(*) FROM MA_EI_ITDocAdditionalData WHERE DocID = @DocId AND DocSubID = 0 AND Line = @Line AND SubLine = 0 AND FieldName = @FieldName",
                    connection, transaction);
                cmdCheck.CommandTimeout = SqlTools.CommandTimeoutSeconds;
                cmdCheck.Parameters.Add("@DocId", SqlDbType.Int).Value = r.DocId;
                cmdCheck.Parameters.Add("@Line", SqlDbType.Int).Value = r.Line;
                cmdCheck.Parameters.Add("@FieldName", SqlDbType.VarChar, 256).Value = r.FieldName;

                int esiste = (int)cmdCheck.ExecuteScalar();

                if (esiste > 0)
                {
                    using var cmdUpd = new SqlCommand(
                        "UPDATE MA_EI_ITDocAdditionalData SET FieldValue = @FieldValue, TBModified = @DataMod, TBModifiedID = @User WHERE DocID = @DocId AND DocSubID = 0 AND Line = @Line AND SubLine = 0 AND FieldName = @FieldName",
                        connection, transaction);
                    cmdUpd.CommandTimeout = SqlTools.CommandTimeoutSeconds;
                    cmdUpd.Parameters.Add("@DocId", SqlDbType.Int).Value = r.DocId;
                    cmdUpd.Parameters.Add("@Line", SqlDbType.Int).Value = r.Line;
                    cmdUpd.Parameters.Add("@FieldName", SqlDbType.VarChar, 256).Value = r.FieldName;
                    cmdUpd.Parameters.Add("@FieldValue", SqlDbType.VarChar, 256).Value = r.FieldValue;
                    cmdUpd.Parameters.Add("@DataMod", SqlDbType.VarChar, 8).Value = now.ToString("yyyyMMdd");
                    cmdUpd.Parameters.Add("@User", SqlDbType.SmallInt, 32).Value = loginId;
                    cmdUpd.ExecuteNonQuery();
                }
                else
                {
                    using var cmdIns = new SqlCommand(
                        "INSERT INTO MA_EI_ITDocAdditionalData (DocID, DocSubID, Line, SubLine, FieldName, FieldValue, TBCreated, TBCreatedID, TBModified, TBModifiedID) VALUES (@DocId, 0, @Line, 0, @FieldName, @FieldValue, GETDATE(), @User, GETDATE(), @User)",
                        connection, transaction);
                    cmdIns.CommandTimeout = SqlTools.CommandTimeoutSeconds;
                    cmdIns.Parameters.Add("@DocId", SqlDbType.Int).Value = r.DocId;
                    cmdIns.Parameters.Add("@Line", SqlDbType.Int).Value = r.Line;
                    cmdIns.Parameters.Add("@FieldName", SqlDbType.VarChar, 256).Value = r.FieldName;
                    cmdIns.Parameters.Add("@FieldValue", SqlDbType.VarChar, 256).Value = r.FieldValue;
                    cmdIns.Parameters.Add("@User", SqlDbType.SmallInt, 32).Value = loginId;
                    cmdIns.ExecuteNonQuery();
                }
                scritti++;
            }

            return scritti;
        }

        private static string? GetStringOrNull(Microsoft.Data.SqlClient.SqlDataReader reader, string columnName)
        {
            int ordinal;
            try { ordinal = reader.GetOrdinal(columnName); }
            catch { return null; }
            if (reader.IsDBNull(ordinal)) return null;
            return reader.GetString(ordinal);
        }

        private static readonly Dictionary<string, int> CausaliPagamentoFe = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            { "", 32440347 }, { "A", 32440320 }, { "B", 32440321 }, { "C", 32440322 },
            { "D", 32440323 }, { "E", 32440324 }, { "G", 32440325 }, { "H", 32440326 },
            { "I", 32440327 }, { "L", 32440328 }, { "M", 32440329 }, { "N", 32440330 },
            { "O", 32440331 }, { "P", 32440332 }, { "Q", 32440333 }, { "R", 32440334 },
            { "S", 32440335 }, { "T", 32440336 }, { "U", 32440337 }, { "V", 32440338 },
            { "W", 32440339 }, { "X", 32440340 }, { "Y", 32440341 }, { "Z", 32440342 },
            { "L1", 32440343 }, { "M1", 32440344 }, { "O1", 32440345 }, { "V1", 32440346 },
            { "M2", 32440348 }, { "ZO", 32440349 }
        };

        private static string? ConvertFeValue(string propName, object? raw)
        {
            if (raw is null) return null;
            if (propName == "F2_1_1_5_4")
            {
                var s = raw as string;
                if (string.IsNullOrWhiteSpace(s)) return null;
                if (CausaliPagamentoFe.TryGetValue(s.Trim(), out int id)) return id.ToString();
                return null;
            }
            if (raw is string str) return string.IsNullOrWhiteSpace(str) ? null : str.Trim();
            if (raw is DateTime dt) return dt.ToString("dd/MM/yyyy");
            if (raw is short sh) return sh.ToString();
            if (raw is int i) return i.ToString();
            if (raw is long l) return l.ToString();
            return raw.ToString();
        }
    }
}