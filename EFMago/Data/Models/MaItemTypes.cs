using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EFMago.Models
{
    /// <summary>
    /// Tipologie servizi (tabella MA_ItemTypes, chiave CodeType).
    /// Aggiunta a mano: lo scaffold EFMago non includeva questa anagrafica, ma è la
    /// fonte ufficiale dei codici scritti in ALLOrdCliTipologiaServizi.Tipologia.
    /// Stessa tabella è referenziata da MA_Items.ItemType.
    /// </summary>
    public partial class MaItemTypes
    {
        public string CodeType { get; set; }
        public string Description { get; set; }
        public double? Discount1 { get; set; }
        public double? Discount2 { get; set; }
        public string DiscountFormula { get; set; }
        public string Notes { get; set; }
        public Guid? Tbguid { get; set; }
        public string HasCustomers { get; set; }
        public string HasSuppliers { get; set; }
        public double? ForfaitOnReturns { get; set; }
        public DateTime? Tbcreated { get; set; }
        public DateTime? Tbmodified { get; set; }
        public int? TbcreatedId { get; set; }
        public int? TbmodifiedId { get; set; }
    }
}
