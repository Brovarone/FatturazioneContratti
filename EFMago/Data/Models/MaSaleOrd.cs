using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EFMago.Models
{
    public partial class MaSaleOrd
    {
        public MaSaleOrd()
        {
            MaSaleOrdComponents = new HashSet<MaSaleOrdComponents>();
            MaSaleOrdDetails = new HashSet<MaSaleOrdDetails>();
            MaSaleOrdPymtSched = new HashSet<MaSaleOrdPymtSched>();
            MaSaleOrdReferences = new HashSet<MaSaleOrdReferences>();
            MaSaleOrdTaxSummary = new HashSet<MaSaleOrdTaxSummary>();
        }

        public string InternalOrdNo { get; set; }
        public string ExternalOrdNo { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public DateTime? ConfirmedDeliveryDate { get; set; }
        public string Customer { get; set; }
        public string Language { get; set; }
        public string OurReference { get; set; }
        public string YourReference { get; set; }
        public string Payment { get; set; }
        public string NonStandardPayment { get; set; }
        public string PriceList { get; set; }
        public string CustomerBank { get; set; }
        public string CompanyBank { get; set; }
        public string SendDocumentsTo { get; set; }
        public string PaymentAddress { get; set; }
        public string NetOfTax { get; set; }
        public string Currency { get; set; }
        public DateTime? FixingDate { get; set; }
        public string FixingIsManual { get; set; }
        public double? Fixing { get; set; }
        public string Area { get; set; }
        public string Salesperson { get; set; }
        public int? AccrualType { get; set; }
        public double? AccrualPercAtInvoiceDate { get; set; }
        public string AreaManager { get; set; }
        // Creato da me Non lo mappo in quanto ntext
        [NotMapped]
        public string Notes { get; set; }
        public string AccTpl { get; set; }
        public string TaxJournal { get; set; }
        public string InvRsn { get; set; }
        public string StubBook { get; set; }
        public string StoragePhase1 { get; set; }
        public string SpecificatorPhase1 { get; set; }
        public string StoragePhase2 { get; set; }
        public string SpecificatorPhase2 { get; set; }
        public string Delivered { get; set; }
        public string Invoiced { get; set; }
        public string PreShipped { get; set; }
        public string Printed { get; set; }
        public string SentByEmail { get; set; }
        public string UseBusinessYear { get; set; }
        public string Cancelled { get; set; }
        public int SaleOrdId { get; set; }
        public string Job { get; set; }
        public string CostCenter { get; set; }
        public string ProductLine { get; set; }
        public string PriceListFromDeliveryDate { get; set; }
        public DateTime? PriceListValidityDate { get; set; }
        public string SingleDelivery { get; set; }
        public string CompanyCa { get; set; }
        public int? Presentation { get; set; }
        public DateTime? CompulsoryDeliveryDate { get; set; }
        public int? LastSubId { get; set; }
        public string InvoicingCustomer { get; set; }
        public short? Priority { get; set; }
        public string ShipToAddress { get; set; }
        public string ActiveSubcontracting { get; set; }
        public string BankAuthorization { get; set; }
        public string InvoiceFollows { get; set; }
        public DateTime? InstallmStartDate { get; set; }
        public string InstallmStartDateIsAuto { get; set; }
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
        public int? CustSuppType { get; set; }
        public int? Specificator1Type { get; set; }
        public int? Specificator2Type { get; set; }
        public string OpenOrder { get; set; }
        public string ContractNo { get; set; }
        public string ShippingReason { get; set; }
        public Guid? Tbguid { get; set; }
        public int? ContractType { get; set; }
        public string AccGroup { get; set; }
        public string Picked { get; set; }
        public string SaleTypeByLine { get; set; }
        public int? SaleType { get; set; }
        public string Allocated { get; set; }
        public string AllocationArea { get; set; }
        public string Carrier1 { get; set; }
        public string ContractCode { get; set; }
        public string ProjectCode { get; set; }
        public string IsBlocked { get; set; }
        public int? BlockType { get; set; }
        public int? UnblockWorker { get; set; }
        public DateTime? UnblockDate { get; set; }
        public string Port { get; set; }
        public string Package { get; set; }
        public string Transport { get; set; }
        public string TaxCommunicationGroup { get; set; }
        public string CompanyPymtCa { get; set; }
        public string SentByPostaLite { get; set; }
        public string Archived { get; set; }
        public int? FromExternalProgram { get; set; }
        public DateTime Tbcreated { get; set; }
        public DateTime Tbmodified { get; set; }
        public int TbcreatedId { get; set; }
        public int TbmodifiedId { get; set; }
        public short? SubIdAttivita { get; set; }
        public short? SubIdContratto { get; set; }
        public short? SubIdDescrizione { get; set; }
        public string ACGCod { get; set; }
        public int? Divisione { get; set; }

        public virtual MaSaleOrdNotes MaSaleOrdNotes { get; set; }
        public virtual MaSaleOrdShipping MaSaleOrdShipping { get; set; }
        public virtual MaSaleOrdSummary MaSaleOrdSummary { get; set; }
        public virtual ICollection<MaSaleOrdComponents> MaSaleOrdComponents { get; set; }
        public virtual ICollection<MaSaleOrdDetails> MaSaleOrdDetails { get; set; }
        public virtual ICollection<MaSaleOrdPymtSched> MaSaleOrdPymtSched { get; set; }
        public virtual ICollection<MaSaleOrdReferences> MaSaleOrdReferences { get; set; }
        public virtual ICollection<MaSaleOrdTaxSummary> MaSaleOrdTaxSummary { get; set; }

        public virtual AllordCliAcc ALLOrdCliAcc { get; set; }
        public virtual AllordCliFattEle ALLOrdCliFattEle { get; set; }
        public virtual ICollection<Allcespiti> ALLCespiti { get; set; }
        public virtual ICollection<AllordCliAttivita> AllordCliAttivita { get; set; }
        public virtual ICollection<AllordCliContratto> ALLordCliContratto { get; set; }
        /// <summary>
        /// Descrizioni testa fattura
        /// </summary>
        public virtual ICollection<AllordCliDescrizioni> ALLordCliDescrizioni { get; set; }
        public virtual ICollection<AllordCliTipologiaServizi> ALLordCliTipologiaServizi { get; set; }
        public virtual ICollection<AllordPadre> AllordPadre { get; set; }
        public virtual ICollection<AllordFiglio> AllordFiglio { get; set; }
        public virtual ICollection<IntegraInterventi> IntegraInterventi { get; set; }        
    }
    public class SaleOrdIdComparer : IEqualityComparer<MaSaleOrd>
    {
        public bool Equals(MaSaleOrd x, MaSaleOrd y)
        {
            return x.SaleOrdId == y.SaleOrdId;
        }

        public int GetHashCode(MaSaleOrd obj)
        {
            return (int)obj.SaleOrdId;
        }
    }
}
