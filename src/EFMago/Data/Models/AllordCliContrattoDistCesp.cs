using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFMago.Models
{
    public partial class AllordCliContrattoDistCesp
    {
        public int IdOrdCli { get; set; }
        /// <summary>
        /// Riferimento Linea Distinta
        /// </summary>
        public short RifLinea { get; set; }
        /// <summary>
        /// Riferimento Linea Contratto
        /// </summary>
        public short RifRifLinea { get; set; }
        public string Cespite { get; set; }
        public DateTime Tbcreated { get; set; }
        public DateTime Tbmodified { get; set; }
        public int TbcreatedId { get; set; }
        public int TbmodifiedId { get; set; }

        // Creato da me
        //public virtual MaSaleOrd SaleOrd { get; set; }
        public virtual AllordCliContrattoDistinta AllordCliContrattoDistinta { get; set; }

    }
}

