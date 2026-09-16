using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EFMago.Models
{
    public partial class MaSaleDocPymtSched
    {
        public int SaleDocId { get; set; }
        public int SubId { get; set; }
        public short InstallmentNo { get; set; }
        public int? InstallmentType { get; set; }
        public DateTime? DueDate { get; set; }
        public double? InstallmentAmount { get; set; }
        public double? InstallmentTaxAmount { get; set; }
        public string PymtCash { get; set; }
        public string PymtAccount { get; set; }
        public string CostCenter { get; set; }
        public DateTime Tbcreated { get; set; }
        public DateTime Tbmodified { get; set; }
        public int TbcreatedId { get; set; }
        public int TbmodifiedId { get; set; }

        public virtual MaSaleDoc SaleDoc { get; set; }
    }
}
