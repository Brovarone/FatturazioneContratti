using System;
using System.Linq;
using EFMago.Models;

namespace FatturazioneContratti.Orders
{
    /// <summary>
    /// Porting 1:1 di CurOrd.vb (3tables/Ordini/CurOrd.vb): rappresenta la testa
    /// dell'ordine mentre lo si costruisce/aggiorna (numerazione righe correnti,
    /// date di scadenza/cessazione della testa contratto).
    /// </summary>
    public class CurOrd
    {
        public int SaleOrdId { get; set; }
        public int LastLine { get; set; }
        public int LastPosition { get; set; }
        public int LastSubId { get; set; }
        public int CurrentLastLine { get; set; }
        public int CurrentLastPosition { get; set; }
        public int CurrentLastSubId { get; set; }
        public string Cliente { get; set; }
        public DateTime OrdDate { get; set; }
        public string OrdNo { get; set; }

        /// <summary>Indica solo che è presente una data. Il controllo va poi fatto sulla riga.</summary>
        public bool HaScadenzaFissa { get; set; }
        public DateTime DataScadenzaFissa { get; set; }
        public bool HaDataCessazione { get; set; }
        public DateTime DataCessazione { get; set; }
        public string Commessa { get; set; }
        public string CdC { get; set; }
        public string CIG { get; set; }
        public string CUP { get; set; }

        /// <summary>
        /// Indica se è presente una delle due date (Cessazione o Scadenza Fissa).
        /// Non è detto che sia raggiunta.
        /// </summary>
        public bool EsisteDataScadenza { get; set; }

        public CurOrd()
        {
            var d = DateTime.Today;
            SaleOrdId = 0;
            LastLine = 0;
            LastPosition = 0;
            LastSubId = 0;
            CurrentLastLine = 0;
            CurrentLastPosition = 0;
            CurrentLastSubId = 0;
            Cliente = string.Empty;
            OrdDate = d;
            OrdNo = string.Empty;
            HaScadenzaFissa = false;
            DataScadenzaFissa = d;
            HaDataCessazione = false;
            DataCessazione = d;
            Commessa = string.Empty;
            CdC = string.Empty;
            CIG = string.Empty;
            CUP = string.Empty;
            EsisteDataScadenza = false;
        }

        /// <summary>Popola la classe a partire da un ordine di vendita esistente (ex New(o As MaSaleOrd)).</summary>
        public CurOrd(MaSaleOrd o) : this()
        {
            SaleOrdId = o.SaleOrdId;
            LastLine = o.MaSaleOrdDetails.Any() ? o.MaSaleOrdDetails.Max(m => m.Line) : 0;
            LastPosition = o.MaSaleOrdDetails.Any() ? o.MaSaleOrdDetails.Max(m => m.Position) : 0;
            LastSubId = (int)(o.MaSaleOrdDetails.Count != 0 ? o.MaSaleOrdDetails.Max(m => m.SubId) : 0);
            CurrentLastLine = LastLine;
            CurrentLastPosition = LastPosition;
            CurrentLastSubId = LastSubId;
            Cliente = o.Customer;
            OrdDate = o.OrderDate ?? DomainConstants.DataNulla;
            OrdNo = o.InternalOrdNo;
            DataScadenzaFissa = o.ALLOrdCliAcc?.DataScadenzaFissa ?? DomainConstants.DataNulla;
            HaScadenzaFissa = DataScadenzaFissa != DomainConstants.DataNulla;
            DataCessazione = o.ALLOrdCliAcc?.DataCessazione ?? DomainConstants.DataNulla;
            HaDataCessazione = DataCessazione != DomainConstants.DataNulla;
            Commessa = o.Job;
            CdC = o.CostCenter;
            CIG = o.ContractCode;
            CUP = o.ProjectCode;
            EsisteDataScadenza = DataCessazione != DomainConstants.DataNulla || DataScadenzaFissa != DomainConstants.DataNulla;
        }
    }
}
