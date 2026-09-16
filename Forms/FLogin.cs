using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using FatturazioneContratti.Config;
using FatturazioneContratti.Data;
using FatturazioneContratti.Invoicing;
using FatturazioneContratti.Orders;

namespace FatturazioneContratti.Forms
{
    public partial class FLogin : Form
    {
        public FLogin()
        {
            InitializeComponent();
        }

        private void FLogin_Load(object sender, EventArgs e)
        {
            txtServer.Text = AppConfig.SqlServer;
            txtUser.Text = AppConfig.UserId;
            txtPassword.Text = AppConfig.Password;
            txtLoginId.Text = AppConfig.LoginId.ToString();
            chkTestDb.Checked = AppConfig.IsTestDb;

            UpdateAziendaStatus();
        }

        private void btnTestConnection_Click(object sender, EventArgs e)
        {
            SyncConfigFromUI();
            try
            {
                using var conn = SqlTools.OpenDirectConnection();
                Log($"Connessione riuscita a: {conn.Database} (Server: {conn.DataSource})");
                MessageBox.Show("Connessione stabilita con successo!", "Connessione", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                Log($"ERRORE Connessione: {ex.Message}");
                MessageBox.Show($"Impossibile connettersi: {ex.Message}", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void rdoAzienda_CheckedChanged(object sender, EventArgs e)
        {
            AppConfig.IsDbUno = rdoUno.Checked;
            UpdateAziendaStatus();
        }

        private void chkTestDb_CheckedChanged(object sender, EventArgs e)
        {
            AppConfig.IsTestDb = chkTestDb.Checked;
            UpdateAziendaStatus();
        }

        private void UpdateAziendaStatus()
        {
            lblDbInUso.Text = $"Database attivo: {AppConfig.DatabaseInUse}";
            lblDbInUso.ForeColor = AppConfig.IsTestDb ? Color.DarkOrange : Color.DarkGreen;
        }

        private void SyncConfigFromUI()
        {
            AppConfig.SqlServer = txtServer.Text.Trim();
            AppConfig.UserId = txtUser.Text.Trim();
            AppConfig.Password = txtPassword.Text;
            AppConfig.LoginId = int.TryParse(txtLoginId.Text.Trim(), out int id) ? id : 1;
            AppConfig.IsTestDb = chkTestDb.Checked;
            AppConfig.IsDbUno = rdoUno.Checked;
            AppConfig.IsDebugging = chkDebug.Checked;
        }

        // --- PROCESSO 1: GENERAZIONE RIGHE ORDINE (CANONI) ---
        private void btnGeneraOrdini_Click(object sender, EventArgs e)
        {
            SyncConfigFromUI();

            using var formFiltri = new FAskFiltriOrdini { IsIstat = false };
            if (formFiltri.ShowDialog() != DialogResult.OK) return;

            var filtri = formFiltri.OttieniFiltri();

            EseguiOperazione("Generazione Canoni da Ordini", () =>
            {
                using var ctx = SqlTools.CreateContext();
                var service = new OrderGenerationService(ctx, AppConfig.LoginId);

                return service.GeneraRigheOrdine(
                    filtri,
                    logMessage: Log,
                    updateStatus: AggiornaStato,
                    advanceProgress: AvanzaProgress);
            });
        }

        // --- PROCESSO 2: ADEGUAMENTO ISTAT ORDINI ---
        private void btnAdeguamentoIstat_Click(object sender, EventArgs e)
        {
            SyncConfigFromUI();

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
                    advanceProgress: AvanzaProgress);
            });
        }

        // --- PROCESSO 3: CONSUNTIVAZIONE FLUSSO INTEGRA & FATTURE ---
        private void btnConsuntivi_Click(object sender, EventArgs e)
        {
            SyncConfigFromUI();

            using var formFiltri = new FAskFiltriOrdiniConsuntivo();
            if (formFiltri.ShowDialog() != DialogResult.OK) return;

            var filtri = formFiltri.OttieniFiltri();

            EseguiOperazione("Consuntivazione Flusso Integra", () =>
            {
                using var ctx = SqlTools.CreateContext();
                var service = new ConsuntivoService(ctx, AppConfig.LoginId);

                Log("--- Step 1: Controllo Corrispondenze Flusso Integra ---");
                bool checkOk = service.ControllaFlussoIntegra(filtri, Log, AggiornaStato, AvanzaProgress);

                if (!checkOk)
                {
                    Log("Controllo flusso terminato con incongruenze. Interruzione generazione fatture.");
                    return false;
                }

                Log("--- Step 2: Generazione Fatture a Consuntivo ---");
                return service.GeneraFattureConsuntivo(filtri, Log, AggiornaStato, AvanzaProgress);
            });
        }

        // --- PROCESSO 4: ALLINEAMENTO DATE CANONI FATTURE DA ORDINI ---
        private void btnAllineaFattureOrdini_Click(object sender, EventArgs e)
        {
            SyncConfigFromUI();

            DateTime fineMese = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month));
            DateTime inizioAnno = new DateTime(DateTime.Today.Year, 1, 1);

            EseguiOperazione("Allineamento Canoni Fatture", () =>
            {
                using var conn = SqlTools.OpenDirectConnection();
                using var trans = conn.BeginTransaction();

                bool ok = FattureDaOrdiniService.AdeguaFattureDaOrdini(
                    conn,
                    trans,
                    inizioAnno,
                    fineMese,
                    nrFirst: string.Empty,
                    nrLast: string.Empty,
                    allNumbers: true,
                    loginId: AppConfig.LoginId,
                    out string msg);

                Log(msg);

                if (ok) trans.Commit();
                else trans.Rollback();

                return ok;
            });
        }

        // --- METODI DI SUPPORTO UI ---
        private void EseguiOperazione(string nomeProcesso, Func<bool> azione)
        {
            Cursor = Cursors.WaitCursor;
            progressBar.Value = 0;
            Log($"=== AVVIO: {nomeProcesso} ({DateTime.Now:dd/MM/yyyy HH:mm:ss}) ===");

            try
            {
                bool esito = azione();
                Log(esito ? $"=== COMPLETATO CON SUCCESSO: {nomeProcesso} ===" : $"=== COMPLETATO CON ERRORI: {nomeProcesso} ===");
            }
            catch (Exception ex)
            {
                Log($"ERRORE CRITICO in {nomeProcesso}: {ex.Message}");
                MessageBox.Show(ex.Message, "Errore di esecuzione", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                progressBar.Value = 0;
                Cursor = Cursors.Default;
            }
        }

        private void Log(string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return;
            lstStato.Items.Add($"[{DateTime.Now:HH:mm:ss}] {message}");
            lstStato.TopIndex = lstStato.Items.Count - 1;
            Application.DoEvents();
        }

        private void AggiornaStato(string status)
        {
            lblStatus.Text = status;
            Application.DoEvents();
        }

        private void AvanzaProgress()
        {
            if (progressBar.Value < progressBar.Maximum)
                progressBar.Value += 1;
            else
                progressBar.Value = 0;
            Application.DoEvents();
        }

        private void btnPulisciLog_Click(object sender, EventArgs e) => lstStato.Items.Clear();
    }
}