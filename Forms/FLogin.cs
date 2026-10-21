using FatturazioneContratti.Invoicing;
using FatturazioneContratti.Orders;
using FatturazioneContratti.Common;
using FatturazioneContratti.Config;
using FatturazioneContratti.Data;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;

namespace FatturazioneContratti.Forms
{
    public partial class FLogin : Form
    {
        private const string AdminPassword = "123456";
        // campi nella classe FLogin
        private UiProgress _progressMain;
        private UiProgress _progressSub;
        //private UiProgress? _progressSubChild;

        // nuovi campi per il logging su file
        private string _logFilePath = string.Empty;
        private readonly object _logLock = new();
        private bool _configUnlocked = false; 

        private void EsciToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ConfigurazioneAvanzataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!_configUnlocked && !FPasswordPrompt.ChiediPassword(this, AdminPassword))
            {
                MessageBox.Show("Password non valida.", "Accesso Negato", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _configUnlocked = true;
            this.BackColor = System.Drawing.Color.FromArgb(230, 245, 230);
            menuStripPrincipal.BackColor = System.Drawing.Color.FromArgb(200, 235, 200);
            lstStato.BackColor = System.Drawing.Color.FromArgb(240, 250, 240);
            this.Text = "FatturazioneContratti (configurazione sbloccata)";
            try { apriJsonToolStripMenuItem.Visible = true; } catch { }
            try { apriCartellaSettingsToolStripMenuItem.Visible = true; } catch { }
            AggiornaStato("Configurazione avanzata sbloccata.");
            Log("Configurazione avanzata sbloccata.");

            using var fSettings = new FSettings();
            if (fSettings.ShowDialog(this) == DialogResult.OK)
            {
                UpdateAziendaStatus();
                Log("Configurazione DB e percorsi salvata correttamente.");
            }
        }

        private void ApriJsonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AppConfig.ApriFileJson();
        }

