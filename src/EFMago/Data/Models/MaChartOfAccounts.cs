using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EFMago.Models
{
    public partial class MaChartOfAccounts
    {
        public MaChartOfAccounts()
        {
            MaChartOfAccountsBalances = new HashSet<MaChartOfAccountsBalances>();
            MaChartOfAccountsLang = new HashSet<MaChartOfAccountsLang>();
        }

        public string Account { get; set; }
        public string Description { get; set; }
        public int? CodeType { get; set; }
        public string PostableInJe { get; set; }
        public string Ledger { get; set; }
        public string Notes { get; set; }
        public string Disabled { get; set; }
        public string PostableInCostAcc { get; set; }
        public int? DebitCreditSign { get; set; }
        public string UoM { get; set; }
        public int? CostCentersDistribution { get; set; }
        public int? JobsDistribution { get; set; }
        public int? DirectCost { get; set; }
        public int? FullCost { get; set; }
        public string InCurrency { get; set; }
        public string Currency { get; set; }
        public string DeferralsAccount { get; set; }
        public short? DeferralsDays { get; set; }
        public string AccrualsAccount { get; set; }
        public short? AccrualsDays { get; set; }
        public string CostAccAccountGroup { get; set; }
        public Guid? Tbguid { get; set; }
        public int? CashFlowType { get; set; }
        public int? DocToBeIssRecType { get; set; }
        public string OmniasubAccount { get; set; }
        public string OmniaintraCode { get; set; }
        public int? PreferredSignForBalance { get; set; }
        public DateTime Tbcreated { get; set; }
        public DateTime Tbmodified { get; set; }
        public int TbcreatedId { get; set; }
        public int TbmodifiedId { get; set; }
        public string Acgcode { get; set; }

        public virtual ICollection<MaChartOfAccountsBalances> MaChartOfAccountsBalances { get; set; }
        public virtual ICollection<MaChartOfAccountsLang> MaChartOfAccountsLang { get; set; }
    }
}
