using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFMago.Models
{
    public partial class IntegraInterventi
    {
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.None)] 
		public string ID { get; set; }

		public string Filiale { get; set; }
		public DateTime? DataMovimento { get; set; }
		public string Contratto { get; set; }
		public string TipoEvento { get; set; }
		public string Fatturare { get; set; }
		public string MotivoEsclusione { get; set; }
		public DateTime? InizioAllarme { get; set; }
		public DateTime? InizioEvento { get; set; }
		public DateTime? FineEvento { get; set; }
		public double? Qta { get; set; }
		public string DescrizioneServizio { get; set; }
		public string EsitoIntervento { get; set; }
		public string UtenteInsert { get; set; }
		public DateTime? DataInsert { get; set; }
		public string CodPerif { get; set; }
		public DateTime? TBCreated { get; set; }
		public DateTime? TBModified { get; set; }
		public int? TBCreatedID { get; set; }
		public int? TBModifiedID { get; set; }
		public string MAssociato { get; set; }
		/// <summary>
		/// Id Ordine di Mago(Line)
		/// </summary>
		public int? MIdOrdCli { get; set; }
		/// <summary>
		/// Numero di Linea Distinta (Line)
		/// </summary>
		public short? MLineaDistinta { get; set; }
		/// <summary>
		/// Riferimento Linea Contratto ( RifLine)
		/// </summary>
		public short? MLineaContratto { get; set; }
		public virtual AllordCliContrattoDistinta AllordCliContrattoDistinta { get; set; }
		public virtual AllordCliContrattoDistinta ALLAllordCliContrattoDistinta { get; set; }
		public virtual MaSaleOrd SaleOrd { get; set; }

	}
}
