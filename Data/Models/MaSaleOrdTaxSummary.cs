using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EFMago.Models
{
    public partial class MaSaleOrdTaxSummary
    {
        public string TaxCode { get; set; }
        public double? TaxableAmount { get; set; }
        public double? TaxAmount { get; set; }
        public double? TotalAmount { get; set; }
        public int SaleOrdId { get; set; }
        public double? TaxableAmountDocCurr { get; set; }
        public double? TaxAmountDocCurr { get; set; }
        public double? TotalAmountDocCurr { get; set; }
        public DateTime Tbcreated { get; set; }
        public DateTime Tbmodified { get; set; }
        public int TbcreatedId { get; set; }
        public int TbmodifiedId { get; set; }

        public virtual MaSaleOrd SaleOrd { get; set; }
    }
}
