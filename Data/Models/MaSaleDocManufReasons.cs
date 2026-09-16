using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EFMago.Models
{
    public partial class MaSaleDocManufReasons
    {
        public int SaleDocId { get; set; }
        public string RmclearingInvRsn { get; set; }
        public string RmclearingStorage1 { get; set; }
        public string RmclearingStorage2 { get; set; }
        public string RmclearingSpecificator1 { get; set; }
        public string RmclearingSpecificator2 { get; set; }
        public string FpissueToProdInvRsn { get; set; }
        public string FpissueToProdStorage1 { get; set; }
        public string FpissueToProdStorage2 { get; set; }
        public string FpissueToProdSpecificator1 { get; set; }
        public string FpissueToProdSpecificator2 { get; set; }
        public string RmclearingProdInvRsn { get; set; }
        public string RmclearingProdStorage1 { get; set; }
        public string RmclearingProdStorage2 { get; set; }
        public string RmclearingProdSpecificator1 { get; set; }
        public string RmclearingProdSpecificator2 { get; set; }
        public string FpreceiptInvRsn { get; set; }
        public string FpreceiptStorage1 { get; set; }
        public string FpreceiptStorage2 { get; set; }
        public string FpreceiptSpecificator1 { get; set; }
        public string FpreceiptSpecificator2 { get; set; }
        public string WasteLoadInvRsn { get; set; }
        public string WasteLoadStorage1 { get; set; }
        public string WasteLoadStorage2 { get; set; }
        public string WasteLoadSpecificator1 { get; set; }
        public string WasteLoadSpecificator2 { get; set; }
        public int? RmclearingSpec1Type { get; set; }
        public int? RmclearingSpec2Type { get; set; }
        public int? FpissueToProdSpec1Type { get; set; }
        public int? FpissueToProdSpec2Type { get; set; }
        public int? RmclearingProdSpec1Type { get; set; }
        public int? RmclearingProdSpec2Type { get; set; }
        public int? FpreceiptSpec1Type { get; set; }
        public int? FpreceiptSpec2Type { get; set; }
        public int? WasteLoadSpec1Type { get; set; }
        public int? WasteLoadSpec2Type { get; set; }
        public string WasteDiffItemLoadInvRsn { get; set; }
        public string WasteDiffItemLoadStorage1 { get; set; }
        public string WasteDiffItemLoadStorage2 { get; set; }
        public string WasteDiffItemLoadSpec1 { get; set; }
        public string WasteDiffItemLoadSpec2 { get; set; }
        public int? WasteDiffItemLoadSpec1Type { get; set; }
        public int? WasteDiffItemLoadSpec2Type { get; set; }
        public DateTime Tbcreated { get; set; }
        public DateTime Tbmodified { get; set; }
        public int TbcreatedId { get; set; }
        public int TbmodifiedId { get; set; }

        public virtual MaSaleDoc SaleDoc { get; set; }
    }
}
