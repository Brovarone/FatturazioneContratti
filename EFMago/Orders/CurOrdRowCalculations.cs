using System;
using EFMago.Models;
using FatturazioneContratti.Common;

namespace FatturazioneContratti.Orders
{
    /// <summary>
    /// Porting 1:1 delle funzioni pure di calcolo di Ordini.vb (righe 1272-1483):
    /// il motore che decide se una riga contratto cade nel periodo di fatturazione,
    /// se è stata sospesa/annullata/cessata nel frattempo, e quanti canoni residuano.
    ///
    /// Nel VB erano funzioni private di modulo che ricevevano "range As CurOrdRow" e lo
    /// mutavano (ByVal su un tipo reference = mutazione visibile al chiamante, come in C#).
    /// Qui le ho rese metodi di estensione su CurOrdRow: stessa semantica, ma il punto di
    /// chiamata (cOrdRow.IsBetweenSospensione(...)) rende esplicito chi viene modificato,
    /// cosa che nel VB era implicita e facile da perdere leggendo ScriviOrdini.
    ///
    /// Nessuna logica di business è stata alterata: stesse soglie, stessi rami, stessi
    /// commenti storici dove spiegavano una scelta non ovvia.
    /// </summary>
    public static class CurOrdRowCalculations
    {
        /// <summary>
        /// True se la riga contratto è un Canone la cui periodicità è tra quelle
        /// selezionate nel filtro (array a 7 posizioni, indici 1..6 usati: 1=mensile
        /// ... 6=annuale, indice 0 non usato — stessa convenzione "p_Periodi(6)" del VB).
        /// </summary>
        public static bool IsBewteenPeriodo(AllordCliContratto c, bool[] periodi)
        {
            var tipologia = (TipologiaServizio)(c.AlltipoRigaServizio.TipologiaServizio ?? 0);
            if (tipologia != TipologiaServizio.Canone) return false;

            var periodicita = (Periodo)(c.AlltipoRigaServizio.Periodicita ?? 0);
            return periodicita switch
            {
                Periodo.Annuale => periodi[6],
                Periodo.Semestrale => periodi[5],
                Periodo.Quadrimestrale => periodi[4],
                Periodo.Trimestrale => periodi[3],
                Periodo.Bimestrale => periodi[2],
                Periodo.Mensile => periodi[1],
                _ => false,
            };
        }

