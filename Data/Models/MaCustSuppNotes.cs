using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EFMago.Models
{
    public partial class MaCustSuppNotes
    {
        public int CustSuppType { get; set; }
        public string CustSupp { get; set; }
        public short Line { get; set; }
        public string Notes { get; set; }
        public string SaleOrd { get; set; }
        public string PurchaseOrd { get; set; }
        public string Invoice { get; set; }
        public string AccompanyingInvoices { get; set; }
        public string Dn { get; set; }
        public string CreditNote { get; set; }
        public string Receipt { get; set; }
        public string Ncreceipt { get; set; }
        public string CustQuota { get; set; }
        public string ShowInSales { get; set; }
        public string PurchaseDoc { get; set; }
        public string BillOfLading { get; set; }
        public string DebitNote { get; set; }
        public string ShowInPurchases { get; set; }
        public string SuppQuota { get; set; }
        public string Proforma { get; set; }
        public string PickingList { get; set; }
        public string InvoiceForAdv { get; set; }
        public string CorrInvoice { get; set; }
        public string CorrAccInvoice { get; set; }
        public string CorrReceipt { get; set; }
        public string RetCust { get; set; }
        public string PurchInvForAdv { get; set; }
        public string PurchCorrInv { get; set; }
        public string RetSupp { get; set; }
        public string CopyInSales { get; set; }
        public string CopyInPurchases { get; set; }
        public string SubcontrPurchOrder { get; set; }
        public string SubcontrDeliveryNote { get; set; }
        public string SubcontrBillOfLading { get; set; }
        public string ShowInAccounting { get; set; }
        public string ShowInPureAccounting { get; set; }
        public DateTime Tbcreated { get; set; }
        public DateTime Tbmodified { get; set; }
        public int TbcreatedId { get; set; }
        public int TbmodifiedId { get; set; }
        public string Einvoice { get; set; }

        public virtual MaCustSupp CustSuppNavigation { get; set; }
    }
}
