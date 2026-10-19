using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EFMago.Models
{
    public partial class AllordCliAttivita
    {
        public int IdOrdCli { get; set; }
        public short Line { get; set; }
        public string Attivita { get; set; }
        public DateTime? DataInizio { get; set; }
        public DateTime? DataFine { get; set; }
        public string DaFatturare { get; set; }
        public DateTime? DataRifatturazione { get; set; }
        public string Fatturata { get; set; }
        public string Nota { get; set; }
        public string RifServizio { get; set; }
        public short? RifLinea { get; set; }
        public DateTime Tbcreated { get; set; }
        public DateTime Tbmodified { get; set; }
        public int TbcreatedId { get; set; }
        public int TbmodifiedId { get; set; }
        public double? ValUnit { get; set; }
        public string TestoFattura { get; set; }

        // Creato da me
        [NotMapped]
        public double CanoniRipresi { get; set; }
        public virtual MaSaleOrd SaleOrd { get; set; }
        public virtual Allattivita Allattivita { get; set; }
        public virtual AllordCliContratto AllordCliContratto { get; set; }

        public string GetTipoAttivita()
        {
            // Se la relazione Allattivita non è stata caricata o è corrotta
            if (Allattivita == null)
            {
                return "X"; // O un codice che indichi "Errore/Sconosciuto"
            }

            if (FormatHelper.StringToBoolean(Allattivita.Sospensione))
            {
                return "S";
            }
            if (FormatHelper.StringToBoolean(Allattivita.Annullamento))
            {
                return "A";
            }
            if (FormatHelper.StringToBoolean(Allattivita.Istat))
            {
                return "I";
            }

            return "X";
        }

    }

    internal class FormatHelper
        {
            public static Boolean StringToBoolean(String str)
            {
                return StringToBoolean(str, false);
            }

            public static Boolean StringToBoolean(String str, Boolean bDefault)
            {
            String[] BooleanStringOff = { "0", "off", "no" };

                if (String.IsNullOrEmpty(str))
                    return bDefault;
                else if (BooleanStringOff.Contains(str, StringComparer.InvariantCultureIgnoreCase))
                    return false;

                Boolean result;
                if (!Boolean.TryParse(str, out result))
                    result = true;

                return result;
            }
        }
}
