using System;
using System.Windows.Forms;
using FatturazioneContratti.Config;
using FatturazioneContratti.Data;

namespace FatturazioneContratti.Forms
{
    public partial class FSettings : Form
    {
        public FSettings()
        {
            InitializeComponent();
            try { this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath); }
            catch { }
        }

        private void FSettings_Load(object sender, EventArgs e)
        {
            // Carica valori attuali da AppConfig
            AppConfig.Load();

            // Connessione e flags
            txtServer.Text = AppConfig.SqlServer;
            txtUser.Text = AppConfig.UserId;
            txtPassword.Text = AppConfig.Password;
            txtLoginId.Text = AppConfig.LoginId.ToString();
            chkTestDb.Checked = AppConfig.IsTestDb;
            chkDebug.Checked = AppConfig.IsDebugging;
            if (AppConfig.IsDbUno) rdoUno.Checked = true; else rdoSpa.Checked = true;
            lblDbInUso.Text = $"Database attivo: {AppConfig.DatabaseInUse}";

            // Database logici e percorsi
            txtDbUno.Text = AppConfig.DatabaseUno;
            txtDbSpa.Text = AppConfig.DatabaseSpa;
            txtTmpDbUno.Text = AppConfig.TmpDatabaseUno;
            txtTmpDbSpa.Text = AppConfig.TmpDatabaseSpa;

            txtPathGenerale.Text = AppConfig.Path;
            txtPathFox.Text = AppConfig.FoxPath;
            txtPathPaghe.Text = AppConfig.PathPaghe;
            txtLastLogPath.Text = AppConfig.LastLogPath;

            if (cboLogMode.Items.Count == 0)
                cboLogMode.Items.AddRange(new object[] { "UI", "File", "Entrambi" });

            cboLogMode.SelectedIndex = AppConfig.LogMode switch
            {
                AppConfig.LogTarget.Ui => 0,
                AppConfig.LogTarget.File => 1,
                _ => 2
            };
        }

        private void BtnSfoglia_Click(object sender, EventArgs e)
        {
            using var fbd = new FolderBrowserDialog();
            if (sender == btnSfogliaGenerale && fbd.ShowDialog() == DialogResult.OK)
                txtPathGenerale.Text = fbd.SelectedPath;
            else if (sender == btnSfogliaFox && fbd.ShowDialog() == DialogResult.OK)
                txtPathFox.Text = fbd.SelectedPath;
            else if (sender == btnSfogliaPaghe && fbd.ShowDialog() == DialogResult.OK)
                txtPathPaghe.Text = fbd.SelectedPath;
            else if (sender == btnSfogliaLastLog && fbd.ShowDialog() == DialogResult.OK)
                txtLastLogPath.Text = fbd.SelectedPath;
        }

        private void BtnApriJson_Click(object sender, EventArgs e)
        {
            AppConfig.ApriFileJson();
        }

        private void BtnTestConnection_Click(object sender, EventArgs e)
        {
            // Sincronizza temporaneamente i valori dalla UI in AppConfig (non ancora salvati)
            AppConfig.SqlServer = txtServer.Text.Trim();
            AppConfig.UserId = txtUser.Text.Trim();
            AppConfig.Password = txtPassword.Text;
            AppConfig.LoginId = int.TryParse(txtLoginId.Text.Trim(), out int id) ? id : AppConfig.LoginId;
            AppConfig.IsTestDb = chkTestDb.Checked;
            AppConfig.IsDbUno = rdoUno.Checked;
            AppConfig.IsDebugging = chkDebug.Checked;

            try
            {
                using var conn = SqlTools.OpenDirectConnection();
                MessageBox.Show($"Connessione OK: {conn.Database} (Server: {conn.DataSource})", "Test Connessione", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Impossibile connettersi: {ex.Message}", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ChkTestDb_CheckedChanged(object sender, EventArgs e)
        {
            // Aggiorna anteprima stato DB in uso nella form (AppConfig è già aggiornato dal Test o dal Save)
            lblDbInUso.Text = $"Database attivo: {AppConfig.DatabaseInUse}";
        }

        private void RdoAzienda_CheckedChanged(object sender, EventArgs e)
        {
            // placeholder: lo stato effettivo viene salvato nel btnSalva
        }

        private void BtnSalva_Click(object sender, EventArgs e)
        {
            // Assegna e salva
            AppConfig.SqlServer = txtServer.Text.Trim();
            AppConfig.UserId = txtUser.Text.Trim();
            AppConfig.Password = txtPassword.Text;
            AppConfig.LoginId = int.TryParse(txtLoginId.Text.Trim(), out int id) ? id : 1;
            AppConfig.IsTestDb = chkTestDb.Checked;
            AppConfig.IsDbUno = rdoUno.Checked;
            AppConfig.IsDebugging = chkDebug.Checked;

            AppConfig.DatabaseUno = txtDbUno.Text.Trim();
            AppConfig.DatabaseSpa = txtDbSpa.Text.Trim();
            AppConfig.TmpDatabaseUno = txtTmpDbUno.Text.Trim();
            AppConfig.TmpDatabaseSpa = txtTmpDbSpa.Text.Trim();

            AppConfig.Path = txtPathGenerale.Text.Trim();
            AppConfig.FoxPath = txtPathFox.Text.Trim();
            AppConfig.PathPaghe = txtPathPaghe.Text.Trim();
            AppConfig.LastLogPath = txtLastLogPath.Text.Trim();

            AppConfig.LogMode = cboLogMode.SelectedIndex switch
            {
                0 => AppConfig.LogTarget.Ui,
                1 => AppConfig.LogTarget.File,
                _ => AppConfig.LogTarget.Both
            };

            AppConfig.Save();
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}