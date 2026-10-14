using System;
using System.Text;

namespace FatturazioneContratti.Invoicing
{
    public enum InterventiFilterMode
    {
        NoFilter = 0,
        MeseAnno = 1,
        DateRange = 2
    }

    public enum ActionMode
    {
        SoloControllo = 0,
        ControlloEFattura = 1,
        SoloFattura = 2
    }

    public class FiltriOrdiniConsuntivo
    {
        public DateTime FromInterventiDate { get; set; }
        public DateTime ToInterventiDate { get; set; }
        public bool SingoloOrdine { get; set; }
        public string NrOrdine { get; set; } = string.Empty;
        public string NrMese { get; set; } = string.Empty;
        public DateTime DataFattura { get; set; }
        public bool SingolaFiliale { get; set; }
        public string Filiale { get; set; } = string.Empty;
        public bool Riassegna { get; set; }
        public InterventiFilterMode InterventiFilter { get; set; } = InterventiFilterMode.MeseAnno;
        public ActionMode Azione { get; set; } = ActionMode.ControlloEFattura;

        public StringBuilder LogFiltri { get; set; } = new StringBuilder();

        public void CostruisciFiltriLog()
        {
            LogFiltri.Clear();
            LogFiltri.AppendLine(" --- Filtri Consuntivo ---");
            LogFiltri.AppendLine($"Modalità Interventi: {InterventiFilter}");
            LogFiltri.AppendLine($"Dal {FromInterventiDate:d} al {ToInterventiDate:d}");
            LogFiltri.AppendLine($"Mese : {NrMese}");
            LogFiltri.AppendLine($"Azione: {Azione}");
            LogFiltri.AppendLine($"Data Fattura: {DataFattura:d}");
            LogFiltri.AppendLine($"Filiale : {(SingolaFiliale ? Filiale : "TUTTI")}");
            LogFiltri.AppendLine($"Ordine : {(SingoloOrdine ? NrOrdine : "TUTTI")}");
            LogFiltri.AppendLine($"Riassegna : {Riassegna}");
        }
    }
}