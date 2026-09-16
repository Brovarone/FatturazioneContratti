using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EFMago.Models
{
    public partial class MaCustSuppParameters
    {
        public int CustSuppParametersId { get; set; }
        public string IsocountryCode { get; set; }
        public string CustomerAutoNum { get; set; }
        public int? LastCustomer { get; set; }
        public string SupplierAutonum { get; set; }
        public int? LastSupplier { get; set; }
        public short? CustSuppMaxChars { get; set; }
        public string ContactsAutoNum { get; set; }
        public int? LastContact { get; set; }
        public short? ContactsMaxChars { get; set; }
        public string ProspSuppAutoNum { get; set; }
        public int? LastProspSupp { get; set; }
        public string CustPaymentTerm { get; set; }
        public string SuppPaymentTerm { get; set; }
        public string EucustomerTaxCode { get; set; }
        public string PrefixWithSiteCode { get; set; }
        public string AutomaticPrefix { get; set; }
        public string ContactsAutomaticPrefix { get; set; }
        public string CustSuppDraftDefault { get; set; }
        public int? CustSuppDraftType { get; set; }
        public int? BankComboMaxItems { get; set; }
        public string TaxIdNumberLinkUrl { get; set; }
        public int? CityComboMaxItems { get; set; }
        public int? CustMailSendingType { get; set; }
        public int? CustDocumentSendingType { get; set; }
        public int? SuppMailSendingType { get; set; }
        public int? SuppDocumentSendingType { get; set; }
        public string UseCityComplete { get; set; }
        public DateTime Tbcreated { get; set; }
        public DateTime Tbmodified { get; set; }
        public int TbcreatedId { get; set; }
        public int TbmodifiedId { get; set; }
    }
}
