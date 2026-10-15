using FatturazioneContratti.Invoicing;
using System;
using System.Windows.Forms;

namespace FatturazioneContratti.Forms
{
    public partial class FAskFiltriAllineamentoFatture : Form
    {
        public FAskFiltriAllineamentoFatture()
        {
            InitializeComponent();

            var now = DateTime.Today;
            dtpDa.Value = new DateTime(now.Year, 1, 1);
            dtpA.Value = new DateTime(now.Year, now.Month, DateTime.DaysInMonth(now.Year, now.Month));

            UpdateNumberInputs();
        }

        public FiltriAllineamentoFatture OttieniFiltri()
        {
            bool allNumbers = radTuttiNumeri.Checked;

            return new FiltriAllineamentoFatture
            {
                DataFattDa = dtpDa.Value.Date,
                DataFattA = dtpA.Value.Date,
                AllNumbers = allNumbers,
                NrFirst = allNumbers ? string.Empty : txtNrFirst.Text.Trim(),
                NrLast = allNumbers ? string.Empty : txtNrLast.Text.Trim()
            };
        }

        private void UpdateNumberInputs()
        {
            bool interval = radIntervalloNumeri.Checked;
            lblNrFirst.Enabled = interval;
            txtNrFirst.Enabled = interval;
            lblNrLast.Enabled = interval;
            txtNrLast.Enabled = interval;
        }

        private void DtpDa_ValueChanged(object sender, EventArgs e)
        {
            if (dtpA.Value < dtpDa.Value)
                dtpA.Value = dtpDa.Value;
        }

        private void RadTuttiNumeri_CheckedChanged(object sender, EventArgs e)
        {
            UpdateNumberInputs();
        }

        private void RadIntervalloNumeri_CheckedChanged(object sender, EventArgs e)
        {
            UpdateNumberInputs();
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            if (radIntervalloNumeri.Checked)
            {
                if (string.IsNullOrWhiteSpace(txtNrFirst.Text) || string.IsNullOrWhiteSpace(txtNrLast.Text))
                {
                    MessageBox.Show("Inserire sia il primo che l'ultimo numero documento.", "Attenzione", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    DialogResult = DialogResult.None;
                    return;
                }
            }
        }
    }
}