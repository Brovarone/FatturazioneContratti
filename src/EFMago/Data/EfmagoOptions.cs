using FatturazioneContratti.Common;

namespace FatturazioneContratti.Data
{
    /// <summary>
    /// Opzioni di funzionamento della libreria EFMago, impostate dall'applicazione ospitante
    /// tramite <see cref="SqlTools.Configure(System.Func{EfmagoOptions})"/>.
    /// Ogni programma (WinForms con AppConfig, sito con appsettings.json) fornisce i propri
    /// valori: la libreria non sa da dove provengono.
    /// </summary>
    public sealed class EfmagoOptions
    {
        public string SqlServer { get; set; } = "";
        public string UserId { get; set; } = "";
        public string Password { get; set; } = "";

        /// <summary>Database attivo già risolto dall'ospite (produzione/test, Uno/Spa).</summary>
        public string DatabaseInUse { get; set; } = "";

        /// <summary>Abilita EF Core detailed errors + sensitive data logging (e la scansione diagnostica nei servizi).</summary>
        public bool IsDebugging { get; set; }

        /// <summary>Login ID corrente (operatore Mago o utente dell'applicazione ospitante).</summary>
        public int LoginId { get; set; } = 1;

        /// <summary>Modalità di log corrente (Ui / File / Both).</summary>
        public LogTarget LogMode { get; set; } = LogTarget.Both;
    }
}
