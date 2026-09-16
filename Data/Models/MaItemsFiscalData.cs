using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EFMago.Models
{
    public partial class MaItemsFiscalData
    {
        public short FiscalYear { get; set; }
        public short FiscalPeriod { get; set; }
        public string Item { get; set; }
        public string Storage { get; set; }
        public int? InventoryValueCriteria { get; set; }
        public string NoAbcvalue { get; set; }
        public double? SecondLastCost { get; set; }
        public double? LastCost { get; set; }
        public double? StandardCost { get; set; }
        public double? InitialOnHand { get; set; }
        public double? FinalOnHand { get; set; }
        public double? ReservedSaleOrd { get; set; }
        public double? OrderedPurchOrd { get; set; }
        public double? InitialBookInv { get; set; }
        public double? BookInv { get; set; }
        public double? InitialBookInvValue { get; set; }
        public double? BookInvValue { get; set; }
        public double? PurchasesQty { get; set; }
        public double? PurchasesValue { get; set; }
        public double? SalesQty { get; set; }
        public double? SalesValue { get; set; }
        public double? ScrapQty { get; set; }
        public double? ScrapsValue { get; set; }
        public double? ReceivedQty { get; set; }
        public double? ReceivedValue { get; set; }
        public double? IssuedQty { get; set; }
        public double? IssuedValue { get; set; }
        public double? Subcontracting { get; set; }
        public double? ForSubcontracting { get; set; }
        public double? ReservedByProd { get; set; }
        public double? OrderedToProd { get; set; }
        public double? SampleGoods { get; set; }
        public double? ReturnedQty { get; set; }
        public double? Sampling { get; set; }
        public double? Bailment { get; set; }
        public double? ForRepairing { get; set; }
        public double? UsedByProduction { get; set; }
        public int? LastLotNo { get; set; }
        public double? CustomQty1 { get; set; }
        public double? CustomValue1 { get; set; }
        public double? CustomQty2 { get; set; }
        public double? CustomValue2 { get; set; }
        public double? CustomQty3 { get; set; }
        public double? CustomValue3 { get; set; }
        public double? CustomQty4 { get; set; }
        public double? CustomValue4 { get; set; }
        public double? CustomQty5 { get; set; }
        public double? CustomValue5 { get; set; }
        public double? InitialCustomQty1 { get; set; }
        public double? InitialCustomValue1 { get; set; }
        public double? InitialCustomQty2 { get; set; }
        public double? InitialCustomValue2 { get; set; }
        public double? InitialCustomQty3 { get; set; }
        public double? InitialCustomValue3 { get; set; }
        public double? InitialCustomQty4 { get; set; }
        public double? InitialCustomValue4 { get; set; }
        public double? InitialCustomQty5 { get; set; }
        public double? InitialCustomValue5 { get; set; }
        public double? InitialOnHandStorageTot { get; set; }
        public double? InitialReservedStorageTot { get; set; }
        public double? OnHandStorageTot { get; set; }
        public double? ReservedStorageTot { get; set; }
        public double? InitialReservedCustQuota { get; set; }
        public double? ReservedCustQuota { get; set; }
        public double? InitialSubcontracting { get; set; }
        public double? InitialForSubcontracting { get; set; }
        public double? InitialSampleGoods { get; set; }
        public double? InitialReturnedQty { get; set; }
        public double? InitialSampling { get; set; }
        public double? InitialBailment { get; set; }
        public double? InitialForRepairing { get; set; }
        public double? InitialUsedByProduction { get; set; }
        public double? ProducedQty { get; set; }
        public double? ProducedValue { get; set; }
        public double? ApprovedPurchaseReq { get; set; }
        public int? ValueType { get; set; }
        public double? Cigvalue { get; set; }
        public double? UsedInProductionValue { get; set; }
        public double? PickingValue { get; set; }
        public double? OrderedToProdIe { get; set; }
        public double? InitialUsedInProductionValue { get; set; }
        public double? PickedQty { get; set; }
        public double? AllocatedQty { get; set; }
        public DateTime Tbcreated { get; set; }
        public DateTime Tbmodified { get; set; }
        public int TbcreatedId { get; set; }
        public int TbmodifiedId { get; set; }

        public virtual MaItems ItemNavigation { get; set; }
    }
}
