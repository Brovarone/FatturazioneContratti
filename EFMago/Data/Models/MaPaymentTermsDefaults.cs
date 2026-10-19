using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EFMago.Models
{
    public partial class MaPaymentTermsDefaults
    {
        public int PaymentTerm { get; set; }
        public string PaymentAccTpl { get; set; }
        public string CollectionAccTpl { get; set; }
        public string PaymentAccRsn { get; set; }
        public string CollectionAccRsn { get; set; }
        public int PaymentTermsDefaultsId { get; set; }
        public DateTime Tbcreated { get; set; }
        public DateTime Tbmodified { get; set; }
        public int TbcreatedId { get; set; }
        public int TbmodifiedId { get; set; }
    }
}
