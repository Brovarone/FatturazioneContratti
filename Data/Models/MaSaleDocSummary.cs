using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EFMago.Models
{
    public partial class MaSaleDocSummary
    {
        public int SaleDocId { get; set; }
        public double? TaxableAmount { get; set; }
        public double? TaxAmount { get; set; }
        public double? TotalAmount { get; set; }
        public double? GoodsAmount { get; set; }
        public double? ServiceAmounts { get; set; }
        public double? DiscountOnGoods { get; set; }
        public double? DiscountOnServices { get; set; }
        public double? PayableAmount { get; set; }
        public double? FreeSamples { get; set; }
        public double? PayableAmountInBaseCurr { get; set; }
        public double? Discount1 { get; set; }
        public double? Discount2 { get; set; }
        public string DiscountFormula { get; set; }
        public double? Discounts { get; set; }
        public double? Allowances { get; set; }
        public double? Advance { get; set; }
        public double? ReturnedMaterial { get; set; }
        public double? PackagingCharges { get; set; }
        public string PackagingChargesIsAuto { get; set; }
        public double? ShippingCharges { get; set; }
        public string ShippingChargesIsAuto { get; set; }
        public double? StampsCharges { get; set; }
        public string StampsChargesIsAuto { get; set; }
        public double? CollectionCharges { get; set; }
        public string CollectionChargesIsAuto { get; set; }
        public double? AdditionalCharges { get; set; }
        public string AdditionalChargesIsAuto { get; set; }
        public double? CashOnDeliveryPercentage { get; set; }
        public double? CashOnDeliveryCharges { get; set; }
        public string CashOnDeliveryChargesIsAuto { get; set; }
        public double? StatisticalCharges { get; set; }
        public string StatisticalChargesIsAuto { get; set; }
        public double? TotalAmountDocCurr { get; set; }
        public double? DistributedChargesTaxPerc { get; set; }
        public double? FreeSamplesDocCurr { get; set; }
        public string PostAdvancesToAcc { get; set; }
        public string AdvanceOffset { get; set; }
        public double? TaxableAmountDocCurr { get; set; }
        public double? TaxAmountDocCurr { get; set; }
        public string StampsChargesTaxCode { get; set; }
        public string CollectionChargesTaxCode { get; set; }
        public double? PrePayedAdvance { get; set; }
        public double? FreeSamplesTaxAmount { get; set; }
        public string ShippingChargesTaxCode { get; set; }
        public string DiscountsIsAuto { get; set; }
        public string WithholdingTaxManagement { get; set; }
        public string ProfessionalsTaxCode { get; set; }
        public double? ProfessionalsCash { get; set; }
        public string ProfessionalsCashAuto { get; set; }
        public double? WithholdingTax { get; set; }
        public double? Contributions { get; set; }
        public double? WithholdingTaxPerc { get; set; }
        public double? WithholdingTaxBasePerc { get; set; }
        public string CreditNotePreviousPeriod { get; set; }
        public int? PaymentTerm { get; set; }
        public string AmountsWithWhtax { get; set; }
        public int? StatisticalChargesCalc { get; set; }
        public double? GoodsAmountWithTax { get; set; }
        public double? ServiceAmountsWithTax { get; set; }
        public double? Advance2 { get; set; }
        public double? Advance3 { get; set; }
        public string AdvanceOffset2 { get; set; }
        public string AdvanceOffset3 { get; set; }
        public double? EnasarcosalesAmount { get; set; }
        public double? EnasarcosalesPerc { get; set; }
        public DateTime Tbcreated { get; set; }
        public DateTime Tbmodified { get; set; }
        public int TbcreatedId { get; set; }
        public int TbmodifiedId { get; set; }
        public string VirtualStampFulfilled { get; set; }
        public double? PercDiscTaxBreak { get; set; }
        public double? DiscAmountTaxBreak { get; set; }
        public string DiscTaxBreakManual { get; set; }
        public string ReasonDiscTaxBreakCode { get; set; }

        public virtual MaSaleDoc SaleDoc { get; set; }
    }
}
