using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EFMago.Models
{
    public partial class MaPaymentTermsPercInstall
    {
        public string Payment { get; set; }
        public short InstallmentNo { get; set; }
        public short? Days { get; set; }
        public double? Perc { get; set; }
        public int? InstallmentType { get; set; }
        public int? DueDateType { get; set; }
        public short? FixedDay { get; set; }
        public int? FixedDayRounding { get; set; }
        public DateTime Tbcreated { get; set; }
        public DateTime Tbmodified { get; set; }
        public int TbcreatedId { get; set; }
        public int TbmodifiedId { get; set; }

        public virtual MaPaymentTerms PaymentNavigation { get; set; }
    }
}
