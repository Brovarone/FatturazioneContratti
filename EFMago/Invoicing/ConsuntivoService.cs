using EFCore.BulkExtensions;
using EFMago.Models;
using FatturazioneContratti.Common;
using FatturazioneContratti.Orders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace FatturazioneContratti.Invoicing
{
    public class ConsuntivoService
    {
        private readonly FattureContext _ordiniCntx;
        private readonly int _loginId;

        /// <summary>
        /// Timestamp unico della fatturazione: Tbcreated/Tbmodified di tutte le entità
        /// generate condividono lo stesso valore (prima ogni proprietà chiamava DateTime.Now
        /// separatamente, con millisecondi diversi sulla stessa riga).
        /// </summary>
        private readonly DateTime _now = DateTime.Now;
        private const string RegIvaPosticipate = "V2";
        private const string TecnoAll1 = "NOLEGGIO SPA";
        private const string EstateSicura = "ESTATE";

        /// <summary>
        /// Dimensione dei blocchi per le progress (stesso default di
        /// <c>GeneraRigheOrdineOptions.DimensioneBlocco</c>): barra grande = blocchi,
        /// barra piccola = record del blocco.
        /// </summary>
        private const int DimensioneBlocco = 100;

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

        private static readonly Dictionary<string, string[]> FilialiMapping = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
        {
            { "TO", new[] { "01" } },
            { "MI", new[] { "02" } },
            { "AT", new[] { "03" } },
            { "AO", new[] { "04" } },
            { "NO", new[] { "05" } },
            { "BI", new[] { "08" } },
            { "VA", new[] { "09" } },
            { "CN", new[] { "11" } },
            { "AL", new[] { "12" } }
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
            Action<string, LogTarget> logMessage = null,
            Action<string> updateStatus = null,
            ProgressCallbacks? progress1 = null,
            ProgressCallbacks? progress2 = null)
        {
            var fatalContrattoErrorList = new List<string>();
            var fatalEventoErrorList = new List<string>();
            var contrattoErrorList = new List<string>();
            var eventoErrorList = new List<string>();
            int associati = 0;   // interventi agganciati a una riga contratto con esito positivo

            try
            {
                // I filtri della run vengono registrati a file/UI: senza questo il log
                // di sessione non dice mai con quali parametri è partito il controllo.
                LogDiagnostica.LogFiltri(logMessage, filtri);

                updateStatus?.Invoke("Estrazione distinte e interventi Integra...");

                IQueryable<AllordCliContrattoDistinta> distintaQuery =
                    _ordiniCntx.AllordCliContrattoDistinta
                    .Include(sa => sa.AllordCliContrattoDistintaServAgg)
                    .Where(wsa => wsa.Servizio != TecnoAll1 && wsa.Servizio != EstateSicura)
                    .Include(o => o.AllordCliContratto)
                        .ThenInclude(ord => ord.SaleOrd)
                            .ThenInclude(info => info.ALLOrdCliAcc);

                // Filtro singolo ordine (FiltriOrdiniConsuntivo.SingoloOrdine/NrOrdine):
                // restringe le distinte candidate; gli interventi si agganciano solo a queste.
                if (filtri.SingoloOrdine && !string.IsNullOrWhiteSpace(filtri.NrOrdine))
                    distintaQuery = distintaQuery.Where(wsa => wsa.AllordCliContratto.SaleOrd.InternalOrdNo == filtri.NrOrdine);                                           

                var allDistinte = distintaQuery
                    .Select(x => new MiniDistinta
                    {
                        NrOrdine = x.AllordCliContratto.SaleOrd.InternalOrdNo,
                        Filiale = x.AllordCliContratto.SaleOrd.CostCenter,
                        Priorita = x.AllordCliContratto.SaleOrd.Priority,
                        CodIntegra = x.CodIntegra,
                        DataCessazione = x.AllordCliContratto.SaleOrd.ALLOrdCliAcc.DataCessazione ?? DomainConstants.DataNulla,
                        DataScadenzaFissa = x.AllordCliContratto.SaleOrd.ALLOrdCliAcc.DataScadenzaFissa ?? DomainConstants.DataNulla,
                        AllordCliContrattoDistintaServAgg = x.AllordCliContrattoDistintaServAgg,
                        AlltipoRigaServizio = x.AlltipoRigaServizio,
                        Servizio = x.Servizio
                    })
                    .ToList();

                IQueryable<IntegraInterventi> intQuery = _ordiniCntx.IntegraInterventi.AsQueryable();

                // FILTRI
                if (!filtri.Riassegna) intQuery = intQuery.Where(o => o.MAssociato == "0");

                // Filtro data solo se non è NoFilter
                if (filtri.InterventiFilter != InterventiFilterMode.NoFilter)
                {
                    intQuery = intQuery.Where(o => o.InizioAllarme.HasValue 
                        && o.InizioAllarme.Value.Date >= filtri.FromInterventiDate 
                        && o.InizioAllarme.Value.Date <= filtri.ToInterventiDate);
                }

                if (filtri.SingolaFiliale) intQuery = intQuery.Where(o => o.Filiale == filtri.Filiale);

                // NrMese (mese selezionato in UI): in modalità MeseAnno ribadisce il range date
                if (filtri.InterventiFilter == InterventiFilterMode.MeseAnno
                    && int.TryParse(filtri.NrMese, out int meseFiltro)
                    && meseFiltro >= 1 && meseFiltro <= 12)
                    intQuery = intQuery.Where(o => o.InizioAllarme.HasValue && o.InizioAllarme.Value.Month == meseFiltro);

                var allInterventi = intQuery.ToList();
                if (allInterventi.Count == 0)
                {
                    LogDiagnostica.Informativo(logMessage, "Nessun intervento Integra da controllare.");
                    return true;
                }

                // Indice per codice contratto: la vecchia Where su lista rieseguiva la scansione
                // delle distinte per ogni intervento (O(interventi × distinte)).
                var distintePerCodice = allDistinte.ToLookup(x => x.CodIntegra, StringComparer.OrdinalIgnoreCase);

                LogDiagnostica.Informativo(logMessage, $"Interventi estratti: {allInterventi.Count}");

                // Progress a blocchi da 100 record (stessa logica di GenerazioneRigheOrdiniService):
                // barra grande = blocchi, barra piccola = record del blocco corrente.
                int numeroBlocchi = Math.Max(1, (allInterventi.Count + DimensioneBlocco - 1) / DimensioneBlocco);
                progress1?.Init(0, numeroBlocchi);
                int indice = 0;

                foreach (var i in allInterventi)
                {
                    // Nuovo blocco: chiudo il precedente e riavvio la barra piccola
                    if (indice % DimensioneBlocco == 0)
                    {
                        if (indice > 0)
                        {
                            progress2?.Finish();
                            progress1?.Advance();
                        }
                        progress2?.Init(0, Math.Max(1, Math.Min(DimensioneBlocco, allInterventi.Count - indice)));
                    }
                    indice++;
                    progress2?.Advance();

                    // Join per codice contratto indicizzato (lookup) + regola di mapping filiale
                    var distinteFiltrate = distintePerCodice[i.Contratto]
                        .Where(x => FilialeCorrisponde(x, i))
                        .ToList();

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
                            i.TBModified = _now;
                            i.TBModifiedID = _loginId;
                            associati++;
                        }
                        else if (allEventi.Count > 1)
                        {
                            // Voce arricchita: nel file di log serve sapere QUALE intervento
                            // ha fallito (evento, filiale, data), non solo il codice contratto.
                            string err = DescriviIntervento(i);
                            if (!fatalEventoErrorList.Contains(err)) fatalEventoErrorList.Add(err);
                        }
                        else
                        {
                            string err = DescriviIntervento(i);
                            if (!eventoErrorList.Contains(err)) eventoErrorList.Add(err);
                        }
                    }
                    else if (distinteFiltrate.Count > 1)
                    {
                        // Per ogni distinta filtrata aggiungo alla lista l'entry con contratto + numero ordine
                        foreach (var d in distinteFiltrate)
                        {
                            var entry = DescriviIntervento(i, d.NrOrdine);
                            if (!fatalContrattoErrorList.Contains(entry))
                                fatalContrattoErrorList.Add(entry);                            
                        }
                    }
                    else
                    {
                        string err = DescriviIntervento(i);
                        if (!contrattoErrorList.Contains(err)) contrattoErrorList.Add(err);
                    }
                }

                // Chiude l'ultimo blocco e la barra generale
                progress2?.Finish();
                progress1?.Advance();
                progress1?.Finish();

                // Salvataggio aggiornamenti interventi
                updateStatus?.Invoke("Salvataggio aggiornamenti interventi...");
                _ordiniCntx.SaveChanges();

                int anomalie = fatalContrattoErrorList.Count + fatalEventoErrorList.Count
                             + contrattoErrorList.Count + eventoErrorList.Count;

                // Esito sempre emesso: senza questa riga il log finiva con "Interventi estratti: N"
                // e non si capiva mai quanti interventi erano stati agganciati.
                LogDiagnostica.Informativo(logMessage,
                    $"Controllo flusso completato: {allInterventi.Count} interventi, {associati} associati, {anomalie} anomalie");

                if (anomalie != 0)
                {
                    string riepilogo = $"Contratti duplicati={fatalContrattoErrorList.Count}, Eventi multipli={fatalEventoErrorList.Count}, Contratti mancanti={contrattoErrorList.Count}, Eventi mancanti={eventoErrorList.Count}";

                    var dettaglio = new StringBuilder();
                    dettaglio.AppendLine("Riscontrate anomalie o incongruenze nel controllo flusso.");
                    dettaglio.AppendLine();

                    if (fatalContrattoErrorList.Count != 0)
                    {
                        dettaglio.AppendLine("ERRORE CRITICO - Contratti duplicati:");
                        foreach (var c in fatalContrattoErrorList) dettaglio.AppendLine($" - {c}");
                        dettaglio.AppendLine();
                    }

                    if (fatalEventoErrorList.Count != 0)
                    {
                        dettaglio.AppendLine("ERRORE CRITICO - Eventi multipli per contratto:");
                        foreach (var e in fatalEventoErrorList) dettaglio.AppendLine($" - {e}");
                        dettaglio.AppendLine();
                    }

                    if (contrattoErrorList.Count != 0)
                    {
                        dettaglio.AppendLine("CONTRATTI NON TROVATI:");
                        foreach (var c in contrattoErrorList) dettaglio.AppendLine($" - {c}");
                        dettaglio.AppendLine();
                    }

                    if (eventoErrorList.Count != 0)
                    {
                        dettaglio.AppendLine("EVENTI NON ASSOCIATI:");
                        foreach (var e in eventoErrorList) dettaglio.AppendLine($" - {e}");
                        dettaglio.AppendLine();
                    }

                    dettaglio.AppendLine($"Riepilogo: {riepilogo}");

                    // UI: una riga sola con i contatori (il dettaglio resterebbe illeggibile
                    // nel ListBox). Va SOLO su UI: il file riceve il dettaglio completo
                    // subito dopo, e con "Both" la testa finirebbe due volte nel log.
                    logMessage?.Invoke($"Riscontrate anomalie o incongruenze nel controllo flusso. {riepilogo}", LogTarget.Ui);
                    // File: dettaglio completo (sezioni + riepilogo), scritto UNA volta sola
                    logMessage?.Invoke(dettaglio.ToString(), LogTarget.File);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                LogDiagnostica.Errore(logMessage, ex);
                return false;
            }
        }

        /// <summary>
        /// Descrizione completa di un intervento per le voci di anomalia: nel file di log
        /// servono contratto, evento, filiale e data allarme per capire che cosa allineare.
        /// </summary>
        private static string DescriviIntervento(IntegraInterventi i, string? nrOrdine = null)
        {
            string testo = $"Contratto {i.Contratto}";
            if (!string.IsNullOrWhiteSpace(nrOrdine)) testo += $" - ordine {nrOrdine}";
            testo += $" - evento {i.TipoEvento} - filiale {i.Filiale} - allarme {i.InizioAllarme:dd/MM/yyyy}";
            return testo;
        }

        #endregion

        #region Genera Fatture Consuntivo
        public bool GeneraFattureConsuntivo(
            FiltriOrdiniConsuntivo filtri,
            Action<string, LogTarget> logMessage = null,
            Action<string> updateStatus = null,
            ProgressCallbacks? progress1 = null,
            ProgressCallbacks? progress2 = null)
        {
            var bulkMessage = new StringBuilder();
            var debugging = new StringBuilder();

            try
            {
                // I filtri della run vengono registrati a file/UI: ogni step del workflow
                // resta autodescrittivo nel log di sessione (anche dopo un controllo).
                LogDiagnostica.LogFiltri(logMessage, filtri);

                updateStatus?.Invoke("Estrazione ordini corrispondenti per consuntivazione...");

                // Precaricamento tabelle core.
                // NOTA: MaCustSupp NON va precaricato con .Load(): la lista "clienti" qui sotto
                // ha già l'Include di MaCustSuppCustomerOptions e la tabella veniva caricata due volte.
                //_ordiniCntx.IntegraInterventi.Load();
                //_ordiniCntx.MaCustSuppBranches.Load();
                //_ordiniCntx.MaCustSuppCustomerOptions.Load();

                IQueryable<IntegraInterventi> intFiltrati = _ordiniCntx.IntegraInterventi
                    .Where(w => w.AllordCliContrattoDistinta != null && w.AllordCliContrattoDistinta.Servizio != TecnoAll1);

                // Filtro data solo se non è NoFilter
                if (filtri.InterventiFilter != InterventiFilterMode.NoFilter)
                {
                    intFiltrati = intFiltrati.Where(o => o.InizioAllarme.HasValue 
                        && o.InizioAllarme.Value.Date >= filtri.FromInterventiDate 
                        && o.InizioAllarme.Value.Date <= filtri.ToInterventiDate);
                }

                if (filtri.SingolaFiliale)
                {
                    intFiltrati = intFiltrati.Where(o => o.Filiale == filtri.Filiale);
                }

                // NrMese (mese selezionato in UI): in modalità MeseAnno ribadisce il range date
                if (filtri.InterventiFilter == InterventiFilterMode.MeseAnno
                    && int.TryParse(filtri.NrMese, out int meseFiltro)
                    && meseFiltro >= 1 && meseFiltro <= 12)
                {
                    intFiltrati = intFiltrati.Where(o => o.InizioAllarme.HasValue && o.InizioAllarme.Value.Month == meseFiltro);
                }

                var interventiList = intFiltrati.ToList();
                // Gli ID si ricavano dalla lista già materializzata: la query non va eseguita due volte.
                int[] aOrdini = interventiList.Select(s => s.MIdOrdCli ?? 0).Where(id => id > 0).Distinct().ToArray();

                // Filtro singolo ordine (FiltriOrdiniConsuntivo.SingoloOrdine/NrOrdine):
                // flag valutato in C# così il Where resta un'espressione traducibile in SQL.
                bool soloOrdine = filtri.SingoloOrdine && !string.IsNullOrWhiteSpace(filtri.NrOrdine);

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
                    // Filtro singolo ordine (FiltriOrdiniConsuntivo.SingoloOrdine/NrOrdine)
                    .Where(o => !soloOrdine || o.InternalOrdNo == filtri.NrOrdine)
                    .OrderBy(o => o.InvoicingCustomer)
                    .ThenBy(o => o.ContractCode)
                    .ThenBy(o => o.Payment)
                    .ThenBy(o => o.Carrier1)
                    .ToList();

                // Progress a blocchi da 100 record (stessa logica di GenerazioneRigheOrdiniService):
                // barra grande = blocchi, barra piccola = ordini del blocco corrente.
                int numeroBlocchi = Math.Max(1, (ordiniList.Count + DimensioneBlocco - 1) / DimensioneBlocco);
                progress1?.Init(0, numeroBlocchi);
                int indice = 0;

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

                var idNumbers = _ordiniCntx.MaIdnumbers.FirstOrDefault(k => k.CodeType == (int)IdType.DocVend);
                if (idNumbers == null)
                {
                    LogDiagnostica.Errore(logMessage, "Contatore MA_IdNumbers per i documenti di vendita non trovato: impossibile generare le fatture.");
                    return false;
                }

                int saleDocId = idNumbers.LastId ?? 0;

                var clienti = _ordiniCntx.MaCustSupp
                    .Include(c => c.MaCustSuppCustomerOptions)
                    .Where(w => w.CustSuppType == 3211264)
                    .ToList();

                // Buffer righe dell'ordine corrente: l'ID fattura viene assegnato solo quando il
                // gruppo produce almeno una riga, così i gruppi senza interventi fatturabili non
                // scrivono teste fattura vuote né cross-reference orfani (il vecchio flusso scriveva
                // la testa al primo ordine del gruppo a prescindere da isNewRows).
                var cFattura = new CurFatt();
                var saleDoc = new MaSaleDoc();
                bool testaDaScrivere = true;

                foreach (var o in ordiniList)
                {
                    // Nuovo blocco: chiudo il precedente e riavvio la barra piccola
                    if (indice % DimensioneBlocco == 0)
                    {
                        if (indice > 0)
                        {
                            progress2?.Finish();
                            progress1?.Advance();
                        }
                        progress2?.Init(0, Math.Max(1, Math.Min(DimensioneBlocco, ordiniList.Count - indice)));
                    }
                    indice++;
                    progress2?.Advance();

                    debugging.AppendLine($"Ordine: {o.InternalOrdNo} Cliente: {o.Customer}");

                    // Criteri di Rottura e accorpamento fattura
                    if (Break(o, saleDoc, cFattura.Cliente, paramSaleOrd))
                    {
                        cFattura = new CurFatt(o)
                        {
                            Cliente = clienti.FirstOrDefault(w => w.CustSupp == o.InvoicingCustomer),
                            DataFattura = filtri.DataFattura
                        };
                        if ((cFattura.PercIva ?? 0) == 0) cFattura.PercIva = dDefPercIva;
                        testaDaScrivere = true;
                    }

                    int iNewRowsCount = 0;
                    var righeOrdine = new List<MaSaleDocDetail>();
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

                                    if (interventiEvento.Count != 0)
                                    {
                                        s.AnnoIntervento = filtri.ToInterventiDate.Year;
                                        var toDataMese = new DateTime(filtri.ToInterventiDate.Year, filtri.ToInterventiDate.Month, DateTime.DaysInMonth(filtri.ToInterventiDate.Year, filtri.ToInterventiDate.Month), 23, 59, 59);
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
                                            righeOrdine.Add(CreaRigaDescrittivaFattura(iNewRowsCount, cFattura, "INTERVENTI SUPPLEMENTARI"));

                                            if (o.ALLordCliDescrizioni != null)
                                            {
                                                foreach (var dTesta in o.ALLordCliDescrizioni)
                                                {
                                                    iNewRowsCount++;
                                                    righeOrdine.Add(CreaRigaDescrittivaFattura(iNewRowsCount, cFattura, dTesta.Descrizione));
                                                }
                                            }
                                        }

                                        // STEP 3: Descrizioni riga contratto
                                        if (c.AllordCliContrattoDescFatt != null)
                                        {
                                            foreach (var dRiga in c.AllordCliContrattoDescFatt)
                                            {
                                                iNewRowsCount++;
                                                righeOrdine.Add(CreaRigaDescrittivaFattura(iNewRowsCount, cFattura, dRiga.Descrizione));
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
                                            Notes = StringHelpers.SubstringSafe(c.Nota, 32),
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
                                            Tbcreated = _now,
                                            Tbmodified = _now,
                                            TbcreatedId = _loginId,
                                            TbmodifiedId = _loginId
                                        };
                                        righeOrdine.Add(r);
                                    }
                                }
                            }
                        }
                    }

                    if (righeOrdine.Count > 0)
                    {
                        // Prima riga utile del gruppo: assegna l'ID documento (e solo ora
                        // consuma il numeratore, cosi' i gruppi senza righe non lasciano buchi).
                        if (cFattura.SaleDocId == 0)
                            cFattura.SaleDocId = ++saleDocId;

                        // Le righe erano state create con SaleDocId provvisorio: allinealo
                        // all'ID effettivo prima di accodarle.
                        foreach (var riga in righeOrdine)
                            riga.SaleDocId = cFattura.SaleDocId;

                        efMaSaleDocDetails.AddRange(righeOrdine);

                        cFattura.LastLine = (cFattura.LastLine ?? 0) + iNewRowsCount;
                        cFattura.LastSubId = (cFattura.LastSubId ?? 0) + iNewRowsCount;

                        if (testaDaScrivere)
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
                                Tbcreated = _now,
                                Tbmodified = _now,
                                TbcreatedId = _loginId,
                                TbmodifiedId = _loginId,
                                WorkerIdissue = _loginId
                            };
                            efMaSaleDoc.Add(saleDoc);
                            testaDaScrivere = false;
                        }

                        // Riferimenti incrociati
                        efMaCrossReferences.Add(CreaCrossReference(o.SaleOrdId, cFattura.SaleDocId));
                        efMaCrossReferencesNotes.Add(CreaCrossReferenceNotes(o.SaleOrdId, cFattura.SaleDocId, string.Empty));

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
                                Tbcreated = _now,
                                Tbmodified = _now,
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
                                Tbcreated = _now,
                                Tbmodified = _now,
                                TbcreatedId = _loginId,
                                TbmodifiedId = _loginId
                            });
                        }
                    }
                }

                // Chiudo l'ultimo blocco e la barra generale (non c'era nessun Finish:
                // la progress restava a metà a fine elaborazione). Con lista vuota non
                // c'è stato nessun blocco da chiudere: la barra piccola resta com'è.
                if (indice > 0) progress2?.Finish();
                progress1?.Advance();
                progress1?.Finish();

                // Aggiornamento LastSubId (conteggio raggruppato: il Count dentro il foreach
                // era O(fatture x righe))
                var righePerFattura = efMaSaleDocDetails
                    .GroupBy(d => d.SaleDocId)
                    .ToDictionary(g => g.Key, g => g.Count());

                foreach (var f in efMaSaleDoc)
                    f.LastSubId = righePerFattura.TryGetValue(f.SaleDocId, out int nRigheFattura) ? nRigheFattura : 0;

                // Bulk Insert transazionale
                if (efMaSaleDoc.Count != 0)
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
                    }, logMessage);

                    if (!esito) return false;

                    LogDiagnostica.Informativo(logMessage,
                        $"Fatture generate con successo: {efMaSaleDoc.Count} testate, {efMaSaleDocDetails.Count} righe");
                }

                return true;
            }
            catch (Exception ex)
            {
                LogDiagnostica.Errore(logMessage, ex);
                return false;
            }
            finally
            {
                // Statistiche bulk e tracciato diagnostico a fine passo, secondo la
                // regola di routing di LogDiagnostica (gira su ogni uscita, compreso il return false).
                LogDiagnostica.Scarica(logMessage, bulk: bulkMessage, debugging: debugging);
            }
        }

        #endregion

        #region Helper Interni

        private MaSaleDocDetail CreaRigaDescrittivaFattura(int iNewRowsCount, CurFatt cFattura, string descrizione)
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
                Tbcreated = _now,
                Tbmodified = _now,
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

        /// <summary>
        /// Verifica la corrispondenza filiale tra distinta e intervento.
        /// x.Filiale è il CostCenter letterale (es. "NO","MI");
        /// i.Filiale è il codice numerico memorizzato su IntegraInterventi (es. "05","08").
        /// </summary>
        private static bool FilialeCorrisponde(MiniDistinta distinta, IntegraInterventi intervento)
        {
            if (string.IsNullOrWhiteSpace(distinta.Filiale) || string.IsNullOrWhiteSpace(intervento.Filiale))
                return false;

            if (FilialiMapping.TryGetValue(distinta.Filiale.Trim(), out var codici))
                return codici.Any(c => string.Equals(c, intervento.Filiale.Trim(), StringComparison.OrdinalIgnoreCase));

            // fallback: se non esiste mapping, confronta direttamente (gestione edge-case)
            return string.Equals(distinta.Filiale.Trim(), intervento.Filiale.Trim(), StringComparison.OrdinalIgnoreCase);
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
            public string NrOrdine { get; set; }
            public string Filiale { get; set; }
            public short? Priorita { get; set; }   // <-- aggiunta
            public string CodIntegra { get; set; }
            public DateTime DataCessazione { get; set; }
            public DateTime DataScadenzaFissa { get; set; }
            public ICollection<AllordCliContrattoDistintaServAgg> AllordCliContrattoDistintaServAgg { get; set; }
            public AlltipoRigaServizio AlltipoRigaServizio { get; set; }
            public String Servizio { get; set; }
        }

        #endregion
    }
}
