using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using FatturazioneContratti.Common;
using FatturazioneContratti.Data;
using Microsoft.Data.SqlClient;

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
            int _loginId,
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
                cmd.Parameters.Add("@User", SqlDbType.SmallInt, 32).Value = _loginId;

                // Interruzione richiesta prima della UPDATE: si lancia OperationCanceledException
                // e la transazione del chiamante viene rollbackata dal dispose.
                token.ThrowIfCancellationRequested();

                int irows = cmd.ExecuteNonQuery();
                LogDiagnostica.Informativo(logMessage, irows <= 0
                    ? "Nessuna riga canoni da allineare."
                    : $"Allineate {irows} righe canoni.");

                // --- OPERAZIONE B: upsert dati fattura elettronica ---
                if (filtri.SincronizzaFatturaElettronica)
                {
                    token.ThrowIfCancellationRequested();
                    (int documenti, int campi) = UpsertDatiFatturaElettronica(connection, transaction, filtri, now, _loginId, logMessage, token);
                    LogDiagnostica.Informativo(logMessage, documenti <= 0
                        ? "Nessun documento di vendita con dati fattura elettronica da sincronizzare"
                        : documenti == 1
                            ? $"Sincronizzati dati fattura elettronica su 1 documento ({campi} campi)."
                            : $"Sincronizzati dati fattura elettronica su {documenti} documenti ({campi} campi).");
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

        private static (int Documenti, int Righe) UpsertDatiFatturaElettronica(
            SqlConnection connection,
            SqlTransaction transaction,
            FiltriAllineamentoFatture filtri,
            DateTime now,
            int loginId,
            Action<string, LogTarget>? _logMessage,
            CancellationToken token)
        {
            // Chiavi delle righe fattura target: stessi filtri dell'operazione A, senza il
            // vincolo ALL_CanoniDataI (si allineano anche righe già processate per i dati FE).
            string sFromDate = filtri.DataFattDa.ToString("yyyyMMdd");
            string sToDate = filtri.DataFattA.ToString("yyyyMMdd");

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

            var managedFieldNames = string.Join(", ", campiFe.Select(c => $"'{c.FieldName}'"));
            var valuesFe = string.Join(",\n            ", campiFe.Select(c =>
                $"('{c.FieldName}', CONVERT(varchar(256), {SqlFeValueExpression(c.PropName)}))"));

            // Un unico batch: target, pulizia delle sole righe gestite, inserimento set-based.
            // Evita le IN con migliaia di parametri e il COUNT+UPDATE/INSERT per riga.
            var sql = new StringBuilder();
            sql.AppendLine("SET NOCOUNT ON;");
            sql.AppendLine("SELECT DISTINCT Doc.SaleDocId, Doc.SaleOrdId");
            sql.AppendLine("INTO #target");
            sql.AppendLine("FROM MA_SaleDoc Testa");
            sql.AppendLine("INNER JOIN MA_SaleDocDetail Doc ON Testa.SaleDocId = Doc.SaleDocId");
            sql.AppendLine("WHERE (Doc.LineType = @LineTypeMerce OR Doc.LineType = @LineTypeServizio)");
            sql.AppendLine("  AND (Testa.DocumentType = @DocFattura OR Testa.DocumentType = @DocFatturaAcc OR Testa.DocumentType = @DocNotaCredito)");
            sql.AppendLine("  AND (Testa.DocumentDate >= @FromDate AND Testa.DocumentDate <= @ToDate)");
            sql.AppendLine("  AND (@AllNumbers = 1 OR (@AllNumbers = 0 AND Testa.DocNo >= @NrFirst AND Testa.DocNo <= @NrLast))");
            sql.AppendLine("  AND Doc.SaleOrdId IS NOT NULL");
            sql.AppendLine("  AND EXISTS (SELECT 1 FROM ALLOrdCliFattEle fe WHERE fe.IdOrdCli = Doc.SaleOrdId);");
            sql.AppendLine("DECLARE @documenti int = (SELECT COUNT(DISTINCT SaleDocId) FROM #target);");
            sql.AppendLine("DELETE e");
            sql.AppendLine("FROM MA_EI_ITDocAdditionalData e");
            sql.AppendLine("INNER JOIN #target t ON t.SaleDocId = e.DocID");
            sql.AppendLine($"WHERE e.DocSubID = 0 AND e.SubLine = 0 AND e.FieldName IN ({managedFieldNames});");
            sql.AppendLine("WITH base AS (");
            sql.AppendLine("    SELECT t.SaleDocId, ISNULL(MAX(e.Line), 0) AS BaseLine");
            sql.AppendLine("    FROM #target t");
            sql.AppendLine("    LEFT JOIN MA_EI_ITDocAdditionalData e ON e.DocID = t.SaleDocId");
            sql.AppendLine("    GROUP BY t.SaleDocId");
            sql.AppendLine("), ordini AS (");
            sql.AppendLine("    SELECT t.SaleDocId, t.SaleOrdId,");
            sql.AppendLine("           CAST(b.BaseLine + ROW_NUMBER() OVER (PARTITION BY t.SaleDocId ORDER BY t.SaleOrdId) AS int) AS Line");
            sql.AppendLine("    FROM #target t");
            sql.AppendLine("    INNER JOIN base b ON b.SaleDocId = t.SaleDocId");
            sql.AppendLine("), righe AS (");
            sql.AppendLine("    SELECT o.SaleDocId, o.Line, v.FieldName, v.FieldValue");
            sql.AppendLine("    FROM ordini o");
            sql.AppendLine("    INNER JOIN ALLOrdCliFattEle r ON r.IdOrdCli = o.SaleOrdId");
            sql.AppendLine("    CROSS APPLY (VALUES");
            sql.AppendLine($"            {valuesFe}");
            sql.AppendLine("    ) v(FieldName, FieldValue)");
            sql.AppendLine("    WHERE v.FieldValue IS NOT NULL AND v.FieldValue <> ''");
            sql.AppendLine("), dedup AS (");
            sql.AppendLine("    SELECT SaleDocId, Line, FieldName, FieldValue,");
            sql.AppendLine("           ROW_NUMBER() OVER (PARTITION BY SaleDocId, FieldName, FieldValue ORDER BY Line) AS rn");
            sql.AppendLine("    FROM righe");
            sql.AppendLine(")");
            sql.AppendLine("INSERT INTO MA_EI_ITDocAdditionalData (DocID, DocSubID, Line, SubLine, FieldName, FieldValue, TBCreated, TBCreatedID, TBModified, TBModifiedID)");
            sql.AppendLine("SELECT SaleDocId, 0, Line, 0, FieldName, FieldValue, @now, @User, @now, @User");
            sql.AppendLine("FROM dedup");
            sql.AppendLine("WHERE rn = 1;");
            sql.AppendLine("DECLARE @inseriti int = @@ROWCOUNT;");
            sql.AppendLine("DROP TABLE #target;");
            sql.AppendLine("SELECT @documenti, @inseriti;");

            using var cmd = new SqlCommand(sql.ToString(), connection, transaction);
            cmd.CommandTimeout = SqlTools.CommandTimeoutSeconds;
            cmd.Parameters.Add("@LineTypeMerce", SqlDbType.Int).Value = (int)LineType.Merce;
            cmd.Parameters.Add("@LineTypeServizio", SqlDbType.Int).Value = (int)LineType.Servizio;
            cmd.Parameters.Add("@DocFattura", SqlDbType.Int).Value = (int)DocumentType.Fattura;
            cmd.Parameters.Add("@DocFatturaAcc", SqlDbType.Int).Value = (int)DocumentType.FatturaAccompagnatoria;
            cmd.Parameters.Add("@DocNotaCredito", SqlDbType.Int).Value = (int)DocumentType.NotaCredito;
            cmd.Parameters.Add("@FromDate", SqlDbType.VarChar, 8).Value = sFromDate;
            cmd.Parameters.Add("@ToDate", SqlDbType.VarChar, 8).Value = sToDate;
            cmd.Parameters.Add("@AllNumbers", SqlDbType.Bit).Value = filtri.AllNumbers;
            cmd.Parameters.Add("@NrFirst", SqlDbType.VarChar, 10).Value = filtri.NrFirst ?? string.Empty;
            cmd.Parameters.Add("@NrLast", SqlDbType.VarChar, 10).Value = filtri.NrLast ?? string.Empty;
            cmd.Parameters.Add("@now", SqlDbType.DateTime).Value = now;
            cmd.Parameters.Add("@User", SqlDbType.SmallInt, 32).Value = loginId;

            token.ThrowIfCancellationRequested();
            using var reader = cmd.ExecuteReader();
            reader.Read();
            return (Convert.ToInt32(reader.GetValue(0)), Convert.ToInt32(reader.GetValue(1)));
        }

        private static string SqlFeValueExpression(string propName)
        {
            if (propName == "F2_1_1_5_4")
            {
                var causali = string.Join(", ", MagoConstants.CausaliPagamento
                    .Where(kv => kv.Key.Length > 0)
                    .Select(kv => $"('{kv.Key}', {kv.Value})"));
                return $"(SELECT c.Id FROM (VALUES {causali}) c(Lettera, Id) WHERE c.Lettera = UPPER(LTRIM(RTRIM(ISNULL(r.{propName}, '')))))";
            }
            var parts = propName.Split('_');
            var suffisso = parts.Length >= 4 ? parts[3] : string.Empty;
            // 0 = riferimento linea non impostato; 1799-12-31 = DataNulla Mago.
            if (suffisso == "1") return $"CONVERT(varchar(10), NULLIF(r.{propName}, 0))";
            if (suffisso == "3") return $"CONVERT(varchar(10), NULLIF(r.{propName}, CONVERT(datetime, '17991231', 112)), 103)";
            return $"NULLIF(LTRIM(RTRIM(r.{propName})), '')";
        }
    }
}