using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EFMago.Models
{
    public partial class MaPaymentTerms
    {
        public MaPaymentTerms()
        {
            MaPaymentTermsLang = new HashSet<MaPaymentTermsLang>();
            MaPaymentTermsPercInstall = new HashSet<MaPaymentTermsPercInstall>();
        }

        public string Payment { get; set; }
        public string Description { get; set; }
        public int? InstallmentType { get; set; }
        public short? NoOfInstallments { get; set; }
        public short? FirstPaymentDays { get; set; }
        public short? FixedDay { get; set; }
        public int? FixedDayRounding { get; set; }
        public short? DaysBetweenInstallments { get; set; }
        public int? DueDateType { get; set; }
        public short? ExcludedMonth1 { get; set; }
        public short? ExcludedMonth2 { get; set; }
        public short? DeferredDayMonth1 { get; set; }
        public short? DeferredDayMonth2 { get; set; }
        public string AtSight { get; set; }
        public string BusinessYear { get; set; }
        public int? TaxInstallment { get; set; }
        public double? CollectionCharges { get; set; }
        public double? Discount1 { get; set; }
        public double? Discount2 { get; set; }
        public string DiscountFormula { get; set; }
        public string Notes { get; set; }
        public string Disabled { get; set; }
        public string PercInstallment { get; set; }
        public string CreditCard { get; set; }
        public Guid? Tbguid { get; set; }
        public int? IntrastatCollectionType { get; set; }
        public string DeferredDayMonth1Same { get; set; }
        public string DeferredDayMonth2Same { get; set; }
        public string Offset { get; set; }
        public string WorkingDays { get; set; }
        public string PymtCash { get; set; }
        public string SettingsOnPercInstallment { get; set; }
        public DateTime Tbcreated { get; set; }
        public DateTime Tbmodified { get; set; }
        public int TbcreatedId { get; set; }
        public int TbmodifiedId { get; set; }
        public string Acgcode { get; set; }
        public string Spacode { get; set; }

        public virtual ICollection<MaPaymentTermsLang> MaPaymentTermsLang { get; set; }
        public virtual ICollection<MaPaymentTermsPercInstall> MaPaymentTermsPercInstall { get; set; }
    }
}
