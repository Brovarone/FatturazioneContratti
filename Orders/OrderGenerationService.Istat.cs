using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using EFCore.BulkExtensions;
using EFMago.Models;
using FatturazioneContratti.Common;
using Microsoft.EntityFrameworkCore;

namespace FatturazioneContratti.Orders
{
    public partial class OrderGenerationService
    {
        /// <summary>
        /// Rivaluta le righe contratto in base alla percentuale ISTAT specificata nei filtri,
        /// aggiorna il valore unitario rivalutato (ValUnitIstat), adegua le eventuali attività sospese
        /// e genera le nuove righe di attività "ISTAT" su ALLOrdCliAttivita.
        /// Porting fedele di Ordini.vb (righe 887-1272).
        /// </summary>
        public bool AdeguaIstatOrdine(
            FiltriOrdini filtri,
            Action<string> logMessage = null,
            Action<string> updateStatus = null,
            Action advanceProgress = null)
        {
            if (filtri.PercIstat == 0)
            {
                logMessage?.Invoke("Elaborazione Interrotta: Percentuale Adeguamento ISTAT pari a Zero o non valida.");
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
                updateStatus?.Invoke("Estrazione ordini per adeguamento ISTAT");

                // Query con Include delle relazioni richieste per il controllo
                IQueryable<MaSaleOrd> query = _context.MaSaleOrd
                    .Include(acc => acc.ALLOrdCliAcc)
                    .Include(con => con.ALLordCliContratto)
                        .ThenInclude(it => it.MaItems)
                    .Include(con => con.ALLordCliContratto)
                        .ThenInclude(trs => trs.AlltipoRigaServizio)
                    .Include(con => con.ALLordCliContratto)
                        .ThenInclude(att => att.AllordCliAttivita)
                            .ThenInclude(at => at.Allattivita)
                    .Include(att => att.AllordCliAttivita)
                        .ThenInclude(at => at.Allattivita);

                // Filtro base: ordini con flag ISTAT attivo e non cessati prima della decorrenza
                query = query.Where(o => o.ALLOrdCliAcc != null 
                    && o.ALLOrdCliAcc.ApplicoIstat == "1"
                    && (!o.ALLOrdCliAcc.DataCessazione.HasValue 
                        || o.ALLOrdCliAcc.DataCessazione == DomainConstants.DataNulla 
                        || o.ALLOrdCliAcc.DataCessazione > filtri.DataDecorrenza));

                // Filtri opzionali utente
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

                if (filtri.SingoloCliente) query = query.Where(o => o.Customer == filtri.Cliente);
                if (filtri.SingoloOrdine) query = query.Where(o => o.InternalOrdNo == filtri.NrOrd);
                if (filtri.SingolaFiliale) query = query.Where(o => o.CostCenter == filtri.Filiale);

                var allOrders = query.ToList();
                if (!allOrders.Any())
                {
                    logMessage?.Invoke("Nessun ordine estratto con i filtri ISTAT specificati.");
                    return true;
                }

                totOrdini = allOrders.Count;
                string msgTot = $"Ordini Estratti per ISTAT: {totOrdini}";
                Debug.WriteLine(msgTot);
                logMessage?.Invoke(msgTot);
                updateStatus?.Invoke(msgTot);

                var efAllordCliAttivita = new List<AllordCliAttivita>();
                var efAllordCliContratto = new List<AllordCliContratto>();

                foreach (var o in allOrders)
                {
                    advanceProgress?.Invoke();
                    debugging.AppendLine($"Ordine: {o.InternalOrdNo}");

                    bool isNewRowsAtt = false;
                    int iNewRowsAttCount = 0;
                    int curLastLineAtt = o.AllordCliAttivita != null && o.AllordCliAttivita.Any() 
                        ? o.AllordCliAttivita.Max(m => m.Line) 
                        : 0;

                    var cOrd = new CurOrd
                    {
                        SaleOrdId = o.SaleOrdId,
                        Cliente = o.Customer,
                        OrdNo = o.InternalOrdNo,
                        DataScadenzaFissa = o.ALLOrdCliAcc?.DataScadenzaFissa ?? DomainConstants.DataNulla
                    };

                    if (o.ALLordCliContratto == null || !o.ALLordCliContratto.Any())
                        continue;

                    foreach (var c in o.ALLordCliContratto)
                    {
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
                            c.DataUltRivIstat = DateTime.Now;
                            c.DataFineElaborazione = DateTime.Now;
                            efAllordCliContratto.Add(c);

                            // Creazione riga nuova attività di tipo ISTAT
                            iNewRowsAttCount++;
                            var rAtt = new AllordCliAttivita
                            {
                                IdOrdCli = cOrd.SaleOrdId,
                                Line = (short)(curLastLineAtt + iNewRowsAttCount),
                                Attivita = "ISTAT",
                                DataInizio = filtri.DataDecorrenza,
                                DataFine = DomainConstants.DataNulla,
                                DaFatturare = "1",
                                DataRifatturazione = cOrdRow.DataPrevistaConsegna,
                                Fatturata = "0",
                                Nota = $"Adeguamento ISTAT ({filtri.PercIstat}%). Precedente: {cOrdRow.ValUnit}",
                                TestoFattura = filtri.TestoFattura,
                                RifServizio = cOrdRow.Item,
                                RifLinea = cOrdRow.Line,
                                ValUnit = newValUnit,
                                Tbcreated = DateTime.Now,
                                Tbmodified = DateTime.Now,
                                TbcreatedId = _loginId,
                                TbmodifiedId = _loginId
                            };
                            efAllordCliAttivita.Add(rAtt);

                            // Adeguamento righe di attività sospese
                            foreach (var adeg in attDaAdeguare)
                            {
                                adeg.ValUnit = newValUnit;
                                adeg.TestoFattura = "Adeguato a ISTAT";
                                adeg.Tbmodified = DateTime.Now;
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

                // Salvataggio Bulk transazionale
                string msgValid = $"Ordini con righe Contratto valide per ISTAT: {totOrdiniOk}";
                logMessage?.Invoke(msgValid);

                if (totOrdiniOk > 0)
                {
                    updateStatus?.Invoke("Salvataggio modifiche ISTAT in corso...");

                    bool esito = BulkPersistenceHelper.ExecuteInBulkTransaction(_context, () =>
                    {
                        BulkPersistenceHelper.BulkExecuteList(_context, efAllordCliAttivita, "ALLOrdCliAttivita", BulkOperation.InsertOrUpdate, "righe attività", bulkMessage);
                        BulkPersistenceHelper.BulkExecuteList(_context, efAllordCliContratto, "ALLOrdCliContratto", BulkOperation.Update, "righe contratto", bulkMessage);
                    }, logMessage, errori);

                    if (esito)
                    {
                        string esitoMsg = $"Processati con successo: {totOrdiniOk} ordini per adeguamento ISTAT.";
                        bulkMessage.AppendLine(esitoMsg);
                        logMessage?.Invoke(esitoMsg);
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                errori.AppendLine($"[Procedura ISTAT] Messaggio: {ex.Message}");
                errori.AppendLine($"[Procedura ISTAT] Stack: {ex.StackTrace}");
                logMessage?.Invoke($"ERRORE ISTAT: {ex.Message}");
                return false;
            }

            return errori.Length == 0;
        }
    }
}