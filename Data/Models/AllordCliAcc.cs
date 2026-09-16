using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EFMago.Models
{
    public partial class AllordCliAcc
    {
        public int IdOrdCli { get; set; }
        public string ApplicoIstat { get; set; }
        public short? MesiDurata { get; set; }
        public short? MesiRinnovo { get; set; }
        public short? Ggdisdetta { get; set; }
        public DateTime? DataScadenzaFissa { get; set; }
        public DateTime? DataRiscatto { get; set; }
        public double? ImportoRiscatto { get; set; }
        public DateTime? DataRiduzione { get; set; }
        public double? ImportoRiduzione { get; set; }
        public double? PercRiduzione { get; set; }
        public DateTime? DataDecorrenza { get; set; }
        public DateTime? DataCessazione { get; set; }
        public string MotivoCessazione { get; set; }
        public int? TipoContratto { get; set; }
        public string ImpiantoProprietaCliente { get; set; }
        public double? ImportoCanone { get; set; }
        public double? ContributoInstallazione { get; set; }
        public string OrdineSospeso { get; set; }
        public DateTime? DataSospensione { get; set; }
        public string MotivoSospensione { get; set; }
        public string Agente { get; set; }
        public double? ImportoProvvigione { get; set; }
        public string Impianto { get; set; }
        public string Nota { get; set; }
        public string CondPag { get; set; }
        public string SedeInvioDoc { get; set; }
        public int? ModelloContratto { get; set; }
        public DateTime Tbcreated { get; set; }
        public DateTime Tbmodified { get; set; }
        public int TbcreatedId { get; set; }
        public int TbmodifiedId { get; set; }
        public string CdC { get; set; }
        public string Vettore { get; set; }
        public string ImpiantoDue { get; set; }
        public DateTime? DataPrevistaScadenza { get; set; }
        public virtual MaSaleOrd SaleOrd { get; set; }
    }
}
