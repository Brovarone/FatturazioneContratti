using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using EFCore.BulkExtensions;
using EFMago.Models;
using FatturazioneContratti.Common;
using FatturazioneContratti.Data;
using Microsoft.EntityFrameworkCore;

namespace FatturazioneContratti.Orders
{
    public partial class GenerazioneRigheOrdiniService
    {
        /// <summary>
        /// Causale attività usata per le righe di adeguamento ISTAT quando
        /// <see cref="FiltriOrdini.CausaleAttivita"/> è vuota (fallback UI).
        /// Deve esistere in ALLAttivita con flag Istat = "1".
        /// </summary>
        private const string CausaleAttivitaIstatDefault = "ISTAT";

        /// <summary>
        /// Rivaluta le righe contratto in base alla percentuale ISTAT specificata nei filtri,
        /// aggiorna il valore unitario rivalutato (ValUnitIstat), adegua le eventuali attività sospese
        /// e genera le nuove righe di attività "ISTAT" su ALLOrdCliAttivita.
        /// Porting fedele di Ordini.vb (righe 887-1272).
        /// </summary>
        public bool AdeguaIstatOrdine(
            FiltriOrdini filtri,
            int dimensioneBlocco = 100,
            Action<string, LogTarget> logMessage = null,
            Action<string> updateStatus = null,
            ProgressCallbacks? progress1 = null,
            ProgressCallbacks? progress2 = null,
            CancellationToken token = default)
        {
            if (filtri.PercIstat == 0)
            {
                LogDiagnostica.Errore(logMessage, "Elaborazione Interrotta: Percentuale Adeguamento ISTAT pari a Zero o non valida.");
                return false;
            }

            var bulkMessage = new StringBuilder();
            var errori = new StringBuilder();
            var avvisi = new StringBuilder();
            var debugging = new StringBuilder();

            int totOrdini = 0;
            int totOrdiniOk = 0;

            try
            {
                // I filtri della run vengono registrati su UI/file: il log di sessione
                // deve dire con quali parametri è partito l'adeguamento.
                LogDiagnostica.LogFiltri(logMessage, filtri);

                updateStatus?.Invoke("Estrazione ordini per adeguamento ISTAT");

                // -------------------------------------------------------------------------
                // STEP 1: ESTRAZIONE SOLO DEGLI ID (stesso pattern ID-first di GeneraRigheOrdine:
                // i filtri sulla navigation ALLOrdCliAcc vengono tradotti in EXISTS, quindi
                // nessun Include necessario e nessuna esplosione cartesiana).
                // Prima qui veniva fatto ToList() sull'intero grafo: con molti ordini la
                // split-query materializzava tutto in RAM in un colpo solo.
                // -------------------------------------------------------------------------
                IQueryable<MaSaleOrd> query = _context.MaSaleOrd.AsNoTracking();

                // Filtro base: ordini con flag ISTAT attivo e non cessati prima della decorrenza
                query = query.Where(o => o.ALLOrdCliAcc != null
                    && o.ALLOrdCliAcc.ApplicoIstat == "1"
                    && (!o.ALLOrdCliAcc.DataCessazione.HasValue
                        || o.ALLOrdCliAcc.DataCessazione == DomainConstants.DataNulla
                        || o.ALLOrdCliAcc.DataCessazione > filtri.DataDecorrenza));

                // Filtri comuni utente (date/cliente/ordine/filiale), condivisi con GeneraRigheOrdine
                query = ApplicaFiltriComuni(query, filtri);

                var allOrderIds = EstraiIdsOrdini(query);

                if (allOrderIds.Count == 0)
                {
                    LogDiagnostica.Informativo(logMessage, "Nessun ordine estratto con i filtri ISTAT specificati.");
                    return true;
                }

                totOrdini = allOrderIds.Count;
                string msgTot = $"Ordini Estratti per ISTAT: {totOrdini}";
                Debug.WriteLine(msgTot);
                LogDiagnostica.Informativo(logMessage, msgTot);
                updateStatus?.Invoke(msgTot);

                var efAllordCliAttivita = new List<AllordCliAttivita>();
                var efAllordCliContratto = new List<AllordCliContratto>();

                // Punto 6: la causale arriva dalla UI (TxtIstatAttivita -> FiltriOrdini.CausaleAttivita).
                // Fallback alla costante se l'utente la lascia vuota.
                string causaleAttivitaIstat = string.IsNullOrWhiteSpace(filtri.CausaleAttivita)
                    ? CausaleAttivitaIstatDefault
                    : filtri.CausaleAttivita.Trim();
                // -------------------------------------------------------------------------
                // STEP 2: CICLO A BLOCCHI SULLA LISTA DI ID (loader condiviso con GeneraRigheOrdine,
                // include anche le attività di testa necessarie a curLastLineAtt).
                // Implementato come iterator locale: il corpo del foreach seguente resta
                // invariato e la progress2 avanza per ordine, la progress1 per blocco.
                // -------------------------------------------------------------------------
                IEnumerable<MaSaleOrd> CaricaOrdiniPerBlocco()
                {
                    for (int i = 0; i < totOrdini; i += dimensioneBlocco)
                    {
                        // Confine di blocco: qui si può interrompere senza lavoro a metà.
                        token.ThrowIfCancellationRequested();

                        int blockSize = Math.Min(dimensioneBlocco, totOrdini - i);
                        progress2?.Init(0, Math.Max(1, blockSize));

                        int[] blocId = allOrderIds.Skip(i).Take(blockSize).ToArray();
                        var allOrdersChunk = LoadOrdersChunkByOrIn(
                            _context,
                            blocId,
                            SqlTools.Options.IsDebugging,
                            // Messaggi della scansione diagnostica: sono sempre segnalazioni
                            // di errore/crash, quindi vanno su UI e file.
                            msg => LogDiagnostica.Errore(logMessage, msg));

                        foreach (var o in allOrdersChunk)
                        {
                            // Confine ordine: nessuna transazione aperta in questa fase.
                            token.ThrowIfCancellationRequested();
                            progress2?.Advance(); // avanza la piccola per ogni ordine
                            yield return o;
                        }

                        progress2?.Finish();   // fine blocco piccolo
                        progress1?.Advance();  // un passo per il blocco completato
                    }
                }

                // inizializzo progress principale (se richiesto): un passo per blocco
                int numeroBlocchi = (totOrdini + dimensioneBlocco - 1) / dimensioneBlocco;
                progress1?.Init(0, Math.Max(1, numeroBlocchi));

                foreach (var o in CaricaOrdiniPerBlocco())
                {
                    // Confine ordine (granularità dell'interruzione).
                    token.ThrowIfCancellationRequested();

                    debugging.AppendLine($"Ordine: {o.InternalOrdNo}");

                    bool isNewRowsAtt = false;
                    int iNewRowsAttCount = 0;
                    int curLastLineAtt = o.AllordCliAttivita != null && o.AllordCliAttivita.Count != 0 
                        ? o.AllordCliAttivita.Max(m => m.Line) 
                        : 0;

                    var cOrd = new CurOrd
                    {
                        SaleOrdId = o.SaleOrdId,
                        Cliente = o.Customer,
                        OrdNo = o.InternalOrdNo,
                        DataScadenzaFissa = o.ALLOrdCliAcc?.DataScadenzaFissa ?? DomainConstants.DataNulla
                    };

                    if (o.ALLordCliContratto == null || o.ALLordCliContratto.Count == 0)
                        continue;

                    foreach (var c in o.ALLordCliContratto)
                    {
                        // Confine contratto: nessuna interruzione a metà riga.
                        token.ThrowIfCancellationRequested();

                        string sEx = $"{c.Line}) {c.TipoRigaServizio} {c.Servizio}";

                        // Controllo Esclusioni
                        var tipologia = (TipologiaServizio)(c.AlltipoRigaServizio?.TipologiaServizio ?? 0);
                        if (tipologia == TipologiaServizio.Consuntivo)
                        {
                            debugging.AppendLine($"# [ESCLUSA] (Consuntivo) R.:({sEx})");
                            continue;
                        }
                        if (c.Fatturato == "1")
                        {
                            debugging.AppendLine($"# [ESCLUSA] (Fatturata) R.:({sEx})");
                            continue;
                        }
                        if (!c.DataDecorrenza.HasValue || c.DataDecorrenza == DomainConstants.DataNulla)
                        {
                            debugging.AppendLine($"# [ESCLUSA] (Decorrenza non Impostata) R.:({sEx})");
                            continue;
                        }
                        if (c.DataDecorrenza > filtri.DataDecorrenza)
                        {
                            debugging.AppendLine($"# [ESCLUSA] (Decorrenza non raggiunta) R.:({sEx})");
                            continue;
                        }

                        debugging.AppendLine($"# R. :({sEx})");

                        var cOrdRow = new CurOrdRow(c);
                        cOrdRow.ValUnit = Math.Round(c.ValUnitIstat ?? 0, DomainConstants.DecimaliTasse);
                        if (cOrdRow.ValUnit <= 0)
                        {
                            errori.AppendLine($"Ordine: {cOrd.OrdNo} Valore Unitario Att <= 0 // Riga: ({sEx})");
                        }

                        var attDaAdeguare = new List<AllordCliAttivita>();
                        bool isIstat = false;

                        // Analisi Attività collegate alla riga di contratto
                        if (c.AllordCliAttivita != null)
                        {
                            foreach (var att in c.AllordCliAttivita)
                            {
                                debugging.AppendLine($"## Attività:({att.Line}) {att.Attivita} {att.DataInizio:d} {att.RifServizio} {att.RifLinea} {att.Nota}");

                                // STEP 3a: Sospensione
                                if (att.Allattivita?.Sospensione == "1")
                                {
                                    if (att.Fatturata == "1")
                                    {
                                        debugging.AppendLine("## [Fatturata] ##");
                                        continue;
                                    }

                                    bool annoInizioUguale = att.DataInizio.HasValue && att.DataInizio.Value.Year == filtri.AnnoAdeguamento;
                                    bool annoFineUguale = att.DataFine.HasValue && att.DataFine.Value.Year == filtri.AnnoAdeguamento;

                                    if (annoInizioUguale || annoFineUguale)
                                    {
                                        cOrdRow.SospesoDaAttivita = true;
                                        debugging.AppendLine($"## [Sospensione] ## : dal {att.DataInizio:d} al {att.DataFine:d}");
                                        attDaAdeguare.Add(att);
                                        cOrdRow.IsOk = true;
                                    }
                                }

                                // STEP 3b: Annullamento
                                if (att.Allattivita?.Annullamento == "1")
                                {
                                    cOrdRow.HaAnnullatoDaAttivita = true;
                                    var dataInizio = att.DataInizio ?? DomainConstants.DataNulla;

                                    if (cOrdRow.IsBetweenAnnullamento(dataInizio, out double dCanoniResidui))
                                    {
                                        if (dCanoniResidui > 0)
                                        {
                                            debugging.AppendLine("## Annullamento Parziale ##");
                                        }
                                        else
                                        {
                                            cOrdRow.IsOk = false;
                                            debugging.AppendLine("## Annullamento Parziale -> Totale ##");
                                        }
                                    }
                                    else if (dataInizio != DomainConstants.DataNulla && dataInizio < cOrdRow.CanoniDataIn)
                                    {
                                        // Completamente annullata
                                        cOrdRow.IsOk = false;
                                        debugging.AppendLine("## Annullamento Totale ##");
                                        break;
                                    }
                                }

                                // STEP 3c: Controllo se l'adeguamento ISTAT è già stato applicato per quest'anno
                                if (att.Allattivita?.Istat == "1")
                                {
                                    if (att.DaFatturare == "1" && att.DataRifatturazione.HasValue && att.DataRifatturazione.Value.Year == filtri.AnnoAdeguamento)
                                    {
                                        isIstat = true;
                                        avvisi.AppendLine($"Ordine: {cOrd.OrdNo} con riga di adeguamento Istat già presente per l'anno: {filtri.AnnoAdeguamento}");
                                        cOrdRow.IsOk = false;
                                    }
                                }
                            }
                        }

                        // STEP 4: Scadenza Fissa
                        if (cOrd.DataScadenzaFissa != DomainConstants.DataNulla && cOrdRow.CanoniDataFin > cOrd.DataScadenzaFissa)
                        {
                            avvisi.AppendLine($"Ordine: {cOrd.OrdNo} Cliente: {cOrd.Cliente} con scadenza fissa. Controllare canoni!");
                            debugging.AppendLine($"Ordine: {cOrd.OrdNo} Cliente: {cOrd.Cliente} con scadenza fissa. Controllare canoni!");

                            if (cOrdRow.IsBetweenAnnullamento(cOrd.DataScadenzaFissa, out double dCanoniFinoA))
                            {
                                if (dCanoniFinoA > 0)
                                {
                                    debugging.AppendLine("## Scadenza Fissa ##");
                                }
                                else
                                {
                                    cOrdRow.IsOk = false;
                                    debugging.AppendLine("## Scadenza Fissa -> Totale ##");
                                }
                            }
                        }

                        // Applicazione Adeguamento ISTAT
                        if (cOrdRow.IsOk && !isIstat)
                        {
                            isNewRowsAtt = true;
                            debugging.AppendLine($"### Riga contratto:({c.Line}) {c.Servizio}");

                            double newValUnit = Math.Round(cOrdRow.ValUnit * (1.0 + (filtri.PercIstat / 100.0)), DomainConstants.DecimaliTasse);
                            c.ValUnitIstat = newValUnit;
                            c.DataUltRivIstat = _now;
                            c.DataFineElaborazione = _now;
                            efAllordCliContratto.Add(c);

                            // Pattern prototipo: materializza i default Fluent API (HasDefaultValueSql)
                            // come il DataSet.AddNew del VB. Con "new AllordCliAttivita" le colonne
                            // non valorizzate restavano al default CLR invece del default Mago.
                            iNewRowsAttCount++;
                            var rAtt = _context.AddNew<AllordCliAttivita>();
                            rAtt.IdOrdCli = cOrd.SaleOrdId;
                            rAtt.Line = (short)(curLastLineAtt + iNewRowsAttCount);
                            rAtt.Attivita = causaleAttivitaIstat;
                            rAtt.DataInizio = filtri.DataDecorrenza;
                            rAtt.DataFine = DomainConstants.DataNulla;
                            rAtt.DaFatturare = "1";
                            rAtt.DataRifatturazione = cOrdRow.DataPrevistaConsegna;
                            rAtt.Fatturata = "0";
                            rAtt.Nota = $"Adeguamento ISTAT ({filtri.PercIstat}%). Precedente: {cOrdRow.ValUnit}";
                            rAtt.TestoFattura = filtri.TestoFattura;
                            rAtt.RifServizio = cOrdRow.Item;
                            rAtt.RifLinea = cOrdRow.Line;
                            rAtt.ValUnit = newValUnit;
                            rAtt.Tbcreated = _now;
                            rAtt.Tbmodified = _now;
                            rAtt.TbcreatedId = _loginId;
                            rAtt.TbmodifiedId = _loginId;
                            efAllordCliAttivita.Add(rAtt);

                            // Adeguamento righe di attività sospese
                            foreach (var adeg in attDaAdeguare)
                            {
                                adeg.ValUnit = newValUnit;
                                adeg.TestoFattura = "Adeguato a ISTAT";
                                adeg.Tbmodified = _now;
                                adeg.TbmodifiedId = _loginId;
                                efAllordCliAttivita.Add(adeg);
                            }
                        }
                    }

                    if (isNewRowsAtt)
                    {
                        totOrdiniOk++;
                        debugging.AppendLine($"#### Ordine: {o.InternalOrdNo} Nuove righe Attività: {iNewRowsAttCount}");
                    }
                }

                // finalize progress
                progress1?.Finish();

                // Salvataggio Bulk transazionale
                string msgValid = $"Ordini con righe Contratto valide per ISTAT: {totOrdiniOk}";
                LogDiagnostica.Informativo(logMessage, msgValid);

                if (totOrdiniOk > 0)
                {
                    updateStatus?.Invoke("Salvataggio modifiche ISTAT in corso...");

                    bool esito = BulkPersistenceHelper.ExecuteInBulkTransaction(_context, () =>
                    {
                        BulkPersistenceHelper.BulkExecuteList(_context, efAllordCliAttivita, "ALLOrdCliAttivita", BulkOperation.InsertOrUpdate, "righe attività", bulkMessage, token);
                        BulkPersistenceHelper.BulkExecuteList(_context, efAllordCliContratto, "ALLOrdCliContratto", BulkOperation.Update, "righe contratto", bulkMessage, token);
                    }, logMessage, token);

                    if (esito)
                    {
                        string esitoMsg = $"Processati con successo: {totOrdiniOk} ordini per adeguamento ISTAT.";
                        LogDiagnostica.Informativo(logMessage, esitoMsg);
                    }
                    else
                    {
                        // Rollback già riportato da BulkPersistenceHelper: qui solo l'esito.
                        return false;
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Interruzione utente: risale a FLogin (esito "interrotto").
                throw;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                // Messaggio + UI/file, stack sul file (prima finiva solo su UI con ex.Message).
                LogDiagnostica.Errore(logMessage, ex);
                return false;
            }
            finally
            {
                // Errori e avvisi sempre, statistiche bulk e tracciato diagnostico
                // secondo la regola di routing (vedi TODO §1b).
                LogDiagnostica.Scarica(logMessage, errori, avvisi, bulkMessage, debugging);
            }

            return errori.Length == 0;
        }
    }
}