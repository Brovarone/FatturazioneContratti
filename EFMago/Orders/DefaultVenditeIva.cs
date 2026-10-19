using EFMago.Models;
using System.Collections.Generic;

namespace FatturazioneContratti.Orders
{
    /// <summary>
    /// Default ERP di vendita/IVA caricati una sola volta per elaborazione:
    /// contropartita di vendita, codice IVA e percentuale di default, più l'elenco
    /// completo dei codici IVA (usato per la ricerca per riga di contratto).
    /// </summary>
    public sealed record DefaultVenditeIva(
        string Contropartita,
        string CodIva,
        double PercIva,
        List<MaTaxCodes> CodiciIva);
}
