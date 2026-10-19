using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EFMago.Models
{
    public partial class MaCrossReferences
    {
        public int OriginDocType { get; set; }
        public int OriginDocId { get; set; }
        public int OriginDocSubId { get; set; }
        public short OriginDocLine { get; set; }
        public int DerivedDocType { get; set; }
        public int DerivedDocId { get; set; }
        public int DerivedDocSubId { get; set; }
        public short DerivedDocLine { get; set; }
        public string Manual { get; set; }
        public DateTime Tbcreated { get; set; }
        public DateTime Tbmodified { get; set; }
        public int TbcreatedId { get; set; }
        public int TbmodifiedId { get; set; }
    }
}
