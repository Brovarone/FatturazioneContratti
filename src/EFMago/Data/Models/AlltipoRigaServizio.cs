using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EFMago.Models
{
    public partial class AlltipoRigaServizio
    {
        public string TipoRigaServizio { get; set; }
        public string Descrizione { get; set; }
        public string Gennaio { get; set; }
        public string Febbraio { get; set; }
        public string Marzo { get; set; }
        public string Aprile { get; set; }
        public string Maggio { get; set; }
        public string Giugno { get; set; }
        public string Luglio { get; set; }
        public string Agosto { get; set; }
        public string Settembre { get; set; }
        public string Ottobre { get; set; }
        public string Novembre { get; set; }
        public string Dicembre { get; set; }
        public int? Periodicita { get; set; }
        public int? Cadenza { get; set; }
        public DateTime Tbcreated { get; set; }
        public DateTime Tbmodified { get; set; }
        public int TbcreatedId { get; set; }
        public int TbmodifiedId { get; set; }
        public string Campagna { get; set; }
        public int? TipologiaServizio { get; set; }
        // Creato da me
        public virtual ICollection<AllordCliContratto> AllordCliContratto { get; set; }
        public virtual ICollection<AllordCliContrattoDistinta> AllordCliContrattoDistinta { get; set; }
        public virtual ICollection<AllordCliContrattoDistintaServAgg> AllordCliContrattoDistintaServAgg { get; set; }

    }
}
