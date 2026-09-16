using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EFMago.Models
{
    public partial class MaItemsWmszones
    {
        public string Item { get; set; }
        public string Storage { get; set; }
        public string Zone { get; set; }
        public string AutoReplenishment { get; set; }
        public double? MinStock { get; set; }
        public double? MaxStock { get; set; }
        public string FixedBinPicking { get; set; }
        public string FixedBinPutaway { get; set; }
        public DateTime Tbcreated { get; set; }
        public DateTime Tbmodified { get; set; }
        public int TbcreatedId { get; set; }
        public int TbmodifiedId { get; set; }

        public virtual MaItems ItemNavigation { get; set; }
    }
}
