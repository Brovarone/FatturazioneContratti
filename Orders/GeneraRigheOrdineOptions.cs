using FatturazioneContratti.Config;
using System;

namespace FatturazioneContratti.Orders
{
    /// <summary>
    /// Opzioni di esecuzione di <see cref="GenerazioneRigheOrdiniService.GeneraRigheOrdine"/>:
    /// raggruppa filtri, dimensione blocco e callback di log/stato/progress in un unico
    /// parametro, evitando le lunghe firme a parametri opzionali.
    /// </summary>
    public sealed record GeneraRigheOrdineOptions(
        FiltriOrdini Filtri,
        int DimensioneBlocco = 100,
        Action<string, AppConfig.LogTarget>? LogMessage = null,
        Action<string>? UpdateStatus = null,
        ProgressCallbacks? ProgressBlocchi = null,
        ProgressCallbacks? ProgressOrdini = null);
}
