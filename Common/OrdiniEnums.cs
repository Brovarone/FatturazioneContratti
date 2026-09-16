namespace FatturazioneContratti.Common
{
    /// <summary>
    /// Porting degli enum definiti in Ordini.vb (righe ~15-38 di 3tables/Ordini/Ordini.vb).
    /// I valori numerici sono i codici "anagrafica" di Mago (non modificarli: sono chiavi
    /// esterne verso tabelle di sistema Mago, non semplici indici locali).
    /// </summary>
    public enum TipologiaServizio
    {
        Canone = 1276116992,
        Consuntivo = 1276116993,
        UnaTantum = 1276116994,
        AConsumo = 1276116995,
        Continuativo = 1276116996,
    }

    public enum Cadenza
    {
        Anticipato = 2009399296,
        Posticipato = 2009399297,
    }

    public enum Periodo
    {
        Annuale = 1094254592,
        Mensile = 1094254593,
        Bimestrale = 1094254594,
        Trimestrale = 1094254595,
        Quadrimestrale = 1094254596,
        Semestrale = 1094254598,
        Variabile = 1094254692,
        Nessuno = 1094254597,
    }

    public enum Divisione
    {
        Nessuna = 1377239040,
        Tecnologia = 1377239041,
        Vigilanza = 1377239042,
    }
}
