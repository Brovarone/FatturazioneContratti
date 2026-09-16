using System;
using System.Text;

namespace FatturazioneContratti.Invoicing
{
    public class FiltriOrdiniConsuntivo
    {
        public DateTime FromLogDate { get; set; }
        public DateTime ToLogDate { get; set; }
        public bool SingoloOrdine { get; set; }
        public string NrMese { get; set; } = string.Empty;
        public DateTime DataFattura { get; set; }
        public bool SingoloCliente { get; set; }
        public string Cliente { get; set; } = string.Empty;
        public bool SingolaFiliale { get; set; }
        public string Filiale { get; set; } = string.Empty;
        public bool SingolaPriorita { get; set; }
        public string Priorita { get; set; } = string.Empty;
        public bool Riassegna { get; set; }
        public bool AskFilter { get; set; }
        public StringBuilder LogFiltri { get; set; } = new StringBuilder();

        public void CostruisciFiltriLog()
        {
            LogFiltri.Clear();
            LogFiltri.AppendLine(" --- Filtri Consuntivo ---");
            LogFiltri.AppendLine($"Dal {FromLogDate:d} al {ToLogDate:d}");
            LogFiltri.AppendLine($"Mese : {NrMese}");
            LogFiltri.AppendLine($"Cliente : {(SingoloCliente ? Cliente : "TUTTI")}");
            LogFiltri.AppendLine($"Filiale : {(SingolaFiliale ? Filiale : "TUTTI")}");
            LogFiltri.AppendLine($"Priorità : {(SingolaPriorita ? Priorita : "TUTTI")}");
            LogFiltri.AppendLine($"Riassegna : {Riassegna}");
        }
    }
}