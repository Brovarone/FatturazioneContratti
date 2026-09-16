using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EFMago.Models
{
    public partial class MaCustSuppSupplierOptions
    {
        public int CustSuppType { get; set; }
        public string Supplier { get; set; }
        public string LastDocNo { get; set; }
        public DateTime? LastDocDate { get; set; }
        public double? LastDocTotal { get; set; }
        public string LastPaymentTerm { get; set; }
        public string GoodsOffset { get; set; }
        public string ServicesOffset { get; set; }
        public string Port { get; set; }
        public string Package { get; set; }
        public string Area { get; set; }
        public string Salesperson { get; set; }
        public int? SupplierType { get; set; }
        public string Transport { get; set; }
        public string SuspendedTax { get; set; }
        public string ExemptFromTax { get; set; }
        public string TaxCode { get; set; }
        public string Blocked { get; set; }
        public string BlockPayments { get; set; }
        public string ShowPricesOnDn { get; set; }
        public int? ReferencesPrintType { get; set; }
        public double? MaximumCredit { get; set; }
        public double? CashOnDeliveryLevel { get; set; }
        public string GroupStampCharges { get; set; }
        public string GroupCollectionCharges { get; set; }
        public int? CreditFreeSamplesTaxAmount { get; set; }
        public string Category { get; set; }
        public string NoDngeneration { get; set; }
        public string Carrier1 { get; set; }
        public string Carrier2 { get; set; }
        public string Carrier3 { get; set; }
        public string Shipping { get; set; }
        public string CustomTariff { get; set; }
        public int? ChargesType { get; set; }
        public int? MaximumCreditCheckType { get; set; }
        public string GroupPymtOrders { get; set; }
        public string SupplierClassification { get; set; }
        public string SupplierSpecification { get; set; }
        public DateTime? MaximumCreditDate { get; set; }
        public DateTime? CashOnDeliveryLevelDate { get; set; }
        public DateTime Tbcreated { get; set; }
        public DateTime Tbmodified { get; set; }
        public int TbcreatedId { get; set; }
        public int TbmodifiedId { get; set; }

        public virtual MaCustSupp MaCustSupp { get; set; }
    }
}
