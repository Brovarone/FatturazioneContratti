using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EFMago.Models
{
    public partial class AllordCliFattEle
    {
        public int IdOrdCli { get; set; }
        /// <summary>
        /// 2.1.1.5.4
        /// <br>FatturaElettronica.FatturaElettronicaBody.DatiGenerali.DatiGeneraliDocumento.DatiRitenuta.CausalePagamento</br>
        /// </summary>
        public string F2_1_1_5_4 { get; set; }
        /// <summary>
        /// 2.1.1.11
        /// <br>FatturaElettronica.FatturaElettronicaBody.DatiGenerali.DatiGeneraliDocumento.Causale</br>
        /// </summary>
        public string F2_1_1_11 { get; set; }
        /// <summary>
        /// 2.1.2.1
        /// FatturaElettronica.FatturaElettronicaBody.DatiGenerali.DatiOrdineAcquisto.RiferimentoNumeroLinea
        /// </summary>
        public short? F2_1_2_1 { get; set; }
        /// <summary>
        /// 2.1.2.2
        /// FatturaElettronica.FatturaElettronicaBody.DatiGenerali.DatiOrdineAcquisto.IdDocumento
        /// </summary>
        public string F2_1_2_2 { get; set; }
        /// <summary>
        /// 2.1.2.3
        /// FatturaElettronica.FatturaElettronicaBody.DatiGenerali.DatiOrdineAcquisto.Data
        /// </summary>
        public DateTime? F2_1_2_3 { get; set; }
        /// <summary>
        /// 2.1.2.4
        /// FatturaElettronica.FatturaElettronicaBody.DatiGenerali.DatiOrdineAcquisto.NumItem
        /// </summary>
        public string F2_1_2_4 { get; set; }
        /// <summary>
        /// 2.1.2.5
        /// FatturaElettronica.FatturaElettronicaBody.DatiGenerali.DatiOrdineAcquisto.CodiceCommessaConvenzione
        /// </summary>
        public string F2_1_2_5 { get; set; }
        /// <summary>
        /// 2.1.2.6
        /// FatturaElettronica.FatturaElettronicaBody.DatiGenerali.DatiOrdineAcquisto.CodiceCUP
        /// </summary>
        public string F2_1_2_6 { get; set; }
        /// <summary>
        /// 2.1.2.7
        /// FatturaElettronica.FatturaElettronicaBody.DatiGenerali.DatiOrdineAcquisto.CodiceCIG
        /// </summary>
        public string F2_1_2_7 { get; set; }
        /// <summary>
        /// 2.1.3.1
        /// FatturaElettronica.FatturaElettronicaBody.DatiGenerali.DatiContratto.RiferimentoNumeroLinea
        /// </summary>
        public short? F2_1_3_1 { get; set; }
        /// <summary>
        /// 2.1.3.2
        /// FatturaElettronica.FatturaElettronicaBody.DatiGenerali.DatiContratto.IdDocumento
        /// </summary>
        public string F2_1_3_2 { get; set; }
        /// <summary>
        /// 2.1.3.3
        /// FatturaElettronica.FatturaElettronicaBody.DatiGenerali.DatiContratto.Data
        /// </summary>
        public DateTime? F2_1_3_3 { get; set; }
        /// <summary>
        /// 2.1.3.4
        /// FatturaElettronica.FatturaElettronicaBody.DatiGenerali.DatiContratto.NumItem
        /// </summary>
        public string F2_1_3_4 { get; set; }
        /// <summary>
        /// 2.1.3.5
        /// FatturaElettronica.FatturaElettronicaBody.DatiGenerali.DatiContratto.CodiceCommessaConvenzione
        /// </summary>
        public string F2_1_3_5 { get; set; }
        /// <summary>
        /// 2.1.3.6
        /// FatturaElettronica.FatturaElettronicaBody.DatiGenerali.DatiContratto.CodiceCUP
        /// </summary>
        public string F2_1_3_6 { get; set; }
        /// <summary>
        /// 2.1.3.7
        /// FatturaElettronica.FatturaElettronicaBody.DatiGenerali.DatiContratto.CodiceCIG
        /// </summary>
        public string F2_1_3_7 { get; set; }      
        /// <summary>
        /// 2.1.4.1
        /// FatturaElettronica.FatturaElettronicaBody.DatiGenerali.DatiConvenzione.RiferimentoNumeroLinea
        /// </summary>
        public short? F2_1_4_1 { get; set; }
        /// <summary>
        /// 2.1.4.2
        /// FatturaElettronica.FatturaElettronicaBody.DatiGenerali.DatiConvenzione.IdDocumento
        /// </summary>
        public string F2_1_4_2 { get; set; }
        /// <summary>
        /// 2.1.4.3
        /// FatturaElettronica.FatturaElettronicaBody.DatiGenerali.DatiConvenzione.Data
        /// </summary>
        public DateTime? F2_1_4_3 { get; set; }
        /// <summary>
        /// 2.1.4.4
        /// FatturaElettronica.FatturaElettronicaBody.DatiGenerali.DatiConvenzione.NumItem
        /// </summary>
        public string F2_1_4_4 { get; set; }
        /// <summary>
        /// 2.1.4.5
        /// FatturaElettronica.FatturaElettronicaBody.DatiGenerali.DatiConvenzione.CodiceCommessaConvenzione
        /// </summary>
        public string F2_1_4_5 { get; set; }
        /// <summary>
        /// 2.1.4.6
        /// FatturaElettronica.FatturaElettronicaBody.DatiGenerali.DatiConvenzione.CodiceCUP
        /// </summary>
        public string F2_1_4_6 { get; set; }
        /// <summary>
        /// 2.1.4.7
        /// FatturaElettronica.FatturaElettronicaBody.DatiGenerali.DatiConvenzione.CodiceCIG
        /// </summary>
        public string F2_1_4_7 { get; set; }      
        /// <summary>
        /// 2.1.5.1
        /// FatturaElettronica.FatturaElettronicaBody.DatiGenerali.DatiRicezione.RiferimentoNumeroLinea
        /// </summary>
        public short? F2_1_5_1 { get; set; }
        /// <summary>
        /// 2.1.5.2
        /// FatturaElettronica.FatturaElettronicaBody.DatiGenerali.DatiRicezione.IdDocumento
        /// </summary>
        public string F2_1_5_2 { get; set; }
        /// <summary>
        /// 2.1.5.3
        /// FatturaElettronica.FatturaElettronicaBody.DatiGenerali.DatiRicezione.Data
        /// </summary>
        public DateTime? F2_1_5_3 { get; set; }
        /// <summary>
        /// 2.1.5.4
        /// FatturaElettronica.FatturaElettronicaBody.DatiGenerali.DatiRicezione.NumItem
        /// </summary>
        public string F2_1_5_4 { get; set; }
        /// <summary>
        /// 2.1.5.5
        /// FatturaElettronica.FatturaElettronicaBody.DatiGenerali.DatiRicezione.CodiceCommessaConvenzione
        /// </summary>
        public string F2_1_5_5 { get; set; }
        /// <summary>
        /// 2.1.5.6
        /// FatturaElettronica.FatturaElettronicaBody.DatiGenerali.DatiRicezione.CodiceCUP
        /// </summary>
        public string F2_1_5_6 { get; set; }
        /// <summary>
        /// 2.1.5.7
        /// FatturaElettronica.FatturaElettronicaBody.DatiGenerali.DatiRicezione.CodiceCIG
        /// </summary>
        public string F2_1_5_7 { get; set; }
        public DateTime Tbcreated { get; set; }
        public DateTime Tbmodified { get; set; }
        public int TbcreatedId { get; set; }
        public int TbmodifiedId { get; set; }
        public virtual MaSaleOrd SaleOrd { get; set; }
    }
}
