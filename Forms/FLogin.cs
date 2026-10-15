using FatturazioneContratti.Invoicing;
using FatturazioneContratti.Orders;
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
        private readonly object _logLock = new object();
        private bool _configUnlocked = false; 

        private void esciToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void configurazioneAvanzataToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void apriJsonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AppConfig.ApriFileJson();
        }

        private void apriCartellaSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AppConfig.ApriCartellaImpostazioni();
        }

        private void apriCartellaLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AppConfig.ApriCartellaLog();
        }

        private void pagheToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Modulo Paghe in fase di migrazione.", "Modulo Paghe", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void informazioniToolStripMenuItem_Click(object sender, EventArgs e)
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
        private void btnGeneraOrdini_Click(object sender, EventArgs e)
        {           
            AggiornaStato("Elaborazione in corso...");

            using var formFiltri = new FAskFiltriOrdini { IsIstat = false };
            if (formFiltri.ShowDialog() != DialogResult.OK) return;

            var filtri = formFiltri.OttieniFiltri();

            EseguiOperazione("Generazione Canoni da Ordini", () =>
            {
                using var ctx = SqlTools.CreateContext();
                var service = new OrderGenerationService(ctx, AppConfig.LoginId);

                return service.GeneraRigheOrdine(new GeneraRigheOrdineOptions(
                    filtri,
                    LogMessage: Log,
                    UpdateStatus: AggiornaStato,
                    ProgressBlocchi: CreaProgress(_progressMain),
                    ProgressOrdini: CreaProgress(_progressSub)));
            });
        }

        // --- PROCESSO 2: ADEGUAMENTO ISTAT ORDINI ---
        private void btnAdeguamentoIstat_Click(object sender, EventArgs e)
        {
            using var formFiltri = new FAskFiltriOrdini { IsIstat = true };
            if (formFiltri.ShowDialog() != DialogResult.OK) return;

            var filtri = formFiltri.OttieniFiltri();
            if (filtri.PercIstat == 0)
            {
                MessageBox.Show("Specificare una percentuale ISTAT valida.", "Attenzione", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            EseguiOperazione("Adeguamento ISTAT", () =>
            {
                using var ctx = SqlTools.CreateContext();
                var service = new OrderGenerationService(ctx, AppConfig.LoginId);

                return service.AdeguaIstatOrdine(
                    filtri,
                    logMessage: Log,
                    updateStatus: AggiornaStato,
                    progress1: CreaProgress(_progressMain),
                    progress2: CreaProgress(_progressSub));
            });
        }

        // --- PROCESSO 3: CONSUNTIVAZIONE FLUSSO INTEGRA & FATTURE ---
        private void btnConsuntivi_Click(object sender, EventArgs e)
        {
            using var formFiltri = new FAskFiltriOrdiniConsuntivo();
            if (formFiltri.ShowDialog() != DialogResult.OK) return;

            var filtri = formFiltri.OttieniFiltri();

            EseguiOperazione("Consuntivazione Flusso Integra", () =>
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
                        CreaProgress(_progressSub)
                        );

                    if (!checkOk)
                    {
                        Log("Controllo flusso terminato con incongruenze. Interruzione generazione fatture.");
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
                        CreaProgress(_progressSub));
                }

                Log("--- Step 2: Saltato (Solo Controllo selezionato) ---");
                return checkOk;
            });
        }

        // --- PROCESSO 4: ALLINEAMENTO DATE CANONI FATTURE DA ORDINI ---
        private void btnAllineaFattureOrdini_Click(object sender, EventArgs e)
        {
            using var formFiltri = new FAskFiltriAllineamentoFatture();
            if (formFiltri.ShowDialog(this) != DialogResult.OK) return;

            var filtri = formFiltri.OttieniFiltri();

            EseguiOperazione("Allineamento Canoni Fatture", () =>
            {
                using var conn = SqlTools.OpenDirectConnection();
                using var trans = conn.BeginTransaction();

                bool ok = FattureDaOrdiniService.AdeguaFattureDaOrdini(
                    conn,
                    trans,
                    filtri,
                    loginId: AppConfig.LoginId,
                    out string msg);

                Log(msg);

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

        private void EseguiOperazione(string nomeProcesso, Func<bool> azione)
        {
            if (_inElaborazione) return;   // protezione extra contro doppi avvii

            // UI iniziale
            SetUiBusy(true);
            lnkApriLog.Visible = false;   // via il link della sessione precedente
            Log($"=== AVVIO: {nomeProcesso} ({DateTime.Now:dd/MM/yyyy HH:mm:ss}) ===");

            // Esegui il lavoro su background per non bloccare la UI
            System.Threading.Tasks.Task.Run(() =>
            {
                bool esito = false;
                try
                {
                    esito = azione();

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
                    // Ripristina UI sul thread UI
                    PostToUi(() => SetUiBusy(false));   // gestisce già il marshaling
                }
            });
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
        /// Sovraccarico compatibile con Action<string, AppConfig.LogTarget> se i servizi vengono aggiornati.
        /// </summary>
        public void Log(string message, AppConfig.LogTarget destination)
        {
            if (string.IsNullOrWhiteSpace(message)) return;

            string timed = $"[{DateTime.Now:HH:mm:ss}] {message}";

            // Se serve scrivere su UI ma siamo in thread non-UI => marshaling
            if ((destination == AppConfig.LogTarget.Ui || destination == AppConfig.LogTarget.Both) && InvokeRequired)
            {
                BeginInvoke(new Action(() => Log(message, destination)));
                return;
            }

            // Scrive su UI
            if (destination == AppConfig.LogTarget.Ui || destination == AppConfig.LogTarget.Both)
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
            if (destination == AppConfig.LogTarget.File || destination == AppConfig.LogTarget.Both)
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

        /// <summary>
        /// Helper compatibile con Action<string> per forzare UI (utile quando passi il delegato ai servizi).
        /// </summary>
            public void LogToUi(string message) => Log(message, AppConfig.LogTarget.Ui);

        /// <summary>
        /// Helper compatibile con Action<string> per forzare file (utile quando passi il delegato ai servizi).
        /// </summary>
        public void LogToFile(string message) => Log(message, AppConfig.LogTarget.File);

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

        private void btnPulisciStato_Click(object sender, EventArgs e) => lstStato.Items.Clear();

        private void lnkApriLog_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
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