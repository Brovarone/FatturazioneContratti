using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EFMago.Models
{
    public partial class MaTaxCodesDefaults
    {
        public int TaxCodesDefaultsId { get; set; }
        public string TaxCode { get; set; }
        public string DistributionTaxCode { get; set; }
        public string ExemptedTaxCode { get; set; }
        public string ExemptedDedAllowTaxCode { get; set; }
        public string ExemptedDedNotAllowTaxCode { get; set; }
        public string ExemptionTaxCode { get; set; }
        public string ExemptedTaxCodeEuad { get; set; }
        public string ExemptedTaxCodeEubc { get; set; }
        public string NotSubjectServicesTaxCode { get; set; }
        public string ExemptServicesTaxCode { get; set; }
        public string NotTaxableGoodsTaxCode { get; set; }
        public string ReverseChargeTaxCode { get; set; }
        public DateTime Tbcreated { get; set; }
        public DateTime Tbmodified { get; set; }
        public int TbcreatedId { get; set; }
        public int TbmodifiedId { get; set; }
    }
}
