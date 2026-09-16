using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using EFCore.BulkExtensions;
using EFMago.Models;
using FatturazioneContratti.Common;
using FatturazioneContratti.Orders;
using Microsoft.EntityFrameworkCore;

namespace FatturazioneContratti.Invoicing
{
    public class ConsuntivoService
    {
        private readonly FattureContext _ordiniCntx;
        private readonly int _loginId;
        private const string RegIvaPosticipate = "V2";
        private const string TecnoAll1 = "NOLEGGIO SPA";

        public ConsuntivoService(FattureContext ordiniCntx, int loginId)
        {
            _ordiniCntx = ordiniCntx;
            _loginId = loginId;
        }

        #region Dizionari e Mapping

        private static readonly Dictionary<string, int> CausaliPagamentoFatturaElettronica = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            { "", 32440347 },
            { "A", 32440320 },
            { "B", 32440321 },
            { "C", 32440322 },
            { "D", 32440323 },
            { "E", 32440324 },
            { "G", 32440325 },
            { "H", 32440326 },
            { "I", 32440327 },
            { "L", 32440328 },
            { "M", 32440329 },
            { "N", 32440330 },
            { "O", 32440331 },
            { "P", 32440332 },
            { "Q", 32440333 },
            { "R", 32440334 },
            { "S", 32440335 },
            { "T", 32440336 },
            { "U", 32440337 },
            { "V", 32440338 },
            { "W", 32440339 },
            { "X", 32440340 },
            { "Y", 32440341 },
            { "Z", 32440342 },
            { "L1", 32440343 },
            { "M1", 32440344 },
            { "O1", 32440345 },
            { "V1", 32440346 },
            { "M2", 32440348 },
            { "ZO", 32440349 }
        };

