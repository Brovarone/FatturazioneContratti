using EFMago.Models;
using FatturazioneContratti.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace FatturazioneContratti.Orders
{
    /// <summary>
    /// Creazione ordini cliente da zero (testata MaSaleOrd + righe + Summary/Shipping/Notes),
    /// usato dal sito web di sola scrittura ordini.
    ///
    /// Convenzioni replicate dai dati reali Mago (ispezione su 732 ordini, ottobre 2026):
    /// - SaleOrdId (PK non-identity) assegnato dall'app dal contatore MA_IDNumbers (CodeType OrdCli),
    ///   allocazione atomica con UPDATE...OUTPUT dentro la transazione (sicura per utenti multipli);
    /// - InternalOrdNo = "yy/NNNNN" (es. "26/00116") da MA_NonFiscalNumbers (CodeType OrdCli),
    ///   riga per BalanceYear = anno di OrderDate; la riga dell'anno nuovo viene creata clonando
    ///   la più recente come template;
    /// - testata: CustSuppType 3211264, SaleType 3670020 (o override richiesta), Divisione default ERP,
    ///   Currency EUR (fallback), AccTpl/InvRsn da MaUserDefaultSaleOrders, Presentation dall'anagrafica,
    ///   TaxJournal "VEN" (o override), OurReference = Sede della richiesta;
    /// - Summary/Shipping/Notes: tabelle 1:1 con la testata, presenti su tutti gli ordini reali → create sempre;
    /// - righe: Line/SubId 1..N, Position = Line (convenzione Mago sulle righe semplici), InternalOrdNo
    ///   denormalizzato copiato dalla testata, Importi = round(qta * prezzo) e round(taxable * (100+iva)/100).
    /// </summary>
    public class CreazioneOrdiniService(FattureContext context, int loginId)
    {
        private readonly FattureContext _context = context;
        private readonly int _loginId = loginId;

        /// <summary>
        /// Timestamp unico dell'intera operazione: tutti i Tbcreated/Tbmodified scritti durante
        /// la creazione usano lo stesso valore (stesso criterio di GenerazioneRigheOrdiniService).
        /// </summary>
        private readonly DateTime _now = DateTime.Now;

        private const int TipoClienteMago = 3211264;       // CustSuppType "cliente"
        private const string ValutaDefault = "EUR";        // 732/732 ordini reali
        private const string RegistroIvaDefault = "VEN";   // registro più frequente nei dati reali (516/732)

        /// <summary>
        /// Crea un ordine cliente completo (testata + righe + satelliti) e ne restituisce l'esito.
        /// In caso di errore di validazione o di scrittura restituisce null: il dettaglio è già
        /// stato emesso su log tramite <see cref="LogDiagnostica"/> / rollback della transazione.
        /// </summary>
        public EsitoCreazioneOrdine? CreaOrdine(CreazioneOrdineRequest request, Action<string, LogTarget>? logMessage = null)
        {
            // ---------------------------------------------------------------- validazione input
            StringBuilder errori = ValidaRequest(request);
            if (errori.Length > 0)
            {
                LogDiagnostica.Errore(logMessage, $"Creazione ordine non eseguita:{Environment.NewLine}{errori}");
                return null;
            }

            string cliente = request.Cliente.Trim();
            DateTime dataOrdine = request.DataOrdine;

            // ---------------------------------------------------------------- anagrafica + default ERP
            MaCustSupp? anagrafica = _context.MaCustSupp.AsNoTracking()
                .FirstOrDefault(c => c.CustSuppType == TipoClienteMago && c.CustSupp == cliente);
            if (anagrafica == null)
            {
                LogDiagnostica.Errore(logMessage, $"Creazione ordine non eseguita: cliente '{cliente}' inesistente in anagrafica (tipo {TipoClienteMago}).");
                return null;
            }

            var defOrdini = _context.MaUserDefaultSaleOrders.AsNoTracking().FirstOrDefault();
            var defIva = _context.MaTaxCodesDefaults.AsNoTracking().FirstOrDefault();
            var codiciIva = _context.MaTaxCodes.AsNoTracking().ToList();

            string codIvaDefault = defIva?.TaxCode ?? string.Empty;
            if (string.IsNullOrWhiteSpace(codIvaDefault))
                LogDiagnostica.Avviso(logMessage, "MaTaxCodesDefaults assente o vuoto: le righe avranno codice IVA vuoto e imposta 0%.");
            var ivaDefault = codiciIva.FirstOrDefault(t => t.TaxCode == codIvaDefault);
            double percIvaDefault = Math.Round((double)(ivaDefault?.Perc ?? 0), DomainConstants.DecimaliPercentuale);

            // ---------------------------------------------------------------- testata (prototipo AddNew)
            MaSaleOrd testata = _context.AddNew<MaSaleOrd>();

            testata.SaleOrdId = 0;                          // assegnato in transazione dal contatore
            testata.InternalOrdNo = string.Empty;           // assegnato in transazione dal numeratore
            testata.Customer = cliente;
            testata.InvoicingCustomer = string.IsNullOrWhiteSpace(request.ClienteFatturazione)
                ? cliente
                : request.ClienteFatturazione.Trim();
            testata.OrderDate = dataOrdine;
            testata.Payment = !string.IsNullOrWhiteSpace(request.Pagamento)
                ? request.Pagamento.Trim()
                : anagrafica.Payment ?? string.Empty;
            testata.PriceList = !string.IsNullOrWhiteSpace(request.Listino)
                ? request.Listino.Trim()
                : anagrafica.PriceList ?? string.Empty;
            testata.Currency = !string.IsNullOrWhiteSpace(request.Valuta)
                ? request.Valuta.Trim()
                : anagrafica.Currency ?? ValutaDefault;
            if (string.IsNullOrWhiteSpace(testata.Currency))
                testata.Currency = ValutaDefault;
            testata.Language = anagrafica.Language ?? string.Empty;
            testata.CustSuppType = TipoClienteMago;
            testata.AccTpl = defOrdini?.SaleOrderAccTpl ?? string.Empty;   // FE
            testata.InvRsn = defOrdini?.SaleOrderInvRsn ?? string.Empty;   // VEN-O
            testata.TaxJournal = string.IsNullOrWhiteSpace(request.RegistroIva)
                ? RegistroIvaDefault
                : request.RegistroIva.Trim();
            testata.OurReference = request.Sede?.Trim() ?? string.Empty;
            if (anagrafica.Presentation.HasValue)
                testata.Presentation = anagrafica.Presentation.Value;
            if (request.Divisione.HasValue)
                testata.Divisione = request.Divisione;
            if (request.TipoVendita.HasValue)
                testata.SaleType = request.TipoVendita;

            // Le 5 colonne flag senza default Fluent: Mago le scrive sempre così (732/732 ordini reali).
            testata.AreaManagerCommAuto = "0";
            testata.AreaManagerCommPercAuto = "0";
            testata.SalespersonCommAuto = "0";
            testata.SalespersonCommPercAuto = "0";
            testata.InstallmStartDateIsAuto = "1";

            // TBCreatedID/TBModifiedID senza default Fluent.
            testata.Tbcreated = _now;
            testata.Tbmodified = _now;
            testata.TbcreatedId = _loginId;
            testata.TbmodifiedId = _loginId;

            // ---------------------------------------------------------------- satelliti 1:1 (Summary/Shipping/Notes)
            MaSaleOrdSummary summary = _context.AddNew<MaSaleOrdSummary>();
            summary.Tbcreated = _now;
            summary.Tbmodified = _now;
            summary.TbcreatedId = _loginId;
            summary.TbmodifiedId = _loginId;

            MaSaleOrdShipping shipping = _context.AddNew<MaSaleOrdShipping>();
            shipping.Tbcreated = _now;
            shipping.Tbmodified = _now;
            shipping.TbcreatedId = _loginId;
            shipping.TbmodifiedId = _loginId;

            MaSaleOrdNotes notes = _context.AddNew<MaSaleOrdNotes>();
            notes.Notes = StringHelpers.SubstringSafe(request.Note, 251);
            notes.Tbcreated = _now;
            notes.Tbmodified = _now;
            notes.TbcreatedId = _loginId;
            notes.TbmodifiedId = _loginId;

            // ---------------------------------------------------------------- righe (prototype-clone)
            MaSaleOrdDetails prototipoRiga = _context.AddNew<MaSaleOrdDetails>();
            var righe = new List<MaSaleOrdDetails>(request.Righe.Count);

            int linea = 0;
            foreach (RigaOrdineCreazione r in request.Righe)
            {
                linea++;

                string descrizione = !string.IsNullOrWhiteSpace(r.Descrizione)
                    ? r.Descrizione.Trim()
                    : r.Item?.Trim() ?? string.Empty;
                string codIvaRiga = string.IsNullOrWhiteSpace(r.CodIva) ? codIvaDefault : r.CodIva.Trim();
                var ivaRiga = codIvaRiga == codIvaDefault
                    ? ivaDefault
                    : codiciIva.FirstOrDefault(t => t.TaxCode == codIvaRiga);
                double percIva = Math.Round((double)(ivaRiga?.Perc ?? percIvaDefault), DomainConstants.DecimaliPercentuale);

                // Importi: stessa arrotondatura dei servizi di generazione (taxable 2 decimali, totale = taxable + imposta).
                double quantita = r.Quantita;
                double valUnit = Math.Round(r.ValoreUnitario, DomainConstants.DecimaliTasse);
                double taxable = Math.Round(quantita * valUnit, DomainConstants.DecimaliTasse);
                double totale = Math.Round(taxable * (100 + percIva) / 100, DomainConstants.DecimaliTasse);

                MaSaleOrdDetails riga = prototipoRiga.CloneFromPrototype();
                riga.SaleOrdId = 0;                            // assegnato in transazione
                riga.InternalOrdNo = string.Empty;             // assegnato in transazione
                riga.Line = (short)linea;
                riga.SubId = linea;
                riga.Position = (short)linea;                  // convenzione Mago sulle righe semplici (Line = Position)
                riga.LineType = (int)r.TipoRiga;
                riga.Item = r.Item?.Trim() ?? string.Empty;
                riga.Description = descrizione;
                riga.UoM = string.IsNullOrWhiteSpace(r.UoM) ? string.Empty : r.UoM.Trim();
                riga.PacksUoM = riga.UoM;
                riga.Qty = quantita;
                riga.UnitValue = valUnit;
                riga.NetPrice = valUnit;
                riga.TaxCode = codIvaRiga;
                riga.TaxableAmount = taxable;
                riga.TotalAmount = totale;
                riga.Invoiced = "0";
                if (r.DataPrevistaConsegna.HasValue)
                    riga.ExpectedDeliveryDate = r.DataPrevistaConsegna.Value;
                if (r.DataConfermaConsegna.HasValue)
                    riga.ConfirmedDeliveryDate = r.DataConfermaConsegna.Value;
                riga.Notes = StringHelpers.SubstringSafe(r.Nota, 32);
                riga.Customer = cliente;
                riga.OrderDate = dataOrdine;
                riga.Job = r.Commessa?.Trim() ?? string.Empty;
                riga.CostCenter = r.CentroCosto?.Trim() ?? string.Empty;
                riga.Offset = r.Contropartita?.Trim() ?? string.Empty;
                riga.ContractCode = r.CIG?.Trim() ?? string.Empty;
                riga.ProjectCode = r.CUP?.Trim() ?? string.Empty;
                riga.Tbcreated = _now;
                riga.Tbmodified = _now;
                riga.TbcreatedId = _loginId;
                riga.TbmodifiedId = _loginId;

                righe.Add(riga);
            }

            // ---------------------------------------------------------------- scrittura in transazione
            LogDiagnostica.Informativo(logMessage,
                $"Creazione ordine cliente {cliente} del {dataOrdine:dd/MM/yyyy}, {righe.Count} riga/e...");

            int saleOrdIdCreato = 0;
            string numeroOrdineCreato = string.Empty;

            bool esito = BulkPersistenceHelper.ExecuteInBulkTransaction(_context, () =>
            {
                // Allocazione atomica dei contatori con UPDATE...OUTPUT: sicura anche con più
                // utenti simultanei dal sito web (il pattern read-then-SaveChanges usato negli
                // altri workflow single-user darebbe chiave duplicata a due creazioni concorrenti).
                saleOrdIdCreato = AllocaProgressivoTabella("MA_IDNumbers", "LastId", "CodeType", (int)IdType.OrdCli, "Contatore ordini");
                numeroOrdineCreato = AllocaNumeroOrdine(dataOrdine, out bool rigaNuovaNumeratore);

                testata.SaleOrdId = saleOrdIdCreato;
                testata.InternalOrdNo = numeroOrdineCreato;
                summary.SaleOrdId = saleOrdIdCreato;
                shipping.SaleOrdId = saleOrdIdCreato;
                notes.SaleOrdId = saleOrdIdCreato;
                foreach (MaSaleOrdDetails riga in righe)
                {
                    riga.SaleOrdId = saleOrdIdCreato;
                    riga.InternalOrdNo = numeroOrdineCreato;
                }

                var sb = new StringBuilder();
                BulkPersistenceHelper.BulkExecuteList(_context, new List<MaSaleOrd> { testata }, "MA_SaleOrd", BulkOperation.Insert, "testata ordine", sb);
                BulkPersistenceHelper.BulkExecuteList(_context, new List<MaSaleOrdSummary> { summary }, "MA_SaleOrdSummary", BulkOperation.Insert, "riepilogo ordine", sb);
                BulkPersistenceHelper.BulkExecuteList(_context, new List<MaSaleOrdShipping> { shipping }, "MA_SaleOrdShipping", BulkOperation.Insert, "spedizioni ordine", sb);
                BulkPersistenceHelper.BulkExecuteList(_context, new List<MaSaleOrdNotes> { notes }, "MA_SaleOrdNotes", BulkOperation.Insert, "note ordine", sb);
                BulkPersistenceHelper.BulkExecuteList(_context, righe, "MA_SaleOrdDetails", BulkOperation.Insert, "righe ordine", sb);
                LogDiagnostica.Scarica(logMessage, bulk: sb);

                if (rigaNuovaNumeratore)
                {
                    // Riga del nuovo anno fiscale del numeratore aggiunta con context.Add:
                    // SaveChanges la inserisce dentro la stessa transazione.
                    _context.SaveChanges();
                }
            }, logMessage);

            if (!esito)
                return null;   // errori già emessi a log dal rollback di ExecuteInBulkTransaction

            LogDiagnostica.Informativo(logMessage,
                $"Ordine creato: {numeroOrdineCreato} (SaleOrdId {saleOrdIdCreato}), {righe.Count} riga/e, cliente {cliente}.");
            return new EsitoCreazioneOrdine(saleOrdIdCreato, numeroOrdineCreato, righe.Count);
        }

        /// <summary>Controlli di forma della richiesta (i valori sono già tipizzati dai record).</summary>
        private static StringBuilder ValidaRequest(CreazioneOrdineRequest request)
        {
            var errori = new StringBuilder();

            if (request == null)
            {
                errori.AppendLine("Richiesta di creazione ordine nulla.");
                return errori;
            }

            if (string.IsNullOrWhiteSpace(request.Cliente))
                errori.AppendLine("Cliente mancante.");
            else if (request.Cliente.Trim().Length > 12)
                errori.AppendLine("Codice cliente troppo lungo (max 12 caratteri).");

            if (request.DataOrdine.Year < 1900 || request.DataOrdine > DateTime.Today.AddYears(2))
                errori.AppendLine($"Data ordine '{request.DataOrdine:dd/MM/yyyy}' fuori dal range ammesso.");

            if (!string.IsNullOrWhiteSpace(request.ClienteFatturazione) && request.ClienteFatturazione.Trim().Length > 12)
                errori.AppendLine("Codice cliente di fatturazione troppo lungo (max 12 caratteri).");
            if (!string.IsNullOrWhiteSpace(request.Pagamento) && request.Pagamento.Trim().Length > 8)
                errori.AppendLine("Codice pagamento troppo lungo (max 8 caratteri).");
            if (!string.IsNullOrWhiteSpace(request.Listino) && request.Listino.Trim().Length > 8)
                errori.AppendLine("Codice listino troppo lungo (max 8 caratteri).");
            if (!string.IsNullOrWhiteSpace(request.Valuta) && request.Valuta.Trim().Length > 8)
                errori.AppendLine("Codice valuta troppo lungo (max 8 caratteri).");
            if (!string.IsNullOrWhiteSpace(request.Sede) && request.Sede.Trim().Length > 32)
                errori.AppendLine("Sede (riferimento nostro) troppo lunga (max 32 caratteri).");
            if (!string.IsNullOrWhiteSpace(request.RegistroIva) && request.RegistroIva.Trim().Length > 8)
                errori.AppendLine("Registro IVA troppo lungo (max 8 caratteri).");

            if (request.Righe == null || request.Righe.Count == 0)
            {
                errori.AppendLine("L'ordine deve contenere almeno una riga.");
                return errori;
            }

            int numeroRiga = 0;
            foreach (RigaOrdineCreazione r in request.Righe)
            {
                numeroRiga++;
                if (string.IsNullOrWhiteSpace(r.Descrizione) && string.IsNullOrWhiteSpace(r.Item))
                    errori.AppendLine($"Riga {numeroRiga}: né descrizione né articolo.");
                if (r.Quantita < 0)
                    errori.AppendLine($"Riga {numeroRiga}: quantità negativa ({r.Quantita}).");
                if (r.ValoreUnitario < 0)
                    errori.AppendLine($"Riga {numeroRiga}: valore unitario negativo ({r.ValoreUnitario}).");
                if (!string.IsNullOrWhiteSpace(r.Descrizione) && r.Descrizione.Trim().Length > 128)
                    errori.AppendLine($"Riga {numeroRiga}: descrizione troppo lunga (max 128 caratteri).");
                if (!string.IsNullOrWhiteSpace(r.Item) && r.Item.Trim().Length > 21)
                    errori.AppendLine($"Riga {numeroRiga}: codice articolo troppo lungo (max 21 caratteri).");
                if (!string.IsNullOrWhiteSpace(r.UoM) && r.UoM.Trim().Length > 8)
                    errori.AppendLine($"Riga {numeroRiga}: unità di misura troppo lunga (max 8 caratteri).");
                if (!string.IsNullOrWhiteSpace(r.CodIva) && r.CodIva.Trim().Length > 4)
                    errori.AppendLine($"Riga {numeroRiga}: codice IVA troppo lungo (max 4 caratteri).");
                if (!string.IsNullOrWhiteSpace(r.Commessa) && r.Commessa.Trim().Length > 10)
                    errori.AppendLine($"Riga {numeroRiga}: commessa troppo lunga (max 10 caratteri).");
                if (!string.IsNullOrWhiteSpace(r.CentroCosto) && r.CentroCosto.Trim().Length > 8)
                    errori.AppendLine($"Riga {numeroRiga}: centro costo troppo lungo (max 8 caratteri).");
                if (!string.IsNullOrWhiteSpace(r.CIG) && r.CIG.Trim().Length > 15)
                    errori.AppendLine($"Riga {numeroRiga}: CIG troppo lungo (max 15 caratteri).");
                if (!string.IsNullOrWhiteSpace(r.CUP) && r.CUP.Trim().Length > 16)
                    errori.AppendLine($"Riga {numeroRiga}: CUP troppo lungo (max 16 caratteri).");
                if (!string.IsNullOrWhiteSpace(r.Contropartita) && r.Contropartita.Trim().Length > 16)
                    errori.AppendLine($"Riga {numeroRiga}: contropartita troppo lunga (max 16 caratteri).");
                if (!string.IsNullOrWhiteSpace(r.Nota) && r.Nota.Trim().Length > 32)
                    errori.AppendLine($"Riga {numeroRiga}: nota troppo lunga (max 32 caratteri).");
            }

            return errori;
        }

        /// <summary>
        /// Alloca il prossimo valore di un contatore Mago con UPDATE...OUTPUT: l'incremento e la
        /// lettura avvengono in un unico comando, così due creazioni concorrenti non possono
        /// ottenere lo stesso ID. Le tabelle/colonne sono quelle reali Mago (non tipizzate).
        /// </summary>
        private int AllocaProgressivoTabella(string tabella, string colonna, string colonnaChiave, int valoreChiave, string descrizione)
        {
            DbConnection conn = _context.Database.GetDbConnection();
            bool apertaDaNoi = conn.State != ConnectionState.Open;
            if (apertaDaNoi) conn.Open();
            try
            {
                using DbCommand cmd = conn.CreateCommand();
                cmd.Transaction = _context.Database.CurrentTransaction?.GetDbTransaction();
                cmd.CommandText = $"UPDATE {tabella} SET {colonna} = ISNULL({colonna}, 0) + 1 OUTPUT INSERTED.{colonna} WHERE {colonnaChiave} = @chiave";

                DbParameter par = cmd.CreateParameter();
                par.ParameterName = "@chiave";
                par.Value = valoreChiave;
                cmd.Parameters.Add(par);

                object? risultato = cmd.ExecuteScalar();
                if (risultato == null || risultato == DBNull.Value)
                    throw new InvalidOperationException($"{descrizione} ({tabella}, {colonnaChiave}={valoreChiave}) assente: impossibile allocare un ID ordine.");
                return Convert.ToInt32(risultato);
            }
            finally
            {
                if (apertaDaNoi) conn.Close();
            }
        }

        /// <summary>
        /// Alloca il progressivo di numerazione dell'ordine (MA_NonFiscalNumbers, CodeType OrdCli)
        /// per l'anno fiscale della data ordine e restituisce il numero formattato "yy/NNNNN".
        /// Se la riga dell'anno non esiste (primo ordine dell'anno) la crea clonando la più recente
        /// come template: la riga viene tracciata sul context e inserita dal SaveChanges finale
        /// dentro la stessa transazione.
        /// </summary>
        private string AllocaNumeroOrdine(DateTime dataOrdine, out bool aggiuntaRigaNumeratore)
        {
            aggiuntaRigaNumeratore = false;
            int anno = dataOrdine.Year;
            int progressivo;

            DbConnection conn = _context.Database.GetDbConnection();
            bool apertaDaNoi = conn.State != ConnectionState.Open;
            if (apertaDaNoi) conn.Open();
            try
            {
                using DbCommand cmd = conn.CreateCommand();
                cmd.Transaction = _context.Database.CurrentTransaction?.GetDbTransaction();
                cmd.CommandText = @"UPDATE MA_NonFiscalNumbers
                                    SET LastDocNo = ISNULL(LastDocNo, 0) + 1, LastDocDate = @data
                                    OUTPUT INSERTED.LastDocNo
                                    WHERE BalanceYear = @anno AND CodeType = @codeType";

                DbParameter pData = cmd.CreateParameter();
                pData.ParameterName = "@data";
                pData.Value = dataOrdine;
                cmd.Parameters.Add(pData);

                DbParameter pAnno = cmd.CreateParameter();
                pAnno.ParameterName = "@anno";
                pAnno.Value = (short)anno;
                cmd.Parameters.Add(pAnno);

                DbParameter pTipo = cmd.CreateParameter();
                pTipo.ParameterName = "@codeType";
                pTipo.Value = (int)CodeType.OrdCli;
                cmd.Parameters.Add(pTipo);

                object? risultato = cmd.ExecuteScalar();
                if (risultato == null || risultato == DBNull.Value)
                {
                    MaNonFiscalNumbers? template = _context.MaNonFiscalNumbers.AsNoTracking()
                        .Where(n => n.CodeType == (int)CodeType.OrdCli)
                        .OrderByDescending(n => n.BalanceYear)
                        .FirstOrDefault();
                    if (template == null)
                        throw new InvalidOperationException("Numeratore ordini (MA_NonFiscalNumbers) assente: impossibile numerare l'ordine.");

                    MaNonFiscalNumbers rigaNuova = template.CloneFromPrototype();
                    rigaNuova.BalanceYear = (short)anno;
                    rigaNuova.LastDocNo = 1;
                    rigaNuova.LastDocDate = dataOrdine;
                    rigaNuova.Tbcreated = _now;
                    rigaNuova.Tbmodified = _now;
                    rigaNuova.TbcreatedId = _loginId;
                    rigaNuova.TbmodifiedId = _loginId;
                    _context.Add(rigaNuova);

                    aggiuntaRigaNumeratore = true;
                    progressivo = 1;
                }
                else
                {
                    progressivo = Convert.ToInt32(risultato);
                }
            }
            finally
            {
                if (apertaDaNoi) conn.Close();
            }

            return FormattaNumeroOrdine(anno, progressivo);
        }

        /// <summary>Formato numerazione osservato su tutti gli ordini reali (es. "26/00116").</summary>
        private static string FormattaNumeroOrdine(int anno, int progressivo)
            => $"{anno % 100:D2}/{progressivo:D5}";
    }
}
