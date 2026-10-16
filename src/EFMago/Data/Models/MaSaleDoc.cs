using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EFMago.Models
{
    public partial class MaSaleDoc
    {
        public MaSaleDoc()
        {            
            MaSaleDocComponents = new HashSet<MaSaleDocComponents>();
            MaSaleDocDetail = new HashSet<MaSaleDocDetail>();
            MaSaleDocPymtSched = new HashSet<MaSaleDocPymtSched>();
            MaSaleDocReferences = new HashSet<MaSaleDocReferences>();
            MaSaleDocTaxSummary = new HashSet<MaSaleDocTaxSummary>();
        }

        public int? DocumentType { get; set; }
        public string DocNo { get; set; }
        public DateTime? DocumentDate { get; set; }
        public int? CustSuppType { get; set; }
        public string CustSupp { get; set; }
        public string Language { get; set; }
        public string OurReference { get; set; }
        public string YourReference { get; set; }
        public string Payment { get; set; }
        public DateTime? InstallmStartDate { get; set; }
        public string InstallmStartDateIsAuto { get; set; }
        public string PriceList { get; set; }
        public DateTime? PostingDate { get; set; }
        public string CustomerBank { get; set; }
        public string CompanyBank { get; set; }
        public string SendDocumentsTo { get; set; }
        public string PaymentAddress { get; set; }
        public string NetOfTax { get; set; }
        public string Summarized { get; set; }
        public string InvoiceFollows { get; set; }
        public string Printed { get; set; }
        public string SentByEmail { get; set; }
        public string InvoicingAccTpl { get; set; }
        public string InvoicingTaxJournal { get; set; }
        public string Currency { get; set; }
        public DateTime? FixingDate { get; set; }
        public string FixingIsManual { get; set; }
        public double? Fixing { get; set; }
        public string Area { get; set; }
        public string Salesperson { get; set; }
        public int? AccrualType { get; set; }
        public double? AccrualPercAtInvoiceDate { get; set; }
        public string AreaManager { get; set; }
        public string AccTpl { get; set; }
        public string TaxJournal { get; set; }
        public string PostedToAccounting { get; set; }
        public string PostedToCommissionEntries { get; set; }
        public string PostedToInventory { get; set; }
        public string Issued { get; set; }
        public string PostedToIntrastat { get; set; }
        public string PostedToPyblsRcvbls { get; set; }
        public string Notes { get; set; }
        public int SaleDocId { get; set; }
        public string InvRsn { get; set; }
        public string StubBook { get; set; }
        public string StoragePhase1 { get; set; }
        public string SpecificatorPhase1 { get; set; }
        public string StoragePhase2 { get; set; }
        public string SpecificatorPhase2 { get; set; }
        public string PreprintedDocNo { get; set; }
        public int? PymtSchedId { get; set; }
        public int? JournalEntryId { get; set; }
        public int? IntrastatId { get; set; }
        public int? NatureOfTransaction { get; set; }
        public int? DeliveryTerms { get; set; }
        public int? ModeOfTransport { get; set; }
        public string CountryOfDestination { get; set; }
        public int? Operation { get; set; }
        public string IncludedInTurnover { get; set; }
        public string Job { get; set; }
        public string PostedToCostAccounting { get; set; }
        public string CostCenter { get; set; }
        public string ProductLine { get; set; }
        public DateTime? DepartureDate { get; set; }
        public string CompanyCa { get; set; }
        public int? Presentation { get; set; }
        public int? InvEntryId { get; set; }
        public string ShipToAddress { get; set; }
        public string InvoiceTypes { get; set; }
        public DateTime? ValueDate { get; set; }
        public string NoChangeExigibility { get; set; }
        public int? LastSubId { get; set; }
        public int? LastSubIdPymtSched { get; set; }
        public string InvoicingCustomer { get; set; }
        public int? AdvancePymtSchedId { get; set; }
        public string BankAuthorization { get; set; }
        public DateTime? IntrastatAccrualDate { get; set; }
        public double? SalespersonCommTot { get; set; }
        public double? AreaManagerCommTot { get; set; }
        public double? BaseSalesperson { get; set; }
        public double? BaseAreaManager { get; set; }
        public double? SalespersonCommPerc { get; set; }
        public double? AreaManagerCommPerc { get; set; }
        public string SalespersonCommAuto { get; set; }
        public string AreaManagerCommAuto { get; set; }
        public string SalespersonCommPercAuto { get; set; }
        public string AreaManagerCommPercAuto { get; set; }
        public string SalespersonPolicy { get; set; }
        public string AreaManagerPolicy { get; set; }
        public int? Specificator1Type { get; set; }
        public int? Specificator2Type { get; set; }
        public string IntrastatBis { get; set; }
        public string IntrastatTer { get; set; }
        public string ShippingReason { get; set; }
        public string CorrectionDocument { get; set; }
        public Guid? Tbguid { get; set; }
        public int? CorrectionDocumentId { get; set; }
        public int? CorrectedDocumentId { get; set; }
        public string IsParagon { get; set; }
        public string FiscalPrinted { get; set; }
        public string CorrectionDocNo { get; set; }
        public DateTime? CorrectionDocumentDate { get; set; }
        public string InvRsnReturn { get; set; }
        public string StoragePhase1Return { get; set; }
        public int? SpecificatorTypePhase1Return { get; set; }
        public string SpecificatorPhase1Return { get; set; }
        public string StoragePhase2Return { get; set; }
        public int? SpecificatorTypePhase2Return { get; set; }
        public string SpecificatorPhase2Return { get; set; }
        public string PostedToInventoryReturn { get; set; }
        public int? InventoryIdreturn { get; set; }
        public int? ParagonId { get; set; }
        public string InvoiceForAdvanceLinked { get; set; }
        public string ProFormaInvoiceLinked { get; set; }
        public string ProFormaDdtlinked { get; set; }
        public int? ProFormaInvoiceId { get; set; }
        public int? PureJecollectionPaymentId { get; set; }
        public string PureJecollectionPaymentNo { get; set; }
        public string InvoicingAccGroup { get; set; }
        public string CorrectionForReturn { get; set; }
        public string DocumentCorrectionInCn { get; set; }
        public int? CorrectionDocumentIdInCn { get; set; }
        public int? WorkerIdissue { get; set; }
        public int? InterStorageDocumentType { get; set; }
        public int? StatisticalPurpose { get; set; }
        public string CountryOfTransport { get; set; }
        public string Delivered { get; set; }
        public string DiscountFormula { get; set; }
        public string CountryOfPayment { get; set; }
        public string Triangulation { get; set; }
        public int? ActionOnLifoFifo { get; set; }
        public string Carrier1 { get; set; }
        public string ModifyOriginalPymtSched { get; set; }
        public string ContractCode { get; set; }
        public string ProjectCode { get; set; }
        public DateTime? AccrualDate { get; set; }
        public DateTime? TaxAccrualDate { get; set; }
        public int? PureJetaxTransferId { get; set; }
        public int? TaxCommunicationOperation { get; set; }
        public string TaxCommunicationGroup { get; set; }
        public string Port { get; set; }
        public string Package { get; set; }
        public string Transport { get; set; }
        public string CompanyPymtCa { get; set; }
        public string StorageReplenishment { get; set; }
        public int? SpecTypeReplenishment { get; set; }
        public string SpecificatorReplenishment { get; set; }
        public string ReceiptIsm { get; set; }
        public int? ReceiptIsmid { get; set; }
        public string ReceiptIsmreason { get; set; }
        public string SentByPostaLite { get; set; }
        public int? PerishablesType { get; set; }
        public string CustomerDocNo { get; set; }
        public DateTime? CustomerDocumentDate { get; set; }
        public string SplitPaymentActive { get; set; }
        public string Archived { get; set; }
        public int? FromExternalProgram { get; set; }
        public int? Eistatus { get; set; }
        public string EiprogTransmission { get; set; }
        public string Sosdone { get; set; }
        public int? ExtAccAeid { get; set; }
        public string GenerateEat { get; set; }
        public DateTime Tbcreated { get; set; }
        public DateTime Tbmodified { get; set; }
        public int TbcreatedId { get; set; }
        public int TbmodifiedId { get; set; }
        public int? EidocumentType { get; set; }
        public string AllUmrcode { get; set; }
        public string AllIban { get; set; }
        public short? Priority { get; set; }
        
        public virtual MaSaleDocManufReasons MaSaleDocManufReasons { get; set; }
        public virtual MaSaleDocNotes MaSaleDocNotes { get; set; }
        public virtual MaSaleDocShipping MaSaleDocShipping { get; set; }
        public virtual MaSaleDocSummary MaSaleDocSummary { get; set; }        
        public virtual ICollection<MaSaleDocComponents> MaSaleDocComponents { get; set; }
        public virtual ICollection<MaSaleDocDetail> MaSaleDocDetail { get; set; }
        public virtual ICollection<MaSaleDocPymtSched> MaSaleDocPymtSched { get; set; }
        public virtual ICollection<MaSaleDocReferences> MaSaleDocReferences { get; set; }
        public virtual ICollection<MaSaleDocTaxSummary> MaSaleDocTaxSummary { get; set; }
    }
}
