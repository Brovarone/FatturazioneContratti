using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EFMago.Models
{
    public partial class MaItemsIntrastat
    {
        public string Item { get; set; }
        public string CombinedNomenclature { get; set; }
        public string CountyOfOrigin { get; set; }
        public string IsoofOrigin { get; set; }
        public double? SpecWeightNetMass { get; set; }
        public string SuppUnitDescription { get; set; }
        public double? SuppUnitSpecWeight { get; set; }
        public string Prodcom { get; set; }
        public int? IntrastatSupplyType { get; set; }
        public string Cpacode { get; set; }
        public DateTime Tbcreated { get; set; }
        public DateTime Tbmodified { get; set; }
        public int TbcreatedId { get; set; }
        public int TbmodifiedId { get; set; }

        public virtual MaItems ItemNavigation { get; set; }
    }
}
