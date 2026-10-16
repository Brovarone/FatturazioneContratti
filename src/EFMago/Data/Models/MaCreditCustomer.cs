using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EFMago.Models
{
    public partial class MaCreditCustomer
    {
        public int CustSuppType { get; set; }
        public string Customer { get; set; }
        public string CreditLimitManage { get; set; }
        public double? MaxOrderValue { get; set; }
        public DateTime? MaxOrderValueDate { get; set; }
        public double? MaxOrderedValue { get; set; }
        public DateTime? MaxOrderedValueDate { get; set; }
        public double? MaximumCredit { get; set; }
        public DateTime? MaximumCreditDate { get; set; }
        public double? TotalExposure { get; set; }
        public DateTime? TotalExposureDate { get; set; }
        public int? MaxOrderValueCheckType { get; set; }
        public int? MaxOrderedValueCheckType { get; set; }
        public int? MaximumCreditCheckType { get; set; }
        public int? TotalExposureCheckType { get; set; }
        public int? MaximumCreditCheckTypeDelDoc { get; set; }
        public int? MaximumCreditCheckTypeDefInv { get; set; }
        public int? MaximumCreditCheckTypeImmInv { get; set; }
        public int? TotalExposureCheckTypeDelDoc { get; set; }
        public int? TotalExposureCheckTypeDefInv { get; set; }
        public int? TotalExposureCheckTypeImmInv { get; set; }
        public DateTime Tbcreated { get; set; }
        public DateTime Tbmodified { get; set; }
        public int TbcreatedId { get; set; }
        public int TbmodifiedId { get; set; }

        public virtual MaCustSupp Cust { get; set; }
    }
}
