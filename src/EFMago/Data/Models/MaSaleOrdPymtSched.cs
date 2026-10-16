using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EFMago.Models
{
    public partial class MaSaleOrdPymtSched
    {
        public short InstallmentNo { get; set; }
        public int? InstallmentType { get; set; }
        public short? DueDateDays { get; set; }
        public double? Amount { get; set; }
        public double? TaxAmount { get; set; }
        public int SaleOrdId { get; set; }
        public int? InstallmStartDate { get; set; }
        public DateTime Tbcreated { get; set; }
        public DateTime Tbmodified { get; set; }
        public int TbcreatedId { get; set; }
        public int TbmodifiedId { get; set; }

        public virtual MaSaleOrd SaleOrd { get; set; }
    }
}
