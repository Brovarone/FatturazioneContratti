using EFMago.Common;
using System;
using System.Collections.Generic;

namespace EFMago.Orders
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
    /// Condizioni 1:1 del contratto (ALLOrdCliAcc).
    /// </summary>
    public sealed record CondizioniContrattoCreazione(
        bool ApplicoIstat = false,
        short? MesiDurata = null,
        short? MesiRinnovo = null,
        short? GgDisdetta = null,
        DateTime? DataScadenzaFissa = null,
        DateTime? DataRiscatto = null,
        double? ImportoRiscatto = null,
        DateTime? DataRiduzione = null,
        double? ImportoRiduzione = null,
        double? PercRiduzione = null,
        DateTime? DataDecorrenza = null,
        DateTime? DataCessazione = null,
        string? MotivoCessazione = null,
        int? TipoContratto = null,
        bool ImpiantoProprietaCliente = false,
        double? ImportoCanone = null,
        double? ContributoInstallazione = null,
        bool OrdineSospeso = false,
        DateTime? DataSospensione = null,
        string? MotivoSospensione = null,
        string? Agente = null,
        double? ImportoProvvigione = null,
        string? Impianto = null,
        string? Nota = null,
        string? CondPag = null,
        string? SedeInvioDoc = null,
        int? ModelloContratto = null,
        string? CdC = null,
        string? Vettore = null,
        string? ImpiantoDue = null,
        DateTime? DataPrevistaScadenza = null);

    /// <summary>Riga di ALLOrdCliContratto, con i suoi figli.</summary>
    public sealed record RigaContrattoCreazione(
        string? Servizio = null,
        string? CodiceIva = null,
        string? Descrizione = null,
        double? Qta = null,
        string? Um = null,
        double? ValUnit = null,
        double? Franchigia = null,
        string? Nota = null,
        string? TipoRigaServizio = null,
        DateTime? DataDecorrenza = null,
        bool NonRiportaInFatt = false,
        DateTime? DataProssimaFatt = null,
        string? CodContratto = null,
        string? CodIntegra = null,
        string? Impianto = null,
        string? CdC = null,
        IReadOnlyList<RigaDistintaCreazione>? Distinta = null,
        IReadOnlyList<AttivitaContrattoCreazione>? Attivita = null,
        IReadOnlyList<DescrizioneFatturaCreazione>? DescrizioniFattura = null);

    /// <summary>Riga di ALLOrdCliContratto_Distinta.</summary>
    public sealed record RigaDistintaCreazione(
        string? Servizio = null,
        string? TipoRigaServizio = null,
        string? Descrizione = null,
        string? Um = null,
        double? Qta = null,
        DateTime? DataDecorrenza = null,
        DateTime? DataProssimaFatt = null,
        double? ValUnit = null,
        string? CodIntegra = null,
        string? CodContratto = null,
        string? Impianto = null,
        string? CdC = null,
        string? Nota = null,
        IReadOnlyList<ServizioAggiuntivoCreazione>? ServiziAggiuntivi = null,
        IReadOnlyList<string>? Cespiti = null);

    /// <summary>Riga di ALLOrdCliContratto_Distinta_ServAgg.</summary>
    public sealed record ServizioAggiuntivoCreazione(
        string? Servizio = null,
        string? TipoRigaServizio = null,
        string? Descrizione = null,
        string? Um = null,
        double? Qta = null,
        double? Franchigia = null,
        DateTime? DataDecorrenza = null,
        DateTime? DataProssimaFatt = null,
        double? ValUnit = null,
        int? Periodicita = null);

    /// <summary>Riga di ALLOrdCliAttivita.</summary>
    public sealed record AttivitaContrattoCreazione(
        string? Attivita = null,
        DateTime? DataInizio = null,
        DateTime? DataFine = null,
        bool DaFatturare = false,
        DateTime? DataRifatturazione = null,
        bool Fatturata = false,
        string? Nota = null,
        string? RifServizio = null,
        double? ValUnit = null,
        string? TestoFattura = null);

    /// <summary>Riga di ALLOrdCliContratto_DescFatt.</summary>
    public sealed record DescrizioneFatturaCreazione(
        string? Codice = null,
        string? Descrizione = null);

    /// <summary>Riga di ALLOrdCliDescrizioni (descrizioni a livello ordine).</summary>
    public sealed record DescrizioneOrdineCreazione(
        string? Codice = null,
        string? Descrizione = null);

    public sealed class FESezioneDati
    {
        public string IdDocumento { get; set; } = "";
        public DateTime? DataDocumento { get; set; }
        public string NumItem { get; set; } = "";
        public string Commessa { get; set; } = "";
        public string Cig { get; set; } = "";
        public string Cup { get; set; } = "";
    }

    /// <summary>Dati fattura elettronica, 1:1 con l'ordine.</summary>
    public sealed record FatturaElettronicaCreazione(
        string? Causale = null,
        string? CausaleRitenuta = null,
        FESezioneDati? DatiOrdineAcquisto  = null,
        FESezioneDati? DatiContratto = null,
        FESezioneDati? DatiConvenzione = null,
        FESezioneDati? DatiRicezione = null);

    /// <summary>Collegamento a un ordine padre/figlio già esistente.</summary>
    public sealed record CollegamentoOrdineCreazione(
        string? Tipo = null,
        string? Numero = null);

    /// <summary>Dati contratto da scrivere nelle tabelle ALLOrdCli*.</summary>
    public sealed record ContrattoCreazione(
        CondizioniContrattoCreazione Condizioni,
        IReadOnlyList<RigaContrattoCreazione> Righe,
        IReadOnlyList<string>? TipologieServizi = null,
        IReadOnlyList<DescrizioneOrdineCreazione>? DescrizioniOrdine = null,
        FatturaElettronicaCreazione? FatturaElettronica = null,
        IReadOnlyList<CollegamentoOrdineCreazione>? Collegamenti = null);

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
        int? TipoVendita = null,         // default ERP: SaleType 3670020
        string? Agente = null,           // Salesperson (max 8)
        string? CentroCosto = null,      // CostCenter (max 8)
        ContrattoCreazione? Contratto = null);

    /// <summary>Esito positivo della creazione: ID ordine assegnato e numerazione Mago.</summary>
    public sealed record EsitoCreazioneOrdine(
        int SaleOrdId,
        string InternalOrdNo,
        int NumeroRighe,
        IReadOnlyList<string>? Avvisi = null);
}
