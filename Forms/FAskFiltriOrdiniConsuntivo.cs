using System;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using FatturazioneContratti.Invoicing;

namespace FatturazioneContratti.Forms
{
    public partial class FAskFiltriOrdiniConsuntivo : Form
    {
        private bool loading;

        public FAskFiltriOrdiniConsuntivo()
        {
            InitializeComponent();
        }

        private void FAskFiltriOrdiniConsuntivo_Load(object sender, EventArgs e)
        {
            loading = true;
            var it = new CultureInfo("it-IT");

            // MonthNames restituisce 13 elementi (l'ultimo è vuoto): prendiamo solo i 12 mesi reali
            CmbMonth.Items.AddRange(it.DateTimeFormat.MonthNames.Take(12).Cast<object>().ToArray());

            int currentYear = DateTime.Today.Year;
            CmbYear.Items.Add(currentYear - 1);
            CmbYear.Items.Add(currentYear);

            if (DateTime.Today.Month == 1)
            {
                // Gennaio: mese precedente Dicembre dell'anno scorso
                CmbMonth.SelectedIndex = 11;
                CmbYear.SelectedIndex = 0;
            }
            else
            {
                CmbMonth.SelectedIndex = DateTime.Today.Month - 1;
                CmbYear.SelectedIndex = 1;
            }

            // NOTA dal codice originale: "todo: levare dopo aver fatto test" (settaggio su Settembre anno scorso)
            CmbMonth.SelectedIndex = 9 - 1;
            CmbYear.SelectedIndex = 0;

            loading = false;
            CalcolaDate();
        }

        private void OK_Button_Click(object sender, EventArgs e)
        {
            if (ChkNoFilter.Checked)
            {
                if (CmbMonth.SelectedIndex < 0 || CmbMonth.SelectedIndex > 11)
                {
                    MessageBox.Show("Il mese indicato non è valido.", "Attenzione", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void Cancel_Button_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void CmbMonth_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CmbMonth.SelectedIndex >= 0 && CmbMonth.SelectedIndex <= 11)
            {
                CalcolaDate();
            }
        }

        private void CmbYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CmbYear.SelectedIndex >= 0 && CmbYear.SelectedIndex <= 1)
            {
                CalcolaDate();
            }
        }

        private void CalcolaDate()
        {
            if (!loading && CmbYear.SelectedItem != null && CmbMonth.SelectedIndex >= 0)
            {
                int y = Convert.ToInt32(CmbYear.SelectedItem);
                int m = CmbMonth.SelectedIndex + 1;

                DtaFatt.Value = new DateTime(y, m, DateTime.DaysInMonth(y, m));
                lastLogDate.Value = DtaFatt.Value;
                firstLogDate.Value = new DateTime(lastLogDate.Value.Year, lastLogDate.Value.Month, 1);
            }
        }

        private void ChkFiliale_CheckedChanged(object sender, EventArgs e)
        {
            TxtOrdFiliale.Enabled = ChkFiliale.Checked;
        }

        private void ChkNoFilter_CheckedChanged(object sender, EventArgs e)
        {
            bool b = ChkNoFilter.Checked;

            CmbMonth.Enabled = b;
            CmbYear.Enabled = b;
            RadOnlyCheck.Enabled = b;
            RadCheckAndFatt.Enabled = b;
            ChkRiassegna.Enabled = b;
            RadOnlyFatt.Enabled = b;
            DtaFatt.Enabled = b;
            ChkFiliale.Enabled = b;
            TxtOrdFiliale.Enabled = b && ChkFiliale.Checked;
        }

        private void RadOnlyFatt_CheckedChanged(object sender, EventArgs e)
        {
            ChkRiassegna.Enabled = !RadOnlyFatt.Checked;
        }

        /// <summary>
        /// Restituisce l'oggetto DTO FiltriOrdiniConsuntivo popolato con i valori selezionati nella form.
        /// </summary>
        public FiltriOrdiniConsuntivo OttieniFiltri()
        {
            var filtri = new FiltriOrdiniConsuntivo
            {
                AskFilter = ChkNoFilter.Checked,
                FromLogDate = firstLogDate.Value.Date,
                // Fine mese inclusiva: ultimo giorno alle 23:59:59
                ToLogDate = lastLogDate.Value.Date.AddDays(1).AddSeconds(-1),
                NrMese = (CmbMonth.SelectedIndex + 1).ToString(),
                SingolaFiliale = ChkFiliale.Checked,
                Filiale = TxtOrdFiliale.Text,
                DataFattura = DtaFatt.Value.Date
            };

            if (RadOnlyCheck.Checked || RadCheckAndFatt.Checked)
            {
                filtri.Riassegna = ChkRiassegna.Checked;
            }

            filtri.CostruisciFiltriLog();
            return filtri;
        }
    }
}