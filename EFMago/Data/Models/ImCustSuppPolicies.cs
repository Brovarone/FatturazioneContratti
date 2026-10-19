using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EFMago.Models
{
    public partial class ImCustSuppPolicies
    {
        public int CustSuppType { get; set; }
        public string CustSupp { get; set; }
        public string Policy { get; set; }
        public int? InvoiceType { get; set; }
        public int? SoarefType { get; set; }
        public int? InvoiceRefType { get; set; }
        public string SoawithDescriptions { get; set; }
        public DateTime Tbcreated { get; set; }
        public DateTime Tbmodified { get; set; }
        public int TbcreatedId { get; set; }
        public int TbmodifiedId { get; set; }
        public string WrbyEmail { get; set; }
        public string EmailTo { get; set; }
        public string EmailCc { get; set; }
        public string EmailBcc { get; set; }

        public virtual MaCustSupp CustSuppNavigation { get; set; }
    }
}
