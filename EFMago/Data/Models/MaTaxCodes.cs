using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EFMago.Models
{
    public partial class MaTaxCodes
    {
        public MaTaxCodes()
        {
            MaTaxCodesLang = new HashSet<MaTaxCodesLang>();
        }

        public string TaxCode { get; set; }
        public string Description { get; set; }
        public double? Perc { get; set; }
        public double? UndeductiblePerc { get; set; }
        public string DistributionPerc { get; set; }
        public string InProRataTurnover { get; set; }
        public string InPlafondTurnover { get; set; }
        public string InExportPlafond { get; set; }
        public string ProRataExempt { get; set; }
        public int? PlafondType { get; set; }
        public string ExemptInvoice { get; set; }
        public string Notes { get; set; }
        public string Disabled { get; set; }
        public string NoChargesDistribution { get; set; }
        public string NoTaxableAmount { get; set; }
        public string NoIntrastat { get; set; }
        public double? FarmerTaxPerc { get; set; }
        public string Exempt { get; set; }
        public string NonTaxable { get; set; }
        public string LetterForFiscalPrinter { get; set; }
        public string TravelAgencyVat { get; set; }
        public string TravelExtraUe { get; set; }
        public Guid? Tbguid { get; set; }
        public string ReverseCharge { get; set; }
        public int? PurchaseType { get; set; }
        public string BuyerObligedToPayTax { get; set; }
        public string UseSecondLumpSumRate { get; set; }
        public string FixedAssets { get; set; }
        public string BlackListExempt { get; set; }
        public string BlackListNonTaxable { get; set; }
        public string NotInBlackList { get; set; }
        public string Gold { get; set; }
        public string Scrap { get; set; }
        public string OmnialawCode { get; set; }
        public string OmniataxCode { get; set; }
        public string Eicode { get; set; }
        public DateTime Tbcreated { get; set; }
        public DateTime Tbmodified { get; set; }
        public int TbcreatedId { get; set; }
        public int TbmodifiedId { get; set; }
        public string SalesNoTaxableAmount { get; set; }
        public string PurchasesNoTaxableAmount { get; set; }
        public string EisubCode { get; set; }
        public string Acgcode { get; set; }
        public string EidataType { get; set; }
        public string EitextReference { get; set; }

        public virtual MaTaxCodesLists MaTaxCodesLists { get; set; }
        public virtual ICollection<MaTaxCodesLang> MaTaxCodesLang { get; set; }
    }
}
