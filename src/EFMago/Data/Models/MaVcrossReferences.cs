using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EFMago.Models
{
    public partial class MaVcrossReferences
    {
        public int OriginDocType { get; set; }
        public int OriginDocId { get; set; }
        public int OriginDocSubId { get; set; }
        public int DerivedDocType { get; set; }
        public int DerivedDocId { get; set; }
        public int DerivedDocSubId { get; set; }
        public DateTime? OriginDocDate { get; set; }
        public string OriginDocNo { get; set; }
        public DateTime? DerivedDocDate { get; set; }
        public string DerivedDocNo { get; set; }
        public string Item { get; set; }
    }
}
