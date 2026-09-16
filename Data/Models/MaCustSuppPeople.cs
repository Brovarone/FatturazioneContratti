using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EFMago.Models
{
    public partial class MaCustSuppPeople
    {
        public int CustSuppType { get; set; }
        public string CustSupp { get; set; }
        public short Line { get; set; }
        public string TitleCode { get; set; }
        public string Name { get; set; }
        public string Telephone1 { get; set; }
        public string Telephone2 { get; set; }
        public string Telex { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string WorkingPosition { get; set; }
        public string Notes { get; set; }
        public string LastName { get; set; }
        public string LegalRepresentative { get; set; }
        public int? AllDocuments { get; set; }
        public int? SaleOrder { get; set; }
        public int? PurchaseOrder { get; set; }
        public int? Invoice { get; set; }
        public int? AccompanyingInvoices { get; set; }
        public int? DeliveryNote { get; set; }
        public int? CreditNote { get; set; }
        public int? Receipt { get; set; }
        public int? NonCollectedReceipt { get; set; }
        public int? CustomerQuotation { get; set; }
        public int? DebitNote { get; set; }
        public int? SupplierQuotation { get; set; }
        public int? ProformaInvoice { get; set; }
        public int? PickingList { get; set; }
        public int? InvoiceForAdvance { get; set; }
        public int? CorrectionInvoice { get; set; }
        public int? CorrectionAccInvoice { get; set; }
        public int? CorrectionReceipt { get; set; }
        public int? ReturnFromCustomer { get; set; }
        public int? ReturnToSupplier { get; set; }
        public int? SubcontrDeliveryNote { get; set; }
        public string ExternalCode { get; set; }
        public string Branch { get; set; }
        public string SkypeId { get; set; }
        public DateTime Tbcreated { get; set; }
        public DateTime Tbmodified { get; set; }
        public int TbcreatedId { get; set; }
        public int TbmodifiedId { get; set; }

        public virtual MaCustSupp CustSuppNavigation { get; set; }
    }
}
