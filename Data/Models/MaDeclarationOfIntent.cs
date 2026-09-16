using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EFMago.Models
{
    public partial class MaDeclarationOfIntent
    {
        public int? CustSuppType { get; set; }
        public string CustSupp { get; set; }
        public short? DeclYear { get; set; }
        public DateTime? DeclDate { get; set; }
        public string LogNo { get; set; }
        public string CustomerNo { get; set; }
        public DateTime? CustomerDate { get; set; }
        public int? DeclType { get; set; }
        public double? LimitAmount { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string Printed { get; set; }
        public DateTime? PrintDate { get; set; }
        public string PrintedOnFile { get; set; }
        public DateTime? PrintFileDate { get; set; }
        public string PrintedLetter { get; set; }
        public DateTime? PrintLetterDate { get; set; }
        public string Notes { get; set; }
        public DateTime? AnnulmentDate { get; set; }
        public int DeclId { get; set; }
        public Guid? Tbguid { get; set; }
        public string LetterNotes { get; set; }
        public string PrintedAnnulment { get; set; }
        public DateTime? PrintAnnulmentDate { get; set; }
        public string TelProtocol { get; set; }
        public string DocProtocol { get; set; }
        public DateTime Tbcreated { get; set; }
        public DateTime Tbmodified { get; set; }
        public int TbcreatedId { get; set; }
        public int TbmodifiedId { get; set; }
    }
}
