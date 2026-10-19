using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EFMago.Models
{
    public partial class MaEiItparameters
    {
        public int ParameterId { get; set; }
        public string TaxJournal { get; set; }
        public string TaxJournalCreditNote { get; set; }
        public string ContractProjectFromOrders { get; set; }
        public string ContractProjectFromJobs { get; set; }
        public string SetItemCode { get; set; }
        public string ItemCodeType { get; set; }
        public string Link { get; set; }
        public string ManualRefManagement { get; set; }
        public string OrderLineNumberReference { get; set; }
        public string AlwaysReportSalesOrderData { get; set; }
        public string StampChargesInSummary { get; set; }
        public DateTime Tbcreated { get; set; }
        public DateTime Tbmodified { get; set; }
        public int TbcreatedId { get; set; }
        public int TbmodifiedId { get; set; }
        public string UseFatel { get; set; }
        public string UseFatelWeb { get; set; }
        public string UseCadi { get; set; }
        public string UseCadiWeb { get; set; }
        public string Xmlpath { get; set; }
        public string IncludeEiinTaxDoc { get; set; }
        public short? MaxNoLineInTaxDoc { get; set; }
        public string ExcludedSummaryDocuments { get; set; }
        public string StdAswPa { get; set; }
        public string StdAswB2b { get; set; }
        public int? RegisterReceivedEi { get; set; }
        public string ReceivedPath { get; set; }
        public string ImportedPath { get; set; }
        public string NotCompliantPath { get; set; }
        public string TaxJournalB2b { get; set; }
        public string TaxJournalCreditNoteB2b { get; set; }
        public string StdAswUseExternalIdNo { get; set; }
        public string UseFeeDocumentType { get; set; }
        public string UseKitDescrLine { get; set; }
        public string DescrLinesInEi { get; set; }
        public string NoteLinesInEi { get; set; }
        public string RefLinesInEi { get; set; }
        public string UseNetPrice { get; set; }
        public string EmailHost { get; set; }
        public int? EmailPort { get; set; }
        public string EmailEnableSsl { get; set; }
        public string EmailUser { get; set; }
        public string EmailPassword { get; set; }
        public string EmailAddress { get; set; }
        public string TruncateCustomersData { get; set; }
        public string StampNote { get; set; }
        public int? StylesheetSndDoc { get; set; }
        public int? StylesheetRcvDoc { get; set; }
        public DateTime? ValidityDateNewSpecs { get; set; }
        public int? EnasarcopaymentReason { get; set; }
        public int? EidocumentTypeInDninvoicing { get; set; }
    }
}
