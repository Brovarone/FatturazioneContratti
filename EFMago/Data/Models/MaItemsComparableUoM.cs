using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EFMago.Models
{
    public partial class MaItemsComparableUoM
    {
        public string Item { get; set; }
        public string ComparableUoM { get; set; }
        public double? NoOfPacksCompUoM { get; set; }
        public double? BaseUoMqty { get; set; }
        public double? CompUoMqty { get; set; }
        public double? Factor1 { get; set; }
        public string Factor1Description { get; set; }
        public double? Factor2 { get; set; }
        public string Factor2Description { get; set; }
        public double? Factor3 { get; set; }
        public string Factor3Description { get; set; }
        public string Notes { get; set; }
        public double? Factor4 { get; set; }
        public string Factor4Description { get; set; }
        public double? GrossWeight { get; set; }
        public double? GrossVolume { get; set; }
        public string Packaging { get; set; }
        public string IsDisabled { get; set; }
        public DateTime Tbcreated { get; set; }
        public DateTime Tbmodified { get; set; }
        public int TbcreatedId { get; set; }
        public int TbmodifiedId { get; set; }

        public virtual MaItems ItemNavigation { get; set; }
    }
}
