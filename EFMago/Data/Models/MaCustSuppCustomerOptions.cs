using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EFMago.Models
{
    public partial class MaCustSuppCustomerOptions
    {
        public int CustSuppType { get; set; }
        public string Customer { get; set; }
        public string Category { get; set; }
        public string CommissionCtg { get; set; }
        public string Area { get; set; }
        public string Salesperson { get; set; }
        public string AreaManager { get; set; }
        public string IsAprivatePerson { get; set; }
        public string SuspendedTax { get; set; }
        public string ExemptFromTax { get; set; }
        public string TaxCode { get; set; }
        public string GoodsOffset { get; set; }
        public string ServicesOffset { get; set; }
        public string Blocked { get; set; }
        public string OpenedAdmCases { get; set; }
        public double? OpenedAdmCasesAmount { get; set; }
        public int? DebitFreeSamplesTaxAmount { get; set; }
        public string Port { get; set; }
        public string Carrier1 { get; set; }
        public string Carrier2 { get; set; }
        public string Carrier3 { get; set; }
        public double? MaxOrderValue { get; set; }
        public double? MaxOrderedValue { get; set; }
        public double? MaximumCredit { get; set; }
        public string LastDocNo { get; set; }
        public DateTime? LastDocDate { get; set; }
        public double? LastDocTotal { get; set; }
        public string LastPaymentTerm { get; set; }
        public string DeclarationOfIntentNo { get; set; }
        public DateTime? DeclarationOfIntentDate { get; set; }
        public string DeclarationOfIntentOurNo { get; set; }
        public string InvoicingGroup { get; set; }
        public double? FreeOfChargeLevel { get; set; }
        public double? CashOnDeliveryLevel { get; set; }
        public string NoCarrierCharges { get; set; }
        public double? PackCharges { get; set; }
        public double? ShippingCharges { get; set; }
        public double? ChargesPercOnTotAmt { get; set; }
        public string ShowPricesOnDn { get; set; }
        public string OneInvoicePerDn { get; set; }
        public string GroupItems { get; set; }
        public string GroupBills { get; set; }
        public string Package { get; set; }
        public string Transport { get; set; }
        public int? ReferencesPrintType { get; set; }
        public double? CashOrderCharges { get; set; }
        public string DebitStampCharges { get; set; }
        public string DebitCollectionCharges { get; set; }
        public string GroupOrders { get; set; }
        public int? LotSelection { get; set; }
        public string LotOverbook { get; set; }
        public string GroupCostAccounting { get; set; }
        public double? ReqForPymtThreshold { get; set; }
        public short? ReqForPymtLastLevel { get; set; }
        public DateTime? ReqForPymtLastDate { get; set; }
        public short? NoOfMaxLevelReqForPymt { get; set; }
        public string UseReqForPymt { get; set; }
        public string NoPrintDueDate { get; set; }
        public string InvoicingCustomer { get; set; }
        public short? Priority { get; set; }
        public string Variant { get; set; }
        public string OneInvoicePerOrder { get; set; }
        public string OneDnperOrder { get; set; }
        public string Shipping { get; set; }
        public double? PenalityPerc { get; set; }
        public string WithholdingTaxManagement { get; set; }
        public double? WithholdingTaxPerc { get; set; }
        public double? WithholdingTaxBasePerc { get; set; }
        public string ExcludedFromWeee { get; set; }
        public int? MaxOrderValueCheckType { get; set; }
        public int? MaxOrderedValueCheckType { get; set; }
        public int? MaximumCreditCheckType { get; set; }
        public DateTime? DeclarationOfIntentDueDate { get; set; }
        public string OneDocumentPerPl { get; set; }
        public string AllocationArea { get; set; }
        public string DirectAllocation { get; set; }
        public string CustomerClassification { get; set; }
        public string CustomerSpecification { get; set; }
        public DateTime? MaxOrderValueDate { get; set; }
        public DateTime? MaxOrderedValueDate { get; set; }
        public DateTime? MaximumCreditDate { get; set; }
        public int? DeclarationOfIntentId { get; set; }
        public string OneReturnFromCustomerPerCn { get; set; }
        public string CrossDocking { get; set; }
        public string ConsignmentPartner { get; set; }
        public string PublicAuthority { get; set; }
        public string Contract { get; set; }
        public DateTime Tbcreated { get; set; }
        public DateTime Tbmodified { get; set; }
        public int TbcreatedId { get; set; }
        public int TbmodifiedId { get; set; }
        public string PasplitPayment { get; set; }
        public string VirtualStampFulfilled { get; set; }
        public string NotUseTd25 { get; set; }
        public string TssendObjection { get; set; }

        public virtual MaCustSupp Cust { get; set; }
    }
}
