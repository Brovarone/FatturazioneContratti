using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EFMago.Models
{
    public partial class MaTaxCodesLists
    {
        public string TaxCode { get; set; }
        public string CustListColumn { get; set; }
        public string SuppListColumn { get; set; }
        public string CustListColumn1 { get; set; }
        public string SuppListColumn1 { get; set; }
        public string ExtraEupurchases { get; set; }
        public string Eupurchases { get; set; }
        public int? CustListType { get; set; }
        public int? SuppListType { get; set; }
        public DateTime Tbcreated { get; set; }
        public DateTime Tbmodified { get; set; }
        public int TbcreatedId { get; set; }
        public int TbmodifiedId { get; set; }

        public virtual MaTaxCodes TaxCodeNavigation { get; set; }
    }
}
