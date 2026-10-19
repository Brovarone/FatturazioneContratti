using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EFMago.Models
{
    public partial class MaEiItdocAdditionalData
    {
        public int DocId { get; set; }
        public int DocSubId { get; set; }
        public int Line { get; set; }
        public int SubLine { get; set; }
        public string FieldName { get; set; }
        public string FieldValue { get; set; }
        public DateTime Tbcreated { get; set; }
        public DateTime Tbmodified { get; set; }
        public int TbcreatedId { get; set; }
        public int TbmodifiedId { get; set; }
    }
}
