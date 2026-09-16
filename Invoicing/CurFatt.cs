using System;
using EFMago.Models;

namespace FatturazioneContratti.Invoicing
{
    public class CurFatt
    {
        public int SaleDocId { get; set; }
        public MaCustSupp Cliente { get; set; }
        public DateTime DataFattura { get; set; }
        public int? LastLine { get; set; }
        public int? LastSubId { get; set; }
        public string CodCliente { get; set; } = string.Empty;
        public string Pagamento { get; set; } = string.Empty;
        public string SedeInvioMerce { get; set; } = string.Empty;
        public string SedeInvioDocumenti { get; set; } = string.Empty;
        public string SedePagamenti { get; set; } = string.Empty;
        public string Contropartita { get; set; } = string.Empty;
        public string CodIva { get; set; } = string.Empty;
        public double? PercIva { get; set; }
        public string Commessa { get; set; } = string.Empty;
        public string CdC { get; set; } = string.Empty;
        public string CIG { get; set; } = string.Empty;
        public string CUP { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public string AreaManager { get; set; } = string.Empty;
        public string SalesPerson { get; set; } = string.Empty;
        public string LetteraCausalePagamento { get; set; } = string.Empty;

        public CurFatt()
        {
            DateTime d = DateTime.Today;
            SaleDocId = 0;
            Cliente = null;
            DataFattura = d;
            LastLine = 0;
            LastSubId = 0;
            PercIva = 0;
        }

        public CurFatt(MaSaleOrd o) : this()
        {
            CodCliente = o.InvoicingCustomer ?? string.Empty;
            Pagamento = o.Payment ?? string.Empty;
            SedeInvioMerce = o.ShipToAddress ?? string.Empty;
            SedeInvioDocumenti = o.SendDocumentsTo ?? string.Empty;
            SedePagamenti = o.PaymentAddress ?? string.Empty;
            Commessa = o.Job ?? string.Empty;
            CdC = o.CostCenter ?? string.Empty;
            CIG = o.ContractCode ?? string.Empty;
            CUP = o.ProjectCode ?? string.Empty;
            Area = o.Area ?? string.Empty;
            AreaManager = o.AreaManager ?? string.Empty;
            SalesPerson = o.Salesperson ?? string.Empty;
            LetteraCausalePagamento = o.ALLOrdCliFattEle?.F2_1_1_5_4 ?? string.Empty;
        }
    }
}