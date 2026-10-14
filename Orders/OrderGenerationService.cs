using EFCore.BulkExtensions;
using EFMago.Models;
using FatturazioneContratti.Common;
using FatturazioneContratti.Config;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data.Common;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace FatturazioneContratti.Orders
{
    public partial class OrderGenerationService
    {
        private readonly FattureContext _context;
        private readonly int _loginId;

        /// <summary>
        /// Timestamp unico dell'intera operazione: tutti i Tbcreated/Tbmodified/DataFineElaborazione
        /// scritti durante il workflow usano lo stesso valore, così le righe di una stessa
        /// elaborazione risultano coerenti (prima ogni proprietà chiamava DateTime.Now separatamente).
        /// </summary>
        private readonly DateTime _now = DateTime.Now;

        public OrderGenerationService(FattureContext context, int loginId)
        {
            _context = context;
            _loginId = loginId;
        }

        public bool GeneraRigheOrdine(
            FiltriOrdini filtri,
            int dimensioneBlocco = 100,
            Action<string, AppConfig.LogTarget> logMessage = null,
            Action<string> updateStatus = null,
            ProgressCallbacks? progress1 = null,
            ProgressCallbacks? progress2 = null,
            ProgressCallbacks? progress3 = null)
        {
            var stato = new StatoElaborazioneOrdini(filtri, _loginId, _now);

            try
            {
                updateStatus?.Invoke("Estrazione preliminare delle chiavi primarie...");

                // -------------------------------------------------------------------------
                // STEP 1: ESTRAZIONE SOLO DEGLI ID (Zero esplosione cartesiana, velocissimo)
                // -------------------------------------------------------------------------
                // Filtri comuni sulla testa ordine (date/cliente/ordine/filiale),
                // condivisi con il workflow ISTAT.
                IQueryable<MaSaleOrd> query = ApplicaFiltriComuni(_context.MaSaleOrd.AsNoTracking(), filtri);

                // Filtri Divisione (specifici della generazione canoni)
                if (filtri.DivisioneTecnologia) query = query.Where(o => o.Divisione == (int?)Divisione.Tecnologia);
                if (filtri.DivisioneVigilanza) query = query.Where(o => o.Divisione == (int?)Divisione.Vigilanza);

                // Filtro priorità (specifico della generazione canoni)
                if (filtri.SingolaPriorita && int.TryParse(filtri.Priorita, out int pVal))
                    query = query.Where(o => o.Priority == pVal);

                // Eseguiamo la query materializzando SOLO i numeri interi degli ID ordini
                var allOrderIds = EstraiIdsOrdini(query);

                if (!allOrderIds.Any())
                {
                    logMessage?.Invoke("Nessun ordine estratto con i filtri specificati.", AppConfig.LogTarget.Ui);
                    return true;
                }

                int totOrdini = allOrderIds.Count;
                string msgTot = $"Trovati {totOrdini} ordini da elaborare";
                Debug.WriteLine(msgTot);
                logMessage?.Invoke(msgTot, AppConfig.LogTarget.Ui);
                updateStatus?.Invoke(msgTot);

                // Default vendite/IVA + prototipo riga: caricati una sola volta per elaborazione.
                stato.Inizializza(_context.AddNew<MaSaleOrdDetails>(), CaricaDefaultVenditeIva());

                // -------------------------------------------------------------------------
                // STEP 2: CICLO A BLOCCHI SULLA LISTA DI ID
                // -------------------------------------------------------------------------
                IEnumerable<MaSaleOrd> CaricaOrdiniPerBlocco()
                {
                    for (int i = 0; i < totOrdini; i += dimensioneBlocco)
                    {
                        int blockSize = Math.Min(dimensioneBlocco, totOrdini - i);
                        progress2?.Init(0, Math.Max(1, blockSize)); // barra piccola: progresso dentro il blocco

                        updateStatus?.Invoke($"Elaborazione blocco {i / dimensioneBlocco + 1} (Ordini {i + 1} - {Math.Min(i + dimensioneBlocco, totOrdini)} di {totOrdini})...");
                        int[] blocId = allOrderIds.Skip(i).Take(blockSize).ToArray();
                        // Chiamo il loader che gestisce la query con gli id
                        if (AppConfig.IsDebugging) logMessage?.Invoke("[DEBUG ATTIVO] Avvio forzato della scansione ordine per ordine...", AppConfig.LogTarget.Ui);
                        var allOrdersChunk = LoadOrdersChunkByOrIn(
                            _context,
                            blocId,
                            AppConfig.IsDebugging,
                            msg => logMessage?.Invoke(msg, AppConfig.LogTarget.Ui)
                        );

                        foreach (var o in allOrdersChunk)
                        {
                            progress2?.Advance(); // avanza la piccola per ogni ordine
                            yield return o;
                        }

                        progress2?.Finish();   // fine blocco piccolo
                        progress1?.Advance();  // un passo per il blocco completato
                    }
                }

                int numeroBlocchi = (totOrdini + dimensioneBlocco - 1) / dimensioneBlocco;
                progress1?.Init(0, Math.Max(1, numeroBlocchi)); // barra grande: un passo per blocco

                foreach (var o in CaricaOrdiniPerBlocco())
                    ElaboraOrdine(stato, o);

                progress1?.Finish();

                // Salvataggio Bulk
                logMessage?.Invoke("Salvataggio su database ERP...", AppConfig.LogTarget.Ui);
                EseguiSalvataggioTransazionale(
                    stato.UpdateMaSaleOrd.Count,
                    stato.UpdateAllordCliContratto.Count,
                    stato.UpdateMaSaleOrd,
                    stato.InsertMaSaleOrdDetails,
                    stato.UpdateAllordCliAcc,
                    stato.UpdateAllordCliAttivita,
                    stato.UpdateAllordCliContratto,
                    stato.BulkMessage,
                    stato.Errori,
                    logMessage);
                logMessage?.Invoke("[COMPLETATO] Elaborazione a blocchi di ID conclusa con successo.", AppConfig.LogTarget.Ui);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                stato.Errori.AppendLine($"[Procedura] Messaggio: {ex.Message}");
                stato.Errori.AppendLine($"[Procedura] Stack: {ex.StackTrace}");
                logMessage?.Invoke($"ERRORE: {ex.Message}", AppConfig.LogTarget.Ui);
                return false;
            }

            return stato.Errori.Length == 0;
        }

        #region Elaborazione ordine e contratto

        /// <summary>
        /// Elabora un singolo ordine: esegue le righe di contratto e aggiorna la testa
        /// ordine se sono state generate nuove righe o adeguamenti.
        /// </summary>
        private void ElaboraOrdine(StatoElaborazioneOrdini st, MaSaleOrd o)
        {
            st.Debugging.AppendLine($"Ordine: {o.InternalOrdNo}");

            var so = new StatoOrdine(
                new CurOrd(o),
                o.MaSaleOrdDetails.Any() ? o.MaSaleOrdDetails.Max(m => m.Position) : 0);

            if (o.ALLordCliContratto != null && o.ALLordCliContratto.Any())
            {
                foreach (var c in o.ALLordCliContratto)
                    ElaboraContratto(st, so, o, c);
            }

            if (so.IsNewRows)
            {
                o.Invoiced = "0";
                o.Delivered = "0";
                o.Picked = "0";
                o.LastSubId = so.COrd.LastSubId + so.INewRowsCount;
                o.Tbmodified = st.Now;
                o.TbmodifiedId = st.LoginId;
                st.UpdateMaSaleOrd.Add(o);
                st.TotOrdiniConNuoveRighe++;
            }

            if (so.IsUpdateRows) st.TotOrdiniConRigheAggiornate++;
        }

        /// <summary>
        /// Elabora una riga di contratto: esclusioni, gestione attività, scadenze/cessazioni,
        /// generazione righe ordine e aggiornamento delle date di prossima fatturazione.
        /// </summary>
        private void ElaboraContratto(StatoElaborazioneOrdini st, StatoOrdine so, MaSaleOrd o, AllordCliContratto c)
        {
            var filtri = st.Filtri;
            var cOrd = so.COrd;

            if (VerificaEsclusioneContratto(c, filtri, st.Debugging, out bool isDaEscludere))
                return;

            var taxRow = st.Default.CodiciIva.FirstOrDefault(t => t.TaxCode == c.CodiceIva);
            var cOrdRow = new CurOrdRow(c)
            {
                CanoneFuoriRangeDate = isDaEscludere,
                Contropartita = string.IsNullOrWhiteSpace(c.MaItems?.SaleOffset) ? st.Default.Contropartita : c.MaItems.SaleOffset,
                CodIva = string.IsNullOrWhiteSpace(c.CodiceIva) ? st.Default.CodIva : c.CodiceIva,
                PercIva = Math.Round((double)(taxRow?.Perc ?? st.Default.PercIva), DomainConstants.DecimaliPercentuale),
                Parent = cOrd
            };

            var attDaRifatturare = new List<AllordCliAttivita>();
            var attIstat = new List<AllordCliAttivita>();

            // Gestione Attività
            ElaboraAttivitaContratto(c, cOrdRow, filtri, attDaRifatturare, attIstat, st.UpdateAllordCliAttivita, st.Errori, st.Debugging);

            // Controllo Scadenze Fisse e Cessazioni
            ElaboraScadenzeECessazioni(o, cOrd, cOrdRow, st.UpdateAllordCliAcc, st.Debugging);

            // Generazione Righe se idoneo
            if (!cOrdRow.PrecendementeCessato
                && ((cOrdRow.IsOk && !cOrdRow.CanoneFuoriRangeDate) || cOrdRow.DaRifatturare)
                && (cOrdRow.QtaCorrente > 0 || cOrdRow.QtaDaRifatturare > 0))
            {
                if (o.MaSaleOrdDetails.Count == 1 && CheckRigaBianca(o.MaSaleOrdDetails.First()))
                {
                    cOrd.LastLine = 0;
                    so.CurLastPosition = 0;
                }

                // Allineamento consegna per attività di rifatturazione
                foreach (var aDaRif in attDaRifatturare)
                {
                    if (aDaRif.DataRifatturazione < cOrdRow.DataPrevistaConsegna)
                    {
                        if (!cOrdRow.CanoneFuoriRangeDate)
                            st.Avvisi.AppendLine($"Ordine {cOrd.OrdNo} Servizio {cOrdRow.Item} con date prevista fatturazione non congruenti!");

                        cOrdRow.DataPrevistaConsegna = aDaRif.DataRifatturazione.Value;
                        cOrdRow.DataConfermaConsegna = aDaRif.DataRifatturazione.Value;
                    }
                }

                so.IsNewRows = true;
                var newRow = st.PrototipoRiga.CloneFromPrototype();
                // Descrizione Testata Ordine
                if (!so.BScrittoDescrizioni && o.ALLordCliDescrizioni != null)
                {
                    so.BScrittoDescrizioni = true;
                    foreach (var d in o.ALLordCliDescrizioni)
                    {
                        so.INewRowsCount++;
                        newRow = st.PrototipoRiga.CloneFromPrototype();
                        st.InsertMaSaleOrdDetails.Add(CreaRigaNota(newRow, cOrd, cOrdRow, d.Descrizione, so.INewRowsCount, LineType.Nota));
                    }
                }

                // Righe da Rifatturare
                foreach (var aDaRif in attDaRifatturare)
                {
                    so.INewRowsCount++;
                    so.INrRigheAValore++;
                    newRow = st.PrototipoRiga.CloneFromPrototype();
                    st.InsertMaSaleOrdDetails.Add(CreaRigaRifatturazione(newRow, cOrd, cOrdRow, aDaRif, so.INewRowsCount, (short)(so.CurLastPosition + so.INrRigheAValore), st.Errori, st.Debugging));
                }

                // Riga Canone / Servizio Effettiva
                if (!cOrdRow.CanoneFuoriRangeDate && cOrdRow.QtaCorrente > 0)
                {
                    // Riga Riferimento Contratto
                    so.INewRowsCount++;
                    newRow = st.PrototipoRiga.CloneFromPrototype();
                    string descriRif = (string.IsNullOrWhiteSpace(cOrdRow.ContrattoFox) ? string.Empty : $"Codice : {cOrdRow.ContrattoFox}") + $" Rif : {cOrdRow.Parent.OrdNo}";
                    st.InsertMaSaleOrdDetails.Add(CreaRigaNota(newRow, cOrd, cOrdRow, descriRif.TrimStart(), so.INewRowsCount, LineType.Riferimento, (short)(so.CurLastPosition + so.INrRigheAValore)));

                    // Righe Descrittive Contratto
                    if (c.AllordCliContrattoDescFatt != null)
                    {
                        foreach (var d in c.AllordCliContrattoDescFatt)
                        {
                            so.INewRowsCount++;
                            newRow = st.PrototipoRiga.CloneFromPrototype();
                            st.InsertMaSaleOrdDetails.Add(CreaRigaNota(newRow, cOrd, cOrdRow, d.Descrizione, so.INewRowsCount, LineType.Nota));
                        }
                    }

                    // Dettaglio Valore
                    so.INewRowsCount++;
                    so.INrRigheAValore++;
                    newRow = st.PrototipoRiga.CloneFromPrototype();
                    st.InsertMaSaleOrdDetails.Add(CreaRigaServizioValore(newRow, c, cOrd, cOrdRow, so.INewRowsCount, (short)(so.CurLastPosition + so.INrRigheAValore)));
                }

                // Righe ISTAT
                foreach (var aIst in attIstat)
                {
                    so.INewRowsCount++;
                    newRow = st.PrototipoRiga.CloneFromPrototype();
                    var rd = CreaRigaNota(newRow, cOrd, cOrdRow, aIst.TestoFattura, so.INewRowsCount, LineType.Nota);
                    st.InsertMaSaleOrdDetails.Add(rd);
                    st.UpdateAllordCliAttivita.Add(aIst);
                }
            }

            // Aggiornamento date prossima fatturazione
            if (!cOrdRow.CanoneFuoriRangeDate
                && (cOrdRow.IsOk || cOrdRow.DaRifatturare || cOrdRow.SospesoDaAttivita)
                && !cOrdRow.PrecendementeCessato)
            {
                if (cOrdRow.DataProssimaFattura < filtri.DataFattA)
                    st.Avvisi.AppendLine($"Ordine {cOrd.OrdNo} Servizio {cOrdRow.Item} con data prossima fatturazione antecedente alla data di fine estrazione!");

                c.DataProssimaFatt = cOrdRow.IsCampagna ? cOrdRow.DataProssimaFatturaCampagna : cOrdRow.DataProssimaFattura;
                c.DataFineElaborazione = st.Now;
                c.Tbmodified = st.Now;
                c.TbmodifiedId = st.LoginId;
                st.UpdateAllordCliContratto.Add(c);
                so.IsUpdateRows = true;
            }
        }

        #endregion

        #region Metodi di supporto estrazione e regole

        /// <summary>
        /// Filtri comuni sulla testa ordine, condivisi da GeneraRigheOrdine e AdeguaIstatOrdine:
        /// intervallo date, cliente, ordine e filiale. I filtri specifici (divisione, priorità)
        /// restano a carico del chiamante.
        /// </summary>
        private static IQueryable<MaSaleOrd> ApplicaFiltriComuni(IQueryable<MaSaleOrd> query, FiltriOrdini filtri)
        {
            // Filtri di data sulla testa ordine
            if (filtri.FiltroDateOrdini)
            {
                query = query.Where(o => o.OrderDate.HasValue
                    && o.OrderDate.Value.Date >= filtri.FromOrdDate
                    && o.OrderDate.Value.Date <= filtri.ToOrdDate);
            }
            else
            {
                query = query.Where(o => o.OrderDate.HasValue && o.OrderDate <= filtri.ToOrdDate);
            }

            // Filtri anagrafici
            if (filtri.SingoloCliente) query = query.Where(o => o.Customer == filtri.Cliente);
            if (filtri.SingoloOrdine) query = query.Where(o => o.InternalOrdNo == filtri.NrOrd);
            if (filtri.SingolaFiliale) query = query.Where(o => o.CostCenter == filtri.Filiale);

            return query;
        }

        /// <summary>
        /// Materializza SOLO gli ID ordine (SaleOrdId) dalla query filtrata, evitando
        /// l'esplosione cartesiana degli Include sul set completo.
        /// </summary>
        private static List<int> EstraiIdsOrdini(IQueryable<MaSaleOrd> query)
            => query.OrderBy(o => o.SaleOrdId).Select(o => o.SaleOrdId).ToList();

        /// <summary>
        /// Carica i default ERP di vendita/IVA usati per contropartita, codice IVA e
        /// percentuale di default delle righe generate.
        /// </summary>
        private DefaultVenditeIva CaricaDefaultVenditeIva()
        {
            var defVendite = _context.MaUserDefaultSales.FirstOrDefault();
            var defIva = _context.MaTaxCodesDefaults.FirstOrDefault();
            var codiciIva = _context.MaTaxCodes.ToList();

            string sDefContropartita = defVendite?.ServicesSalesAccount ?? string.Empty;
            string sDefCodIva = defIva?.TaxCode ?? string.Empty;
            var taxDefault = codiciIva.FirstOrDefault(t => t.TaxCode == sDefCodIva);
            double dDefPercIva = Math.Round((double)(taxDefault?.Perc ?? 0), DomainConstants.DecimaliPercentuale);

            return new DefaultVenditeIva(sDefContropartita, sDefCodIva, dDefPercIva, codiciIva);
        }

        /// <summary>
        /// Carica un lotto ("chunk") di ordini di vendita (MaSaleOrd) comprensivo di tutto il grafo 
        /// delle relazioni (Include/ThenInclude) necessarie per l'elaborazione dei contratti.
        /// </summary>
        /// <param name="context">Il contesto del database <see cref="FattureContext"/> corrente.</param>
        /// <param name="ids">Array contenente gli identificativi univoci (<see cref="MaSaleOrd.SaleOrdId"/>) degli ordini da estrarre per il lotto corrente.</param>
        /// <param name="isDebugMode">Se impostato su <c>true</c>, forza l'esecuzione atomica ordine per ordine per finalità di diagnostica preventiva.</param>
        /// <param name="logMessage">Delegato opzionale per la notifica e il tracciamento dei messaggi di log o di errore sulla UI o su file.</param>
        /// <returns>Una lista di oggetti <see cref="MaSaleOrd"/> completamente popolati e disaccoppiati dal Change Tracker.</returns>
        /// <exception cref="System.Data.SqlTypes.SqlNullValueException">
        /// Rilanciata in modalità fail-safe se viene intercettato un valore NULL non mappato nel database dell'ERP. 
        /// Innesca automaticamente l'isolamento del record corrotto tramite scansione atomica.
        /// </exception>
        /// <exception cref="DbException">Rilanciata in caso di errori strutturali o di timeout lato SQL Server.</exception>
        private static List<MaSaleOrd> LoadOrdersChunkByOrIn(FattureContext context, int[] ids, bool isDebugMode, Action<string> logMessage = null)
        {
            if (ids == null || ids.Length == 0) return new List<MaSaleOrd>();

            // Definizione del grafo di query riutilizzabile sia per il batch che per l'isolamento atomico
            IQueryable<MaSaleOrd> QueryBase(FattureContext ctx)
            {
                return ctx.MaSaleOrd
                    .AsNoTracking()
                    .Include(o => o.ALLOrdCliAcc)
                    .Include(o => o.ALLordCliDescrizioni)
                    .Include(o => o.MaSaleOrdDetails)
                    .Include(o => o.ALLordCliContratto)
                        .ThenInclude(c => c.MaItems)
                    .Include(o => o.ALLordCliContratto)
                        .ThenInclude(c => c.AlltipoRigaServizio)
                    .Include(o => o.ALLordCliContratto)
                        .ThenInclude(c => c.AllordCliContrattoDescFatt)
                    .Include(o => o.ALLordCliContratto)
                        .ThenInclude(c => c.AllordCliAttivita)
                            .ThenInclude(a => a.Allattivita)
                    // Attività collegate direttamente alla testa: servono al workflow ISTAT
                    // (curLastLineAtt su AllordCliAttivita) e sono nel grafo previsto dal caricamento a blocchi.
                    .Include(o => o.AllordCliAttivita)
                        .ThenInclude(a => a.Allattivita)
                    // opzionale: evita query singola troppo pesante
                    .AsSplitQuery();
            }

            // Se il debug è forzato a monte, saltiamo direttamente alla modalità di scansione atomica
            if (isDebugMode)
            {
                // logMessage?.Invoke("[DEBUG ATTIVO] Avvio forzato della scansione ordine per ordine...");
                EseguiScansioneDiagnostica(context, ids, QueryBase, logMessage);
            }

            try
            {
                // Costruisci query con tutti gli Include che vuoi (modifica qui per aggiungere/togliere)
                var query = QueryBase(context);

                // Costruisci predicate: (SaleOrdId == id0) OR (SaleOrdId == id1) OR ...
                var param = Expression.Parameter(typeof(MaSaleOrd), "o");
                var prop = Expression.PropertyOrField(param, nameof(MaSaleOrd.SaleOrdId));

                Expression? predicate = null;
                foreach (var id in ids)
                {
                    var equals = Expression.Equal(prop, Expression.Constant(id));
                    predicate = predicate == null ? equals : Expression.OrElse(predicate, equals);
                }

                if (predicate == null)
                    return new List<MaSaleOrd>();

                var lambda = Expression.Lambda<Func<MaSaleOrd, bool>>(predicate, param);
                var finalQuery = query.Where(lambda);

                return finalQuery.ToList();
            }

            catch (Exception ex) when (ex is System.Data.SqlTypes.SqlNullValueException ||
                               ex.InnerException is System.Data.SqlTypes.SqlNullValueException)
            {
                // Solo il NULL non mappato innesca la diagnostica: i vecchi confronti sul testo
                // dell'eccezione ("Null") catturavano anche errori diversi e attivavano la
                // scansione atomica a sproposito.
                // Se non eravamo in modalità debug ma intercettiamo il crash del record NULL, attiviamo la diagnostica
                logMessage?.Invoke("!!! [CRASH RILEVATO] Rilevata anomalia SqlNullValueException nel blocco. Attivazione isolamento...");
                EseguiScansioneDiagnostica(context, ids, QueryBase, logMessage);
                throw;
            }
            catch (DbException dbEx)
            {
                logMessage?.Invoke($"DB error in LoadOrdersChunkByOrIn: {dbEx.Message}");
                Debug.WriteLine(dbEx);
                throw;
            }
            catch (Exception ex)
            {
                logMessage?.Invoke($"Unexpected error in LoadOrdersChunkByOrIn: {ex.Message}");
                Debug.WriteLine(ex);
                throw;
            }
        }

        /// <summary>
        /// Esegue la query un ID alla volta per catturare matematicamente il record che ospita il NULL corrotto.
        /// </summary>
        private static void EseguiScansioneDiagnostica(
            FattureContext context,
            int[] ids,
            Func<FattureContext, IQueryable<MaSaleOrd>> queryBuilder,
            Action<string> logMessage)
        {
            foreach (var singoloId in ids)
            {
                try
                {
                    // Carica l'ordine singolo eseguendo tutte le Include speculari
                    var singleQuery = queryBuilder(context);
                    var testRecord = singleQuery.FirstOrDefault(o => o.SaleOrdId == singoloId);
                }
                catch (Exception exAtomica)
                {
                    string erroreCritico = $"**********************************************************************\n" +
                                           $"!!! [IDENTIFICATO RECORD CORROTTO] !!!\n" +
                                           $"L'ordine ERP con l'esatto SaleOrdId: {singoloId} contiene un valore NULL non mappato.\n" +
                                           $"Messaggio Eccezione: {exAtomica.Message}\n" +
                                           $"**********************************************************************";

                    logMessage?.Invoke(erroreCritico);
                    Debug.WriteLine(erroreCritico);

                    // Rilanciamo un'eccezione esplicita bloccante contenente l'ID individuato
                    throw new Exception($"Elaborazione interrotta. Trovato record orfano/nullo su SaleOrdId: {singoloId}.", exAtomica);
                }
            }
        }

        private bool VerificaEsclusioneContratto(
            AllordCliContratto c,
            FiltriOrdini filtri,
            StringBuilder debugging,
            out bool isDaEscludere)
        {
            isDaEscludere = false;
            var tipologia = (TipologiaServizio)(c.AlltipoRigaServizio.TipologiaServizio ?? 0);

            if (tipologia == TipologiaServizio.Canone && !filtri.PeriodoTutti && !CurOrdRowCalculations.IsBewteenPeriodo(c, filtri.Periodi))
            {
                debugging.AppendLine(" - [ESCLUSA] Fuori filtro periodo");
                return true;
            }
            if (tipologia == TipologiaServizio.Consuntivo)
            {
                debugging.AppendLine(" - [ESCLUSA] Consuntivo");
                return true;
            }
            if (c.Fatturato == "1")
            {
                debugging.AppendLine(" - [ESCLUSA] Fatturata");
                return true;
            }
            if (c.DataDecorrenza == null || c.DataDecorrenza == DomainConstants.DataNulla)
            {
                debugging.AppendLine(" - [ESCLUSA] Decorrenza non Impostata");
                return true;
            }
            if (c.DataDecorrenza > filtri.DataFattA)
            {
                debugging.AppendLine(" - [ESCLUSA] Decorrenza non raggiunta");
                return true;
            }

            if (!(c.DataProssimaFatt >= filtri.DataFattDa && c.DataProssimaFatt <= filtri.DataFattA))
            {
                debugging.AppendLine(" - [ESCLUSA] Fuori filtro data");
                isDaEscludere = true;
            }
            if (c.DataProssimaFatt > filtri.DataFattA)
            {
                debugging.AppendLine(" - [ESCLUSA] Prossima fattura non raggiunta");
                isDaEscludere = true;
            }

            return false;
        }

        private void ElaboraAttivitaContratto(
            AllordCliContratto c,
            CurOrdRow cOrdRow,
            FiltriOrdini filtri,
            List<AllordCliAttivita> attDaRifatturare,
            List<AllordCliAttivita> attIstat,
            List<AllordCliAttivita> efAllordCliAttivita,
            StringBuilder errori,
            StringBuilder debugging)
        {
            if (c.AllordCliAttivita == null || !c.AllordCliAttivita.Any()) return;

            foreach (var att in c.AllordCliAttivita)
            {
                // Sospensione
                if (att.Allattivita?.Sospensione == "1")
                {
                    if (att.Fatturata == "1") continue;

                    if (att.DataInizio.HasValue && att.DataFine.HasValue)
                    {
                        cOrdRow.IsBetweenSospensione(att.DataInizio.Value, att.DataFine.Value);
                    }

                    if (att.DaFatturare == "1" && CurOrdRowCalculations.IsBetweenRipresi(att, filtri.DataFattDa, filtri.DataFattA, out double canoniRipresi))
                    {
                        cOrdRow.DaRifatturare = true;
                        if (canoniRipresi > 0)
                        {
                            cOrdRow.QtaDaRifatturare += canoniRipresi;
                            attDaRifatturare.Add(att);
                            cOrdRow.IsOk = true;

                            att.Fatturata = "1";
                            att.Tbmodified = _now;
                            att.TbmodifiedId = _loginId;
                            efAllordCliAttivita.Add(att);
                        }
                    }
                }

                // Annullamento
                if (!cOrdRow.CanoneFuoriRangeDate && att.Allattivita?.Annullamento == "1")
                {
                    if (cOrdRow.HaAnnullatoDaAttivita)
                    {
                        errori.AppendLine($"Ordine {cOrdRow.Parent.OrdNo} Servizio {cOrdRow.Item}: Annullato da piu' attività. Verrà considerata la prima.");
                    }
                    else if (att.DataInizio.HasValue)
                    {
                        cOrdRow.AnnullaDaAttivita(att.DataInizio.Value);
                    }
                }

                // ISTAT
                if (att.Allattivita?.Istat == "1")
                {
                    if (att.Fatturata == "1") continue;

                    if (att.DaFatturare == "1" && CurOrdRowCalculations.IsBetweenIstat(att, filtri.DataFattDa, filtri.DataFattA))
                    {
                        cOrdRow.IsOk = true;
                        att.Fatturata = "1";
                        att.Tbmodified = _now;
                        att.TbmodifiedId = _loginId;
                        attIstat.Add(att);
                    }
                }
            }
        }

        private void ElaboraScadenzeECessazioni(
            MaSaleOrd o,
            CurOrd cOrd,
            CurOrdRow cOrdRow,
            List<AllordCliAcc> efAllordCliAcc,
            StringBuilder debugging)
        {
            if (cOrd.HaScadenzaFissa && !cOrdRow.CanoneFuoriRangeDate && cOrdRow.CanoniDataFin >= cOrd.DataScadenzaFissa)
            {
                cOrdRow.HaScadenzaFissa = true;
                cOrdRow.DataScadenzaFissa = cOrd.DataScadenzaFissa;
            }

            if (cOrd.HaDataCessazione && !cOrdRow.CanoneFuoriRangeDate && cOrdRow.CanoniDataFin >= cOrd.DataCessazione)
            {
                cOrdRow.HaDataCessazione = true;
                cOrdRow.DataCessazione = cOrd.DataCessazione;
            }

            if (!cOrdRow.CanoneFuoriRangeDate && (cOrdRow.HaScadenzaFissa || cOrdRow.HaDataCessazione || cOrdRow.HaAnnullatoDaAttivita))
            {
                bool bEsci = false;

                // Priorità 1: Attività
                if (cOrdRow.HaAnnullatoDaAttivita)
                {
                    bEsci = true;
                    if (cOrdRow.IsBetweenAnnullamentoAttivita(out string msg))
                    {
                        debugging.AppendLine(msg);
                        cOrdRow.PeriodoDataFin = cOrdRow.DataCessazioneDaAttivita;
                    }
                }

                // Priorità 2: Scadenza Fissa
                if (!bEsci && cOrdRow.HaScadenzaFissa)
                {
                    bEsci = true;
                    if (cOrdRow.IsBetweenAnnullamentoScadenzaFissa(out string msg))
                    {
                        debugging.AppendLine(msg);
                        cOrdRow.PeriodoDataFin = cOrdRow.DataScadenzaFissa;
                    }

                    if (o.ALLOrdCliAcc != null && o.ALLOrdCliAcc.DataCessazione == DomainConstants.DataNulla)
                    {
                        o.ALLOrdCliAcc.DataCessazione = cOrdRow.DataScadenzaFissa;
                        o.ALLOrdCliAcc.MotivoCessazione = "[AUTO] Scadenza Fissa";
                        o.ALLOrdCliAcc.Tbmodified = _now;
                        o.ALLOrdCliAcc.TbmodifiedId = _loginId;
                        efAllordCliAcc.Add(o.ALLOrdCliAcc);
                    }
                }

                // Priorità 3: Cessazione
                if (!bEsci && cOrdRow.HaDataCessazione)
                {
                    if (cOrdRow.IsBetweenAnnullamentoCessazione(out string msg))
                    {
                        debugging.AppendLine(msg);
                        cOrdRow.PeriodoDataFin = cOrdRow.DataCessazione;
                        if (o.ALLOrdCliAcc != null && string.IsNullOrWhiteSpace(o.ALLOrdCliAcc.MotivoCessazione))
                        {
                            o.ALLOrdCliAcc.MotivoCessazione = "[AUTO] Data Cessazione";
                            o.ALLOrdCliAcc.Tbmodified = _now;
                            o.ALLOrdCliAcc.TbmodifiedId = _loginId;
                            efAllordCliAcc.Add(o.ALLOrdCliAcc);
                        }
                    }
                }
            }
        }

        #endregion

        #region Costruzione record MaSaleOrdDetails

        private MaSaleOrdDetails CreaRigaNota(MaSaleOrdDetails riga, // Unificato: descrizione generica + riferimento
                CurOrd cOrd, CurOrdRow cOrdRow, string descrizione, int iNewRowsCount, LineType lineType, short position = 0)
        {
            riga.Line = (short)(cOrd.LastLine + iNewRowsCount);
            riga.Position = position;
            riga.SubId = cOrd.LastSubId + iNewRowsCount;
            riga.SaleOrdId = cOrd.SaleOrdId;
            riga.LineType = (int)lineType;
            riga.Description = descrizione;
            riga.InEi = "1";
            riga.ExpectedDeliveryDate = cOrdRow.DataPrevistaConsegna;
            riga.ConfirmedDeliveryDate = cOrdRow.DataConfermaConsegna;
            riga.InternalOrdNo = cOrd.OrdNo;
            riga.Customer = cOrd.Cliente;
            riga.OrderDate = cOrd.OrdDate;
            riga.Tbcreated = _now;
            riga.Tbmodified = _now;
            riga.TbcreatedId = _loginId;
            riga.TbmodifiedId = _loginId;
            riga.Item = string.Empty;
            riga.UoM = string.Empty;
            riga.Qty = 0;
            riga.UnitValue = 0;
            riga.TaxableAmount = 0;
            riga.TotalAmount = 0;
            riga.PacksUoM = string.Empty;
            riga.TaxCode = string.Empty;
            riga.Job = string.Empty;
            riga.CostCenter = string.Empty;
            riga.Offset = string.Empty;
            riga.Notes = string.Empty;
            riga.NetPrice = 0;
            riga.ContractCode = string.Empty;
            riga.ProjectCode = string.Empty;
            riga.AllNrCanoni = 0;
            riga.AllCanoniDataI = DomainConstants.DataNulla;
            riga.AllCanoniDataF = DomainConstants.DataNulla;
            riga.Invoiced = "0";

            return riga;
        }

        private MaSaleOrdDetails CreaRigaRifatturazione(
            MaSaleOrdDetails riga,
            CurOrd cOrd,
            CurOrdRow cOrdRow,
            AllordCliAttivita aDaRif,
            int iNewRowsCount,
            short pos,
            StringBuilder errori,
            StringBuilder debugging)
        {
            var dataInizio = aDaRif.DataInizio ?? DomainConstants.DataNulla;
            var dataFine = aDaRif.DataFine ?? DomainConstants.DataNulla;
            string periodoRif = $"Periodo dal {dataInizio:dd/MM/yyyy} al {dataFine:dd/MM/yyyy}";

            if (aDaRif.ValUnit == 0)
                errori.AppendLine($"Ordine: {cOrd.OrdNo} Pos.: {pos} Servizio: {cOrdRow.Item} con valore unitario di ripresa pari a 0.00");

            double valUnit = Math.Round(aDaRif.ValUnit ?? 0, DomainConstants.DecimaliTasse);
            double qty = cOrdRow.QtaDaRifatturare;
            double taxable = Math.Round(qty * valUnit, DomainConstants.DecimaliTasse);
            double total = Math.Round(taxable * ((100 + cOrdRow.PercIva) / 100), DomainConstants.DecimaliTasse);

            riga.Line = (short)(cOrd.LastLine + iNewRowsCount);
            riga.Position = pos;
            riga.SubId = cOrd.LastSubId + iNewRowsCount;
            riga.SaleOrdId = cOrd.SaleOrdId;
            riga.LineType = (int)LineType.Servizio;
            riga.Item = cOrdRow.Item;
            riga.Description = string.IsNullOrWhiteSpace(cOrdRow.Description) ? periodoRif : $"{cOrdRow.Description} {periodoRif}";
            riga.UoM = cOrdRow.UoM;
            riga.PacksUoM = cOrdRow.UoM;
            riga.Qty = qty;
            riga.UnitValue = valUnit;
            riga.NetPrice = valUnit;
            riga.TaxableAmount = taxable;
            riga.TaxCode = cOrdRow.CodIva;
            riga.TotalAmount = total;
            riga.ExpectedDeliveryDate = cOrdRow.DataPrevistaConsegna;
            riga.ConfirmedDeliveryDate = cOrdRow.DataConfermaConsegna;
            riga.AllNrCanoni = qty;
            riga.AllCanoniDataI = dataInizio;
            riga.AllCanoniDataF = dataFine;
            riga.Invoiced = "0";
            riga.Notes = StringHelpers.SubstringSafe(aDaRif.Nota, 32);
            riga.Job = cOrd.Commessa;
            riga.CostCenter = cOrd.CdC;
            riga.ContractCode = cOrd.CIG;
            riga.ProjectCode = cOrd.CUP;
            riga.Offset = cOrdRow.Contropartita;
            riga.InternalOrdNo = cOrd.OrdNo;
            riga.Customer = cOrd.Cliente;
            riga.OrderDate = cOrd.OrdDate;
            riga.Tbcreated = _now;
            riga.Tbmodified = _now;
            riga.TbcreatedId = _loginId;
            riga.TbmodifiedId = _loginId;

            return riga;
        }



        private MaSaleOrdDetails CreaRigaServizioValore(
            MaSaleOrdDetails riga,
            AllordCliContratto c,
            CurOrd cOrd,
            CurOrdRow cOrdRow,
            int iNewRowsCount,
            short pos)
        {
            string periodo = $"Periodo dal {cOrdRow.PeriodoDataIn:d} al {cOrdRow.PeriodoDataFin:d}";
            string descrizioneFinale;

            if (cOrdRow.IsUnaTantum)
            {
                descrizioneFinale = cOrdRow.Description;
                c.Fatturato = "1";
            }
            else if (cOrdRow.IsContinuativo)
            {
                descrizioneFinale = cOrdRow.Description;
            }
            else
            {
                descrizioneFinale = string.IsNullOrWhiteSpace(cOrdRow.Description) ? periodo : $"{cOrdRow.Description} {periodo}";
            }

            // Calcolo Qty
            double qty;
            if (cOrdRow.IsAConsumo)
            {
                if (cOrdRow.QtaCorrente + cOrdRow.QtaFranchigia >= cOrdRow.QtaOrdine)
                {
                    c.Fatturato = "1";
                    qty = Math.Max(0, cOrdRow.QtaOrdine - cOrdRow.QtaFranchigia);
                }
                else
                {
                    qty = cOrdRow.QtaCorrente;
                }
                cOrdRow.QtaFranchigia += qty;
                c.Franchigia = cOrdRow.QtaFranchigia;
            }
            else if (cOrdRow.IsContinuativo)
            {
                qty = cOrdRow.QtaOrdine;
            }
            else
            {
                qty = cOrdRow.QtaCorrente;
            }

            cOrdRow.QtaCorrente = qty;
            double unitVal = Math.Round(cOrdRow.ValUnit, DomainConstants.DecimaliTasse);
            double taxable = Math.Round(qty * unitVal, DomainConstants.DecimaliTasse);
            double total = Math.Round(taxable * ((100 + cOrdRow.PercIva) / 100), DomainConstants.DecimaliTasse);

            riga.Line = (short)(cOrd.LastLine + iNewRowsCount);
            riga.Position = pos;
            riga.SubId = cOrd.LastSubId + iNewRowsCount;
            riga.SaleOrdId = cOrd.SaleOrdId;
            riga.LineType = (int)LineType.Servizio;
            riga.Item = cOrdRow.Item;
            riga.Description = descrizioneFinale;
            riga.UoM = cOrdRow.UoM;
            riga.PacksUoM = cOrdRow.UoM;
            riga.Qty = qty;
            riga.UnitValue = unitVal;
            riga.NetPrice = unitVal;
            riga.TaxableAmount = taxable;
            riga.TaxCode = cOrdRow.CodIva;
            riga.TotalAmount = total;
            riga.ExpectedDeliveryDate = cOrdRow.DataPrevistaConsegna;
            riga.ConfirmedDeliveryDate = cOrdRow.DataConfermaConsegna;
            riga.AllNrCanoni = qty;
            riga.AllCanoniDataI = cOrdRow.PeriodoDataIn;
            riga.AllCanoniDataF = cOrdRow.PeriodoDataFin;
            riga.Invoiced = "0";
            riga.Notes = StringHelpers.SubstringSafe(c.Nota, 32);
            riga.Job = cOrd.Commessa;
            riga.CostCenter = cOrd.CdC;
            riga.ContractCode = cOrd.CIG;
            riga.ProjectCode = cOrd.CUP;
            riga.Offset = cOrdRow.Contropartita;
            riga.InternalOrdNo = cOrd.OrdNo;
            riga.Customer = cOrd.Cliente;
            riga.OrderDate = cOrd.OrdDate;
            riga.Tbcreated = _now;
            riga.Tbmodified = _now;
            riga.TbcreatedId = _loginId;
            riga.TbmodifiedId = _loginId;

            return riga;
        }

        private static bool CheckRigaBianca(MaSaleOrdDetails d)
        {
            if (string.IsNullOrWhiteSpace(d.Item) && d.LineType == (int)LineType.Merce && d.Line == 1 && d.Position == 1)
                return true;

            if ((d.LineType == (int)LineType.Merce || d.LineType == (int)LineType.Servizio) && d.UnitValue == 0 && d.Line == 1 && d.Position == 1)
                return true;

            return false;
        }

        #endregion

        #region Bulk Execution

        private void EseguiSalvataggioTransazionale(
            int totNuove,
            int totAggiornate,
            List<MaSaleOrd> ordini,
            List<MaSaleOrdDetails> details,
            //List<MaSaleOrdSummary> summaries,
            List<AllordCliAcc> accessorie,
            List<AllordCliAttivita> attivita,
            List<AllordCliContratto> contratti,
            StringBuilder bulkMessage,
            StringBuilder errori,
            Action<string, AppConfig.LogTarget> logMessage)
        {
            if (totNuove == 0 && totAggiornate == 0) return;

            bool esito = BulkPersistenceHelper.ExecuteInBulkTransaction(_context, () =>
            {
                BulkPersistenceHelper.BulkExecuteList(_context, ordini, "MaSaleOrd", BulkOperation.Update, "teste ordini", bulkMessage);
                BulkPersistenceHelper.BulkExecuteList(_context, details, "MaSaleOrdDetails", BulkOperation.Insert, "righe ordini", bulkMessage);
                //BulkPersistenceHelper.BulkExecuteList(_context, summaries, "MaSaleOrdSummary", BulkOperation.InsertOrUpdate, "totali ordini", bulkMessage);
                BulkPersistenceHelper.BulkExecuteList(_context, accessorie, "AllordCliAcc", BulkOperation.Update, "dati accessori ordini", bulkMessage);
                BulkPersistenceHelper.BulkExecuteList(_context, attivita, "AllordCliAttivita", BulkOperation.Update, "righe attività", bulkMessage);
                BulkPersistenceHelper.BulkExecuteList(_context, contratti, "AllordCliContratto", BulkOperation.Update, "righe contratto", bulkMessage);
            }, logMessage, errori);

            if (esito)
            {
                string riassunto = $"[RIASSUNTO] Ordini con Righe: {totNuove} | Ordini con Adeguamenti: {totAggiornate}";
                bulkMessage.AppendLine(riassunto);
                logMessage?.Invoke(riassunto, AppConfig.LogTarget.Ui);
            }
        }

        #endregion

        #region Stato elaborazione

        /// <summary>
        /// Stato mutabile dell'elaborazione canoni: collettori di update/insert, accumulatori
        /// di log, default ERP e prototipo riga. Consente di tenere l'elaborazione di singolo
        /// ordine/contratto in metodi dedicati senza lunghe liste di parametri.
        /// </summary>
        private sealed class StatoElaborazioneOrdini
        {
            public FiltriOrdini Filtri { get; }
            public int LoginId { get; }
            public DateTime Now { get; }

            public MaSaleOrdDetails PrototipoRiga { get; private set; }
            public DefaultVenditeIva Default { get; private set; }

            public StringBuilder BulkMessage { get; } = new StringBuilder();
            public StringBuilder Errori { get; } = new StringBuilder();
            public StringBuilder Avvisi { get; } = new StringBuilder();
            public StringBuilder Debugging { get; } = new StringBuilder();

            public List<MaSaleOrd> UpdateMaSaleOrd { get; } = new List<MaSaleOrd>();
            public List<AllordCliContratto> UpdateAllordCliContratto { get; } = new List<AllordCliContratto>();
            public List<AllordCliAttivita> UpdateAllordCliAttivita { get; } = new List<AllordCliAttivita>();
            public List<AllordCliAcc> UpdateAllordCliAcc { get; } = new List<AllordCliAcc>();
            public List<MaSaleOrdDetails> InsertMaSaleOrdDetails { get; } = new List<MaSaleOrdDetails>();

            public int TotOrdiniConNuoveRighe { get; set; }
            public int TotOrdiniConRigheAggiornate { get; set; }

            public StatoElaborazioneOrdini(FiltriOrdini filtri, int loginId, DateTime now)
            {
                Filtri = filtri;
                LoginId = loginId;
                Now = now;
            }

            /// <summary>Imposta prototipo riga e default ERP (dopo la connessione/contesto).</summary>
            public void Inizializza(MaSaleOrdDetails prototipoRiga, DefaultVenditeIva def)
            {
                PrototipoRiga = prototipoRiga;
                Default = def;
            }
        }

        /// <summary>Stato locale relativo all'elaborazione di un singolo ordine.</summary>
        private sealed class StatoOrdine
        {
            public CurOrd COrd { get; }
            public int CurLastPosition { get; set; }
            public int INewRowsCount { get; set; }
            public int INrRigheAValore { get; set; }
            public bool IsNewRows { get; set; }
            public bool IsUpdateRows { get; set; }
            public bool BScrittoDescrizioni { get; set; }

            public StatoOrdine(CurOrd cOrd, int curLastPosition)
            {
                COrd = cOrd;
                CurLastPosition = curLastPosition;
            }
        }

        #endregion
    }
}