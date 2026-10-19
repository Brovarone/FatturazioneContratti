using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EFMago.Models
{
    public partial class AllordCliContratto
    {
        public int IdOrdCli { get; set; }
        public short Line { get; set; }
        public string Servizio { get; set; }
        public string CodiceIva { get; set; }
		public string Descrizione { get; set; }
        public double? Qta { get; set; }
        public string Um { get; set; }
        public double? ValUnit { get; set; }
        public double? ValUnitIstat { get; set; }
        public DateTime? DataUltRivIstat { get; set; }
        public double? Franchigia { get; set; }
        public string Nota { get; set; }
        public string TipoRigaServizio { get; set; }
        public DateTime? DataDecorrenza { get; set; }
        public string NonRiportaInFatt { get; set; }
        public string Fatturato { get; set; }
        public DateTime Tbcreated { get; set; }
        public DateTime Tbmodified { get; set; }
        public int TbcreatedId { get; set; }
        public int TbmodifiedId { get; set; }
        public DateTime? DataFineElaborazione { get; set; }
        public DateTime? DataProssimaFatt { get; set; }
        public string CodContratto { get; set; }
        public string CodIntegra { get; set; }
        public short? SubLineAttivita { get; set; }
        public short? SubLineDescFatt { get; set; }
        public short? SubLineDistinta { get; set; }
        public string Impianto { get; set; }
        public string MultiImpianto { get; set; }
        public string CdC { get; set; }
       
        // Creato da me
        public virtual MaSaleOrd SaleOrd { get; set; }
        public virtual MaItems MaItems { get; set; }
        public virtual AlltipoRigaServizio AlltipoRigaServizio { get; set; }
        public virtual ICollection<AllordCliAttivita> AllordCliAttivita { get; set; }
        /// <summary>
        /// Descrizioni fattura per ogni singola riga contratto
        /// </summary>
        public virtual ICollection<AllordCliContrattoDescFatt> AllordCliContrattoDescFatt { get; set; }
        public virtual ICollection<AllordCliContrattoDistinta> AllordCliContrattoDistinta { get; set; }

    }
}
