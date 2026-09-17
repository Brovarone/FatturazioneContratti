using System;
using System.Windows.Forms;
using FatturazioneContratti.Config;

namespace FatturazioneContratti.Forms
{
    public partial class FSettings : Form
    {
        public FSettings()
        {
            InitializeComponent();
        }

        private void FSettings_Load(object sender, EventArgs e)
        {
            // Carica valori attuali da AppConfig
            txtDbUno.Text = AppConfig.DatabaseUno;
            txtDbSpa.Text = AppConfig.DatabaseSpa;
            txtTmpDbUno.Text = AppConfig.TmpDatabaseUno;
            txtTmpDbSpa.Text = AppConfig.TmpDatabaseSpa;

            txtPathGenerale.Text = AppConfig.Path;
            txtPathFox.Text = AppConfig.FoxPath;
            txtPathPaghe.Text = AppConfig.PathPaghe;
        }

        private void btnSfoglia_Click(object sender, EventArgs e)
        {
            using var fbd = new FolderBrowserDialog();
            if (sender == btnSfogliaGenerale && fbd.ShowDialog() == DialogResult.OK)
                txtPathGenerale.Text = fbd.SelectedPath;
            else if (sender == btnSfogliaFox && fbd.ShowDialog() == DialogResult.OK)
                txtPathFox.Text = fbd.SelectedPath;
            else if (sender == btnSfogliaPaghe && fbd.ShowDialog() == DialogResult.OK)
                txtPathPaghe.Text = fbd.SelectedPath;
        }

        private void btnApriJson_Click(object sender, EventArgs e)
        {
            AppConfig.ApriFileJson();
        }

        private void btnSalva_Click(object sender, EventArgs e)
        {
            // Assegna e salva
            AppConfig.DatabaseUno = txtDbUno.Text.Trim();
            AppConfig.DatabaseSpa = txtDbSpa.Text.Trim();
            AppConfig.TmpDatabaseUno = txtTmpDbUno.Text.Trim();
            AppConfig.TmpDatabaseSpa = txtTmpDbSpa.Text.Trim();

            AppConfig.Path = txtPathGenerale.Text.Trim();
            AppConfig.FoxPath = txtPathFox.Text.Trim();
            AppConfig.PathPaghe = txtPathPaghe.Text.Trim();

            AppConfig.Save();
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}