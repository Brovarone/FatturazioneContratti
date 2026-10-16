using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EFMago.Models
{
    public partial class MaSaleParameters
    {
        public int SaleParametersId { get; set; }
        public string UseCustomerExemption { get; set; }
        public string ZeroAmountInvoice { get; set; }
        public string InvoiceClearsInventory { get; set; }
        public string BalanceEutaxSummaryDocCurr { get; set; }
        public string GeneratePymtSchedInBaseCurr { get; set; }
        public string ServicesInDn { get; set; }
        public string ShowSf { get; set; }
        public string DescriptiveLinesAreDelivered { get; set; }
        public string KitComponentsPricePrompt { get; set; }
        public string UseAdditionalSupplierCharges { get; set; }
        public string WarnOnCustomerType { get; set; }
        public string WarnNoPrintedLine { get; set; }
        public string DnupdateData { get; set; }
        public string UseCustomerOffset { get; set; }
        public double? ExemptThreshold { get; set; }
        public double? StampThreshold { get; set; }
        public string DeferredInvoiceByArea { get; set; }
        public string DeferredInvoiceByInvRsn { get; set; }
        public string DeferredInvoiceByDocBranch { get; set; }
        public string DeferredInvoiceByGoodBranch { get; set; }
        public string DeferredInvoiceByJob { get; set; }
        public string DeferredInvoiceByShippingRsn { get; set; }
        public string DeferredInvoiceByPymtTerm { get; set; }
        public string DeferredInvoiceByDiscounts { get; set; }
        public string DeferredInvoiceByCig { get; set; }
        public string DeferredInvoiceByTaxCommGroup { get; set; }
        public double? BillOfExchStamps { get; set; }
        public double? ForeignBillOfExchStamps { get; set; }
        public double? BillOfExchStampsRounding { get; set; }
        public double? BillOfExchStampsMin { get; set; }
        public string NoDeferredInvoice { get; set; }
        public string BlockCustomers { get; set; }
        public string UseOrderPort { get; set; }
        public string FinalDiscountIncluded { get; set; }
        public string IncludeCharges { get; set; }
        public int? ReferencesPrintType { get; set; }
        public int? ShortageCheckType { get; set; }
        public int? CombNomenclatureCheckType { get; set; }
        public string UseDndateInDeferredInvoice { get; set; }
        public int? ReturnFromCustomerQtyCheck { get; set; }
        public int? InvoiceQtyCheck { get; set; }
        public string PrintParagonOnFiscalPrinter { get; set; }
        public int? ClearingDncheck { get; set; }
        public string PureJecollectionPaymentMng { get; set; }
        public string NegativeValueInLedgerCard { get; set; }
        public int? DisplayItemsInRadar { get; set; }
        public int? SalesShortageCheckType { get; set; }
        public string PrintNotOwnedStorageTransfIn { get; set; }
        public int? DefInterStorageDocumentType { get; set; }
        public string SetLastDndateInInvoice { get; set; }
        public int? FreeBorderDefault { get; set; }
        public string TaxexigibilityOnCollectNote { get; set; }
        public string ProFormaUnloadInventory { get; set; }
        public string OrderFullCustPrec { get; set; }
        public int? SalesScarcityCheckType { get; set; }
        public string VatonFreeSamplesForEucustomers { get; set; }
        public string InvoicingOfPerishables { get; set; }
        public string PaymentPeriShablesWithin60 { get; set; }
        public string PaymentPeriShablesOver60 { get; set; }
        public string PerishablesNotes { get; set; }
        public string TaxexigibilityCashRegimeNote { get; set; }
        public string CheckPerishables { get; set; }
        public string SplitPaymentNoteEnabled { get; set; }
        public string SplitPaymentNote { get; set; }
        public string InvoiceToCustomerBank { get; set; }
        public string SaleOrderRowFulfillment { get; set; }
        public DateTime Tbcreated { get; set; }
        public DateTime Tbmodified { get; set; }
        public int TbcreatedId { get; set; }
        public int TbmodifiedId { get; set; }
        public string TaxexigibilityOnCollectNoteEi { get; set; }
        public string SplitPaymentNoteEnabledEi { get; set; }
        public string PerishablesNotesEi { get; set; }
    }
}
