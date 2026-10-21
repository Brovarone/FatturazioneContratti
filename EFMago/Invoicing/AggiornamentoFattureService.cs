using System;
using System.Data;
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
        /// Il messaggio di esito viene loggato qui dentro (filtri ed esito come informativo,
        /// eccezione con stack come errore): il caller passa solo il delegato.
        /// </summary>
        public static bool AdeguaFattureDaOrdini(
            SqlConnection connection,
            SqlTransaction transaction,
            FiltriAllineamentoFatture filtri,
            int loginId,
            Action<string, LogTarget>? logMessage = null)
        {
            // I filtri della run vengono registrati su UI/file come negli altri workflow.
            LogDiagnostica.LogFiltri(logMessage, filtri);

            // Timestamp unico dell'operazione (convenzione degli altri workflow):
            // tutte le righe aggiornate portano lo stesso valore.
            DateTime now = DateTime.Now;

            string sFromDate = filtri.DataFattDa.ToString("yyyyMMdd");
            string sToDate = filtri.DataFattA.ToString("yyyyMMdd");

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

                int irows = cmd.ExecuteNonQuery();
                LogDiagnostica.Informativo(logMessage, irows <= 0
                    ? "Nessun documento di vendita da aggiornare"
                    : $"Adeguate {irows} righe.");
                return true;
            }
            catch (Exception ex)
            {
                // Errore: messaggio su UI e file, stack sul file.
                LogDiagnostica.Errore(logMessage, ex);
                return false;
            }
        }
    }
}