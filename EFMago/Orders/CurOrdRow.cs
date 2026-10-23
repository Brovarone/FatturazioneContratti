using System;
using EFMago.Models;
using FatturazioneContratti.Common;

namespace FatturazioneContratti.Orders
{
    /// <summary>
    /// Porting 1:1 di CurOrdRow.vb (3tables/Ordini/CurOrdRow.vb, 421 righe).
    /// È il modello di dominio "riga di calcolo" per canoni/una tantum/a consumo/continuativi:
    /// data una riga di contratto (o di servizio aggiuntivo) calcola il periodo di competenza,
    /// la quantità di canoni da fatturare e le date della prossima fattura.
    ///
    /// NOTE DI PORTING:
    /// - Gli enum EF-scaffolded (AlltipoRigaServizio.TipologiaServizio/Periodicita/Cadenza)
    ///   sono int? a livello di modello: qui li castiamo esplicitamente ai nostri enum
    ///   (in VB con Option Strict Off il confronto Integer/Enum passava senza cast).
    /// - Logica di business INVARIATA rispetto al VB: stesse soglie, stessi rami, inclusi
    ///   i commenti storici (es. "28/02/2022", "23/03/2022"...) che spiegano perché certe
    ///   date sono calcolate come sono — li ho lasciati per non perdere il "perché".
    /// </summary>
    public class CurOrdRow
    {
        public bool IsOk { get; set; } = true; // Usata anche per aggiornare data prossima fatturazione
        public short Line { get; set; }
        public string Item { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string UoM { get; set; } = string.Empty;
        public double ValUnit { get; set; }
        public DateTime DataDecorrenza { get; set; }
        public DateTime DataProssimaFattura { get; set; }
        public DateTime DataProssimaFatturaCampagna { get; set; }
        public DateTime DataPrevistaConsegna { get; set; }
        public DateTime DataConfermaConsegna { get; set; }
        public double QtaOrdine { get; set; }
        public int NrCanoniIniziale { get; set; }

        /// <summary>
        /// Iniziale = QtaOrdine. Per Canoni e A Consumo riporta i canoni calcolati dalle
        /// date considerando i periodi.
        /// </summary>
        public double QtaCorrente { get; set; }

        /// <summary>
        /// Contiene la franchigia sui servizi a Consuntivo; le rate già fatturate nel caso
        /// di A Consumo.
        /// </summary>
        public double QtaFranchigia { get; set; }

        public double QtaDaRifatturare { get; set; }
        public bool DaRifatturare { get; set; }
        public DateTime DataProssimaRifatturazione { get; set; }
        public bool CanoneFuoriRangeDate { get; set; }
        public double QtaSospesa { get; set; }
        public bool SospesoDaAttivita { get; set; }
        public double QtaAnnullata { get; set; }

        /// <summary>Se true esiste un'attività di annullamento: controllare le date.</summary>
        public bool HaAnnullatoDaAttivita { get; set; }
        public DateTime DataCessazioneDaAttivita { get; set; }
        public DateTime CanoniDataIn { get; set; }
        public DateTime CanoniDataFin { get; set; }
        public DateTime PeriodoDataIn { get; set; }
        public DateTime PeriodoDataFin { get; set; }
        public bool IsCanone { get; set; }
        public bool IsConsuntivo { get; set; }
        public bool IsUnaTantum { get; set; }
        public bool IsAConsumo { get; set; }
        public bool IsContinuativo { get; set; }
        public bool IsCampagna { get; set; }

        /// <summary>Vale per righe attività di annullamento.</summary>
        public bool PrecendementeCessato { get; set; }

        /// <summary>Se true la data di Scadenza cessazione è congrua con l'estrazione.</summary>
        public bool HaDataCessazione { get; set; }
        public DateTime DataCessazione { get; set; }

        /// <summary>Se true la data di Scadenza fissa è congrua con l'estrazione.</summary>
        public bool HaScadenzaFissa { get; set; }
        public DateTime DataScadenzaFissa { get; set; }
        public string CodIva { get; set; } = string.Empty;
        public double PercIva { get; set; }
        public string Contropartita { get; set; } = string.Empty;
        /// <summary>Ordine di appartenenza (back-reference), assegnato alla costruzione dal flusso chiamante.</summary>
        public required CurOrd Parent { get; set; }
        public string ContrattoFox { get; set; } = string.Empty;
        public string CodiceIntegra { get; set; } = string.Empty;
        public FatturazioneContratti.Common.TipologiaServizio TipologiaServizio { get; set; } = FatturazioneContratti.Common.TipologiaServizio.Canone;

        // Da qua sono per i servizi aggiuntivi
        public int AnnoIntervento { get; set; }
        public double NrInterventiFranchigia { get; set; }
        public double NrInterventiPeriodo { get; set; }
        public double NrInterventiMese { get; set; }
        public double NrInterventiOltreFranchigia { get; set; }

        public CurOrdRow()
        {
            // I default CLR (0/false) e gli inizializzatori di proprietà (string.Empty,
            // IsOk = true, TipologiaServizio = Canone) coprono già tutto il resto:
            // qui restano solo le date, che richiedono un valore calcolato.
            var d = DateTime.Today;
            DataDecorrenza = d;
            DataProssimaFattura = d;
            DataProssimaFatturaCampagna = d;
            DataPrevistaConsegna = d;
            DataConfermaConsegna = d;
            DataProssimaRifatturazione = d;
            DataCessazioneDaAttivita = d;
            CanoniDataIn = d;
            CanoniDataFin = d;
            PeriodoDataIn = d;
            PeriodoDataFin = d;
            DataCessazione = DomainConstants.DataNulla;
            DataScadenzaFissa = DomainConstants.DataNulla;
        }

        /// <summary>Popola la classe a partire da una riga contratto (ex New(c As AllordCliContratto)).</summary>
        public CurOrdRow(AllordCliContratto c) : this()
        {
            DateTime nextDate = c.DataProssimaFatt ?? DomainConstants.DataNulla;
            DateTime d = DomainConstants.DataNulla;

            // Solo i valori letti da riga/tipo riga: i default (0/false/string.Empty,
            // IsOk = true) arrivano già da inizializzatori + this().
            Line = c.Line;
            Item = c.Servizio;
            Description = c.Descrizione;
            UoM = c.Um;
            ValUnit = Math.Round(c.ValUnitIstat ?? 0, DomainConstants.DecimaliValoreUnitario);
            DataDecorrenza = c.DataDecorrenza ?? d;
            DataProssimaFattura = nextDate;
            DataProssimaFatturaCampagna = nextDate;
            DataPrevistaConsegna = nextDate;
            DataConfermaConsegna = nextDate;
            QtaOrdine = Math.Round(c.Qta ?? 0, DomainConstants.DecimaliPercentuale);
            QtaCorrente = Math.Round(c.Qta ?? 0, DomainConstants.DecimaliPercentuale);
            QtaFranchigia = Math.Round(c.Franchigia ?? 0, DomainConstants.DecimaliPercentuale);
            DataProssimaRifatturazione = d;
            DataCessazioneDaAttivita = d;
            CanoniDataIn = d;
            CanoniDataFin = d;
            PeriodoDataIn = d;
            PeriodoDataFin = d;

            var tipoRiga = c.AlltipoRigaServizio;
            TipologiaServizio = (FatturazioneContratti.Common.TipologiaServizio)(tipoRiga.TipologiaServizio ?? 0);
            IsCanone = TipologiaServizio == FatturazioneContratti.Common.TipologiaServizio.Canone;
            IsConsuntivo = TipologiaServizio == FatturazioneContratti.Common.TipologiaServizio.Consuntivo;
            IsUnaTantum = TipologiaServizio == FatturazioneContratti.Common.TipologiaServizio.UnaTantum;
            IsAConsumo = TipologiaServizio == FatturazioneContratti.Common.TipologiaServizio.AConsumo;
            IsContinuativo = TipologiaServizio == FatturazioneContratti.Common.TipologiaServizio.Continuativo;
            // ATTENZIONE - punto da verificare: il VB fa CBool(c.AlltipoRigaServizio.Campagna)
            // su un campo string. CBool in VB accetta "True"/"False" (case-insensitive) e
            // stringhe numeriche (0 = False, tutto il resto = True). Qui riproduco la stessa
            // semantica, ma vale la pena controllare sui dati reali cosa contiene davvero
            // questo campo (es. "-1"/"0", "S"/"N", vuoto...) prima di fidarsi ciecamente.
            IsCampagna = VbStyleToBool(tipoRiga.Campagna);
            ContrattoFox = c.CodContratto;
            CodiceIntegra = c.CodIntegra;

            int iNrCanoni = 0;
            var periodicita = (FatturazioneContratti.Common.Periodo)(tipoRiga.Periodicita ?? 0);
            var cadenza = (FatturazioneContratti.Common.Cadenza)(tipoRiga.Cadenza ?? 0);

            // Canoni e valori a Consumo seguono il corso delle cadenze pertanto considero i mesi
            if (IsCanone || IsAConsumo || IsContinuativo)
            {
                if (IsContinuativo && !IsCampagna)
                {
                    iNrCanoni = 1;
                }
                else
                {
                    iNrCanoni = periodicita switch
                    {
                        FatturazioneContratti.Common.Periodo.Annuale => 12,
                        FatturazioneContratti.Common.Periodo.Semestrale => 6,
                        FatturazioneContratti.Common.Periodo.Quadrimestrale => 4,
                        FatturazioneContratti.Common.Periodo.Trimestrale => 3,
                        FatturazioneContratti.Common.Periodo.Bimestrale => 2,
                        FatturazioneContratti.Common.Periodo.Mensile => 1,
                        _ => 0,
                    };
                }

                // 28/02/2022: Devono sempre essere primo del mese / fine mese
                // 23/03/2022: Creo una nuova data per i POSTICIPATI che e' sempre fine mese
                //             e sempre il primo per gli ANTICIPATI
                if (cadenza == FatturazioneContratti.Common.Cadenza.Anticipato)
                {
                    var nextDateAnt = new DateTime(nextDate.Year, nextDate.Month, 1);
                    // Sempre il primo del mese
                    DataPrevistaConsegna = nextDateAnt;
                    DataConfermaConsegna = nextDateAnt;
                    CanoniDataIn = nextDateAnt;

                    // 28/11/2022 Le date devono sempre essere corrispondenti ai periodi.
                    // In caso di decorrenza 1/11 e trimestrale si fattura solo fino al 31/12
                    if (IsCampagna)
                        DataProssimaFatturaCampagna = nextDateAnt.AddMonths(iNrCanoni);

                    DateTime dta = CalcolaDataProssimaFattura(nextDateAnt.AddMonths(iNrCanoni), periodicita);
                    DataProssimaFattura = dta;
                    CanoniDataFin = dta.AddDays(-1);

                    // Per i continuativi non mi interessa
                    if (IsCanone || IsAConsumo)
                    {
                        QtaCorrente = CanoniDataFin.Year > CanoniDataIn.Year
                            ? 12 + CanoniDataFin.Month - CanoniDataIn.Month + 1
                            : CanoniDataFin.Month - CanoniDataIn.Month + 1;
                    }
                }
                else
                {
                    // Imposto l'ultimo giorno del mese
                    var nextDatePos = new DateTime(nextDate.Year, nextDate.Month, DateTime.DaysInMonth(nextDate.Year, nextDate.Month));
                    DataPrevistaConsegna = nextDatePos;
                    DataConfermaConsegna = nextDatePos;

                    // La data e' posticipata quindi devo sottrarre dei mesi.
                    // Prima aggiungo un giorno in modo da poter sottrarre correttamente.
                    var postPrimoGiorno = nextDatePos.AddDays(1).AddMonths(-iNrCanoni);
                    // Poi porto al primo (non dovrebbe essere necessario ma lo faccio)
                    postPrimoGiorno = new DateTime(postPrimoGiorno.Year, postPrimoGiorno.Month, 1);
                    CanoniDataIn = postPrimoGiorno;
                    CanoniDataFin = nextDatePos;

                    // Calcolo la data prossima fattura aggiungendo i mesi e andando all'ultimo giorno
                    var postUltimoGiorno = nextDatePos.AddMonths(iNrCanoni);
                    postUltimoGiorno = new DateTime(postUltimoGiorno.Year, postUltimoGiorno.Month, DateTime.DaysInMonth(postUltimoGiorno.Year, postUltimoGiorno.Month));
                    DataProssimaFattura = postUltimoGiorno;

                    if (IsCampagna)
                        DataProssimaFatturaCampagna = nextDatePos;
                }

                // Sui continuativi avendo solo un valore imposto data fattura/dataInizio
                if (IsContinuativo)
                    CanoniDataFin = CanoniDataIn;
            }
            else if (IsUnaTantum)
            {
                iNrCanoni = 1;
                CanoniDataIn = nextDate;
                CanoniDataFin = nextDate;
            }

            NrCanoniIniziale = iNrCanoni;
            PeriodoDataIn = CanoniDataIn;
            PeriodoDataFin = CanoniDataFin;
        }

        /// <summary>
        /// Popola la classe a partire da una riga di Servizio Aggiuntivo
        /// (ex New(s As AllordCliContrattoDistintaServAgg)).
        /// NOTA ereditata dal VB: "per ora lavora male non usare" — porting fedele,
        /// ma da validare/rivedere prima di usarla in produzione.
        /// ATTENZIONE: nessun flusso la usa e legge s.DataProssimaFatt, che è una delle
        /// due colonne deprecate (popolate solo dall'import Fox, mai mantenute: valore
        /// fermo all'import — vedi AGENTS.md, convenzione 14). Se in futuro si adotta
        /// questo costruttore, prima va riallineata quella data.
        /// </summary>
        public CurOrdRow(AllordCliContrattoDistintaServAgg s) : this()
        {
            DateTime nextDate = s.DataProssimaFatt ?? DomainConstants.DataNulla;
            DateTime d = DomainConstants.DataNulla;

            // Solo i valori letti da riga: i default (0/false/string.Empty,
            // IsOk = true) arrivano già da inizializzatori + this().
            Item = s.Servizio;
            Description = s.Descrizione;
            UoM = s.Um;
            ValUnit = Math.Round(s.ValUnitIstat ?? 0, DomainConstants.DecimaliValoreUnitario);
            DataDecorrenza = s.DataDecorrenza ?? d;
            DataProssimaFattura = nextDate;
            DataProssimaFatturaCampagna = nextDate;
            DataPrevistaConsegna = nextDate;
            DataConfermaConsegna = nextDate;
            QtaOrdine = Math.Round(s.Qta ?? 0, DomainConstants.DecimaliPercentuale);
            QtaCorrente = Math.Round(s.Qta ?? 0, DomainConstants.DecimaliPercentuale);
            QtaFranchigia = Math.Round(s.Franchigia ?? 0, DomainConstants.DecimaliPercentuale);
            DataProssimaRifatturazione = d;
            CanoniDataIn = d;
            CanoniDataFin = d;
            PeriodoDataIn = d;
            PeriodoDataFin = d;
            TipologiaServizio = (FatturazioneContratti.Common.TipologiaServizio)(s.AlltipoRigaServizio.TipologiaServizio ?? 0);
            AnnoIntervento = s.AnnoIntervento ?? 0;
            NrInterventiFranchigia = s.NrInterventiInFranchigia ?? 0;
            NrInterventiMese = s.NrInterventiMese ?? 0;
            NrInterventiOltreFranchigia = s.NrInterventiOltreFranchigia ?? 0;

            // Imposto l'ultimo giorno del mese
            var nextDatePos = new DateTime(nextDate.Year, nextDate.Month, DateTime.DaysInMonth(nextDate.Year, nextDate.Month));
            DataPrevistaConsegna = nextDatePos;
            DataConfermaConsegna = nextDatePos;

            var postPrimoGiorno = nextDatePos.AddDays(1).AddMonths(-1);
            postPrimoGiorno = new DateTime(postPrimoGiorno.Year, postPrimoGiorno.Month, 1);
            CanoniDataIn = postPrimoGiorno;
            CanoniDataFin = nextDatePos;

            var postUltimoGiorno = nextDatePos.AddMonths(1);
            postUltimoGiorno = new DateTime(postUltimoGiorno.Year, postUltimoGiorno.Month, DateTime.DaysInMonth(postUltimoGiorno.Year, postUltimoGiorno.Month));
            DataProssimaFattura = postUltimoGiorno;

            PeriodoDataIn = CanoniDataIn;
            PeriodoDataFin = CanoniDataFin;
        }

        /// <summary>
        /// La data è sempre il primo del mese ed è già adeguata/nuova, occorre solo
        /// correggerla se esce dai range.
        /// </summary>
        private static DateTime CalcolaDataProssimaFattura(DateTime dataFine, FatturazioneContratti.Common.Periodo periodo)
        {
            int m = dataFine.Month;
            return periodo switch
            {
                FatturazioneContratti.Common.Periodo.Annuale => new DateTime(dataFine.Year, 1, 1),
                FatturazioneContratti.Common.Periodo.Semestrale =>
                    m >= 7 ? new DateTime(dataFine.Year, 7, 1) : new DateTime(dataFine.Year, 1, 1),
                FatturazioneContratti.Common.Periodo.Quadrimestrale =>
                    m >= 9 ? new DateTime(dataFine.Year, 9, 1)
                    : m >= 5 ? new DateTime(dataFine.Year, 5, 1)
                    : new DateTime(dataFine.Year, 1, 1),
                FatturazioneContratti.Common.Periodo.Trimestrale =>
                    m >= 10 ? new DateTime(dataFine.Year, 10, 1)
                    : m >= 7 ? new DateTime(dataFine.Year, 7, 1)
                    : m >= 4 ? new DateTime(dataFine.Year, 4, 1)
                    : new DateTime(dataFine.Year, 1, 1),
                FatturazioneContratti.Common.Periodo.Bimestrale =>
                    m >= 11 ? new DateTime(dataFine.Year, 11, 1)
                    : m >= 9 ? new DateTime(dataFine.Year, 9, 1)
                    : m >= 7 ? new DateTime(dataFine.Year, 7, 1)
                    : m >= 5 ? new DateTime(dataFine.Year, 5, 1)
                    : m >= 3 ? new DateTime(dataFine.Year, 3, 1)
                    : new DateTime(dataFine.Year, 1, 1),
                FatturazioneContratti.Common.Periodo.Mensile => dataFine,
                _ => dataFine,
            };
        }

        public void AnnullaDaAttivita(DateTime dataAnnullamento)
        {
            HaAnnullatoDaAttivita = true;
            DataCessazioneDaAttivita = dataAnnullamento;
        }

        /// <summary>Riproduce la semantica di VB.NET CBool(String) per un campo che nel VB veniva convertito così.</summary>
        private static bool VbStyleToBool(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return false;
            if (bool.TryParse(value, out var b)) return b;
            if (double.TryParse(value, out var n)) return n != 0;
            return false;
        }
    }
}
