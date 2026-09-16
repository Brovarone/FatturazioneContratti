using System;
using System.Windows.Forms;
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
            // Inizializza DPI, stili visivi e font (standard per .NET 8 WinForms)
            ApplicationConfiguration.Initialize();

            // Qui indichi quale Form mostrare all'avvio:
            Application.Run(new FLogin());
        }
    }
}