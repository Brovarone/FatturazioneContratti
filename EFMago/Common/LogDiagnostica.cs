using FatturazioneContratti.Data;
using System;
using System.Text;

namespace FatturazioneContratti.Common
{
    /// <summary>
    /// Routing dei log in un punto solo (decisioni del 16/10/2026, vedi TODO §1b):
    /// - errori / anomalie / rollback  -> <see cref="LogTarget.Both"/> (sempre visibili)
    /// - filtri della run              -> <see cref="LogTarget.File"/> (solo file, sempre)
    /// - informativi (conteggi, riepiloghi di esito) -> <see cref="SqlTools.Options.LogMode"/>
    /// - statistiche bulk (dettaglio tabella per tabella) -> <see cref="LogTarget.File"/> (solo file, sempre)
    /// - debugging (tracciato lungo)  -> <see cref="LogTarget.File"/> solo se <see cref="SqlTools.Options.IsDebugging"/>
    ///
    /// I blocchi multi-riga, quando il target include la UI, vanno emessi riga per riga:
    /// il ListBox di FLogin non rende i '\n' e l'elemento risulta illeggibile.
    /// Quando il target è solo file, il blocco parte in un colpo solo (più veloce e
    /// preserva le righe vuote di separazione).
    /// </summary>
    public static class LogDiagnostica
    {
        /// <summary>Messaggio informativo: segue la modalità scelta dall'utente in FSettings.</summary>
        public static void Informativo(Action<string, LogTarget>? log, string messaggio)
            => log?.Invoke(messaggio, SqlTools.Options.LogMode);

        /// <summary>Errore/anomalia: sempre su UI e file, qualunque sia LogMode.</summary>
        public static void Errore(Action<string, LogTarget>? log, string messaggio)
            => log?.Invoke(messaggio, LogTarget.Both);

        /// <summary>
        /// Eccezione: riga sintetica (messaggio) su UI+file, intero oggetto con stack trace
        /// sul file — così il log di sessione resta analizzabile senza sporcare la UI.
        /// </summary>
        public static void Errore(Action<string, LogTarget>? log, Exception? ex)
        {
            if (log == null || ex == null) return;
            log(ex.Message, LogTarget.Both);
            log(ex.ToString(), LogTarget.File);
        }

        /// <summary>Avviso operativo (es. "controllare canoni!"): trattato come errore, cioè visibile sempre.</summary>
        public static void Avviso(Action<string, LogTarget>? log, string messaggio)
            => log?.Invoke(messaggio, LogTarget.Both);

        /// <summary>
        /// Emette i filtri della run **solo su file** (testo intero in un colpo solo):
        /// è il registro completo della run, non serve a video e con LogMode = Ui
        /// inondava il ListBox. Ricostruisce sempre il testo, così il servizio non
        /// dipende dalla chiamata fatta dal form.
        /// </summary>
        public static void LogFiltri(Action<string, LogTarget>? log, ILogFiltri? filtri)
        {
            if (log == null || filtri == null) return;
            filtri.CostruisciFiltriLog();
            Emetti(log, filtri.LogFiltri.ToString(), LogTarget.File);
        }

        /// <summary>
        /// Scarica gli accumulatori di fine workflow secondo la regola di routing.
        /// Tutti i parametri sono opzionali e null-passabili; i blocchi vuoti vengono ignorati.
        /// </summary>
        public static void Scarica(
            Action<string, LogTarget>? log,
            StringBuilder? errori = null,
            StringBuilder? avvisi = null,
            StringBuilder? bulk = null,
            StringBuilder? debugging = null)
        {
            if (log == null) return;

            if (errori != null && errori.Length > 0)
                Emetti(log, errori.ToString(), LogTarget.Both);

            if (avvisi != null && avvisi.Length > 0)
                Emetti(log, avvisi.ToString(), LogTarget.Both);

            // Statistiche bulk (una riga per tabella): dettaglio tecnico, va solo sul file.
            // A video l'utente vuole la sintesi di esito, che il servizio emette con Informativo.
            if (bulk != null && bulk.Length > 0)
                log(bulk.ToString(), LogTarget.File);

            // Tracciato diagnostico voluminoso: solo file, solo in modalità debug.
            if (debugging != null && debugging.Length > 0 && SqlTools.Options.IsDebugging)
                log(debugging.ToString(), LogTarget.File);
        }

        /// <summary>Emissione di un blocco di testo rispettando la regola UI-riga-per-riga.</summary>
        private static void Emetti(Action<string, LogTarget>? log, string testo, LogTarget target)
        {
            if (log == null || string.IsNullOrEmpty(testo)) return;

            bool includeUi = target == LogTarget.Ui || target == LogTarget.Both;
            if (!includeUi)
            {
                log(testo, target);
                return;
            }

            foreach (var riga in testo.Split('\n'))
            {
                var r = riga.TrimEnd('\r');
                if (r.Trim().Length == 0) continue;   // righe vuote: inutili nel ListBox
                log(r, target);
            }
        }
    }
}
