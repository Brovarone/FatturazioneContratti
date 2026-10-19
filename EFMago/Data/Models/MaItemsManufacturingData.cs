using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EFMago.Models
{
    public partial class MaItemsManufacturingData
    {
        public string Item { get; set; }
        public int? Mrppolicy { get; set; }
        public double? ScrapPerc { get; set; }
        public double? LeadTime { get; set; }
        public short? OrderReleaseDays { get; set; }
        public short? AnticipationDays { get; set; }
        public double? ReferenceLot { get; set; }
        public int? TimeRoundingType { get; set; }
        public double? EconomicOrderQty { get; set; }
        public double? Rmcost { get; set; }
        public double? InhouseProcessingCost { get; set; }
        public double? OutsourcedProcessingCost { get; set; }
        public double? SetupCost { get; set; }
        public double? ProductionCost { get; set; }
        public DateTime? ProductionCostLastChange { get; set; }
        public string NoMrp { get; set; }
        public double? MinProductionQty { get; set; }
        public double? LastProductionCost { get; set; }
        public string ProductionCostMono { get; set; }
        public short? StockLevelHorizon { get; set; }
        public double? MaxProductionQty { get; set; }
        public double? ReorderPoint { get; set; }
        public string ProportionalLeadTime { get; set; }
        public string NetByJobMopurchOrd { get; set; }
        public string NetByMoemptyJob { get; set; }
        public double? ProductionLot { get; set; }
        public int? LoadingCriterionValuation { get; set; }
        public string Factory { get; set; }
        public string IsKanban { get; set; }
        public double? KanbanCardSize { get; set; }
        public short? KanbanCardsNum { get; set; }
        public short? KanbanCardsToReorder { get; set; }
        public double? Bomcost { get; set; }
        public double? VariantCost { get; set; }
        public short? MrpconfirmationRank { get; set; }
        public string MakeOrBuy { get; set; }
        public int? MakeOrBuyType { get; set; }
        public string UseItemManufParameters { get; set; }
        public string ConfirmMajorPicking { get; set; }
        public string ConfirmIntegratePl { get; set; }
        public string ConfirmMinorPicking { get; set; }
        public string ConfirmReturnMaterial { get; set; }
        public string ItemManufStorage { get; set; }
        public string ItemPickAlsoShortages { get; set; }
        public int? ItemManQtyRounding { get; set; }
        public short? ItemManQtyDigit { get; set; }
        public string Model { get; set; }
        public string Maker { get; set; }
        public string OrdinaryMaintenance { get; set; }
        public string ExtraordinaryMaintenance { get; set; }
        public string Notes { get; set; }
        public short? RoundQty { get; set; }
        public string IsTool { get; set; }
        public string NetByMoconfirmed { get; set; }
        public int? LotGenerationMoment { get; set; }
        public string MultipleLots { get; set; }
        public double? MultipleRoundQty { get; set; }
        public double? SamplesQty { get; set; }
        public double? ScrapQty { get; set; }
        public DateTime Tbcreated { get; set; }
        public DateTime Tbmodified { get; set; }
        public int TbcreatedId { get; set; }
        public int TbmodifiedId { get; set; }

        public virtual MaItems ItemNavigation { get; set; }
    }
}
