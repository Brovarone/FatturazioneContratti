using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EFMago.Models
{
    public partial class MaItems
    {
        public MaItems()
        {
            MaItemNotes = new HashSet<MaItemNotes>();
            MaItemsComparableUoM = new HashSet<MaItemsComparableUoM>();
            MaItemsFifo = new HashSet<MaItemsFifo>();
            MaItemsFifodomCurr = new HashSet<MaItemsFifodomCurr>();
            MaItemsFiscalData = new HashSet<MaItemsFiscalData>();
            MaItemsFiscalDataDomCurr = new HashSet<MaItemsFiscalDataDomCurr>();
            MaItemsKit = new HashSet<MaItemsKit>();
            MaItemsLanguageDescri = new HashSet<MaItemsLanguageDescri>();
            MaItemsLifo = new HashSet<MaItemsLifo>();
            MaItemsLifodomCurr = new HashSet<MaItemsLifodomCurr>();
            MaItemsMonthlyBalances = new HashSet<MaItemsMonthlyBalances>();
            MaItemsPriceLists = new HashSet<MaItemsPriceLists>();
            MaItemsPurchaseBarCode = new HashSet<MaItemsPurchaseBarCode>();
            MaItemsStorageQty = new HashSet<MaItemsStorageQty>();
            MaItemsStorageQtyMonthly = new HashSet<MaItemsStorageQtyMonthly>();
            MaItemsSubstitute = new HashSet<MaItemsSubstitute>();
            MaItemsWmszones = new HashSet<MaItemsWmszones>();
            MaStandardCostHistorical = new HashSet<MaStandardCostHistorical>();
        }

        public string Item { get; set; }
        public string SaleBarCode { get; set; }
        public string Description { get; set; }
        public string IsGood { get; set; }
        public string TaxCode { get; set; }
        public string BaseUoM { get; set; }
        public double? BasePrice { get; set; }
        public double? Discount1 { get; set; }
        public double? Discount2 { get; set; }
        public string DiscountFormula { get; set; }
        public double? Markup { get; set; }
        public string ItemType { get; set; }
        public string CommodityCtg { get; set; }
        public string HomogeneousCtg { get; set; }
        public string CommissionCtg { get; set; }
        public string SaleOffset { get; set; }
        public string PurchaseOffset { get; set; }
        public DateTime? AvailabilityDate { get; set; }
        public int? SaleType { get; set; }
        public string HasCustomers { get; set; }
        public string HasSuppliers { get; set; }
        public string InternalNote { get; set; }
        public string PublicNote { get; set; }
        public string Producer { get; set; }
        public string UseSerialNo { get; set; }
        public string OldItem { get; set; }
        public string Disabled { get; set; }
        public string ProductCtg { get; set; }
        public string ProductSubCtg { get; set; }
        public string KitExpansion { get; set; }
        public string PostKitComponents { get; set; }
        public string Picture { get; set; }
        public DateTime? StandardCostDate { get; set; }
        public int? Nature { get; set; }
        public string SecondRateUoM { get; set; }
        public string SecondRate { get; set; }
        public int? PurchaseType { get; set; }
        public string ConsuptionOffset { get; set; }
        public string NotPostable { get; set; }
        public double? SalespersonComm { get; set; }
        public string CostCenter { get; set; }
        public string NoUoMsearch { get; set; }
        public string Job { get; set; }
        public string DescriptionText { get; set; }
        public string CanBeDisabled { get; set; }
        public string ProductLine { get; set; }
        public string ShortDescription { get; set; }
        public Guid? Tbguid { get; set; }
        public string BasePriceWithTax { get; set; }
        public string SubjectToWithholdingTax { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? ModificationDate { get; set; }
        public string NoAddDiscountInSaleDoc { get; set; }
        public string BarcodeSegment { get; set; }
        public string ReverseCharge { get; set; }
        public string RctaxCode { get; set; }
        public string Draft { get; set; }
        public string Valorize { get; set; }
        public string RetailCtg { get; set; }
        public string TschargeType { get; set; }
        public string TschargeTypeFlag { get; set; }
        public string Isbn { get; set; }
        public string AuthorCode { get; set; }
        public double? CoverPrice { get; set; }
        public string ItemCodes { get; set; }
        public string EitypeCode { get; set; }
        public string EivalueCode { get; set; }
        public string AdditionalCharge { get; set; }
        public DateTime Tbcreated { get; set; }
        public DateTime Tbmodified { get; set; }
        public int TbcreatedId { get; set; }
        public int TbmodifiedId { get; set; }
        public string ImSubcontractService { get; set; }
        public string ImMacroGroupCode { get; set; }
        public string ImGroupCode { get; set; }
        public string ImSubGroupCode { get; set; }
        public string ImPappDontShow { get; set; }
        public string ImPappAskValue { get; set; }
        public int? Allcadenza { get; set; }
        public int? Allperiodo { get; set; }
        public string AllesplodiInOrdine { get; set; }
        public string AllisCanone { get; set; }
        public string EiadminstrativeRef { get; set; }

        public virtual MaItemsGoodsData MaItemsGoodsData { get; set; }
        public virtual MaItemsIntrastat MaItemsIntrastat { get; set; }
        public virtual MaItemsManufacturingData MaItemsManufacturingData { get; set; }
        public virtual ICollection<MaItemNotes> MaItemNotes { get; set; }
        public virtual ICollection<MaItemsComparableUoM> MaItemsComparableUoM { get; set; }
        public virtual ICollection<MaItemsFifo> MaItemsFifo { get; set; }
        public virtual ICollection<MaItemsFifodomCurr> MaItemsFifodomCurr { get; set; }
        public virtual ICollection<MaItemsFiscalData> MaItemsFiscalData { get; set; }
        public virtual ICollection<MaItemsFiscalDataDomCurr> MaItemsFiscalDataDomCurr { get; set; }
        public virtual ICollection<MaItemsKit> MaItemsKit { get; set; }
        public virtual ICollection<MaItemsLanguageDescri> MaItemsLanguageDescri { get; set; }
        public virtual ICollection<MaItemsLifo> MaItemsLifo { get; set; }
        public virtual ICollection<MaItemsLifodomCurr> MaItemsLifodomCurr { get; set; }
        public virtual ICollection<MaItemsMonthlyBalances> MaItemsMonthlyBalances { get; set; }
        public virtual ICollection<MaItemsPriceLists> MaItemsPriceLists { get; set; }
        public virtual ICollection<MaItemsPurchaseBarCode> MaItemsPurchaseBarCode { get; set; }
        public virtual ICollection<MaItemsStorageQty> MaItemsStorageQty { get; set; }
        public virtual ICollection<MaItemsStorageQtyMonthly> MaItemsStorageQtyMonthly { get; set; }
        public virtual ICollection<MaItemsSubstitute> MaItemsSubstitute { get; set; }
        public virtual ICollection<MaItemsWmszones> MaItemsWmszones { get; set; }
        public virtual ICollection<MaStandardCostHistorical> MaStandardCostHistorical { get; set; }

        // Creato da me
        public virtual ICollection<AllordCliContratto> AllordCliContratto { get; set; }
        public virtual ICollection<AllordCliContrattoDistinta> AllordCliContrattoDistinta { get; set; }
        public virtual ICollection<AllordCliContrattoDistintaServAgg> AllordCliContrattoDistintaServAgg { get; set; }
    }
}
