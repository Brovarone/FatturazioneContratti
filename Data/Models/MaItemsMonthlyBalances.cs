using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EFMago.Models
{
    public partial class MaItemsMonthlyBalances
    {
        public string Item { get; set; }
        public string Storage { get; set; }
        public short FiscalYear { get; set; }
        public short BalanceYear { get; set; }
        public int Balance { get; set; }
        public short BalanceMonth { get; set; }
        public double? FinalOnHand { get; set; }
        public double? BookInv { get; set; }
        public double? BookInvValue { get; set; }
        public double? PurchasesQty { get; set; }
        public double? SalesQty { get; set; }
        public double? ReceivedQty { get; set; }
        public double? IssuedQty { get; set; }
        public double? PurchasesValue { get; set; }
        public double? SalesValue { get; set; }
        public double? ReceivedValue { get; set; }
        public double? IssuedValue { get; set; }
        public double? ProducedQty { get; set; }
        public double? ProducedValue { get; set; }
        public double? SalesQtyForFiscalPeriod { get; set; }
        public double? SalesValueForFiscalPeriod { get; set; }
        public double? SecondLastCost { get; set; }
        public double? LastCost { get; set; }
        public double? Cigvalue { get; set; }
        public double? CigvalueForFiscalPeriod { get; set; }
        public double? UsedInProductionQty { get; set; }
        public double? UsedInProductionValue { get; set; }
        public double? PickingValue { get; set; }
        public double? PickingValueForFiscalPeriod { get; set; }
        public double? ScrapQty { get; set; }
        public double? ScrapsValue { get; set; }
        public double? Subcontracting { get; set; }
        public double? ForSubcontracting { get; set; }
        public double? ReservedByProd { get; set; }
        public double? OrderedToProd { get; set; }
        public double? SampleGoods { get; set; }
        public double? ReturnedQty { get; set; }
        public double? Sampling { get; set; }
        public double? Bailment { get; set; }
        public double? ForRepairing { get; set; }
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
        public double? OnHandStorageTot { get; set; }
        public double? ReservedStorageTot { get; set; }
        public double? FinalBookInv { get; set; }
        public double? FinalBookInvValue { get; set; }
        public DateTime Tbcreated { get; set; }
        public DateTime Tbmodified { get; set; }
        public int TbcreatedId { get; set; }
        public int TbmodifiedId { get; set; }

        public virtual MaItems ItemNavigation { get; set; }
    }
}
