using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EFMago.Models
{
    public partial class MaSaleDocComponents
    {
        public int SaleDocId { get; set; }
        public int SubId { get; set; }
        public short Line { get; set; }
        public string Component { get; set; }
        public string Description { get; set; }
        public double? NeededQty { get; set; }
        public string UoM { get; set; }
        public double? UnitValue { get; set; }
        public double? NeededQtyWaste { get; set; }
        public string WasteUoM { get; set; }
        public string Waste { get; set; }
        public double? WasteUnitValue { get; set; }
        public string FixedQty { get; set; }
        public DateTime Tbcreated { get; set; }
        public DateTime Tbmodified { get; set; }
        public int TbcreatedId { get; set; }
        public int TbmodifiedId { get; set; }

        public virtual MaSaleDoc SaleDoc { get; set; }
    }
}
