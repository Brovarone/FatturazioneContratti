using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EFMago.Models
{
    public partial class MaStandardCostHistorical
    {
        public short FiscalYear { get; set; }
        public short FiscalPeriod { get; set; }
        public string Item { get; set; }
        public double? StandardCost { get; set; }
        public DateTime ToValueDate { get; set; }
        public string OperationType { get; set; }
        public DateTime Tbcreated { get; set; }
        public DateTime Tbmodified { get; set; }
        public int TbcreatedId { get; set; }
        public int TbmodifiedId { get; set; }

        public virtual MaItems ItemNavigation { get; set; }
    }
}
