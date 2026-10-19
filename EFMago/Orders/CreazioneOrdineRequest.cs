using FatturazioneContratti.Common;
using System;
using System.Collections.Generic;

namespace FatturazioneContratti.Orders
{
    /// <summary>
    /// Riga da creare in un nuovo ordine (sito web di sola scrittura ordini).
    /// I valori non impostati vengono lasciati ai default Mago o copiati dalla
    /// configurazione utente / anagrafica cliente (vedi <see cref="CreazioneOrdiniService"/>).
    /// </summary>
    public sealed record RigaOrdineCreazione(
        string? Descrizione = null,
        string? Item = null,
        double Quantita = 1,
        double ValoreUnitario = 0,
        string? UoM = null,
        LineType TipoRiga = LineType.Merce,
        string? CodIva = null,          // default: codice IVA di default del database (MaTaxCodesDefaults)
        string? Contropartita = null,   // Offset (MA_SaleOrdDetails.Offset, max 16)
        string? Commessa = null,        // Job (max 10)
        string? CentroCosto = null,     // CostCenter (max 8)
        string? CIG = null,             // ContractCode (max 15)
        string? CUP = null,             // ProjectCode (max 16)
        string? Nota = null,            // Notes riga (max 32)
        DateTime? DataPrevistaConsegna = null,
        DateTime? DataConfermaConsegna = null);

    /// <summary>
    /// Richiesta di creazione di un nuovo ordine cliente (testata MaSaleOrd + righe da zero).
    /// </summary>
    public sealed record CreazioneOrdineRequest(
        string Cliente,
        DateTime DataOrdine,
        IReadOnlyList<RigaOrdineCreazione> Righe,
        string? ClienteFatturazione = null,
        string? Pagamento = null,
        string? Listino = null,
        string? Valuta = null,
        string? Sede = null,             // OurReference (max 32), es. "05"/"08"/"09"/"12"
        string? RegistroIva = null,      // TaxJournal (max 8); default: "VEN"
        string? Note = null,             // note testata -> MA_SaleOrdNotes (max 251)
        int? Divisione = null,           // default ERP: MaDivision (1377239040)
        int? TipoVendita = null);        // default ERP: SaleType 3670020

    /// <summary>Esito positivo della creazione: ID ordine assegnato e numerazione Mago.</summary>
    public sealed record EsitoCreazioneOrdine(int SaleOrdId, string InternalOrdNo, int NumeroRighe);
}
