using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EFMago.Models
{
    public partial class MaCustSuppBlackList
    {
        public int CustSuppType { get; set; }
        public string CustSupp { get; set; }
        public short BalanceYear { get; set; }
        public short BalanceMonth { get; set; }
        public string IsManual { get; set; }
        public string CreditNotesPrevPeriod { get; set; }
        public string CreditNotesPrevYear { get; set; }
        public double? TaxableAmountGoods { get; set; }
        public double? TaxAmountGoods { get; set; }
        public double? TaxableAmountServices { get; set; }
        public double? TaxAmountServices { get; set; }
        public double? NoTaxableGoods { get; set; }
        public double? NoTaxableServices { get; set; }
        public double? Exempt { get; set; }
        public double? NotSubjectGoods { get; set; }
        public double? NotSubjectServices { get; set; }
        public double? NotInBlackListAmount { get; set; }
        public string Grouping { get; set; }
        public Guid? Tbguid { get; set; }
        public DateTime Tbcreated { get; set; }
        public DateTime Tbmodified { get; set; }
        public int TbcreatedId { get; set; }
        public int TbmodifiedId { get; set; }
    }
}
