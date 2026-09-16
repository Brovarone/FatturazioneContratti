using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EFMago.Models
{
    public partial class MaSaleOrdDetails
    {
        public short Line { get; set; }
        public short Position { get; set; }
        public int? LineType { get; set; }
        public string Description { get; set; }
        public string NoPrint { get; set; }
        public string NoDn { get; set; }
        public string NoInvoice { get; set; }
        public string Item { get; set; }
        public string Variant { get; set; }
        public string Drawing { get; set; }
        public string Customer { get; set; }
        public DateTime? OrderDate { get; set; }
        public string Department { get; set; }
        public string UoM { get; set; }
        public double? Qty { get; set; }
        public double? AdditionalQty1 { get; set; }
        public double? AdditionalQty2 { get; set; }
        public double? AdditionalQty3 { get; set; }
        public double? AdditionalQty { get; set; }
        public double? UnitValue { get; set; }
        public double? TaxableAmount { get; set; }
        public string TaxCode { get; set; }
        public double? TotalAmount { get; set; }
        public double? Discount1 { get; set; }
        public double? Discount2 { get; set; }
        public string DiscountFormula { get; set; }
        public double? DiscountAmount { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public DateTime? ConfirmedDeliveryDate { get; set; }
        public double? DeliveredQty { get; set; }
        public double? InvoicedQty { get; set; }
        public double? PreShippedQty { get; set; }
        public int? SaleType { get; set; }
        public short? NoOfPacks { get; set; }
        public string PacksUoM { get; set; }
        public double? GrossWeight { get; set; }
        public double? NetWeight { get; set; }
        public double? GrossVolume { get; set; }
        public string Delivered { get; set; }
        public string Invoiced { get; set; }
        public string PreShipped { get; set; }
        public string Cancelled { get; set; }
        public int SaleOrdId { get; set; }
        public short KitNo { get; set; }
        public double? KitQty { get; set; }
        public string Job { get; set; }
        public string CostCenter { get; set; }
        public string ProductLine { get; set; }
        public double? CommPerc { get; set; }
        public double? AreaManagerCommPerc { get; set; }
        public double? SalespersonDiscount { get; set; }
        public string CommissionCtg { get; set; }
        public int? ProductionPlanId { get; set; }
        public string ProductionPlanNo { get; set; }
        public short? ProductionPlanLine { get; set; }
        public string Offset { get; set; }
        public double? BookedQty { get; set; }
        public double? SalespersonComm { get; set; }
        public int? SubId { get; set; }
        public string Bomitem { get; set; }
        public string StoragePhase1 { get; set; }
        public string StoragePhase2 { get; set; }
        public string SpecificatorPhase1 { get; set; }
        public string SpecificatorPhase2 { get; set; }
        public short? ExternalLineReference { get; set; }
        public string CustomerLotNo { get; set; }
        public int? ProductionJobId { get; set; }
        public string ProductionJobNo { get; set; }
        public int? ProductionJobSubId { get; set; }
        public int? ReferenceDocId { get; set; }
        public string ReferenceDocNo { get; set; }
        public int? ReferenceQuotationId { get; set; }
        public int? ReferenceQuotationSubId { get; set; }
        public string ReferenceQuotation { get; set; }
        public string Notes { get; set; }
        public double? BaseSalesperson { get; set; }
        public double? BaseAreaManager { get; set; }
        public double? AreaManagerComm { get; set; }
        public string AreaManagerCommCtg { get; set; }
        public string CommPercAuto { get; set; }
        public string AreaManagerCommPercAuto { get; set; }
        public string SalespersonCommAuto { get; set; }
        public string AreaManagerCommAuto { get; set; }
        public string SalespersonCommCtgAuto { get; set; }
        public string AreaManagerCommCtgAuto { get; set; }
        public string ConfirmationLevel { get; set; }
        public string CustomerCode { get; set; }
        public string PriceList { get; set; }
        public string Picked { get; set; }
        public double? PickedQty { get; set; }
        public double? PickedAndDeliveredQty { get; set; }
        public string Allocated { get; set; }
        public double? AllocatedQty { get; set; }
        public string InternalOrdNo { get; set; }
        public double? PreShippedAndDeliveredQty { get; set; }
        public short? ReferredPosition { get; set; }
        public int? CrrefType { get; set; }
        public int? CrrefId { get; set; }
        public int? CrrefSubId { get; set; }
        public string CrossDocking { get; set; }
        public double? NetPrice { get; set; }
        public string NetPriceIsAuto { get; set; }
        public string ContractCode { get; set; }
        public string ProjectCode { get; set; }
        public DateTime Tbcreated { get; set; }
        public DateTime Tbmodified { get; set; }
        public int TbcreatedId { get; set; }
        public int TbmodifiedId { get; set; }
        public string InEi { get; set; }
        public DateTime? AllCanoniDataI { get; set; }
        public DateTime? AllCanoniDataF { get; set; }
        public double? AllNrCanoni { get; set; }

        public virtual MaSaleOrd SaleOrd { get; set; }
    }
}
