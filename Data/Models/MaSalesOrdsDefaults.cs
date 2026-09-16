using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EFMago.Models
{
    public partial class MaSalesOrdsDefaults
    {
        public int SaleOrdsDefaulstId { get; set; }
        public string SaleOrderAccTpl { get; set; }
        public string EusaleOrderAccTpl { get; set; }
        public string SuspTaxSaleOrderAccTpl { get; set; }
        public string SaleOrderInvRsn { get; set; }
        public string ExtraEusaleOrderAccTpl { get; set; }
        public DateTime Tbcreated { get; set; }
        public DateTime Tbmodified { get; set; }
        public int TbcreatedId { get; set; }
        public int TbmodifiedId { get; set; }
    }
}
