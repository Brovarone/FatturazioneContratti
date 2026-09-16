using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EFMago.Models
{
    public partial class MaPlafond
    {
        public short BalanceYear { get; set; }
        public short BalanceMonth { get; set; }
        public double? Inside { get; set; }
        public double? Importing { get; set; }
        public double? Eupurchases { get; set; }
        public Guid? Tbguid { get; set; }
        public double? ForecastInside { get; set; }
        public double? ForecastImporting { get; set; }
        public double? ForecastEupurchases { get; set; }
        public DateTime Tbcreated { get; set; }
        public DateTime Tbmodified { get; set; }
        public int TbcreatedId { get; set; }
        public int TbmodifiedId { get; set; }
    }
}
