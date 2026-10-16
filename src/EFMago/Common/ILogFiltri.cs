using System.Text;

namespace FatturazioneContratti.Common
{
    /// <summary>
    /// Contratto per i DTO filtri che sanno descriversi nel log.
    /// Permette a <see cref="LogDiagnostica.LogFiltri"/> di emettere i parametri di una run
    /// in un punto solo: è il servizio a loggarli all'avvio, non il form.
    /// </summary>
    public interface ILogFiltri
    {
        StringBuilder LogFiltri { get; }
        void CostruisciFiltriLog();
    }
}
