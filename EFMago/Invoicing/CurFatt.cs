using System;
using EFMago.Models;

namespace FatturazioneContratti.Invoicing
{
    public class CurFatt
    {
        public int SaleDocId { get; set; }
        /// <summary>
        /// Anagrafica del cliente di fatturazione. Il codice cliente si risolve con
        /// <see cref="CodiceClienteFatturazione"/> (InvoicingCustomer, altrimenti Customer).
        /// Invariante di dominio: ogni ordine da consuntivare ha il cliente in anagrafica
        /// (CustSuppType 3211264); in <c>GeneraFattureConsuntivo</c> una mancanza viene
        /// segnalata con un [AVVISO] e la run prosegue coi fallback (accTpl EU, campi banca
        /// vuoti). Il <c>CurFatt()</c> vuoto usato come buffer viene valorizzato prima di
        /// qualsiasi scrittura.
        /// </summary>
        public MaCustSupp Cliente { get; set; } = null!;
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
            // SaleDocId/Cliente restano ai default CLR (0/null); LastLine/LastSubId/PercIva
            // sono int?/double? e lo 0 è un valore intenzionale, non il default null.
            DateTime d = DateTime.Today;
            DataFattura = d;
            LastLine = 0;
            LastSubId = 0;
            PercIva = 0;
        }

        public CurFatt(MaSaleOrd o) : this()
        {
            CodCliente = CodiceClienteFatturazione(o);
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

        /// <summary>
        /// Codice cliente da riportare sulla fattura: <c>InvoicingCustomer</c> dell'ordine e, se
        /// vuoto, il codice cliente dell'ordine (<c>Customer</c>). È la stessa regola usata per
        /// caricare l'anagrafica e per il criterio di rottura/accorpamento della fattura, così
        /// testa fattura, lookup anagrafica e raggruppamento non possono divergere.
        /// </summary>
        public static string CodiceClienteFatturazione(MaSaleOrd o) =>
            string.IsNullOrWhiteSpace(o.InvoicingCustomer)
                ? o.Customer ?? string.Empty
                : o.InvoicingCustomer;
    }
}