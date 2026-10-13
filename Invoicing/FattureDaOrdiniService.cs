using System;
using System.Data;
using System.Text;
using Microsoft.Data.SqlClient;

namespace FatturazioneContratti.Invoicing
{
    public static class FattureDaOrdiniService
    {
        public static bool AdeguaFattureDaOrdini(
            SqlConnection connection,
            SqlTransaction transaction,
            FiltriAllineamentoFatture filtri,
            int loginId,
            out string resultMessage)
        {
            resultMessage = string.Empty;

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
            sb.Append("WHERE (Doc.LineType = 3538947 OR Doc.LineType = 3538946) "); // Merce o Servizio
            sb.Append("AND (Testa.DocumentType = 3407874 OR Testa.DocumentType = 3407875 OR Testa.DocumentType = 3407876) "); // Fattura, Accompagnatoria, NotaCredito
            sb.Append("AND (Testa.DocumentDate >= @FromDate AND Testa.DocumentDate <= @ToDate) ");
            sb.Append("AND (@AllNumbers = 1 OR (@AllNumbers = 0 AND Testa.DocNo >= @NrFirst AND Testa.DocNo <= @NrLast)) ");
            sb.Append("AND Doc.ALL_CanoniDataI = '17991231'");

            try
            {
                using var cmd = new SqlCommand(sb.ToString(), connection, transaction);
                cmd.Parameters.Add("@FromDate", SqlDbType.VarChar, 8).Value = sFromDate;
                cmd.Parameters.Add("@ToDate", SqlDbType.VarChar, 8).Value = sToDate;
                cmd.Parameters.Add("@AllNumbers", SqlDbType.Bit).Value = filtri.AllNumbers;
                cmd.Parameters.Add("@NrFirst", SqlDbType.VarChar, 10).Value = filtri.NrFirst ?? string.Empty;
                cmd.Parameters.Add("@NrLast", SqlDbType.VarChar, 10).Value = filtri.NrLast ?? string.Empty;
                cmd.Parameters.Add("@DataMod", SqlDbType.VarChar, 8).Value = DateTime.Now.ToString("yyyyMMdd");
                cmd.Parameters.Add("@User", SqlDbType.SmallInt, 32).Value = loginId;

                int irows = cmd.ExecuteNonQuery();
                resultMessage = irows <= 0
                    ? "Nessun documento di vendita da aggiornare"
                    : $"Adeguate {irows} righe.";
                return true;
            }
            catch (Exception ex)
            {
                resultMessage = $"Errore nell'adeguamento fatture: {ex.Message}";
                return false;
            }
        }
    }
}