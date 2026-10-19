namespace FatturazioneContratti.Common
{
    /// <summary>
    /// Destinazione di un messaggio di log.
    /// Enum spostato qui da <c>AppConfig.LogTarget</c> durante l'estrazione della libreria
    /// EFMago: la libreria non può dipendere da Config (che resta nel progetto WinForms).
    /// </summary>
    public enum LogTarget
    {
        Ui = 0,
        File = 1,
        Both = 2
    }
}
