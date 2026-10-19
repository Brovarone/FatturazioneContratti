using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EFMago.Models
{
    public partial class MaSaleOrdSummary
    {
        public double? TaxableAmount { get; set; }
        public double? TaxAmount { get; set; }
        public double? TotalAmount { get; set; }
        public double? TotalAmountDocCurr { get; set; }
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
        public string PostAdvancesToAcc { get; set; }
        public double? TaxableAmountDocCurr { get; set; }
        public double? TaxAmountDocCurr { get; set; }
        public string StampsChargesTaxCode { get; set; }
        public string CollectionChargesTaxCode { get; set; }
        public int SaleOrdId { get; set; }
        public string ShippingChargesTaxCode { get; set; }
        public double? FreeSamplesDocCurr { get; set; }
        public string DiscountsIsAuto { get; set; }
        public double? GoodsAmountWithTax { get; set; }
        public double? ServiceAmountsWithTax { get; set; }
        public double? InsuranceCharges { get; set; }
        public string InsuranceChargesIsAuto { get; set; }
        public DateTime Tbcreated { get; set; }
        public DateTime Tbmodified { get; set; }
        public int TbcreatedId { get; set; }
        public int TbmodifiedId { get; set; }

        public virtual MaSaleOrd SaleOrd { get; set; }
    }
}
