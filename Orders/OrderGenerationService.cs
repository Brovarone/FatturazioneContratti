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
        private readonly FattureContext _context;
        private readonly int _loginId;

        public OrderGenerationService(FattureContext context, int loginId)
        {
            _context = context;
            _loginId = loginId;
        }

        public bool GeneraRigheOrdine(
            FiltriOrdini filtri,
            Action<string> logMessage = null,
            Action<string> updateStatus = null,
            Action advanceProgress = null)
        {
            var bulkMessage = new StringBuilder();
            var errori = new StringBuilder();
            var avvisi = new StringBuilder();
            var debugging = new StringBuilder();

            int totOrdini = 0;
            int totOrdiniConNuoveRighe = 0;
            int totOrdiniConRigheAggiornate = 0;

            try
            {
                updateStatus?.Invoke("Precaricamento Tabelle");
                PrecaricaTabelleOrdini();

                updateStatus?.Invoke("Estrazione ordini");
                IQueryable<MaSaleOrd> query = _context.MaSaleOrd.AsQueryable();

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
                if (filtri.SingolaPriorita && int.TryParse(filtri.Priorita, out int pVal))
                    query = query.Where(o => o.Priority == pVal);

                var allOrders = query.ToList();
                if (!allOrders.Any())
                {
                    logMessage?.Invoke("Nessun ordine estratto con i filtri specificati.");
                    return true;
                }

                totOrdini = allOrders.Count;
                string msgTot = $"Ordini Estratti : {totOrdini}";
                Debug.WriteLine(msgTot);
                logMessage?.Invoke(msgTot);
                updateStatus?.Invoke(msgTot);

                // Default vendite e IVA
                var defVendite = _context.MaUserDefaultSales.FirstOrDefault();
                var defIva = _context.MaTaxCodesDefaults.FirstOrDefault();
                var codiciIva = _context.MaTaxCodes.ToList();

                string sDefContropartita = defVendite?.ServicesSalesAccount ?? string.Empty;
                string sDefCodIva = defIva?.TaxCode ?? string.Empty;
                var taxDefault = codiciIva.FirstOrDefault(t => t.TaxCode == sDefCodIva);
                double dDefPercIva = Math.Round((double)(taxDefault?.Perc ?? 0), DomainConstants.DecimaliPercentuale);

                // Collezioni per il bulk insert finale
                var efMaSaleOrd = new List<MaSaleOrd>();
                var efMaSaleOrdDetails = new List<MaSaleOrdDetails>();
                var efMaSaleOrdSummary = new List<MaSaleOrdSummary>();
                var efAllordCliAcc = new List<AllordCliAcc>();
                var efAllordCliAttivita = new List<AllordCliAttivita>();
                var efAllordCliContratto = new List<AllordCliContratto>();

                // Ciclo Ordini
                foreach (var o in allOrders)
                {
                    advanceProgress?.Invoke();
                    debugging.AppendLine($"Ordine: {o.InternalOrdNo}");

                    int iNewRowsCount = 0;
                    bool isNewRows = false;
                    bool isUpdateRows = false;
                    int iNrRigheNota = 0;
                    int iNrRigheAValore = 0;
                    bool bScrittoDescrizioni = false;

                    var cOrd = new CurOrd(o);
                    int curLastPosition = o.MaSaleOrdDetails.Any() ? o.MaSaleOrdDetails.Max(m => m.Position) : 0;

                    if (o.ALLordCliContratto != null && o.ALLordCliContratto.Any())
                    {
                        foreach (var c in o.ALLordCliContratto)
                        {
                            if (VerificaEsclusioneContratto(c, filtri, debugging, out bool isDaEscludere))
                                continue;

                            var taxRow = codiciIva.FirstOrDefault(t => t.TaxCode == c.CodiceIva);
                            var cOrdRow = new CurOrdRow(c)
                            {
                                CanoneFuoriRangeDate = isDaEscludere,
                                Contropartita = string.IsNullOrWhiteSpace(c.MaItems?.SaleOffset) ? sDefContropartita : c.MaItems.SaleOffset,
                                CodIva = string.IsNullOrWhiteSpace(c.CodiceIva) ? sDefCodIva : c.CodiceIva,
                                PercIva = Math.Round((double)(taxRow?.Perc ?? dDefPercIva), DomainConstants.DecimaliPercentuale),
                                Parent = cOrd
                            };

                            var attDaRifatturare = new List<AllordCliAttivita>();
                            var attIstat = new List<AllordCliAttivita>();

                            // Gestione Attività
                            ElaboraAttivitaContratto(c, cOrdRow, filtri, attDaRifatturare, attIstat, efAllordCliAttivita, errori, debugging);

                            // Controllo Scadenze Fisse e Cessazioni
                            ElaboraScadenzeECessazioni(o, cOrd, cOrdRow, efAllordCliAcc, debugging);

                            // Generazione Righe se idoneo
                            if (!cOrdRow.PrecendementeCessato
                                && ((cOrdRow.IsOk && !cOrdRow.CanoneFuoriRangeDate) || cOrdRow.DaRifatturare)
                                && (cOrdRow.QtaCorrente > 0 || cOrdRow.QtaDaRifatturare > 0))
                            {
                                if (o.MaSaleOrdDetails.Count == 1 && CheckRigaBianca(o.MaSaleOrdDetails.First()))
                                {
                                    cOrd.LastLine = 0;
                                    curLastPosition = 0;
                                }

                                // Allineamento consegna per attività di rifatturazione
                                foreach (var aDaRif in attDaRifatturare)
                                {
                                    if (aDaRif.DataRifatturazione < cOrdRow.DataPrevistaConsegna)
                                    {
                                        if (!cOrdRow.CanoneFuoriRangeDate)
                                            avvisi.AppendLine($"Ordine {cOrd.OrdNo} Servizio {cOrdRow.Item} con date prevista fatturazione non congruenti!");

                                        cOrdRow.DataPrevistaConsegna = aDaRif.DataRifatturazione.Value;
                                        cOrdRow.DataConfermaConsegna = aDaRif.DataRifatturazione.Value;
                                    }
                                }

                                isNewRows = true;

                                // Descrizione Testata Ordine
                                if (!bScrittoDescrizioni && o.ALLordCliDescrizioni != null)
                                {
                                    bScrittoDescrizioni = true;
                                    foreach (var d in o.ALLordCliDescrizioni)
                                    {
                                        iNewRowsCount++;
                                        iNrRigheNota++;
                                        efMaSaleOrdDetails.Add(CreaRigaDescrittiva(iNewRowsCount, cOrdRow, d.Descrizione));
                                    }
                                }

                                // Righe da Rifatturare
                                foreach (var aDaRif in attDaRifatturare)
                                {
                                    iNewRowsCount++;
                                    iNrRigheAValore++;
                                    efMaSaleOrdDetails.Add(CreaRigaRifatturazione(cOrd, cOrdRow, aDaRif, iNewRowsCount, (short)(curLastPosition + iNrRigheAValore), errori, debugging));
                                }

                                // Riga Canone / Servizio Effettiva
                                if (!cOrdRow.CanoneFuoriRangeDate && cOrdRow.QtaCorrente > 0)
                                {
                                    // Riga Riferimento Contratto
                                    iNewRowsCount++;
                                    iNrRigheNota++;
                                    string descriRif = (string.IsNullOrWhiteSpace(cOrdRow.ContrattoFox) ? string.Empty : $"Codice : {cOrdRow.ContrattoFox}") + $" Rif : {cOrdRow.Parent.OrdNo}";
                                    efMaSaleOrdDetails.Add(CreaRigaRiferimento(cOrd, cOrdRow, descriRif.TrimStart(), iNewRowsCount, (short)(curLastPosition + iNrRigheAValore)));

                                    // Righe Descrittive Contratto
                                    if (c.AllordCliContrattoDescFatt != null)
                                    {
                                        foreach (var d in c.AllordCliContrattoDescFatt)
                                        {
                                            iNewRowsCount++;
                                            iNrRigheNota++;
                                            efMaSaleOrdDetails.Add(CreaRigaDescrittiva(iNewRowsCount, cOrdRow, d.Descrizione));
                                        }
                                    }

                                    // Dettaglio Valore
                                    iNewRowsCount++;
                                    iNrRigheAValore++;
                                    efMaSaleOrdDetails.Add(CreaRigaServizioValore(c, cOrd, cOrdRow, iNewRowsCount, (short)(curLastPosition + iNrRigheAValore)));
                                }

                                // Righe ISTAT
                                foreach (var aIst in attIstat)
                                {
                                    iNewRowsCount++;
                                    iNrRigheNota++;
                                    var rd = CreaRigaDescrittiva(iNewRowsCount, cOrdRow, aIst.TestoFattura);
                                    efMaSaleOrdDetails.Add(rd);
                                    efAllordCliAttivita.Add(aIst);
                                }
                            }

                            // Aggiornamento date prossima fatturazione
                            if (!cOrdRow.CanoneFuoriRangeDate
                                && (cOrdRow.IsOk || cOrdRow.DaRifatturare || cOrdRow.SospesoDaAttivita)
                                && !cOrdRow.PrecendementeCessato)
                            {
                                if (cOrdRow.DataProssimaFattura < filtri.DataFattA)
                                    avvisi.AppendLine($"Ordine {cOrd.OrdNo} Servizio {cOrdRow.Item} con data prossima fatturazione antecedente alla data di fine estrazione!");

                                c.DataProssimaFatt = cOrdRow.IsCampagna ? cOrdRow.DataProssimaFatturaCampagna : cOrdRow.DataProssimaFattura;
                                c.DataFineElaborazione = DateTime.Now;
                                c.Tbmodified = DateTime.Now;
                                c.TbmodifiedId = _loginId;
                                efAllordCliContratto.Add(c);
                                isUpdateRows = true;
                            }
                        }
                    }

                    if (isNewRows)
                    {
                        o.Invoiced = "0";
                        o.Delivered = "0";
                        o.Picked = "0";
                        o.LastSubId = cOrd.LastSubId + iNewRowsCount;
                        o.Tbmodified = DateTime.Now;
                        o.TbmodifiedId = _loginId;
                        efMaSaleOrd.Add(o);
                        totOrdiniConNuoveRighe++;
                    }

                    if (isUpdateRows) totOrdiniConRigheAggiornate++;
                }

                // Salvataggio Bulk
                EseguiSalvataggioTransazionale(
                    totOrdiniConNuoveRighe,
                    totOrdiniConRigheAggiornate,
                    efMaSaleOrd,
                    efMaSaleOrdDetails,
                    efMaSaleOrdSummary,
                    efAllordCliAcc,
                    efAllordCliAttivita,
                    efAllordCliContratto,
                    bulkMessage,
                    errori,
                    logMessage);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                errori.AppendLine($"[Procedura] Messaggio: {ex.Message}");
                errori.AppendLine($"[Procedura] Stack: {ex.StackTrace}");
                logMessage?.Invoke($"ERRORE: {ex.Message}");
                return false;
            }

            return errori.Length == 0;
        }

        #region Metodi di supporto estrazione e regole

        private void PrecaricaTabelleOrdini()
        {
            _context.Allattivita.Load();
            _context.MaItems.Load();
            _context.AlltipoRigaServizio.Load();
            _context.Alldescrizioni.Load();
            _context.MaSaleOrd.Load();
            _context.AllordCliAcc.Load();
            _context.AllordCliFattEle.Load();
            _context.AllordCliDescrizioni.Load();
            _context.MaSaleOrdDetails.Load();
            _context.AllordCliContratto.Load();
            _context.AllordCliContrattoDescFatt.Load();
            _context.AllordCliAttivita.Load();
            _context.AllordCliContrattoDistinta.Load();
            _context.AllordCliContrattoDistintaServAgg.Load();
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
                            att.Tbmodified = DateTime.Now;
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
                        att.Tbmodified = DateTime.Now;
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
                        o.ALLOrdCliAcc.Tbmodified = DateTime.Now;
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
                            o.ALLOrdCliAcc.Tbmodified = DateTime.Now;
                            o.ALLOrdCliAcc.TbmodifiedId = _loginId;
                            efAllordCliAcc.Add(o.ALLOrdCliAcc);
                        }
                    }
                }
            }
        }

        #endregion

        #region Costruzione record MaSaleOrdDetails

        private MaSaleOrdDetails CreaRigaDescrittiva(int iNewRowsCount, CurOrdRow cOrdRow, string descrizione)
        {
            return new MaSaleOrdDetails
            {
                Line = (short)(cOrdRow.Parent.LastLine + iNewRowsCount),
                Position = 0,
                SubId = cOrdRow.Parent.LastSubId + iNewRowsCount,
                SaleOrdId = cOrdRow.Parent.SaleOrdId,
                LineType = (int)LineType.Nota,
                Description = descrizione,
                InEi = "1",
                ExpectedDeliveryDate = cOrdRow.DataPrevistaConsegna,
                ConfirmedDeliveryDate = cOrdRow.DataConfermaConsegna,
                InternalOrdNo = cOrdRow.Parent.OrdNo,
                Customer = cOrdRow.Parent.Cliente,
                OrderDate = cOrdRow.Parent.OrdDate,
                Tbcreated = DateTime.Now,
                Tbmodified = DateTime.Now,
                TbcreatedId = _loginId,
                TbmodifiedId = _loginId,
                Item = string.Empty,
                UoM = string.Empty,
                Qty = 0,
                UnitValue = 0,
                TaxableAmount = 0,
                TotalAmount = 0,
                PacksUoM = string.Empty,
                TaxCode = string.Empty,
                Job = string.Empty,
                CostCenter = string.Empty,
                Offset = string.Empty,
                Notes = string.Empty,
                NetPrice = 0,
                ContractCode = string.Empty,
                ProjectCode = string.Empty,
                AllNrCanoni = 0,
                AllCanoniDataI = DomainConstants.DataNulla,
                AllCanoniDataF = DomainConstants.DataNulla,
                Invoiced = "0"
            };
        }

        private MaSaleOrdDetails CreaRigaRiferimento(CurOrd cOrd, CurOrdRow cOrdRow, string descriRif, int iNewRowsCount, short pos)
        {
            return new MaSaleOrdDetails
            {
                Line = (short)(cOrd.LastLine + iNewRowsCount),
                Position = pos,
                SubId = cOrd.LastSubId + iNewRowsCount,
                SaleOrdId = cOrd.SaleOrdId,
                LineType = (int)LineType.Riferimento,
                Description = descriRif,
                InEi = "1",
                ExpectedDeliveryDate = cOrdRow.DataPrevistaConsegna,
                ConfirmedDeliveryDate = cOrdRow.DataConfermaConsegna,
                InternalOrdNo = cOrdRow.Parent.OrdNo,
                Customer = cOrdRow.Parent.Cliente,
                OrderDate = cOrdRow.Parent.OrdDate,
                Tbcreated = DateTime.Now,
                Tbmodified = DateTime.Now,
                TbcreatedId = _loginId,
                TbmodifiedId = _loginId,
                Item = string.Empty,
                UoM = string.Empty,
                Qty = 0,
                UnitValue = 0,
                TaxableAmount = 0,
                TotalAmount = 0,
                PacksUoM = string.Empty,
                TaxCode = string.Empty,
                Job = string.Empty,
                CostCenter = string.Empty,
                Offset = string.Empty,
                Notes = string.Empty,
                NetPrice = 0,
                ContractCode = string.Empty,
                ProjectCode = string.Empty,
                AllNrCanoni = 0,
                AllCanoniDataI = DomainConstants.DataNulla,
                AllCanoniDataF = DomainConstants.DataNulla,
                Invoiced = "0"
            };
        }

        private MaSaleOrdDetails CreaRigaRifatturazione(
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

            return new MaSaleOrdDetails
            {
                Line = (short)(cOrd.LastLine + iNewRowsCount),
                Position = pos,
                SubId = cOrd.LastSubId + iNewRowsCount,
                SaleOrdId = cOrd.SaleOrdId,
                LineType = (int)LineType.Servizio,
                Item = cOrdRow.Item,
                Description = string.IsNullOrWhiteSpace(cOrdRow.Description) ? periodoRif : $"{cOrdRow.Description} {periodoRif}",
                UoM = cOrdRow.UoM,
                PacksUoM = cOrdRow.UoM,
                Qty = qty,
                UnitValue = valUnit,
                NetPrice = valUnit,
                TaxableAmount = taxable,
                TaxCode = cOrdRow.CodIva,
                TotalAmount = total,
                ExpectedDeliveryDate = cOrdRow.DataPrevistaConsegna,
                ConfirmedDeliveryDate = cOrdRow.DataConfermaConsegna,
                AllNrCanoni = qty,
                AllCanoniDataI = dataInizio,
                AllCanoniDataF = dataFine,
                Invoiced = "0",
                Notes = SubstringSafe(aDaRif.Nota, 32),
                Job = cOrd.Commessa,
                CostCenter = cOrd.CdC,
                ContractCode = cOrd.CIG,
                ProjectCode = cOrd.CUP,
                Offset = cOrdRow.Contropartita,
                InternalOrdNo = cOrd.OrdNo,
                Customer = cOrd.Cliente,
                OrderDate = cOrd.OrdDate,
                Tbcreated = DateTime.Now,
                Tbmodified = DateTime.Now,
                TbcreatedId = _loginId,
                TbmodifiedId = _loginId
            };
        }

        private MaSaleOrdDetails CreaRigaServizioValore(
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

            return new MaSaleOrdDetails
            {
                Line = (short)(cOrd.LastLine + iNewRowsCount),
                Position = pos,
                SubId = cOrd.LastSubId + iNewRowsCount,
                SaleOrdId = cOrd.SaleOrdId,
                LineType = (int)LineType.Servizio,
                Item = cOrdRow.Item,
                Description = descrizioneFinale,
                UoM = cOrdRow.UoM,
                PacksUoM = cOrdRow.UoM,
                Qty = qty,
                UnitValue = unitVal,
                NetPrice = unitVal,
                TaxableAmount = taxable,
                TaxCode = cOrdRow.CodIva,
                TotalAmount = total,
                ExpectedDeliveryDate = cOrdRow.DataPrevistaConsegna,
                ConfirmedDeliveryDate = cOrdRow.DataConfermaConsegna,
                AllNrCanoni = qty,
                AllCanoniDataI = cOrdRow.PeriodoDataIn,
                AllCanoniDataF = cOrdRow.PeriodoDataFin,
                Invoiced = "0",
                Notes = SubstringSafe(c.Nota, 32),
                Job = cOrd.Commessa,
                CostCenter = cOrd.CdC,
                ContractCode = cOrd.CIG,
                ProjectCode = cOrd.CUP,
                Offset = cOrdRow.Contropartita,
                InternalOrdNo = cOrd.OrdNo,
                Customer = cOrd.Cliente,
                OrderDate = cOrd.OrdDate,
                Tbcreated = DateTime.Now,
                Tbmodified = DateTime.Now,
                TbcreatedId = _loginId,
                TbmodifiedId = _loginId
            };
        }

        private static bool CheckRigaBianca(MaSaleOrdDetails d)
        {
            if (string.IsNullOrWhiteSpace(d.Item) && d.LineType == (int)LineType.Merce && d.Line == 1 && d.Position == 1)
                return true;

            if ((d.LineType == (int)LineType.Merce || d.LineType == (int)LineType.Servizio) && d.UnitValue == 0 && d.Line == 1 && d.Position == 1)
                return true;

            return false;
        }

        private static string SubstringSafe(string val, int maxLen)
        {
            if (string.IsNullOrEmpty(val)) return string.Empty;
            string t = val.Trim();
            return t.Length <= maxLen ? t : t.Substring(0, maxLen);
        }

        #endregion

        #region Bulk Execution

        private void EseguiSalvataggioTransazionale(
            int totNuove,
            int totAggiornate,
            List<MaSaleOrd> ordini,
            List<MaSaleOrdDetails> details,
            List<MaSaleOrdSummary> summaries,
            List<AllordCliAcc> accessorie,
            List<AllordCliAttivita> attivita,
            List<AllordCliContratto> contratti,
            StringBuilder bulkMessage,
            StringBuilder errori,
            Action<string> logMessage)
        {
            if (totNuove == 0 && totAggiornate == 0) return;

            bool esito = BulkPersistenceHelper.ExecuteInBulkTransaction(_context, () =>
            {
                BulkPersistenceHelper.BulkExecuteList(_context, ordini, "MaSaleOrd", BulkOperation.Update, "teste ordini", bulkMessage);
                BulkPersistenceHelper.BulkExecuteList(_context, details, "MaSaleOrdDetails", BulkOperation.InsertOrUpdate, "righe ordini", bulkMessage);
                BulkPersistenceHelper.BulkExecuteList(_context, summaries, "MaSaleOrdSummary", BulkOperation.InsertOrUpdate, "totali ordini", bulkMessage);
                BulkPersistenceHelper.BulkExecuteList(_context, accessorie, "AllordCliAcc", BulkOperation.InsertOrUpdate, "dati accessori ordini", bulkMessage);
                BulkPersistenceHelper.BulkExecuteList(_context, attivita, "AllordCliAttivita", BulkOperation.Update, "righe attività", bulkMessage);
                BulkPersistenceHelper.BulkExecuteList(_context, contratti, "AllordCliContratto", BulkOperation.Update, "righe contratto", bulkMessage);
            }, logMessage, errori);

            if (esito)
            {
                string riassunto = $"[RIASSUNTO] Ordini con Righe: {totNuove} | Ordini con Adeguamenti: {totAggiornate}";
                bulkMessage.AppendLine(riassunto);
                logMessage?.Invoke(riassunto);
            }
        }

        #endregion
    }
}