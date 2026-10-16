using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EFMago.Models
{
    public partial class MaItemsStorageQty
    {
        public short FiscalYear { get; set; }
        public short FiscalPeriod { get; set; }
        public string Item { get; set; }
        public string Storage { get; set; }
        public string Specificator { get; set; }
        public int SpecificatorType { get; set; }
        public double? InitialQty { get; set; }
        public double? ReceivedQty { get; set; }
        public double? IssuedQty { get; set; }
        public double? MinimumStock { get; set; }
        public string DefaultLocation { get; set; }
        public double? ReservedSaleOrd { get; set; }
        public double? OrderedPurchOrd { get; set; }
        public double? ReservedByProd { get; set; }
        public double? OrderedToProd { get; set; }
        public double? ApprovedPurchaseReq { get; set; }
        public double? MaximumStock { get; set; }
        public double? ReorderingLotSize { get; set; }
        public string MathematicRounding { get; set; }
        public double? PickedQty { get; set; }
        public double? AllocatedQty { get; set; }
        public DateTime Tbcreated { get; set; }
        public DateTime Tbmodified { get; set; }
        public int TbcreatedId { get; set; }
        public int TbmodifiedId { get; set; }

        public virtual MaItems ItemNavigation { get; set; }
    }
}
