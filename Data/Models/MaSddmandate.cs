using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EFMago.Models
{
    public partial class MaSddmandate
    {
        public string MandateCode { get; set; }
        public DateTime? MandateDate { get; set; }
        public string MandateOneOff { get; set; }
        public DateTime? MandateFirstDate { get; set; }
        public DateTime? MandateLastDate { get; set; }
        public string Customer { get; set; }
        public string CustomerBank { get; set; }
        public string CustomerCa { get; set; }
        public string CustomerIban { get; set; }
        public string CustomerIbanisManual { get; set; }
        public string Notes { get; set; }
        public string Ddmandate { get; set; }
        public int? MandateType { get; set; }
        public string Draft { get; set; }
        public string Umrcode { get; set; }
        public string Printed { get; set; }
        public DateTime? PrintDate { get; set; }
        public DateTime Tbcreated { get; set; }
        public DateTime Tbmodified { get; set; }
        public int TbcreatedId { get; set; }
        public int TbmodifiedId { get; set; }
    }
}
