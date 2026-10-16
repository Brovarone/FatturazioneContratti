using FatturazioneContratti.Common;
using System;
using System.Text;

namespace FatturazioneContratti.Invoicing
{
    public class FiltriAllineamentoFatture : ILogFiltri
    {
        public DateTime DataFattDa { get; set; } = new DateTime(DateTime.Today.Year, 1, 1);
        public DateTime DataFattA { get; set; } = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month));

        public bool AllNumbers { get; set; } = true;
        public string NrFirst { get; set; } = string.Empty;
        public string NrLast { get; set; } = string.Empty;

        public StringBuilder LogFiltri { get; set; } = new StringBuilder();

        public void CostruisciFiltriLog()
        {
            LogFiltri.Clear();
            LogFiltri.AppendLine(" --- Filtri Allineamento Fatture ---");
            LogFiltri.AppendLine($"Dal {DataFattDa:d} al {DataFattA:d}");
            LogFiltri.AppendLine(AllNumbers
                ? "Numeri documento: TUTTI"
                : $"Numeri documento: dal {NrFirst} al {NrLast}");
        }
    }
}