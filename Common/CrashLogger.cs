using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using FatturazioneContratti.Config;

namespace FatturazioneContratti.Common
{
    /// <summary>
    /// Gestore centralizzato di crash non gestiti: scrive un file di log con
    /// stack trace completo e, ove possibile, informazioni di contesto, indipendentemente
    /// dal fatto che il crash avvenga sul thread UI, su thread di background o in un Task.
    /// </summary>
    public static class CrashLogger
    {
        public static string GetLogsDirectory()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(AppConfig.LastLogPath) && Directory.Exists(AppConfig.LastLogPath))
                    return AppConfig.LastLogPath;
            }
            catch
            {
                // Ignora: fallback sotto.
            }

            string fallback = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Brovarone Cristiano",
                "Logs");

            try { Directory.CreateDirectory(fallback); } catch { }

            return fallback;
        }

        public static void LogCrash(Exception? exception, string context)
        {
            try
            {
                string dir = GetLogsDirectory();
                try { Directory.CreateDirectory(dir); } catch { }

                string filePath = Path.Combine(dir, $"crash_{DateTime.Now:yyyyMMdd_HHmmss_fff}.log");

                var sb = new StringBuilder();
                sb.AppendLine("============================================================");
                sb.AppendLine("CRASH NON GESTITO");
                sb.AppendLine("============================================================");
                sb.AppendLine($"Timestamp      : {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");
                sb.AppendLine($"Contesto       : {context}");
                sb.AppendLine($"Thread         : {Thread.CurrentThread.ManagedThreadId} ({Thread.CurrentThread.Name})");
                sb.AppendLine($"IsBackground   : {Thread.CurrentThread.IsBackground}");
                try { sb.AppendLine($"Database in uso: {AppConfig.DatabaseInUse}"); } catch { }
                try { sb.AppendLine($"LogMode        : {AppConfig.LogMode}"); } catch { }
                try { sb.AppendLine($"Eseguibile     : {Application.ExecutablePath}"); } catch { }
                sb.AppendLine();
                sb.AppendLine("---- DETTAGLIO ECCEZIONE ----");
                sb.AppendLine(exception != null ? exception.ToString() : "(eccezione nulla)");

                if (exception is AggregateException agg)
                {
                    sb.AppendLine();
                    sb.AppendLine("---- INNER EXCEPTIONS ----");
                    foreach (Exception inner in agg.InnerExceptions)
                    {
                        sb.AppendLine(inner.ToString());
                        sb.AppendLine();
                    }
                }

                sb.AppendLine();
                sb.AppendLine("============================================================");
                sb.AppendLine("FINE CRASH LOG");

                File.WriteAllText(filePath, sb.ToString());
            }
            catch (Exception ex)
            {
                // Ultima spiaggia: almeno in DebugOutput / trace.
                try { Debug.WriteLine($"[CrashLogger] impossibile scrivere crash log: {ex}"); } catch { }
                try { Trace.WriteLine($"[CrashLogger] impossibile scrivere crash log: {ex}"); } catch { }
            }
        }
    }
}