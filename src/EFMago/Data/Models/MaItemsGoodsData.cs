using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EFMago.Models
{
    public partial class MaItemsGoodsData
    {
        public string Item { get; set; }
        public string Supplier { get; set; }
        public string Location { get; set; }
        public string DefaultLocation { get; set; }
        public string Department { get; set; }
        public string Appearance { get; set; }
        public short? NoOfPacks { get; set; }
        public double? NetWeight { get; set; }
        public double? GrossWeight { get; set; }
        public double? GrossVolume { get; set; }
        public string PacksIssueUoM { get; set; }
        public string ReceiptUoM { get; set; }
        public string IssueUoM { get; set; }
        public string ReportUoM { get; set; }
        public string OnInventoryLevel { get; set; }
        public string OnInventorySheets { get; set; }
        public string NoAbc { get; set; }
        public short? MaxUnsoldMonths { get; set; }
        public double? MinimumStock { get; set; }
        public double? MaximumStock { get; set; }
        public double? ReorderingLotSize { get; set; }
        public DateTime? LastReceiptDate { get; set; }
        public DateTime? LastIssueDate { get; set; }
        public string LastSupplier { get; set; }
        public string UseLots { get; set; }
        public short? LotPreexpiringDays { get; set; }
        public short? LotValidityDays { get; set; }
        public string PacksReceiptUoM { get; set; }
        public double? MinimumSaleQty { get; set; }
        public string TraceabilityCritical { get; set; }
        public string Weeectg { get; set; }
        public double? Weeeamount { get; set; }
        public string Weeectg2 { get; set; }
        public double? Weeeamount2 { get; set; }
        public string PostToInspection { get; set; }
        [NotMapped]
        public string SpecificationsForSupplier { get; set; }
        public string InsertAnalParam { get; set; }
        public string ManageSample { get; set; }
        public double? PercSample { get; set; }
        public double? SampleQty { get; set; }
        public int? UseSupplierLotAsNewLotNumber { get; set; }
        public DateTime Tbcreated { get; set; }
        public DateTime Tbmodified { get; set; }
        public int TbcreatedId { get; set; }
        public int TbmodifiedId { get; set; }
        public double? ImWeeeamount { get; set; }

        public virtual MaItems ItemNavigation { get; set; }
    }
}
