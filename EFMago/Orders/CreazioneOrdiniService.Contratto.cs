using EFMago.Common;
using EFMago.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EFMago.Orders
{
    /// <summary>
    /// Scrittura delle tabelle contratto collegate all'ordine (ALLOrdCli*).
    /// Le entità vengono preparate con i default Mago tramite <c>AddNew</c> e poi
    /// inserite con le stesse bulk della testata, dentro la stessa transazione.
    /// </summary>
    public partial class CreazioneOrdiniService
    {
        private const string TipoRigaServizioSupplementare = "SUPPLEMENTAR";

        /// <summary>
        /// Controlla che i codici del contratto esistano nelle anagrafiche Mago. Le tabelle
        /// ALLOrdCli* non hanno FK a livello di database, ma il tipo riga servizio guida la
        /// generazione canoni e l'articolo è l'anagrafica di riferimento di Mago: un codice
        /// inesistente produrrebbe un contratto formalmente scritto ma non elaborabile.
        /// Restituisce null se non c'è nulla da segnalare.
        /// </summary>
        private StringBuilder? ValidaAnagraficheContratto(ContrattoCreazione? contratto)
        {
            if (contratto?.Righe is null || contratto.Righe.Count == 0)
                return null;

            var servizi = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var tipiRiga = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var attivita = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var codiciIva = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            static void Aggiungi(HashSet<string> set, string? valore)
            {
                if (!string.IsNullOrWhiteSpace(valore))
                    set.Add(valore.Trim());
            }

            foreach (RigaContrattoCreazione r in contratto.Righe)
            {
                Aggiungi(servizi, r.Servizio);
                Aggiungi(tipiRiga, r.TipoRigaServizio);
                Aggiungi(codiciIva, r.CodiceIva);

                foreach (RigaDistintaCreazione d in r.Distinta ?? Array.Empty<RigaDistintaCreazione>())
                {
                    Aggiungi(servizi, d.Servizio);
                    Aggiungi(tipiRiga, string.IsNullOrWhiteSpace(d.TipoRigaServizio) ? r.TipoRigaServizio : d.TipoRigaServizio);

                    foreach (ServizioAggiuntivoCreazione sa in d.ServiziAggiuntivi ?? Array.Empty<ServizioAggiuntivoCreazione>())
                        Aggiungi(servizi, sa.Servizio);
                }

                foreach (AttivitaContrattoCreazione a in r.Attivita ?? Array.Empty<AttivitaContrattoCreazione>())
                    Aggiungi(attivita, a.Attivita);
            }

            var errori = new StringBuilder();

            if (servizi.Count > 0)
            {
                var esistenti = _context.MaItems.AsNoTracking()
                    .Where(i => servizi.Contains(i.Item))
                    .Select(i => i.Item)
                    .ToList();
                var mancanti = servizi.Except(esistenti, StringComparer.OrdinalIgnoreCase).ToList();
                if (mancanti.Count > 0)
                    errori.AppendLine($"Servizi inesistenti in anagrafica articoli (MA_Items): {string.Join(", ", mancanti)}.");
            }

            if (tipiRiga.Count > 0)
            {
                var esistenti = _context.AlltipoRigaServizio.AsNoTracking()
                    .Where(t => tipiRiga.Contains(t.TipoRigaServizio))
                    .Select(t => t.TipoRigaServizio)
                    .ToList();
                var mancanti = tipiRiga.Except(esistenti, StringComparer.OrdinalIgnoreCase).ToList();
                if (mancanti.Count > 0)
                    errori.AppendLine($"Tipi riga servizio inesistenti (ALLTipoRigaServizio): {string.Join(", ", mancanti)}.");
            }

            if (attivita.Count > 0)
            {
                var esistenti = _context.Allattivita.AsNoTracking()
                    .Where(a => attivita.Contains(a.Attivita))
                    .Select(a => a.Attivita)
                    .ToList();
                var mancanti = attivita.Except(esistenti, StringComparer.OrdinalIgnoreCase).ToList();
                if (mancanti.Count > 0)
                    errori.AppendLine($"Attività inesistenti (ALLAttivita): {string.Join(", ", mancanti)}.");
            }

            if (codiciIva.Count > 0)
            {
                var esistenti = _context.MaTaxCodes.AsNoTracking()
                    .Where(t => codiciIva.Contains(t.TaxCode))
                    .Select(t => t.TaxCode)
                    .ToList();
                var mancanti = codiciIva.Except(esistenti, StringComparer.OrdinalIgnoreCase).ToList();
                if (mancanti.Count > 0)
                    errori.AppendLine($"Codici IVA inesistenti (MA_TaxCodes): {string.Join(", ", mancanti)}.");
            }

            var tipologie = new HashSet<string>(
                (contratto.TipologieServizi ?? Array.Empty<string>())
                    .Where(t => !string.IsNullOrWhiteSpace(t))
                    .Select(t => t.Trim()),
                StringComparer.OrdinalIgnoreCase);

            if (tipologie.Count > 0)
            {
                var esistenti = _context.MaItemTypes.AsNoTracking()
                    .Where(t => tipologie.Contains(t.CodeType))
                    .Select(t => t.CodeType)
                    .ToList();
                var mancanti = tipologie.Except(esistenti, StringComparer.OrdinalIgnoreCase).ToList();
                if (mancanti.Count > 0)
                    errori.AppendLine($"Tipologie servizi inesistenti (MA_ItemTypes): {string.Join(", ", mancanti)}.");
            }

            return errori.Length == 0 ? null : errori;
        }

        private sealed class ContrattoCreato
        {
            public AllordCliAcc? Acc { get; set; }
            public AllordCliFattEle? FattEle { get; set; }

            public List<AllordCliContratto> Contratti { get; } = new();
            public List<AllordCliContrattoDistinta> Distinte { get; } = new();
            public List<AllordCliContrattoDistintaServAgg> ServiziAggiuntivi { get; } = new();
            public List<AllordCliContrattoDistCesp> Cespiti { get; } = new();
            public List<Allcespiti> CespitiOrdine { get; } = new();
            public List<AllordCliAttivita> Attivita { get; } = new();
            public List<AllordCliContrattoDescFatt> DescrizioniFattura { get; } = new();
            public List<AllordCliDescrizioni> DescrizioniOrdine { get; } = new();
            public List<AllordCliTipologiaServizi> Tipologie { get; } = new();
            public List<AllordPadre> Padri { get; } = new();
            public List<AllordFiglio> Figli { get; } = new();

            public void AssegnaIdOrdine(int idOrdCli)
            {
                if (Acc is not null) Acc.IdOrdCli = idOrdCli;
                if (FattEle is not null) FattEle.IdOrdCli = idOrdCli;

                foreach (var e in Contratti) e.IdOrdCli = idOrdCli;
                foreach (var e in Distinte) e.IdOrdCli = idOrdCli;
                foreach (var e in ServiziAggiuntivi) e.IdOrdCli = idOrdCli;
                foreach (var e in Cespiti) e.IdOrdCli = idOrdCli;
                foreach (var e in CespitiOrdine) e.IdOrdCli = idOrdCli;
                foreach (var e in Attivita) e.IdOrdCli = idOrdCli;
                foreach (var e in DescrizioniFattura) e.IdOrdCli = idOrdCli;
                foreach (var e in DescrizioniOrdine) e.IdOrdCli = idOrdCli;
                foreach (var e in Tipologie) e.IdOrdCli = idOrdCli;
                foreach (var e in Padri) e.IdOrdCli = idOrdCli;
                foreach (var e in Figli) e.IdOrdCli = idOrdCli;
            }
        }

        /// <summary>
        /// Costruisce le entità ALLOrdCli* dai dati della richiesta. Non scrive nulla:
        /// l'IdOrdCli viene assegnato solo dopo l'allocazione del contatore ordine.
        /// </summary>
        private ContrattoCreato? CostruisciContratto(
            CreazioneOrdineRequest request,
            Action<string, LogTarget>? logMessage,
            List<string> avvisi)
        {
            ContrattoCreazione? contratto = request.Contratto;

            if (contratto is null)
                return null;

            if (contratto.Righe is null || contratto.Righe.Count == 0)
            {
                LogDiagnostica.Errore(logMessage,
                    "Contratto richiesto senza righe: le tabelle ALLOrdCli* non verranno create.");
                return null;
            }

            if (contratto.Condizioni is null)
            {
                LogDiagnostica.Errore(logMessage,
                    "Contratto richiesto senza condizioni: le tabelle ALLOrdCli* non verranno create.");
                return null;
            }

            var creato = new ContrattoCreato();
            CondizioniContrattoCreazione cond = contratto.Condizioni;
            var cespitiPerRiga = new HashSet<(short Distinta, short Contratto, string Cespite)>();
            var cespitiOrdine = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            // Importo canone: nei contratti reali coincide con la somma delle distinte.
            // Il valore del form è editabile: se c'è e non torna è solo un avviso, non un errore.
            decimal totaleDistinte = 0;
            foreach (RigaContrattoCreazione rigaContratto in contratto.Righe)
            {
                foreach (RigaDistintaCreazione distinta in rigaContratto.Distinta ?? Array.Empty<RigaDistintaCreazione>())
                {
                    totaleDistinte += (decimal)Math.Round(
                        (distinta.Qta ?? 0) * (distinta.ValUnit ?? 0),
                        DomainConstants.DecimaliTasse);
                }
            }

            double importoCanone = cond.ImportoCanone ?? (double)totaleDistinte;
            if (cond.ImportoCanone.HasValue
                && Math.Abs(cond.ImportoCanone.Value - (double)totaleDistinte) > 0.01)
            {
                string avviso =
                    $"Importo canone {cond.ImportoCanone.Value:0.00} diverso dalla somma delle distinte {(double)totaleDistinte:0.00}.";
                avvisi.Add(avviso);
                LogDiagnostica.Avviso(logMessage, avviso);
            }


            // ------------------------------------------------------------ ALLOrdCliAcc
            AllordCliAcc acc = _context.AddNew<AllordCliAcc>();
            acc.IdOrdCli = 0;
            acc.ApplicoIstat = SqlBool(cond.ApplicoIstat);
            if (cond.MesiDurata.HasValue) acc.MesiDurata = cond.MesiDurata;
            if (cond.MesiRinnovo.HasValue) acc.MesiRinnovo = cond.MesiRinnovo;
            if (cond.GgDisdetta.HasValue) acc.Ggdisdetta = cond.GgDisdetta;
            if (cond.DataScadenzaFissa.HasValue) acc.DataScadenzaFissa = cond.DataScadenzaFissa;
            if (cond.DataRiscatto.HasValue) acc.DataRiscatto = cond.DataRiscatto;
            if (cond.ImportoRiscatto.HasValue) acc.ImportoRiscatto = cond.ImportoRiscatto;
            if (cond.DataRiduzione.HasValue) acc.DataRiduzione = cond.DataRiduzione;
            if (cond.ImportoRiduzione.HasValue) acc.ImportoRiduzione = cond.ImportoRiduzione;
            if (cond.PercRiduzione.HasValue) acc.PercRiduzione = cond.PercRiduzione;
            if (cond.DataDecorrenza.HasValue) acc.DataDecorrenza = cond.DataDecorrenza;
            if (cond.DataCessazione.HasValue) acc.DataCessazione = cond.DataCessazione;
            acc.MotivoCessazione = Taglia(cond.MotivoCessazione, 50);
            if (cond.TipoContratto.HasValue) acc.TipoContratto = cond.TipoContratto;
            acc.ImpiantoProprietaCliente = SqlBool(cond.ImpiantoProprietaCliente);
            if (cond.ImportoCanone.HasValue)
                acc.ImportoCanone = cond.ImportoCanone;
            else
                acc.ImportoCanone = importoCanone;
            if (cond.ContributoInstallazione.HasValue) acc.ContributoInstallazione = cond.ContributoInstallazione;
            acc.OrdineSospeso = SqlBool(cond.OrdineSospeso);
            if (cond.DataSospensione.HasValue) acc.DataSospensione = cond.DataSospensione;
            acc.MotivoSospensione = Taglia(cond.MotivoSospensione, 50);
            acc.Agente = Taglia(
                string.IsNullOrWhiteSpace(cond.Agente) ? request.Agente : cond.Agente, 8);
            if (cond.ImportoProvvigione.HasValue) acc.ImportoProvvigione = cond.ImportoProvvigione;
            acc.Impianto = Taglia(cond.Impianto, 8);
            acc.Nota = Taglia(cond.Nota, 255);
            acc.CondPag = Taglia(cond.CondPag, 8);
            acc.SedeInvioDoc = Taglia(cond.SedeInvioDoc, 8);
            acc.ModelloContratto = cond.ModelloContratto ?? ModelloContrattoSpa;
            acc.CdC = Taglia(cond.CdC, 8);
            acc.Vettore = Taglia(cond.Vettore, 8);
            acc.ImpiantoDue = Taglia(cond.ImpiantoDue, 8);
            if (cond.DataPrevistaScadenza.HasValue) acc.DataPrevistaScadenza = cond.DataPrevistaScadenza;
            Timbra(acc);
            creato.Acc = acc;

            short linea = 0;
            short lineaAttivita = 0;

            // Codice IVA di default del database, usato quando la riga contratto non lo indica
            // (stesso criterio di MA_SaleOrdDetails nella testata).
            string codIvaDefault = _context.MaTaxCodesDefaults.AsNoTracking()
                .Select(t => t.TaxCode)
                .FirstOrDefault() ?? string.Empty;

            foreach (RigaContrattoCreazione r in contratto.Righe)
            {
                linea++;

                var riga = _context.AddNew<AllordCliContratto>();
                riga.IdOrdCli = 0;
                riga.Line = linea;
                riga.Servizio = Taglia(r.Servizio, 21);
                riga.CodiceIva = Taglia(string.IsNullOrWhiteSpace(r.CodiceIva) ? codIvaDefault : r.CodiceIva, 4);
                riga.Descrizione = Taglia(r.Descrizione, 128);
                if (r.Qta.HasValue) riga.Qta = r.Qta;
                riga.Um = Taglia(r.Um, 8);
                if (r.ValUnit.HasValue) riga.ValUnit = r.ValUnit;
                if (r.Franchigia.HasValue) riga.Franchigia = r.Franchigia;
                riga.Nota = Taglia(r.Nota, 128);
                riga.TipoRigaServizio = Taglia(r.TipoRigaServizio, 12);
                if (r.DataDecorrenza.HasValue) riga.DataDecorrenza = r.DataDecorrenza;
                riga.NonRiportaInFatt = SqlBool(r.NonRiportaInFatt);
                if (r.DataProssimaFatt.HasValue) riga.DataProssimaFatt = r.DataProssimaFatt;
                riga.CodContratto = Taglia(r.CodContratto, 15);
                riga.CodIntegra = Taglia(r.CodIntegra, 15);
                riga.Impianto = Taglia(r.Impianto, 8);
                riga.CdC = Taglia(r.CdC, 8);

                var distinta = r.Distinta ?? Array.Empty<RigaDistintaCreazione>();
                var attivita = r.Attivita ?? Array.Empty<AttivitaContrattoCreazione>();
                var descrizioni = r.DescrizioniFattura ?? Array.Empty<DescrizioneFatturaCreazione>();

                riga.SubLineDistinta = ToShort(distinta.Count);
                riga.SubLineAttivita = ToShort(attivita.Count);
                riga.SubLineDescFatt = ToShort(descrizioni.Count);
                Timbra(riga);
                creato.Contratti.Add(riga);

                short lineaDistinta = 0;

                foreach (RigaDistintaCreazione d in distinta)
                {
                    lineaDistinta++;

                    var dist = _context.AddNew<AllordCliContrattoDistinta>();
                    dist.IdOrdCli = 0;
                    dist.Line = lineaDistinta;
                    dist.RifLinea = linea;
                    dist.Servizio = Taglia(d.Servizio, 21);
                    dist.TipoRigaServizio = Taglia(
                        string.IsNullOrWhiteSpace(d.TipoRigaServizio) ? r.TipoRigaServizio : d.TipoRigaServizio, 12);
                    dist.Descrizione = Taglia(d.Descrizione, 128);
                    dist.Um = Taglia(d.Um, 8);
                    if (d.Qta.HasValue) dist.Qta = d.Qta;
                    if (d.DataDecorrenza.HasValue) dist.DataDecorrenza = d.DataDecorrenza;
                    if (d.DataProssimaFatt.HasValue) dist.DataProssimaFatt = d.DataProssimaFatt;
                    if (d.ValUnit.HasValue) dist.ValUnit = d.ValUnit;
                    dist.CodIntegra = Taglia(d.CodIntegra, 15);
                    dist.CodContratto = Taglia(d.CodContratto, 15);
                    dist.Impianto = Taglia(d.Impianto, 8);
                    dist.CdC = Taglia(d.CdC, 8);
                    dist.Nota = Taglia(d.Nota, 128);

                    var servizi = d.ServiziAggiuntivi ?? Array.Empty<ServizioAggiuntivoCreazione>();
                    dist.SubLineServAgg = ToShort(servizi.Count);
                    Timbra(dist);
                    creato.Distinte.Add(dist);

                    short lineaServizio = 0;
                    foreach (ServizioAggiuntivoCreazione sa in servizi)
                    {
                        lineaServizio++;

                        var s = _context.AddNew<AllordCliContrattoDistintaServAgg>();
                        s.IdOrdCli = 0;
                        s.Line = lineaServizio;
                        s.RifLinea = lineaDistinta;
                        s.RifRifLinea = linea;
                        s.Servizio = Taglia(sa.Servizio, 21);
                        // Sui servizi aggiuntivi Mago usa il tipo dedicato "SUPPLEMENTAR"
                        // (2693/2694 righe reali), non il tipo della riga contratto/distinta.
                        s.TipoRigaServizio = Taglia(
                            string.IsNullOrWhiteSpace(sa.TipoRigaServizio) ? TipoRigaServizioSupplementare : sa.TipoRigaServizio,
                            12);
                        s.Descrizione = Taglia(sa.Descrizione, 128);
                        s.Um = Taglia(sa.Um, 8);
                        if (sa.Qta.HasValue) s.Qta = sa.Qta;
                        if (sa.Franchigia.HasValue) s.Franchigia = sa.Franchigia;
                        if (sa.DataDecorrenza.HasValue) s.DataDecorrenza = sa.DataDecorrenza;
                        if (sa.DataProssimaFatt.HasValue) s.DataProssimaFatt = sa.DataProssimaFatt;
                        if (sa.ValUnit.HasValue) s.ValUnit = sa.ValUnit;
                        if (sa.Periodicita.HasValue) s.Periodicita = sa.Periodicita;
                        Timbra(s);
                        creato.ServiziAggiuntivi.Add(s);
                    }

                    foreach (string cespite in d.Cespiti ?? Array.Empty<string>())
                    {
                        if (string.IsNullOrWhiteSpace(cespite)) continue;

                        string codice = Taglia(cespite, 10);
                        // La chiave reale è (IdOrdCli, RifLinea, RifRifLinea, Cespite):
                        // lo stesso cespite ripetuto sulla stessa distinta romperebbe l'insert.
                        if (!cespitiPerRiga.Add((lineaDistinta, linea, codice)))
                            continue;

                        var c = _context.AddNew<AllordCliContrattoDistCesp>();
                        c.IdOrdCli = 0;
                        c.RifLinea = lineaDistinta;
                        c.RifRifLinea = linea;
                        c.Cespite = codice;
                        Timbra(c);
                        creato.Cespiti.Add(c);

                        // Mago tiene l'elenco cespiti anche a livello ordine (ALLCespiti):
                        // nei contratti reali è l'unica tabella popolata, quindi va scritta.
                        if (cespitiOrdine.Add(codice))
                        {
                            var co = _context.AddNew<Allcespiti>();
                            co.IdOrdCli = 0;
                            co.Cespite = codice;
                            Timbra(co);
                            creato.CespitiOrdine.Add(co);
                        }
                    }
                }

                // Attività: numerazione globale sull'ordine (la RifLinea lega l'attività al
                // contratto). Così la scrittura resta valida sia sul PK attuale
                // (IdOrdCli, Line, RifLinea) sia su quello precedente (IdOrdCli, Line).
                foreach (AttivitaContrattoCreazione a in attivita)
                {
                    lineaAttivita++;

                    var att = _context.AddNew<AllordCliAttivita>();
                    att.IdOrdCli = 0;
                    att.Line = lineaAttivita;
                    att.RifLinea = linea;
                    att.Attivita = Taglia(a.Attivita, 10);
                    if (a.DataInizio.HasValue) att.DataInizio = a.DataInizio;
                    if (a.DataFine.HasValue) att.DataFine = a.DataFine;
                    att.DaFatturare = SqlBool(a.DaFatturare);
                    if (a.DataRifatturazione.HasValue) att.DataRifatturazione = a.DataRifatturazione;
                    att.Fatturata = "0";
                    att.Nota = Taglia(a.Nota, 128);
                    att.RifServizio = Taglia(a.RifServizio, 21);
                    if (a.ValUnit.HasValue) att.ValUnit = a.ValUnit;
                    att.TestoFattura = Taglia(a.TestoFattura, 128);
                    Timbra(att);
                    creato.Attivita.Add(att);
                }

                short lineaDescrizione = 0;
                foreach (DescrizioneFatturaCreazione df in descrizioni)
                {
                    lineaDescrizione++;

                    var d = _context.AddNew<AllordCliContrattoDescFatt>();
                    d.IdOrdCli = 0;
                    d.Line = lineaDescrizione;
                    d.RifLinea = linea;
                    d.Codice = Taglia(df.Codice, 12);
                    d.Descrizione = Taglia(df.Descrizione, 128);
                    Timbra(d);
                    creato.DescrizioniFattura.Add(d);
                }
            }

            var tipologie = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (string? grezza in contratto.TipologieServizi ?? Array.Empty<string>())
            {
                if (string.IsNullOrWhiteSpace(grezza)) continue;

                string tipologia = grezza.Trim();
                if (tipologia.Length > 8)
                {
                    LogDiagnostica.Avviso(logMessage,
                        $"Tipologia servizio '{tipologia}' ignorata: il campo Mago ammette max 8 caratteri (codice, non descrizione).");
                    continue;
                }

                if (!tipologie.Add(tipologia)) continue;

                var t = _context.AddNew<AllordCliTipologiaServizi>();
                t.IdOrdCli = 0;
                t.Tipologia = tipologia;
                Timbra(t);
                creato.Tipologie.Add(t);
            }

            short lineaDescrizioneOrdine = 0;
            foreach (DescrizioneOrdineCreazione dord in contratto.DescrizioniOrdine ?? Array.Empty<DescrizioneOrdineCreazione>())
            {
                if (string.IsNullOrWhiteSpace(dord.Codice) && string.IsNullOrWhiteSpace(dord.Descrizione))
                    continue;

                lineaDescrizioneOrdine++;

                var d = _context.AddNew<AllordCliDescrizioni>();
                d.IdOrdCli = 0;
                d.Line = lineaDescrizioneOrdine;
                d.Codice = Taglia(dord.Codice, 12);
                d.Descrizione = Taglia(dord.Descrizione, 128);
                Timbra(d);
                creato.DescrizioniOrdine.Add(d);
            }

            FatturaElettronicaCreazione? fe = contratto.FatturaElettronica;
            if (fe is not null && HasFatturaElettronica(fe))
            {
                var f = _context.AddNew<AllordCliFattEle>();
                f.IdOrdCli = 0;
                f.F2_1_1_5_4 = Taglia(fe.CausaleRitenuta, 1);
                f.F2_1_1_11 = Taglia(fe.Causale, 200);
                f.F2_1_2_2 = Taglia(fe.DatiOrdineAcquisto?.IdDocumento ?? "", 20);
                if (fe.DatiOrdineAcquisto?.DataDocumento.HasValue == true) f.F2_1_2_3 = fe.DatiOrdineAcquisto.DataDocumento.Value;
                f.F2_1_2_4 = Taglia(fe.DatiOrdineAcquisto?.NumItem ?? "", 20);
                f.F2_1_2_5 = Taglia(fe.DatiOrdineAcquisto?.Commessa ?? "", 100);
                f.F2_1_2_6 = Taglia(fe.DatiOrdineAcquisto?.Cup ?? "", 15);
                f.F2_1_2_7 = Taglia(fe.DatiOrdineAcquisto?.Cig ?? "", 15);
                f.F2_1_3_2 = Taglia(fe.DatiContratto?.IdDocumento ?? "", 20);
                if (fe.DatiContratto?.DataDocumento.HasValue == true) f.F2_1_3_3 = fe.DatiContratto.DataDocumento.Value;
                f.F2_1_3_4 = Taglia(fe.DatiContratto?.NumItem ?? "", 20);
                f.F2_1_3_5 = Taglia(fe.DatiContratto?.Commessa ?? "", 100);
                f.F2_1_3_6 = Taglia(fe.DatiContratto?.Cup ?? "", 15);
                f.F2_1_3_7 = Taglia(fe.DatiContratto?.Cig ?? "", 15);
                f.F2_1_4_2 = Taglia(fe.DatiConvenzione?.IdDocumento ?? "", 20);
                if (fe.DatiConvenzione?.DataDocumento.HasValue == true) f.F2_1_4_3 = fe.DatiConvenzione.DataDocumento.Value;
                f.F2_1_4_4 = Taglia(fe.DatiConvenzione?.NumItem ?? "", 20);
                f.F2_1_4_5 = Taglia(fe.DatiConvenzione?.Commessa ?? "", 100);
                f.F2_1_4_6 = Taglia(fe.DatiConvenzione?.Cup ?? "", 15);
                f.F2_1_4_7 = Taglia(fe.DatiConvenzione?.Cig ?? "", 15);
                f.F2_1_5_2 = Taglia(fe.DatiRicezione?.IdDocumento ?? "", 20);
                if (fe.DatiRicezione?.DataDocumento.HasValue == true) f.F2_1_5_3 = fe.DatiRicezione.DataDocumento.Value;
                f.F2_1_5_4 = Taglia(fe.DatiRicezione?.NumItem ?? "", 20);
                f.F2_1_5_5 = Taglia(fe.DatiRicezione?.Commessa ?? "", 100);
                f.F2_1_5_6 = Taglia(fe.DatiRicezione?.Cup ?? "", 15);
                f.F2_1_5_7 = Taglia(fe.DatiRicezione?.Cig ?? "", 15);                
                Timbra(f);
                creato.FattEle = f;
            }

            // Collegamenti: il numero ordine viene risolto sull'ordine esistente.
            // Se non esiste, il collegamento viene saltato con un avviso (non è un
            // dato indispensabile per la creazione del contratto).
            foreach (CollegamentoOrdineCreazione link in contratto.Collegamenti ?? Array.Empty<CollegamentoOrdineCreazione>())
            {
                if (string.IsNullOrWhiteSpace(link.Numero)) continue;

                string numero = link.Numero.Trim();
                MaSaleOrd? collegato = _context.MaSaleOrd.AsNoTracking()
                    .FirstOrDefault(o => o.InternalOrdNo == numero);
                if (collegato is null)
                {
                    LogDiagnostica.Avviso(logMessage,
                        $"Ordine collegato '{numero}' non trovato: collegamento ignorato.");
                    continue;
                }

                if (string.Equals(link.Tipo, "Padre", StringComparison.OrdinalIgnoreCase))
                {
                    var p = _context.AddNew<AllordPadre>();
                    p.IdOrdCli = 0;
                    p.IdOrdPadre = collegato.SaleOrdId;
                    p.NrOrdPadre = Taglia(numero, 10);
                    Timbra(p);
                    creato.Padri.Add(p);
                }
                else
                {
                    var f = _context.AddNew<AllordFiglio>();
                    f.IdOrdCli = 0;
                    f.IdOrdFiglio = collegato.SaleOrdId;
                    f.NrOrdFiglio = Taglia(numero, 10);
                    Timbra(f);
                    creato.Figli.Add(f);
                }
            }

            return creato;
        }

        /// <summary>Inserisce le tabelle contratto nell'ordine di dipendenza delle FK.</summary>
        private void InserisciContratto(ContrattoCreato c, System.Text.StringBuilder sb)
        {
            if (c.Acc is not null)
                BulkPersistenceHelper.BulkExecuteList(_context, new List<AllordCliAcc> { c.Acc },
                    "ALLOrdCliAcc", BulkOperation.Insert, "condizioni contratto", sb);

            BulkPersistenceHelper.BulkExecuteList(_context, c.Contratti,
                "ALLOrdCliContratto", BulkOperation.Insert, "righe contratto", sb);
            BulkPersistenceHelper.BulkExecuteList(_context, c.Distinte,
                "ALLOrdCliContratto_Distinta", BulkOperation.Insert, "distinta contratto", sb);
            BulkPersistenceHelper.BulkExecuteList(_context, c.ServiziAggiuntivi,
                "ALLOrdCliContratto_Distinta_ServAgg", BulkOperation.Insert, "servizi aggiuntivi", sb);
            BulkPersistenceHelper.BulkExecuteList(_context, c.Cespiti,
                "ALLOrdCliContratto_Dist_Cesp", BulkOperation.Insert, "cespiti", sb);
            BulkPersistenceHelper.BulkExecuteList(_context, c.CespitiOrdine,
                "ALLCespiti", BulkOperation.Insert, "cespiti ordine", sb);
            BulkPersistenceHelper.BulkExecuteList(_context, c.Attivita,
                "ALLOrdCliAttivita", BulkOperation.Insert, "attività contratto", sb);
            BulkPersistenceHelper.BulkExecuteList(_context, c.DescrizioniFattura,
                "ALLOrdCliContratto_DescFatt", BulkOperation.Insert, "descrizioni fattura", sb);
            BulkPersistenceHelper.BulkExecuteList(_context, c.DescrizioniOrdine,
                "ALLOrdCliDescrizioni", BulkOperation.Insert, "descrizioni ordine", sb);
            BulkPersistenceHelper.BulkExecuteList(_context, c.Tipologie,
                "ALLOrdCliTipologiaServizi", BulkOperation.Insert, "tipologie servizi", sb);
            BulkPersistenceHelper.BulkExecuteList(_context, c.Padri,
                "ALLOrdPadre", BulkOperation.Insert, "ordini padre", sb);
            BulkPersistenceHelper.BulkExecuteList(_context, c.Figli,
                "ALLOrdFiglio", BulkOperation.Insert, "ordini figlio", sb);

            if (c.FattEle is not null)
                BulkPersistenceHelper.BulkExecuteList(_context, new List<AllordCliFattEle> { c.FattEle },
                    "ALLOrdCliFattEle", BulkOperation.Insert, "dati fattura elettronica", sb);
        }

        /// <summary>Imposta i campi di audit comuni a tutte le tabelle ALLOrdCli*.</summary>
        private void Timbra(object entity)
        {
            switch (entity)
            {
                case AllordCliAcc e:
                    e.Tbcreated = _now; e.Tbmodified = _now; e.TbcreatedId = _loginId; e.TbmodifiedId = _loginId; break;
                case AllordCliContratto e:
                    e.Tbcreated = _now; e.Tbmodified = _now; e.TbcreatedId = _loginId; e.TbmodifiedId = _loginId; break;
                case AllordCliContrattoDistinta e:
                    e.Tbcreated = _now; e.Tbmodified = _now; e.TbcreatedId = _loginId; e.TbmodifiedId = _loginId; break;
                case AllordCliContrattoDistintaServAgg e:
                    e.Tbcreated = _now; e.Tbmodified = _now; e.TbcreatedId = _loginId; e.TbmodifiedId = _loginId; break;
                case AllordCliContrattoDistCesp e:
                    e.Tbcreated = _now; e.Tbmodified = _now; e.TbcreatedId = _loginId; e.TbmodifiedId = _loginId; break;
                case Allcespiti e:
                    e.Tbcreated = _now; e.Tbmodified = _now; e.TbcreatedId = _loginId; e.TbmodifiedId = _loginId; break;
                case AllordCliAttivita e:
                    e.Tbcreated = _now; e.Tbmodified = _now; e.TbcreatedId = _loginId; e.TbmodifiedId = _loginId; break;
                case AllordCliContrattoDescFatt e:
                    e.Tbcreated = _now; e.Tbmodified = _now; e.TbcreatedId = _loginId; e.TbmodifiedId = _loginId; break;
                case AllordCliDescrizioni e:
                    e.Tbcreated = _now; e.Tbmodified = _now; e.TbcreatedId = _loginId; e.TbmodifiedId = _loginId; break;
                case AllordCliTipologiaServizi e:
                    e.Tbcreated = _now; e.Tbmodified = _now; e.TbcreatedId = _loginId; e.TbmodifiedId = _loginId; break;
                case AllordCliFattEle e:
                    e.Tbcreated = _now; e.Tbmodified = _now; e.TbcreatedId = _loginId; e.TbmodifiedId = _loginId; break;
                case AllordPadre e:
                    e.Tbcreated = _now; e.Tbmodified = _now; e.TbcreatedId = _loginId; e.TbmodifiedId = _loginId; break;
                case AllordFiglio e:
                    e.Tbcreated = _now; e.Tbmodified = _now; e.TbcreatedId = _loginId; e.TbmodifiedId = _loginId; break;
            }
        }

        private static string SqlBool(bool value) => value ? "1" : "0";

        private static string Taglia(string? value, int max) => StringHelpers.SubstringSafe(value, max);

        private static short ToShort(int value) =>
            value <= 0 ? (short)0 : value >= short.MaxValue ? short.MaxValue : (short)value;

        private static bool HasFatturaElettronica(FatturaElettronicaCreazione fe) =>
            !string.IsNullOrWhiteSpace(fe.Causale)
            || !string.IsNullOrWhiteSpace(fe.CausaleRitenuta)
            || fe.DatiOrdineAcquisto?.IdDocumento != null
            || fe.DatiOrdineAcquisto?.DataDocumento.HasValue == true
            || fe.DatiOrdineAcquisto?.Commessa != null
            || fe.DatiOrdineAcquisto?.Cig != null
            || fe.DatiOrdineAcquisto?.Cup != null
            || fe.DatiOrdineAcquisto?.NumItem != null
            || fe.DatiContratto?.IdDocumento != null
            || fe.DatiContratto?.DataDocumento.HasValue == true
            || fe.DatiContratto?.Commessa != null
            || fe.DatiContratto?.Cig != null
            || fe.DatiContratto?.Cup != null
            || fe.DatiContratto?.NumItem != null
            || fe.DatiConvenzione?.IdDocumento != null
            || fe.DatiConvenzione?.DataDocumento.HasValue == true
            || fe.DatiConvenzione?.Commessa != null
            || fe.DatiConvenzione?.Cig != null
            || fe.DatiConvenzione?.Cup != null
            || fe.DatiConvenzione?.NumItem != null
            || fe.DatiRicezione?.IdDocumento != null
            || fe.DatiRicezione?.DataDocumento.HasValue == true
            || fe.DatiRicezione?.Commessa != null
            || fe.DatiRicezione?.Cig != null
            || fe.DatiRicezione?.Cup != null
            || fe.DatiRicezione?.NumItem != null;
    }
}
