using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EFMago.Models
{
    public partial class MaSalesOrdParameters
    {
        public int SaleOrdParametersId { get; set; }
        public string FulfillmentBreakByArea { get; set; }
        public string FulfillmentBreakByInvRsn { get; set; }
        public string FulfillmentBreakByDocBranch { get; set; }
        public string FulfillmentBreakByGoodBranch { get; set; }
        public string FulfillmentBreakByJob { get; set; }
        public string FulfillmentBreakByShippRsn { get; set; }
        public string FulfillmentBreakByCarrier { get; set; }
        public string FulfillmentBreakByCig { get; set; }
        public string FulfillmentBreakByPort { get; set; }
        public string FulfillmentBreakByPackage { get; set; }
        public string FulfillmentBreakByTransport { get; set; }
        public string ValidPrices { get; set; }
        public string SortOpenOrders { get; set; }
        public short? CustQuotaExpiringDays { get; set; }
        public int? SalesOrdersShortageCheckType { get; set; }
        public string UpdateDnrowsValue { get; set; }
        public string DefaultEmptyConfDelivDate { get; set; }
        public string AllocationManage { get; set; }
        public int? CheckAllocatedQtyType { get; set; }
        public int? CheckPreShippedQtyType { get; set; }
        public string DisplayFirstSaleOrder { get; set; }
        public double? PercSaleOrdAllocation { get; set; }
        public string FulfillmentBreakByTcg { get; set; }
        public DateTime Tbcreated { get; set; }
        public DateTime Tbmodified { get; set; }
        public int TbcreatedId { get; set; }
        public int TbmodifiedId { get; set; }
    }
}
