using System;
using System.Windows.Forms;
using FatturazioneContratti.Common;
using FatturazioneContratti.Config;
using FatturazioneContratti.Forms; // <-- Assicurati che corrisponda al namespace della tua FLogin

namespace FatturazioneContratti
{
    internal static class Program
    {
        /// <summary>
        /// Punto di ingresso principale dell'applicazione.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Carica la configurazione il prima possibile: serve per determinare
            // la cartella dei log già in caso di crash all'avvio.
            try { AppConfig.Load(); }
            catch { /* proseguiamo comunque con i valori di default */ }

            // Registra i gestori globali di eccezioni non gestite.
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

            Application.ThreadException += (sender, args) =>
            {
                CrashLogger.LogCrash(args.Exception, "Application.ThreadException (thread UI)");
                try
                {
                    MessageBox.Show(
                        "Si è verificato un errore non gestito.\n" +
                        "Dettagli salvati nel file di crash log.\n\n" + args.Exception.Message,
                        "Errore non gestito",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
                catch { /* non bloccare in caso di fallimento della MessageBox */ }
            };

            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                var ex = args.ExceptionObject as Exception;
                CrashLogger.LogCrash(ex, $"AppDomain.UnhandledException (IsTerminating={args.IsTerminating})");
            };

            TaskScheduler.UnobservedTaskException += (sender, args) =>
            {
                CrashLogger.LogCrash(args.Exception, "TaskScheduler.UnobservedTaskException");
                // Marca l'eccezione come osservata per evitare il crash fatale del processo.
                args.SetObserved();
            };

            // Inizializza DPI, stili visivi e font (standard per .NET 8 WinForms)
            ApplicationConfiguration.Initialize();

            try
            {
                // Qui indichi quale Form mostrare all'avvio:
                Application.Run(new FLogin());
            }
            catch (Exception ex)
            {
                CrashLogger.LogCrash(ex, "Main / Application.Run");
                try
                {
                    MessageBox.Show(
                        "Errore fatale all'avvio.\nDettagli salvati nel file di crash log.\n\n" + ex.Message,
                        "Errore fatale",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
                catch { }
            }
        }
    }
}