        /// <summary>True se <paramref name="data"/> cade nel periodo canoni della riga; calcola i canoni residui.</summary>
        public static bool IsBetweenAnnullamento(this CurOrdRow range, DateTime data, out double canoniResidui)
        {
            canoniResidui = 0;
            if (data >= range.CanoniDataIn && data <= range.CanoniDataFin)
            {
                canoniResidui = CalcolaMesi(range.CanoniDataIn, data, false);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gestisce l'annullamento derivante da un'attività di cessazione: aggiorna
        /// QtaCorrente/QtaAnnullata/IsOk sulla riga e produce un messaggio descrittivo.
        /// </summary>
        public static bool IsBetweenAnnullamentoAttivita(this CurOrdRow range, out string message)
        {
            message = string.Empty;
            bool result = false;

            if (range.DataCessazioneDaAttivita >= range.CanoniDataIn && range.DataCessazioneDaAttivita <= range.CanoniDataFin)
            {
                result = true;
                double canoniResidui = CalcolaMesi(range.CanoniDataIn, range.DataCessazioneDaAttivita, false);

                if (canoniResidui > 0)
                {
                    range.QtaCorrente = canoniResidui - range.QtaOrdine + range.QtaCorrente;
                    range.QtaAnnullata = range.QtaOrdine - canoniResidui;
                    message = "  - Annullamento Parziale";
                }
                else
                {
                    range.QtaCorrente = 0;
                    range.QtaAnnullata = range.QtaOrdine;
                    range.IsOk = false;
                    message = "  - Annullamento Parziale -> Totale";
                }
            }
            else if (range.DataCessazioneDaAttivita != DomainConstants.DataNulla && range.DataCessazioneDaAttivita < range.CanoniDataIn)
            {
                // Completamente annullata
                result = true;
                range.PrecendementeCessato = true;
                range.QtaAnnullata = range.QtaOrdine;
                range.IsOk = false;
                message = " - [ESCLUSA] Precedentemente Cessato";
            }
            return result;
        }

        /// <summary>Gestisce la cessazione testa ordine (ALLOrdCliAcc.DataCessazione) sulla riga corrente.</summary>
        public static bool IsBetweenAnnullamentoCessazione(this CurOrdRow range, out string message)
        {
            double canoniResidui = 0;
            bool result = false;

            if (range.DataCessazione >= range.CanoniDataIn && range.DataCessazione <= range.CanoniDataFin)
            {
                result = true;
                canoniResidui = CalcolaMesi(range.CanoniDataIn, range.DataCessazione, false);
            }

            if (canoniResidui > 0)
            {
                range.QtaCorrente = canoniResidui - range.QtaOrdine + range.QtaCorrente;
                message = "  - Cessato";
            }
            else
            {
                range.QtaCorrente = 0;
                range.IsOk = false;
                message = "  - Cessato -> Totale";
            }
            return result;
        }

        /// <summary>Gestisce la scadenza fissa testa ordine (ALLOrdCliAcc.DataScadenzaFissa) sulla riga corrente.</summary>
        public static bool IsBetweenAnnullamentoScadenzaFissa(this CurOrdRow range, out string message)
        {
            double canoniResidui = 0;
            bool result = false;

            if (range.DataScadenzaFissa >= range.CanoniDataIn && range.DataScadenzaFissa <= range.CanoniDataFin)
            {
                result = true;
                canoniResidui = CalcolaMesi(range.CanoniDataIn, range.DataScadenzaFissa, false);
            }

            if (canoniResidui > 0)
            {
                range.QtaCorrente = canoniResidui - range.QtaOrdine + range.QtaCorrente;
                message = "  - Scadenza Fissa";
            }
            else
            {
                range.QtaCorrente = 0;
                range.IsOk = false;
                message = "  - Scadenza Fissa -> Totale";
            }
            return result;
        }

        /// <summary>
        /// Gestisce una sospensione (dataInizioSosp/dataFineSosp) che intercetta il periodo
        /// canoni della riga, coprendo sia il caso "infraperiodo" sia i casi limite in cui le
        /// date di sospensione escono dal periodo (es. sospeso "a tempo indeterminato").
        /// </summary>
        public static bool IsBetweenSospensione(this CurOrdRow range, DateTime dataInizioSosp, DateTime dataFineSosp)
        {
            if (dataInizioSosp >= range.CanoniDataFin) return false;
            if (dataFineSosp <= range.CanoniDataIn) return false;

            double canoniSospesi = 0;
            bool result;

            bool inizioDentro = dataInizioSosp >= range.CanoniDataIn && dataInizioSosp <= range.CanoniDataFin;
            bool fineDentro = dataFineSosp >= range.CanoniDataIn && dataFineSosp <= range.CanoniDataFin;

            if (inizioDentro && fineDentro)
            {
                // Caso di sospensione infraperiodo (es: 1 mese sui trimestrali)
                result = true;
                canoniSospesi = CalcolaMesi(dataInizioSosp, dataFineSosp, false);
            }
            else
            {
                result = false;

                // Spesso invece vengono indicate date a cavallo del periodo di fatturazione
                // (esempio sospeso a tempo indeterminato 31/12/xxxx)

                // Data Inizio antecedente al periodo (con Data Fine compresa)
                if (dataInizioSosp < range.CanoniDataIn && fineDentro)
                {
                    result = true;
                    canoniSospesi = CalcolaMesi(range.CanoniDataIn, dataFineSosp, false);
                }

                // (Data Inizio compresa) con Data Fine successiva al periodo [Sospensione Indeterminata]
                if (inizioDentro && dataFineSosp > range.CanoniDataFin)
                {
                    result = true;
                    canoniSospesi = CalcolaMesi(dataInizioSosp, range.CanoniDataFin, true);
                }

                // Entrambe le date oltre le date periodo
                if (dataInizioSosp < range.CanoniDataIn && dataFineSosp > range.CanoniDataFin)
                {
                    result = true;
                    canoniSospesi = range.NrCanoniIniziale;
                }
            }

            range.QtaSospesa = canoniSospesi;

            if (result)
            {
                range.SospesoDaAttivita = true;
                if (canoniSospesi == range.QtaOrdine) range.IsOk = false;
                if (canoniSospesi > 0) range.QtaCorrente -= canoniSospesi;
            }

            return result;
        }

        /// <summary>True se la data di rifatturazione ISTAT dell'attività cade nel periodo canoni della riga.</summary>
        public static bool IsBetweenIstatCanoni(this CurOrdRow range, AllordCliAttivita a)
        {
            var dataRifatt = a.DataRifatturazione ?? DomainConstants.DataNulla;
            return dataRifatt >= range.CanoniDataIn && dataRifatt <= range.CanoniDataFin;
        }

        /// <summary>True se la data di rifatturazione ISTAT dell'attività cade nel range [da, al] indicato (filtro utente).</summary>
        public static bool IsBetweenIstat(AllordCliAttivita a, DateTime da, DateTime al)
        {
            var dataRifatt = a.DataRifatturazione ?? DomainConstants.DataNulla;
            return dataRifatt >= da && dataRifatt <= al;
        }

        /// <summary>
        /// DEPRECATO 26/01/2022 nel VB originale: "Cambiato comportamento, cerco sempre se
        /// ci sono canoni da rifatturare e non solo all'avverarsi della condizione di
        /// fatturazione canone". Portata comunque perché ancora richiamata da altre parti
        /// del vecchio codice: verificare se ha ancora un chiamante reale prima di eliminarla.
        /// </summary>
        public static bool IsBetweenRipresiCanoni(this CurOrdRow range, AllordCliAttivita a, out double canoniRipresi)
        {
            canoniRipresi = 0;
            var dataRifatt = a.DataRifatturazione ?? DomainConstants.DataNulla;
            if (dataRifatt >= range.CanoniDataIn && dataRifatt <= range.CanoniDataFin)
            {
                canoniRipresi = CalcolaMesi(a.DataInizio ?? DomainConstants.DataNulla, a.DataFine ?? DomainConstants.DataNulla, false);
                return true;
            }
            return false;
        }

        public static bool IsBetweenRipresi(AllordCliAttivita a, DateTime da, DateTime al, out double canoniRipresi)
        {
            canoniRipresi = 0;
            var dataRifatt = a.DataRifatturazione ?? DomainConstants.DataNulla;
            if (dataRifatt >= da && dataRifatt <= al)
            {
                canoniRipresi = CalcolaMesi(a.DataInizio ?? DomainConstants.DataNulla, a.DataFine ?? DomainConstants.DataNulla, false);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Numero di mesi (frazionabile a metà) fra due date, con la logica originale:
        /// "1-8 = 0 // 9-23 = 0,5 // 24-31 = 1" applicata al giorno di una delle due date
        /// a seconda di <paramref name="checkGiorniSuPrimadata"/>.
        /// Porting 1:1 di CalcolaMesi (Ordini.vb): il calcolo mesi "di calendario" di
        /// VB.NET DateDiff(DateInterval.Month, d1, d2) è (anno2-anno1)*12 + (mese2-mese1),
        /// indipendente dal giorno — riprodotto qui esplicitamente invece di usare
        /// Microsoft.VisualBasic.DateAndTime (da evitare in .NET Core).
        /// </summary>
        public static double CalcolaMesi(DateTime d1, DateTime d2, bool checkGiorniSuPrimadata)
        {
            double monthNr;

            // Determino il fine mese del primo valore
            var firstEndDate = new DateTime(d1.Year, d1.Month, 1).AddMonths(1).AddDays(-1);

            if (firstEndDate == d1)
            {
                var d1Actual = firstEndDate.AddDays(1);
                monthNr = MonthDiff(d1Actual, d2);
            }
            else
            {
                monthNr = MonthDiff(d1, d2);
            }

            // Controllo su giorno come da specifiche
            if (checkGiorniSuPrimadata)
            {
                // Se considero la prima data il valore e' invertito
                monthNr += d1.Day switch
                {
                    >= 1 and <= 8 => 1,
                    >= 9 and <= 23 => 0.5,
                    _ => 0, // 24 a 31
                };
            }
            else
            {
                monthNr += d2.Day switch
                {
                    >= 1 and <= 8 => 0,
                    >= 9 and <= 23 => 0.5,
                    _ => 1, // 24 a 31
                };
            }

            return monthNr;
        }

        /// <summary>Equivalente di Microsoft.VisualBasic.DateAndTime.DateDiff(DateInterval.Month, d1, d2): differenza in mesi "di calendario", giorno ignorato.</summary>
        private static int MonthDiff(DateTime d1, DateTime d2) =>
            (d2.Year - d1.Year) * 12 + (d2.Month - d1.Month);
    }
}
