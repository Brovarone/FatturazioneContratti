using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EFMago.Models
{
    public partial class MaSaleDocShipping
    {
        public int SaleDocId { get; set; }
        public string Port { get; set; }
        public string Package { get; set; }
        public string Shipping { get; set; }
        public string Appearance { get; set; }
        public short? NoOfPacks { get; set; }
        public double? NetWeight { get; set; }
        public double? GrossWeight { get; set; }
        public double? GrossVolume { get; set; }
        public DateTime? DepartureDate { get; set; }
        public short? DepartureHr { get; set; }
        public short? DepartureMn { get; set; }
        public string Carrier1 { get; set; }
        public string Carrier2 { get; set; }
        public string Carrier3 { get; set; }
        public string ShipToAddress { get; set; }
        public string Transport { get; set; }
        public string ExcludeCharges { get; set; }
        public string Vehicle { get; set; }
        public string Trailer { get; set; }
        public string PackageDescription { get; set; }
        public string RecalculateDisabled { get; set; }
        public string NoOfPacksIsAuto { get; set; }
        public string NetWeightIsAuto { get; set; }
        public string GrossWeightIsAuto { get; set; }
        public string GrossVolumeIsAuto { get; set; }
        public string PortAuto { get; set; }
        public string TransportationForm { get; set; }
        public int? CustomerType { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerBranch { get; set; }
        public int? LoaderType { get; set; }
        public string LoaderCode { get; set; }
        public string LoaderBranch { get; set; }
        public int? OwnerType { get; set; }
        public string OwnerCode { get; set; }
        public string OwnerBranch { get; set; }
        public string Declarations { get; set; }
        public string Instructions { get; set; }
        public string LoadingPlace { get; set; }
        public string UnloadingPlace { get; set; }
        public string CompilationPlace { get; set; }
        public DateTime? CompilationDate { get; set; }
        public string CompilationData { get; set; }
        public DateTime Tbcreated { get; set; }
        public DateTime Tbmodified { get; set; }
        public int TbcreatedId { get; set; }
        public int TbmodifiedId { get; set; }

        public virtual MaSaleDoc SaleDoc { get; set; }
    }
}