        private static readonly Dictionary<string, string> MappingsEventoIntegra = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "ISP", "ISPEZIONI" },
            { "ISPD", "Ispezioni" },
            { "INT", "INTERVENTI" },
            { "INTD", "Interventi" },
            { "1", "APECHIU" },
            { "1d", "Apertura Chiusura" },
            { "2", "ASSISTENZA" },
            { "2d", "Assistenza" },
            { "3", "PIANTONI" },
            { "3d", "Piantoni" },
            { "4", "VIDEOISPEZIONI" },
            { "4d", "Video Ispezioni" },
            { "5", "SUPPLEMENTAR" }
        };

        public static string TrascodificaServizioAggiuntivo(string tipoEvento) =>
            MappingsEventoIntegra.TryGetValue(tipoEvento ?? string.Empty, out string val) ? val : string.Empty;

        public static string TranscodificaEventoIntegra(string servizioMago)
        {
            var match = MappingsEventoIntegra.FirstOrDefault(kvp => string.Equals(kvp.Value, servizioMago, StringComparison.OrdinalIgnoreCase));
            return match.Key ?? string.Empty;
        }

        #endregion

        #region Controlla Flusso Integra

        public bool ControllaFlussoIntegra(
            FiltriOrdiniConsuntivo filtri,
            Action<string> logMessage = null,
            Action<string> updateStatus = null,
            Action advanceProgress = null)
        {
            var errori = new StringBuilder();
            var fatalContrattoErrorList = new List<string>();
            var fatalEventoErrorList = new List<string>();
            var contrattoErrorList = new List<string>();
            var eventoErrorList = new List<string>();

            try
            {
                updateStatus?.Invoke("Estrazione distinte e interventi Integra...");

                var allDistinte = _ordiniCntx.AllordCliContrattoDistinta
                    .Include(sa => sa.AllordCliContrattoDistintaServAgg)
                    .Where(wsa => wsa.Servizio != TecnoAll1)
                    .Include(o => o.AllordCliContratto)
                        .ThenInclude(ord => ord.SaleOrd)
                            .ThenInclude(info => info.ALLOrdCliAcc)
                    .Select(x => new MiniDistinta
                    {
                        CodIntegra = x.CodIntegra,
                        DataCessazione = x.AllordCliContratto.SaleOrd.ALLOrdCliAcc.DataCessazione ?? DomainConstants.DataNulla,
                        DataScadenzaFissa = x.AllordCliContratto.SaleOrd.ALLOrdCliAcc.DataScadenzaFissa ?? DomainConstants.DataNulla,
                        AllordCliContrattoDistintaServAgg = x.AllordCliContrattoDistintaServAgg
                    })
                    .ToList();

                IQueryable<IntegraInterventi> intQuery = _ordiniCntx.IntegraInterventi.AsQueryable();

                if (filtri.AskFilter)
                {
                    if (filtri.Riassegna) intQuery = intQuery.Where(o => o.MAssociato == "0");
                    intQuery = intQuery.Where(o => o.InizioAllarme.HasValue 
                        && o.InizioAllarme.Value.Date >= filtri.FromLogDate 
                        && o.InizioAllarme.Value.Date <= filtri.ToLogDate);

                    if (filtri.SingolaFiliale) intQuery = intQuery.Where(o => o.Filiale == filtri.Filiale);
                }

                var allInterventi = intQuery.ToList();
                if (!allInterventi.Any())
                {
                    logMessage?.Invoke("Nessun intervento Integra da controllare.");
                    return true;
                }

                logMessage?.Invoke($"Interventi estratti: {allInterventi.Count}");

                foreach (var i in allInterventi)
                {
                    advanceProgress?.Invoke();

                    var distinteFiltrate = allDistinte.Where(x => string.Equals(x.CodIntegra, i.Contratto, StringComparison.OrdinalIgnoreCase)).ToList();

                    if (distinteFiltrate.Count == 1)
                    {
                        var distinta = distinteFiltrate.Single();
                        string eventoMago = TrascodificaServizioAggiuntivo(i.TipoEvento);
                        var allEventi = distinta.AllordCliContrattoDistintaServAgg
                            .Where(y => string.Equals(y.Servizio, eventoMago, StringComparison.OrdinalIgnoreCase))
                            .ToList();

                        if (allEventi.Count == 1)
                        {
                            var row = allEventi.Single();
                            i.MAssociato = "1";
                            i.MIdOrdCli = row.IdOrdCli;
                            i.MLineaContratto = row.RifRifLinea;
                            i.MLineaDistinta = row.RifLinea;
                        }
                        else if (allEventi.Count > 1)
                        {
                            string err = $"Contratto {i.Contratto} Evento: {i.TipoEvento}";
                            if (!fatalEventoErrorList.Contains(err)) fatalEventoErrorList.Add(err);
                        }
                        else
                        {
                            string err = $"Contratto {i.Contratto} Evento: {i.TipoEvento}";
                            if (!eventoErrorList.Contains(err)) eventoErrorList.Add(err);
                        }
                    }
                    else if (distinteFiltrate.Count > 1)
                    {
                        if (!fatalContrattoErrorList.Contains(i.Contratto)) fatalContrattoErrorList.Add(i.Contratto);
                    }
                    else
                    {
                        if (!contrattoErrorList.Contains(i.Contratto)) contrattoErrorList.Add(i.Contratto);
                    }
                }

                // Salvataggio aggiornamenti interventi
                updateStatus?.Invoke("Salvataggio aggiornamenti interventi...");
                _ordiniCntx.SaveChanges();

                if (fatalContrattoErrorList.Any() || fatalEventoErrorList.Any() || contrattoErrorList.Any() || eventoErrorList.Any())
                {
                    logMessage?.Invoke("Riscontrate anomalie o incongruenze nel controllo flusso.");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                logMessage?.Invoke($"Errore durante il controllo del flusso Integra: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region Genera Fatture Consuntivo

        public bool GeneraFattureConsuntivo(
            FiltriOrdiniConsuntivo filtri,
            Action<string> logMessage = null,
            Action<string> updateStatus = null,
            Action advanceProgress = null)
        {
            var bulkMessage = new StringBuilder();
            var errori = new StringBuilder();
            var debugging = new StringBuilder();

            try
            {
                updateStatus?.Invoke("Estrazione ordini corrispondenti per consuntivazione...");

                // Precaricamento tabelle core
                _ordiniCntx.IntegraInterventi.Load();
                _ordiniCntx.MaCustSupp.Where(w => w.CustSuppType == 3211264).Load();
                _ordiniCntx.MaCustSuppBranches.Load();
                _ordiniCntx.MaCustSuppCustomerOptions.Load();

                IQueryable<IntegraInterventi> intFiltrati = _ordiniCntx.IntegraInterventi
                    .Where(w => w.AllordCliContrattoDistinta != null && w.AllordCliContrattoDistinta.Servizio != TecnoAll1);

                if (filtri.AskFilter && filtri.SingolaFiliale)
                {
                    intFiltrati = intFiltrati.Where(o => o.Filiale == filtri.Filiale);
                }

                var interventiList = intFiltrati.ToList();
                int[] aOrdini = intFiltrati.Select(s => s.MIdOrdCli ?? 0).Where(id => id > 0).Distinct().ToArray();

                var ordiniList = _ordiniCntx.MaSaleOrd
                    .Include(o => o.ALLordCliContratto)
                        .ThenInclude(c => c.AllordCliContrattoDistinta)
                            .ThenInclude(d => d.IntegraInterventi)
                    .Include(o => o.ALLordCliContratto)
                        .ThenInclude(c => c.AllordCliContrattoDistinta)
                            .ThenInclude(d => d.AllordCliContrattoDistintaServAgg)
                                .ThenInclude(s => s.MaItems)
                    .Include(o => o.ALLordCliContratto)
                        .ThenInclude(c => c.AllordCliContrattoDescFatt)
                    .Include(o => o.ALLordCliDescrizioni)
                    .Include(o => o.ALLOrdCliAcc)
                    .Include(o => o.ALLOrdCliFattEle)
                    .Where(o => aOrdini.Contains(o.SaleOrdId))
                    .OrderBy(o => o.InvoicingCustomer)
                    .ThenBy(o => o.ContractCode)
                    .ThenBy(o => o.Payment)
                    .ThenBy(o => o.Carrier1)
                    .ToList();

                var paramSaleOrd = _ordiniCntx.MaSalesOrdParameters.FirstOrDefault() ?? new MaSalesOrdParameters();

                // Collection per il Bulk Insert delle fatture
                var efMaSaleDoc = new List<MaSaleDoc>();
                var efMaSaleDocDetails = new List<MaSaleDocDetail>();
                var efMaSaleDocSummary = new List<MaSaleDocSummary>();
                var efMaCrossReferences = new List<MaCrossReferences>();
                var efMaCrossReferencesNotes = new List<MaCrossReferencesNotes>();
                var efMaEiItdocAdditionalData = new List<MaEiItdocAdditionalData>();

                var defVendite = _ordiniCntx.MaUserDefaultSales.FirstOrDefault() ?? new MaUserDefaultSales();
                var defIva = _ordiniCntx.MaTaxCodesDefaults.FirstOrDefault();
                var codiciIva = _ordiniCntx.MaTaxCodes.ToList();
                string sDefContropartita = defVendite.ServicesSalesAccount ?? string.Empty;
                string sDefCodIva = defIva?.TaxCode ?? string.Empty;
                double dDefPercIva = Math.Round((double)(codiciIva.FirstOrDefault(tax => tax.TaxCode == sDefCodIva)?.Perc ?? 0), DomainConstants.DecimaliPercentuale);

                var idNumbers = _ordiniCntx.MaIdnumbers.First(k => k.CodeType == (int)IdType.DocVend);
                int saleDocId = idNumbers.LastId ?? 0;

                var clienti = _ordiniCntx.MaCustSupp
                    .Include(c => c.MaCustSuppCustomerOptions)
                    .Where(w => w.CustSuppType == 3211264)
                    .ToList();

                var cFattura = new CurFatt();
                var saleDoc = new MaSaleDoc();
                bool scriviTesta = true;

                foreach (var o in ordiniList)
                {
                    advanceProgress?.Invoke();
                    debugging.AppendLine($"Ordine: {o.InternalOrdNo} Cliente: {o.Customer}");

                    // Criteri di Rottura e accorpamento fattura
                    if (Break(o, saleDoc, cFattura.Cliente, paramSaleOrd))
                    {
                        scriviTesta = true;
                        cFattura = new CurFatt(o);
                        saleDocId++;
                        cFattura.SaleDocId = saleDocId;
                        cFattura.Cliente = clienti.FirstOrDefault(w => w.CustSupp == o.InvoicingCustomer);
                        cFattura.DataFattura = filtri.DataFattura;
                        if ((cFattura.PercIva ?? 0) == 0) cFattura.PercIva = dDefPercIva;
                    }

                    int iNewRowsCount = 0;
                    bool isNewRows = false;
                    bool bWritedHeadDescription = false;

                    if (o.ALLordCliContratto != null)
                    {
                        foreach (var c in o.ALLordCliContratto)
                        {
                            if (c.AllordCliContrattoDistinta == null) continue;

                            foreach (var d in c.AllordCliContrattoDistinta)
                            {
                                if (d.IntegraInterventi == null || d.AllordCliContrattoDistintaServAgg == null) continue;

                                foreach (var s in d.AllordCliContrattoDistintaServAgg)
                                {
                                    string evento = TranscodificaEventoIntegra(s.Servizio);
                                    var periodFranchigia = (Periodo)(s.Periodicita ?? 0);
                                    var interventiEvento = d.IntegraInterventi.Where(w => string.Equals(w.TipoEvento, evento, StringComparison.OrdinalIgnoreCase)).ToList();

                                    if (interventiEvento.Any())
                                    {
                                        s.AnnoIntervento = filtri.ToLogDate.Year;
                                        var toDataMese = new DateTime(filtri.ToLogDate.Year, filtri.ToLogDate.Month, DateTime.DaysInMonth(filtri.ToLogDate.Year, filtri.ToLogDate.Month), 23, 59, 59);
                                        var fromDataMese = new DateTime(toDataMese.Year, toDataMese.Month, 1);

                                        s.NrInterventiMese = interventiEvento
                                            .Where(wp => wp.InizioAllarme >= fromDataMese && wp.InizioAllarme <= toDataMese)
                                            .Sum(q => q.Qta ?? 0);

                                        var dtaPeriodoFranchigiaFrom = GetDataPeriodoFranchigia(toDataMese, periodFranchigia, true);
                                        var dtaPeriodoFranchigiaTo = GetDataPeriodoFranchigia(toDataMese, periodFranchigia, false);

                                        s.NrInterventiInFranchigia = interventiEvento
                                            .Where(wp => wp.InizioAllarme >= dtaPeriodoFranchigiaFrom && wp.InizioAllarme <= dtaPeriodoFranchigiaTo)
                                            .Sum(q => q.Qta ?? 0);

                                        if (periodFranchigia == Periodo.Nessuno)
                                        {
                                            s.NrInterventiInFranchigia = 0;
                                            s.NrInterventiOltreFranchigia = s.NrInterventiMese;
                                        }
                                        else
                                        {
                                            double extra = (s.NrInterventiInFranchigia ?? 0) - (s.Franchigia ?? 0);
                                            s.NrInterventiOltreFranchigia = Math.Max(0, extra);
                                            s.NrInterventiInFranchigia = extra <= 0 ? s.NrInterventiInFranchigia : s.Franchigia;
                                        }
                                    }

                                    if ((s.NrInterventiOltreFranchigia ?? 0) > 0 && (o.ALLOrdCliAcc?.DataCessazione ?? DomainConstants.DataNulla) != DomainConstants.DataNulla)
                                    {
                                        isNewRows = true;

                                        var cFattRow = new CurFattRow
                                        {
                                            Contropartita = string.IsNullOrWhiteSpace(s.MaItems?.SaleOffset) ? sDefContropartita : s.MaItems.SaleOffset,
                                            CodIva = string.IsNullOrWhiteSpace(c.CodiceIva) ? sDefCodIva : c.CodiceIva,
                                            ContrattoFox = d.CodContratto,
                                            CodiceIntegra = d.CodIntegra,
                                            CdC = d.CdC,
                                            Parent = cFattura
                                        };

                                        var taxRow = codiciIva.FirstOrDefault(tax => tax.TaxCode == cFattRow.CodIva);
                                        cFattRow.PercIva = Math.Round((double)(taxRow?.Perc ?? (cFattura.PercIva ?? 0)), DomainConstants.DecimaliPercentuale);

                                        // STEP 1 e 2: Descrizioni di testata
                                        if (!bWritedHeadDescription)
                                        {
                                            bWritedHeadDescription = true;
                                            iNewRowsCount++;
                                            efMaSaleDocDetails.Add(CreaRigaDescrittivaFattura(iNewRowsCount, cFattura, "INTERVENTI SUPPLEMENTARI"));

                                            if (o.ALLordCliDescrizioni != null)
                                            {
                                                foreach (var dTesta in o.ALLordCliDescrizioni)
                                                {
                                                    iNewRowsCount++;
                                                    efMaSaleDocDetails.Add(CreaRigaDescrittivaFattura(iNewRowsCount, cFattura, dTesta.Descrizione));
                                                }
                                            }
                                        }

                                        // STEP 3: Descrizioni riga contratto
                                        if (c.AllordCliContrattoDescFatt != null)
                                        {
                                            foreach (var dRiga in c.AllordCliContrattoDescFatt)
                                            {
                                                iNewRowsCount++;
                                                efMaSaleDocDetails.Add(CreaRigaDescrittivaFattura(iNewRowsCount, cFattura, dRiga.Descrizione));
                                            }
                                        }

                                        // STEP 4: Riga Dettaglio Servizio a Valore
                                        iNewRowsCount++;
                                        double qty = s.NrInterventiOltreFranchigia ?? 0;
                                        double unitVal = Math.Round(s.ValUnit ?? 0, DomainConstants.DecimaliTasse);
                                        double taxable = Math.Round(qty * unitVal, DomainConstants.DecimaliTasse);
                                        double total = Math.Round(taxable * ((100 + (cFattRow.PercIva ?? 0)) / 100), DomainConstants.DecimaliTasse);

                                        var r = new MaSaleDocDetail
                                        {
                                            Line = (short)((cFattura.LastLine ?? 0) + iNewRowsCount),
                                            SubId = (cFattura.LastSubId ?? 0) + iNewRowsCount,
                                            SaleDocId = cFattura.SaleDocId,
                                            LineType = (int)LineType.Servizio,
                                            Item = s.Servizio,
                                            IncludedInTurnover = "1",
                                            Description = string.IsNullOrWhiteSpace(s.Descrizione) ? "Interventi a consuntivo" : s.Descrizione,
                                            UoM = s.Um,
                                            PacksUoM = s.Um,
                                            Qty = qty,
                                            UnitValue = unitVal,
                                            NetPrice = unitVal,
                                            TaxableAmount = taxable,
                                            TotalAmount = total,
                                            AllNrCanoni = (int)qty,
                                            Invoiced = "0",
                                            Notes = c.Nota != null && c.Nota.Length > 32 ? c.Nota.Substring(0, 32) : c.Nota,
                                            CustSupp = cFattura.CodCliente,
                                            CustSuppType = 3211264,
                                            DocumentDate = cFattura.DataFattura,
                                            ReferenceDocType = (int)DocumentType.Fattura,
                                            CrrefType = (int)CrossReference.Tutti,
                                            SubjectToWithholdingTax = cFattura.Cliente?.MaCustSuppCustomerOptions?.WithholdingTaxManagement ?? "0",
                                            Job = cFattRow.Commessa,
                                            CostCenter = cFattRow.CdC,
                                            Offset = cFattRow.Contropartita,
                                            TaxCode = cFattRow.CodIva,
                                            Tbcreated = DateTime.Now,
                                            Tbmodified = DateTime.Now,
                                            TbcreatedId = _loginId,
                                            TbmodifiedId = _loginId
                                        };
                                        efMaSaleDocDetails.Add(r);
                                    }
                                }
                            }
                        }
                    }

                    if (isNewRows)
                    {
                        cFattura.LastLine = (cFattura.LastLine ?? 0) + iNewRowsCount;
                        cFattura.LastSubId = (cFattura.LastSubId ?? 0) + iNewRowsCount;
                    }

                    if (scriviTesta)
                    {
                        saleDoc = new MaSaleDoc
                        {
                            SaleDocId = cFattura.SaleDocId,
                            DocumentType = (int)DocumentType.Fattura,
                            AccTpl = (cFattura.Cliente?.CustSuppKind == (int)CustSuppKind.Nazionale) ? defVendite.InvoiceAccTpl : defVendite.EuinvoiceAccTpl,
                            CustSupp = cFattura.CodCliente,
                            CustSuppType = 3211264,
                            DocumentDate = cFattura.DataFattura,
                            InstallmStartDate = cFattura.DataFattura,
                            PostingDate = cFattura.DataFattura,
                            TaxJournal = RegIvaPosticipate,
                            CustomerBank = cFattura.Cliente?.CustSuppBank,
                            CompanyCa = cFattura.Cliente?.CompanyCa,
                            CompanyPymtCa = cFattura.Cliente?.CustomerCompanyCa,
                            CompanyBank = cFattura.Cliente?.CompanyBank,
                            InvoiceFollows = "0",
                            EidocumentType = 22151169, // TD01
                            Area = cFattura.Area,
                            AreaManager = cFattura.AreaManager,
                            Salesperson = cFattura.SalesPerson,
                            AccrualPercAtInvoiceDate = 100,
                            Issued = "0",
                            DeliveryTerms = 5963782,
                            CountryOfDestination = "IT",
                            CountryOfPayment = "IT",
                            DepartureDate = cFattura.DataFattura,
                            Presentation = 1376260,
                            ValueDate = cFattura.DataFattura,
                            IntrastatAccrualDate = cFattura.DataFattura,
                            IntrastatBis = "1",
                            ActionOnLifoFifo = 26411008,
                            ModifyOriginalPymtSched = "0",
                            Tbguid = Guid.NewGuid(),
                            Payment = cFattura.Pagamento,
                            Currency = "EUR",
                            SplitPaymentActive = "0",
                            Tbcreated = DateTime.Now,
                            Tbmodified = DateTime.Now,
                            TbcreatedId = _loginId,
                            TbmodifiedId = _loginId,
                            WorkerIdissue = _loginId
                        };
                        efMaSaleDoc.Add(saleDoc);
                        scriviTesta = false;
                    }

                    // Riferimenti incrociati
                    efMaCrossReferences.Add(CreaCrossReference(o.SaleOrdId, saleDocId));
                    efMaCrossReferencesNotes.Add(CreaCrossReferenceNotes(o.SaleOrdId, saleDocId, string.Empty));

                    // Ritenuta d'acconto
                    if (cFattura.Cliente?.MaCustSuppCustomerOptions?.WithholdingTaxManagement == "1")
                    {
                        var opt = cFattura.Cliente.MaCustSuppCustomerOptions;
                        efMaSaleDocSummary.Add(new MaSaleDocSummary
                        {
                            SaleDocId = cFattura.SaleDocId,
                            WithholdingTaxManagement = "1",
                            WithholdingTax = 0,
                            WithholdingTaxPerc = opt.WithholdingTaxPerc,
                            WithholdingTaxBasePerc = opt.WithholdingTaxBasePerc,
                            Tbcreated = DateTime.Now,
                            Tbmodified = DateTime.Now,
                            TbcreatedId = _loginId,
                            TbmodifiedId = _loginId
                        });

                        int causaleId = CausaliPagamentoFatturaElettronica.TryGetValue(cFattura.LetteraCausalePagamento ?? string.Empty, out int cId) 
                            ? cId 
                            : 32440347;

                        efMaEiItdocAdditionalData.Add(new MaEiItdocAdditionalData
                        {
                            DocId = cFattura.SaleDocId,
                            DocSubId = 0,
                            Line = 0,
                            SubLine = 0,
                            FieldName = "FatturaElettronica.FatturaElettronicaBody.DatiGenerali.DatiGeneraliDocumento.DatiRitenuta.CausalePagamento",
                            FieldValue = causaleId.ToString(),
                            Tbcreated = DateTime.Now,
                            Tbmodified = DateTime.Now,
                            TbcreatedId = _loginId,
                            TbmodifiedId = _loginId
                        });
                    }
                }

                // Aggiornamento LastSubId
                foreach (var f in efMaSaleDoc)
                {
                    f.LastSubId = efMaSaleDocDetails.Count(d => d.SaleDocId == f.SaleDocId);
                }

                // Bulk Insert transazionale
                if (efMaSaleDoc.Any())
                {
                    bool esito = BulkPersistenceHelper.ExecuteInBulkTransaction(_ordiniCntx, () =>
                    {
                        BulkPersistenceHelper.BulkExecuteList(_ordiniCntx, efMaSaleDoc, "MaSaleDoc", BulkOperation.Insert, "fatture", bulkMessage);
                        BulkPersistenceHelper.BulkExecuteList(_ordiniCntx, efMaSaleDocDetails, "MaSaleDocDetails", BulkOperation.Insert, "dettagli fatture", bulkMessage);
                        BulkPersistenceHelper.BulkExecuteList(_ordiniCntx, efMaSaleDocSummary, "MaSaleDocSummary", BulkOperation.Insert, "summary fatture", bulkMessage);
                        BulkPersistenceHelper.BulkExecuteList(_ordiniCntx, efMaCrossReferences, "MaCrossReferences", BulkOperation.Insert, "cross ref", bulkMessage);
                        BulkPersistenceHelper.BulkExecuteList(_ordiniCntx, efMaCrossReferencesNotes, "MaCrossReferencesNotes", BulkOperation.Insert, "cross ref notes", bulkMessage);
                        BulkPersistenceHelper.BulkExecuteList(_ordiniCntx, efMaEiItdocAdditionalData, "MaEiItdocAdditionalData", BulkOperation.Insert, "dati aggiuntivi FE", bulkMessage);

                        // Aggiornamento contatore ID
                        idNumbers.LastId = saleDocId;
                        _ordiniCntx.SaveChanges();
                    }, logMessage, errori);

                    if (!esito) return false;

                    logMessage?.Invoke($"Fatture generate con successo: {efMaSaleDoc.Count}");
                }

                return true;
            }
            catch (Exception ex)
            {
                logMessage?.Invoke($"Errore generale nella procedura Consuntivi: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region Helper Interni

        private static MaSaleDocDetail CreaRigaDescrittivaFattura(int iNewRowsCount, CurFatt cFattura, string descrizione)
        {
            return new MaSaleDocDetail
            {
                Line = (short)((cFattura.LastLine ?? 0) + iNewRowsCount),
                SubId = (cFattura.LastSubId ?? 0) + iNewRowsCount,
                SaleDocId = cFattura.SaleDocId,
                LineType = (int)LineType.Nota,
                Description = descrizione,
                InEi = "1",
                DocumentType = (int)DocumentType.Fattura,
                IncludedInTurnover = "0",
                CustSupp = cFattura.CodCliente,
                CustSuppType = 3211264,
                DepartureDate = cFattura.DataFattura,
                Tbcreated = DateTime.Now,
                Tbmodified = DateTime.Now,
                TbcreatedId = 0,
                TbmodifiedId = 0,
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
                AllNrCanoni = 0,
                AllCanoniDataI = DomainConstants.DataNulla,
                AllCanoniDataF = DomainConstants.DataNulla
            };
        }

        private MaCrossReferences CreaCrossReference(int idOrdine, int idFattura)
        {
            return new MaCrossReferences
            {
                OriginDocType = (int)CrossReference.OrdineCliente,
                OriginDocId = idOrdine,
                OriginDocSubId = 0,
                OriginDocLine = 0,
                DerivedDocType = (int)CrossReference.FatturaImmediata,
                DerivedDocId = idFattura,
                DerivedDocSubId = 0,
                DerivedDocLine = 0,
                Manual = "1",
                TbcreatedId = _loginId,
                TbmodifiedId = _loginId
            };
        }

        private MaCrossReferencesNotes CreaCrossReferenceNotes(int idOrdine, int idFattura, string notes)
        {
            return new MaCrossReferencesNotes
            {
                OriginDocType = (int)CrossReference.OrdineCliente,
                OriginDocId = idOrdine,
                DerivedDocType = (int)CrossReference.FatturaImmediata,
                DerivedDocId = idFattura,
                Notes = notes,
                TbcreatedId = _loginId,
                TbmodifiedId = _loginId
            };
        }

        public static bool Break(MaSaleOrd ordine, MaSaleDoc testaFattura, MaCustSupp customer, MaSalesOrdParameters param)
        {
            var options = customer?.MaCustSuppCustomerOptions ?? new MaCustSuppCustomerOptions();

            if (testaFattura.CustSupp != ordine.InvoicingCustomer ||
                testaFattura.Payment != ordine.Payment ||
                testaFattura.Currency != ordine.Currency ||
                testaFattura.Salesperson != ordine.Salesperson ||
                (!string.IsNullOrEmpty(testaFattura.Salesperson) && (
                    testaFattura.SalespersonPolicy != ordine.SalespersonPolicy ||
                    testaFattura.SalespersonCommAuto != ordine.SalespersonCommAuto ||
                    testaFattura.SalespersonCommPercAuto != ordine.SalespersonCommPercAuto)) ||
                testaFattura.AreaManager != ordine.AreaManager ||
                (!string.IsNullOrEmpty(testaFattura.AreaManager) && (
                    testaFattura.AreaManagerPolicy != ordine.AreaManagerPolicy ||
                    testaFattura.AreaManagerCommAuto != ordine.AreaManagerCommAuto ||
                    testaFattura.AreaManagerCommPercAuto != ordine.AreaManagerCommPercAuto)) ||
                options.OneInvoicePerOrder == "1")
            {
                return true;
            }

            if (param.FulfillmentBreakByArea == "1" && testaFattura.Area != ordine.Area) return true;
            if (param.FulfillmentBreakByInvRsn == "1" && testaFattura.InvRsn != ordine.InvRsn) return true;
            if (param.FulfillmentBreakByJob == "1" && testaFattura.Job != ordine.Job) return true;
            if (param.FulfillmentBreakByDocBranch == "1" && testaFattura.SendDocumentsTo != ordine.SendDocumentsTo) return true;
            if (param.FulfillmentBreakByGoodBranch == "1" && testaFattura.ShipToAddress != ordine.ShipToAddress) return true;
            if (param.FulfillmentBreakByShippRsn == "1" && testaFattura.ShippingReason != ordine.ShippingReason) return true;
            if (param.FulfillmentBreakByCarrier == "1" && testaFattura.Carrier1 != ordine.Carrier1) return true;
            if (param.FulfillmentBreakByPort == "1" && testaFattura.Port != ordine.Port) return true;
            if (param.FulfillmentBreakByPackage == "1" && testaFattura.Package != ordine.Package) return true;
            if (param.FulfillmentBreakByTransport == "1" && testaFattura.Transport != ordine.Transport) return true;
            if (param.FulfillmentBreakByCig == "1" && (testaFattura.ContractCode != ordine.ContractCode || testaFattura.ProjectCode != ordine.ProjectCode)) return true;
            if (param.FulfillmentBreakByTcg == "1" && testaFattura.TaxCommunicationGroup != ordine.TaxCommunicationGroup) return true;

            return false;
        }

        public static DateTime GetDataPeriodoFranchigia(DateTime dataRiferimento, Periodo periodicita, bool isFrom)
        {
            int y = dataRiferimento.Year;
            int m = dataRiferimento.Month;

            return periodicita switch
            {
                Periodo.Annuale => isFrom ? new DateTime(y, 1, 1) : new DateTime(y, 12, 31, 23, 59, 59),
                Periodo.Semestrale => m <= 6
                    ? (isFrom ? new DateTime(y, 1, 1) : new DateTime(y, 6, 30, 23, 59, 59))
                    : (isFrom ? new DateTime(y, 7, 1) : new DateTime(y, 12, 31, 23, 59, 59)),
                Periodo.Quadrimestrale => m switch
                {
                    <= 4 => isFrom ? new DateTime(y, 1, 1) : new DateTime(y, 4, 30, 23, 59, 59),
                    <= 8 => isFrom ? new DateTime(y, 5, 1) : new DateTime(y, 8, 31, 23, 59, 59),
                    _ => isFrom ? new DateTime(y, 9, 1) : new DateTime(y, 12, 31, 23, 59, 59)
                },
                Periodo.Trimestrale => m switch
                {
                    <= 3 => isFrom ? new DateTime(y, 1, 1) : new DateTime(y, 3, 31, 23, 59, 59),
                    <= 6 => isFrom ? new DateTime(y, 4, 1) : new DateTime(y, 6, 30, 23, 59, 59),
                    <= 9 => isFrom ? new DateTime(y, 7, 1) : new DateTime(y, 9, 30, 23, 59, 59),
                    _ => isFrom ? new DateTime(y, 10, 1) : new DateTime(y, 12, 31, 23, 59, 59)
                },
                Periodo.Bimestrale => (m % 2 == 1)
                    ? (isFrom ? new DateTime(y, m, 1) : new DateTime(y, m + 1, DateTime.DaysInMonth(y, m + 1), 23, 59, 59))
                    : (isFrom ? new DateTime(y, m - 1, 1) : new DateTime(y, m, DateTime.DaysInMonth(y, m), 23, 59, 59)),
                _ => isFrom ? new DateTime(y, m, 1) : new DateTime(y, m, DateTime.DaysInMonth(y, m), 23, 59, 59)
            };
        }

        private class MiniDistinta
        {
            public string CodIntegra { get; set; }
            public DateTime DataCessazione { get; set; }
            public DateTime DataScadenzaFissa { get; set; }
            public ICollection<AllordCliContrattoDistintaServAgg> AllordCliContrattoDistintaServAgg { get; set; }
        }

        #endregion
    }
}