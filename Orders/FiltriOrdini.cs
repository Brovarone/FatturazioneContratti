using System;
using System.Text;

namespace FatturazioneContratti.Orders
{
    public class FiltriOrdini
    {
        public bool FiltroDateOrdini { get; set; }
        public DateTime FromOrdDate { get; set; }
        public DateTime ToOrdDate { get; set; }
        public bool SingoloOrdine { get; set; }
        public string NrOrd { get; set; } = string.Empty;
        public bool SingoloCliente { get; set; }
        public string Cliente { get; set; } = string.Empty;
        public bool SingolaFiliale { get; set; }
        public string Filiale { get; set; } = string.Empty;
        public bool SingolaPriorita { get; set; }
        public string Priorita { get; set; } = string.Empty;
        public DateTime DataFattDa { get; set; }
        public DateTime DataFattA { get; set; }
        public bool TipoCanoni { get; set; }
        public bool TipoServiziAggiuntivi { get; set; }
        public bool PeriodoTutti { get; set; }
        public bool PeriodoAnnuali { get; set; }
        public bool PeriodoSemestrali { get; set; }
        public bool PeriodoQuadrimestrali { get; set; }
        public bool PeriodoTrimestrali { get; set; }
        public bool PeriodoBimestrali { get; set; }
        public bool PeriodoMensili { get; set; }
        public bool[] Periodi { get; set; } = new bool[7];
        public bool DivisioneTutti { get; set; }
        public bool DivisioneTecnologia { get; set; }
        public bool DivisioneVigilanza { get; set; }

        // ISTAT
        public bool IsIstat { get; set; }
        public DateTime DataDecorrenza { get; set; }
        public double PercIstat { get; set; }
        public string CausaleAttivita { get; set; } = string.Empty;
        public int AnnoAdeguamento { get; set; }
        public string TestoFattura { get; set; } = string.Empty;

        public StringBuilder LogFiltri { get; set; } = new StringBuilder();

        public void CostruisciFiltriLog()
        {
            LogFiltri.Clear();
            LogFiltri.AppendLine(" --- Filtri ---");
            LogFiltri.AppendLine(FiltroDateOrdini
                ? $"Dal {FromOrdDate:d} al {ToOrdDate:d}"
                : $"Fino al {ToOrdDate:d}");
            LogFiltri.AppendLine($"Ordine : {(SingoloOrdine ? NrOrd : "TUTTI")}");
            LogFiltri.AppendLine($"Cliente : {(SingoloCliente ? Cliente : "TUTTI")}");
            LogFiltri.AppendLine($"Filiale : {(SingolaFiliale ? Filiale : "TUTTI")}");
            LogFiltri.AppendLine($"Priorità : {(SingolaPriorita ? Priorita : "TUTTI")}");

            if (IsIstat)
            {
                LogFiltri.AppendLine($"Data Decorrenza: {DataDecorrenza:d}");
                LogFiltri.AppendLine($"Percentuale ISTAT: {PercIstat}");
                LogFiltri.AppendLine($"Attività: {CausaleAttivita}");
                LogFiltri.AppendLine($"Anno adeguamento: {AnnoAdeguamento}");
                LogFiltri.AppendLine($"Testo: {TestoFattura}");
            }
            else
            {
                if (TipoCanoni)
                {
                    LogFiltri.AppendLine("Tipo : Canoni");
                    LogFiltri.AppendLine($"Dalla data Fatt. :{DataFattDa:d}");
                    LogFiltri.AppendLine($"Alla data Fatt. :{DataFattA:d}");
                }
                else
                {
                    LogFiltri.AppendLine("Tipo : Servizi Aggiuntivi");
                }

                if (PeriodoTutti)
                {
                    LogFiltri.AppendLine("Periodo : Tutti");
                }
                else
                {
                    if (PeriodoAnnuali) LogFiltri.AppendLine("Periodo : Annuali");
                    if (PeriodoSemestrali) LogFiltri.AppendLine("Periodo : Semestrali");
                    if (PeriodoQuadrimestrali) LogFiltri.AppendLine("Periodo : Quadrimestrali");
                    if (PeriodoTrimestrali) LogFiltri.AppendLine("Periodo : Trimestrali");
                    if (PeriodoBimestrali) LogFiltri.AppendLine("Periodo : Bimestrali");
                    if (PeriodoMensili) LogFiltri.AppendLine("Periodo : Mensili");
                }
            }

            if (DivisioneTutti) LogFiltri.AppendLine("Divisione : Tutti");
            if (DivisioneTecnologia) LogFiltri.AppendLine("Divisione : Tecnologia");
            if (DivisioneVigilanza) LogFiltri.AppendLine("Divisione : Vigilanza");
        }
    }
}