        private void ApriCartellaSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AppConfig.ApriCartellaImpostazioni();
        }

        private void ApriCartellaLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AppConfig.ApriCartellaLog();
        }

        private void PagheToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Modulo Paghe in fase di migrazione.", "Modulo Paghe", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void InformazioniToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string versione = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "?";
            MessageBox.Show(
                $"FatturazioneContratti{Environment.NewLine}Versione {versione}{Environment.NewLine}{Environment.NewLine}" +
                $"Database attivo: {AppConfig.DatabaseInUse}{Environment.NewLine}" +
                "Automazione fatturazione contratti di servizio su Mago.net.",
                "Informazioni su FatturazioneContratti",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        public FLogin()
        {
            InitializeComponent();
            // Usa l'icona dell'eseguibile come icona della form
            try { this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath); }
            catch { }

            // Log "colorato" e raggomitolato: OwnerDrawVariable per andare a capo sulle
            // righe lunghe (MeasureItem calcola l'altezza in base al testo) e per
            // colorare la riga in base al tag emesso da LogDiagnostica ([ERRORE]/[AVVISO]).
            // Il ListBox resta identico come uso: Items.Add / TopIndex / Clear non cambiano.
            // L'altezza viene calcolata UNA volta, quando la riga entra nel log: nessun
            // ricalcolo al resize (il vecchio toggle/rimisura causava crash e loop di
            // repaint, 21/10/2026). Se la finestra viene allargata, le righe lunghe
            // semplicemente non si redistribuiscono: il log è temporaneo, va bene così.            lstStato.DrawMode = DrawMode.OwnerDrawVariable;
            lstStato.MeasureItem += LstStato_MeasureItem;
            lstStato.DrawItem += LstStato_DrawItem;
        }

        private void FLogin_Load(object sender, EventArgs e)
        {
            // 1. Carica da JSON
            AppConfig.Load();
                        
            UpdateAziendaStatus();
            // Le voci per aprire JSON/cartella impostazioni restano disponibili solo dal dialog FSettings
            try { apriJsonToolStripMenuItem.Visible = false; } catch { }
            try { apriCartellaSettingsToolStripMenuItem.Visible = false; } catch { }
            InizializzaControlliBloccabili();

            // inizializza (ad esempio in FLogin_Load o lazy al primo uso)
            _progressMain = new UiProgress(progressBar);
            _progressSub = new UiProgress(subProgressBar);
            //_progressSubChild = new UiProgress(subProgressBar3);

            // Inizializza logging su file per la sessione
            try
            {
                string logsDir;
                if (!string.IsNullOrWhiteSpace(AppConfig.LastLogPath) && Directory.Exists(AppConfig.LastLogPath))
                {
                    logsDir = AppConfig.LastLogPath;
                }
                else
                {
                    logsDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Brovarone Cristiano", "Logs");
                    Directory.CreateDirectory(logsDir);
                }

                _logFilePath = Path.Combine(logsDir, $"FatturazioneContratti_{DateTime.Now:yyyyMMdd_HHmmss}.log");

                // Assicura che il file esista fin dall'avvio, così il comando "Apri log" lo trova sempre
                if (!File.Exists(_logFilePath))
                    File.WriteAllText(_logFilePath, "");

                // salva la cartella log in configurazione per eventuale accesso futuro
                AppConfig.LastLogPath = logsDir;
                AppConfig.Save();

                // indico nel log che il file è pronto (rispetta modalità di log)
                //Log($"Log file: {_logFilePath}");
                //Log($"Modalità log corrente: {AppConfig.LogMode}");
            }
            catch
            {
                // Non bloccare l'app se il logging su file fallisce; resta il log su UI
            }
        }                              

        private void UpdateAziendaStatus()
        {
            lblDbInUso.Text = $"Database attivo: {AppConfig.DatabaseInUse}";
            lblDbInUso.ForeColor = AppConfig.IsTestDb ? Color.DarkOrange : Color.DarkGreen;
        }

        // --- PROCESSO 1: GENERAZIONE RIGHE ORDINE (CANONI) ---
        private void BtnGeneraOrdini_Click(object sender, EventArgs e)
        {           
            AggiornaStato("Elaborazione in corso...");

            using var formFiltri = new FAskFiltriOrdini { IsIstat = false };
            if (formFiltri.ShowDialog() != DialogResult.OK) return;

            var filtri = formFiltri.OttieniFiltri();

            EseguiOperazione("Generazione Canoni da Ordini", (CancellationToken token) =>
            {
                using var ctx = SqlTools.CreateContext();
                var service = new GenerazioneRigheOrdiniService(ctx, AppConfig.LoginId);

                return service.GeneraRigheOrdine(new GeneraRigheOrdineOptions(
                    filtri,
                    LogMessage: Log,
                    UpdateStatus: AggiornaStato,
                    ProgressBlocchi: CreaProgress(_progressMain),
                    ProgressOrdini: CreaProgress(_progressSub)), token);
            });
        }

        // --- PROCESSO 2: ADEGUAMENTO ISTAT ORDINI ---
        private void BtnAdeguamentoIstat_Click(object sender, EventArgs e)
        {
            using var formFiltri = new FAskFiltriOrdini { IsIstat = true };
            if (formFiltri.ShowDialog() != DialogResult.OK) return;

            var filtri = formFiltri.OttieniFiltri();
            if (filtri.PercIstat == 0)
            {
                MessageBox.Show("Specificare una percentuale ISTAT valida.", "Attenzione", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            EseguiOperazione("Adeguamento ISTAT", (CancellationToken token) =>
            {
                using var ctx = SqlTools.CreateContext();
                var service = new GenerazioneRigheOrdiniService(ctx, AppConfig.LoginId);

                return service.AdeguaIstatOrdine(
                    filtri,
                    logMessage: Log,
                    updateStatus: AggiornaStato,
                    progress1: CreaProgress(_progressMain),
                    progress2: CreaProgress(_progressSub),
                    token: token);
            });
        }

        // --- PROCESSO 3: CONSUNTIVAZIONE FLUSSO INTEGRA & FATTURE ---
        private void BtnConsuntivi_Click(object sender, EventArgs e)
        {
            using var formFiltri = new FAskFiltriOrdiniConsuntivo();
            if (formFiltri.ShowDialog() != DialogResult.OK) return;

            var filtri = formFiltri.OttieniFiltri();

            EseguiOperazione("Consuntivazione Flusso Integra", (CancellationToken token) =>
            {
                using var ctx = SqlTools.CreateContext();
                var service = new ConsuntivoService(ctx, AppConfig.LoginId);

                bool checkOk = true;

                // Step 1: Controllo (se richiesto)
                if (filtri.Azione == ActionMode.SoloControllo || filtri.Azione == ActionMode.ControlloEFattura)
                {
                    Log("--- Step 1: Controllo Corrispondenze Flusso Integra ---");
                    checkOk = service.ControllaFlussoIntegra(
                        filtri,
                        Log,
                        AggiornaStato,
                        CreaProgress(_progressMain),
                        CreaProgress(_progressSub),
                        token
                        );

                    if (!checkOk)
                    {
                        // Interruzione: è un esito negativo, quindi va su UI e file
                        // anche quando la modalità log è impostata solo su file.
                        Log("Controllo flusso terminato con incongruenze. Interruzione generazione fatture.",
                            LogTarget.Both);
                        return false;
                    }
                }
                else
                {
                    Log("--- Step 1: Saltato (Solo Fatturazione selezionato) ---");
                }

                // Step 2: Fatturazione (se richiesto)
                if (filtri.Azione == ActionMode.ControlloEFattura || filtri.Azione == ActionMode.SoloFattura)
                {
                    Log("--- Step 2: Generazione Fatture a Consuntivo ---");
                    return service.GeneraFattureConsuntivo(
                        filtri,
                        Log,
                        AggiornaStato,
                        CreaProgress(_progressMain),
                        CreaProgress(_progressSub),
                        token);
                }

                Log("--- Step 2: Saltato (Solo Controllo selezionato) ---");
                return checkOk;
            });
        }

        // --- PROCESSO 4: ALLINEAMENTO DATE CANONI FATTURE DA ORDINI ---
        private void BtnAllineaFattureOrdini_Click(object sender, EventArgs e)
        {
            using var formFiltri = new FAskFiltriAllineamentoFatture();
            if (formFiltri.ShowDialog(this) != DialogResult.OK) return;

            var filtri = formFiltri.OttieniFiltri();

            EseguiOperazione("Allineamento Canoni Fatture", (CancellationToken token) =>
            {
                using var conn = SqlTools.OpenDirectConnection();
                using var trans = conn.BeginTransaction();

                bool ok = AggiornamentoFattureService.AdeguaFattureDaOrdini(
                    conn,
                    trans,
                    filtri,
                    loginId: AppConfig.LoginId,
                    logMessage: Log,
                    token: token);

                if (ok) trans.Commit();
                else trans.Rollback();

                return ok;
            });
        }

        // --- METODI DI SUPPORTO UI ---
        /// <summary>
        /// Factory centralizzata per i callback di avanzamento legati alle barre UI.
        /// Evita di ripetere la costruzione di <see cref="ProgressCallbacks"/> in ogni workflow.
        /// </summary>
        private static ProgressCallbacks CreaProgress(UiProgress? barra) => new(
            (min, max) => barra?.Init(min, max),
            () => barra?.Advance(),
            () => barra?.Finish());

        private void EseguiOperazione(string nomeProcesso, Func<CancellationToken, bool> azione)
        {
            if (_inElaborazione) return;   // protezione extra contro doppi avvii

            // CTS dedicata alla run corrente: il pulsante Interrompi annulla questa.
            var cts = new CancellationTokenSource();
            _ctsCurrent = cts;

            // UI iniziale
            SetUiBusy(true);
            lnkApriLog.Visible = false;   // via il link della sessione precedente

            // Reset della label di stato: senza questo restava rossa/di esito
            // della run precedente per tutta la nuova elaborazione.
            AggiornaStato("Elaborazione in corso...", Color.Black);

            Log($"=== {nomeProcesso} ({DateTime.Now:dd/MM/yyyy HH:mm:ss}) ===");
            // Nel file di sessione deve restare traccia del DB usato (produzione o test):
            // l'etichetta lo mostra, ma il log no, e in lettura è la prima cosa che serve.
            Log($"Database: {AppConfig.DatabaseInUse}{(AppConfig.IsTestDb ? " [TEST]" : "")}");

            // Esegui il lavoro su background per non bloccare la UI
            System.Threading.Tasks.Task.Run(() =>
            {
                bool esito = false;
                try
                {
                    esito = azione(cts.Token);

                    // Posta il risultato sul thread UI in modo sicuro
                    PostToUi(() =>
                    {
                        Log(esito ? $"=== COMPLETATO CON SUCCESSO: {nomeProcesso} ==="
                                  : $"=== COMPLETATO CON ERRORI: {nomeProcesso} ===");

                        // La label di stato deve mostrare l'esito finale: senza questo
                        // restava sull'ultimo messaggio intermedio del servizio
                        // (es. "Salvataggio ... in corso...") e non si capiva che aveva finito.
                        AggiornaStato(
                            esito ? $"Completato con successo: {nomeProcesso}"
                                  : $"Completato con errori: {nomeProcesso}",
                            esito ? Color.DarkGreen : Color.DarkRed);

                        // Link al file di log della sessione appena conclusa
                        lnkApriLog.Visible = true;
                    });
                }
                catch (OperationCanceledException)
                {
                    // Interruzione richiesta dall'utente: esito "interrotto", non errore critico.
                    PostToUi(() =>
                    {
                        LogDiagnostica.Avviso(Log, $"=== INTERROTTO DALL'UTENTE: {nomeProcesso} ===");
                        AggiornaStato($"Interrotto: {nomeProcesso}", Color.DarkOrange);
                        lnkApriLog.Visible = true;
                    });
                }
                catch (Exception ex)
                {
                    // Marshal dell'errore su UI in modo sicuro
                    PostToUi(() =>
                    {
                        Log($"ERRORE CRITICO in {nomeProcesso}: {ex.Message}");
                        AggiornaStato($"Errore critico in {nomeProcesso}: {ex.Message}", Color.DarkRed);
                        lnkApriLog.Visible = true;   // il log può contenere il dettaglio dell'errore
                        MessageBox.Show(ex.Message, "Errore di esecuzione", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    });
                }
                finally
                {
                    // Ripristina UI sul thread UI e rilascia la CTS della run
                    PostToUi(() =>
                    {
                        if (ReferenceEquals(_ctsCurrent, cts)) _ctsCurrent = null;
                        cts.Dispose();
                        SetUiBusy(false);   // gestisce già il marshaling
                    });
                }
            });
        }

        /// <summary>
        /// Richiesta di interruzione della run corrente: annulla il token e lascia che i
        /// servizi arrivino al confine di blocco, eseguendo l'eventuale rollback.
        /// </summary>
        private void BtnInterrompi_Click(object sender, EventArgs e)
        {
            var cts = _ctsCurrent;
            if (cts == null || cts.IsCancellationRequested) return;

            cts.Cancel();
            btnInterrompi.Enabled = false;   // una sola richiesta per run
            LogDiagnostica.Avviso(Log, "Richiesta di interruzione: attesa completamento/rollback...");
            AggiornaStato("Interruzione richiesta: attesa completamento...", Color.DarkOrange);
        }

        /// <summary>
        /// Scrive il log SOLO su UI (comportamento di default quando non viene specificata destinazione).
        /// Compatibile con Action<string> usato dai servizi.
        /// </summary>
        public void Log(string message)
        {
            // default: rispetta la modalità di log configurata (default = entrambi)
            Log(message, AppConfig.LogMode);
        }

        /// <summary>
        /// Scrive il log su UI / file / entrambi in base a <paramref name="destination"/>.
        /// Sovraccarico compatibile con Action<string, LogTarget> usato dai servizi.
        /// </summary>
        public void Log(string message, LogTarget destination)
        {
            if (string.IsNullOrWhiteSpace(message)) return;

            string timed = $"[{DateTime.Now:HH:mm:ss}] {message}";

            // Se serve scrivere su UI ma siamo in thread non-UI => marshaling
            if ((destination == LogTarget.Ui || destination == LogTarget.Both) && InvokeRequired)
            {
                BeginInvoke(new Action(() => Log(message, destination)));
                return;
            }

            // Scrive su UI
            if (destination == LogTarget.Ui || destination == LogTarget.Both)
            {
                try
                {
                    lstStato.Items.Add(timed);
                    lstStato.TopIndex = Math.Max(0, lstStato.Items.Count - 1);
                }
                catch
                {
                    // non bloccare l'app per errori UI
                }
            }

            // Scrive su file (se impostato)
            if (destination == LogTarget.File || destination == LogTarget.Both)
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(_logFilePath))
                    {
                        lock (_logLock)
                        {
                            File.AppendAllText(_logFilePath, $"{timed}{Environment.NewLine}");
                        }
                    }
                }
                catch
                {
                    // ignoro errori di scrittura su file
                }
            }
        }

        private void AggiornaStato(string status)
        {
            AggiornaStato(status, null);
        }

        private void AggiornaStato(string status, Color? colore)
        {
            if (IsDisposed || !IsHandleCreated) return;
            if (InvokeRequired)
            {
                // Marshal verso UI in modo sicuro
                try
                {
                    if (!IsDisposed && IsHandleCreated)
                        BeginInvoke(new Action(() => AggiornaStato(status, colore)));
                }
                catch { /* form smaltita */ }
                return;
            }

            lblStatus.Text = status;
            if (colore.HasValue) lblStatus.ForeColor = colore.Value;
        }        

        private void BtnPulisciStato_Click(object sender, EventArgs e) => lstStato.Items.Clear();

        /// <summary>
        /// Misura l'altezza della riga di log: il testo viene spezzato a capo sulla
        /// larghezza utile del ListBox (meno la eventuale scrollbar verticale) e la
        /// riga cresce di conseguenza. Righe corte restano a una sola altezza.
        /// </summary>
        private void LstStato_MeasureItem(object? sender, MeasureItemEventArgs e)
        {
            if (e.Index < 0 || e.Index >= lstStato.Items.Count) return;
            e.ItemHeight = CalcolaAltezzaRigaLog(lstStato.Items[e.Index]?.ToString() ?? string.Empty);
        }

        /// <summary>Larghezza utile per il testo: totale meno l'eventuale scrollbar verticale.</summary>
        private int LarghezzaUtileRigheLog()
            => Math.Max(24, lstStato.ClientSize.Width - SystemInformation.VerticalScrollBarWidth - 8);

        /// <summary>
        /// Altezza necessaria a mostrare la riga di log andando a capo (WordBreak):
        /// 1 riga per i messaggi corti, più righe per quelli lunghi. Almeno una riga.
        /// </summary>
        private int CalcolaAltezzaRigaLog(string testo)
        {
            var misura = TextRenderer.MeasureText(testo, lstStato.Font,
                new Size(LarghezzaUtileRigheLog(), int.MaxValue),
                TextFormatFlags.WordBreak | TextFormatFlags.NoPrefix | TextFormatFlags.Left);

            return Math.Max(lstStato.Font.Height + 2, misura.Height + 2);
        }

        /// <summary>
        /// Disegno personalizzato del log a video. Colori ad alto contrasto, con tinta
        /// di sfondo oltre al colore del testo:
        /// - [ERRORE]  → sfondo rosso chiaro, testo rosso scuro
        /// - [AVVISO]  → sfondo ambra chiaro, testo ambra scuro
        /// - altre righe → sfondo del controllo, testo normale
        /// Con la selezione si torna all'evidenziazione standard di Windows (HighlightText),
        /// così il testo resta sempre leggibile. Il testo va a capo (WordBreak).
        /// </summary>
        private void LstStato_DrawItem(object? sender, DrawItemEventArgs e)
        {
            if (e.Index < 0 || e.Index >= lstStato.Items.Count) return;

            string testo = lstStato.Items[e.Index]?.ToString() ?? string.Empty;

            bool errore = testo.IndexOf("[ERRORE]", StringComparison.OrdinalIgnoreCase) >= 0;
            bool avviso = !errore && testo.IndexOf("[AVVISO]", StringComparison.OrdinalIgnoreCase) >= 0;

            Color sfondo;
            Color colore;
            if (errore)
            {
                sfondo = Color.FromArgb(253, 231, 231);
                colore = Color.FromArgb(139, 0, 0);
            }
            else if (avviso)
            {
                sfondo = Color.FromArgb(255, 244, 206);
                colore = Color.FromArgb(122, 74, 0);
            }
            else
            {
                sfondo = lstStato.BackColor;
                colore = SystemColors.WindowText;
            }

            if (e.State.HasFlag(DrawItemState.Selected))
            {
                // Selezione: evidenziazione standard, testo del tema.
                e.DrawBackground();
                colore = SystemColors.HighlightText;
            }
            else
            {
                using var pennello = new SolidBrush(sfondo);
                e.Graphics.FillRectangle(pennello, e.Bounds);
            }

            var area = new Rectangle(e.Bounds.X + 2, e.Bounds.Y + 1,
                Math.Max(1, e.Bounds.Width - 4), Math.Max(1, e.Bounds.Height - 2));

            TextRenderer.DrawText(e.Graphics, testo, e.Font, area, colore,
                TextFormatFlags.Left | TextFormatFlags.Top
                | TextFormatFlags.WordBreak | TextFormatFlags.NoPrefix);

            if (e.State.HasFlag(DrawItemState.Focus)) e.DrawFocusRectangle();
        }

        /// <summary>
        /// Il ListBox in OwnerDrawVariable memorizza le altezze calcolate al momento
        /// dell'inserimento: NON si ricalcolano più al resize (il vecchio toggle di
        /// DrawMode ricreava l'handle → Win32Exception, e la rimisura con LB_SETITEMHEIGHT
        /// poteva innescare un loop di repaint con la scrollbar, 21/10/2026).
        /// </summary>
        private void LnkApriLog_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ApriFileLog();
        }

        /// <summary>
        /// Apre il file di log della sessione con l'editor associato all'estensione .log
        /// (associazione di sistema, es. Notepad). Se il file non è disponibile,
        /// ripiega sul Notepad e, in ultima istanza, apre la cartella dei log.
        /// </summary>
        private void ApriFileLog()
        {
            if (string.IsNullOrWhiteSpace(_logFilePath) || !File.Exists(_logFilePath))
            {
                MessageBox.Show(
                    "File di log della sessione non disponibile (scrittura su file non attiva). Apro la cartella dei log.",
                    "Apri log", MessageBoxButtons.OK, MessageBoxIcon.Information);
                AppConfig.ApriCartellaLog();
                return;
            }

            try
            {
                // UseShellExecute = rispetta l'editor di testo predefinito associato a .log
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = _logFilePath,
                    UseShellExecute = true
                });
            }
            catch (System.ComponentModel.Win32Exception)
            {
                // Nessuna applicazione associata a .log: ripiego sul Notepad di Windows
                try
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = "notepad.exe",
                        Arguments = _logFilePath,
                        UseShellExecute = true
                    });
                }
                catch (Exception exNotepad)
                {
                    MessageBox.Show($"Impossibile aprire il file di log: {exNotepad.Message}",
                        "Apri log", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    AppConfig.ApriCartellaLog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Impossibile aprire il file di log: {ex.Message}",
                    "Apri log", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                AppConfig.ApriCartellaLog();
            }
        }

        //  --- Controlli da bloccare durante un'elaborazione. Adatta i nomi a quelli reali della tua form.  --- 
        private Control[] _controlliDaBloccare;
        private volatile bool _inElaborazione;

        /// <summary>CTS della run in corso: creata in EseguiOperazione, annullata da "Interrompi".</summary>
        private CancellationTokenSource? _ctsCurrent;

        private void InizializzaControlliBloccabili()
        {
            _controlliDaBloccare = new Control[]
            {
                btnGeneraOrdini,
                btnAdeguamentoIstat,
                btnConsuntivi,
                btnAllineaFattureOrdini                
                // NON metto btnPulisciLog né lstStato: devono restare usabili
            };
        }

        private void PostToUi(Action action)
        {
            if (action == null) return;
            try
            {
                if (IsDisposed || !IsHandleCreated) return;
                if (InvokeRequired)
                {
                    BeginInvoke(action);
                }
                else
                {
                    action();
                }
            }
            catch (ObjectDisposedException)
            {
                // Form già smaltita: ignora.
            }
            catch (InvalidOperationException)
            {
                // Handle non più valido: ignora.
            }
        }

        private void SetUiBusy(bool busy)
        {
            if (IsDisposed || !IsHandleCreated) return;
            if (InvokeRequired)
            {
                try { Invoke(new Action(() => SetUiBusy(busy))); } catch { /* form smaltita */ }
                return;
            }

            _inElaborazione = busy;

            foreach (var c in _controlliDaBloccare)
                c.Enabled = !busy;

            // "Interrompi" è l'unico pulsante attivo durante l'elaborazione.
            btnInterrompi.Enabled = busy;

            menuStripPrincipal.Enabled = !busy;

            Cursor = busy ? Cursors.WaitCursor : Cursors.Default;

            if (!busy)
            {
                progressBar.Visible = false;
                subProgressBar.Visible = false;
            }
        }
    }
}