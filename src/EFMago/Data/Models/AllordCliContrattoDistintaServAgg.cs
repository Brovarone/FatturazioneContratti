using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EFMago.Models
{
    public partial class AllordCliContrattoDistintaServAgg
    {
        public int IdOrdCli { get; set; }
        /// <summary>
        /// Numero di Linea
        /// </summary>
        public short Line { get; set; }
        /// <summary>
        /// Riferimento Linea Distinta
        /// </summary>
        public short RifLinea { get; set; }
        /// <summary>
        /// Riferimento Linea Contratto
        /// </summary>
        public short RifRifLinea { get; set; }
        public string Servizio { get; set; }
        public string TipoRigaServizio { get; set; }
        //public string CodiceIva { get; set; }
        public string Descrizione { get; set; }
        public string Um { get; set; }
        public double? Qta { get; set; }
        public double? Franchigia { get; set; }
        public DateTime? DataDecorrenza { get; set; }
        public DateTime? DataProssimaFatt { get; set; }
        public double? ValUnit { get; set; }
        public double? ValUnitIstat { get; set; }
        public DateTime? DataUltRivIstat { get; set; }
        public DateTime? DataFineElaborazione { get; set; }
        //public string CodIntegra { get; set; }
        //public string Nota { get; set; }
        //public string NonRiportaInFatt { get; set; }
        //public string Fatturato { get; set; }
        public DateTime Tbcreated { get; set; }
        public DateTime Tbmodified { get; set; }
        public int TbcreatedId { get; set; }
        public int TbmodifiedId { get; set; }
        public int? Periodicita { get; set; }

        // Creato da me
        public virtual AllordCliContrattoDistinta AllordCliContrattoDistinta { get; set; }
        public virtual MaItems MaItems { get; set; }
        public virtual AlltipoRigaServizio AlltipoRigaServizio { get; set; }
        //public virtual ICollection<AllordCliAttivita> AllordCliAttivita { get; set; }

        //Sezione interventi presi da Distinta (Parent) non presenti 
        [NotMapped]
        public int? AnnoIntervento { get; set; }
        /// <summary>
        /// Interventi da Integra estratti n base al periodo di Franchigia
        /// </summary>
        [NotMapped]        
        public double? NrInterventiInFranchigia { get; set; }
 
        /// <summary>
        /// Interventi da Integra
        /// </summary>
        [NotMapped]
        public double? NrInterventiMese { get; set; }
        /// <summary>
        /// Interventi da Fatturare
        /// </summary>
        [NotMapped]
        public double? NrInterventiOltreFranchigia { get; set; }

        //[NotMapped]     
        //public double? NrInterventiPeriodo { get; set; }


    }
}
