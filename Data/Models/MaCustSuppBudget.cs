using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EFMago.Models
{
    public partial class MaCustSuppBudget
    {
        public int CustSuppType { get; set; }
        public string CustSupp { get; set; }
        public short FiscalYear { get; set; }
        public short BalanceYear { get; set; }
        public int BalanceType { get; set; }
        public short BalanceMonth { get; set; }
        public double? TurnoverBudget { get; set; }
        public double? OrderedBudget { get; set; }
        public double? ActualTurnover { get; set; }
        public double? ActualOrdered { get; set; }
        public double? ActualCreditNotesAmount { get; set; }
        public double? ActualDebitNotesAmount { get; set; }
        public DateTime Tbcreated { get; set; }
        public DateTime Tbmodified { get; set; }
        public int TbcreatedId { get; set; }
        public int TbmodifiedId { get; set; }

        public virtual MaCustSupp CustSuppNavigation { get; set; }
    }
}
