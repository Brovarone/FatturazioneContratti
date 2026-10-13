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
        private readonly ToolTip _toolTip = new();
        private readonly List<KeyValuePair<string, string>> _filiali = new()
        {
            new("TO", "TORINO"),
            new("MI", "MILANO"),
            new("AT", "ASTI"),
            new("AO", "AOSTA"),
            new("NO", "NOVARA"),
            new("BI", "VERCELLI/BIELLA"),
            new("VA", "VARESE"),
            new("CN", "CUNEO"),
            new("AL", "ALESSANDRIA")
        };

        public FAskFiltriOrdiniConsuntivo()
        {
            InitializeComponent();
        }

        private void FAskFiltriOrdiniConsuntivo_Load(object sender, EventArgs e)
        {
            loading = true;
            var it = new CultureInfo("it-IT");

            // Month names
            CmbMonth.Items.AddRange(it.DateTimeFormat.MonthNames.Take(12).Cast<object>().ToArray());

            int currentYear = DateTime.Today.Year;
            CmbYear.Items.Add(currentYear);
            CmbYear.Items.Add(currentYear - 1);

            // Default: anno più alto (es. 2026 se abbiamo 2026 e 2025), mese scorso
            CmbYear.SelectedIndex = 0; // anno corrente (il più alto)
            if (DateTime.Today.Month == 1)
            {
                CmbMonth.SelectedIndex = 11; // Dicembre
            }
            else
            {
                CmbMonth.SelectedIndex = DateTime.Today.Month - 2; // mese precedente
            }

            // Filiali
            CmbFiliale.DisplayMember = "Value";
            CmbFiliale.ValueMember = "Key";
            CmbFiliale.DataSource = _filiali.ToList();
            CmbFiliale.SelectedIndex = -1;

            // Default radio selections
            RadInterventiMeseAnno.Checked = true;
            RadAzioneControlloFattura.Checked = true;

            // Tooltip su Riassegna
            _toolTip.SetToolTip(ChkRiassegna, "ATTENZIONE: sovrascrive i contratti già assegnati agli interventi");

            loading = false;
            UpdateUiState();
            CalcolaDateDaMeseAnno();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _toolTip?.Dispose();
            base.OnFormClosed(e);
        }

        private void OK_Button_Click(object sender, EventArgs e)
        {
            // Validazione
            if (RadInterventiMeseAnno.Checked)
            {
                if (CmbMonth.SelectedIndex < 0 || CmbYear.SelectedIndex < 0)
                {
                    MessageBox.Show("Selezionare Mese e Anno validi.", "Attenzione", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            else if (RadInterventiDateRange.Checked)
            {
                if (DtpInterventiDa.Value.Date > DtpInterventiA.Value.Date)
                {
                    MessageBox.Show("Data 'Da' non può essere successiva a data 'A'.", "Attenzione", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            if (ChkFiliale.Checked && CmbFiliale.SelectedIndex < 0)
            {
                MessageBox.Show("Selezionare una filiale.", "Attenzione", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if ((RadAzioneControlloFattura.Checked || RadAzioneSoloFattura.Checked) && DtpDataFattura.Value.Date < new DateTime(2000, 1, 1))
            {
                MessageBox.Show("Specificare una Data Fattura valida.", "Attenzione", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void Cancel_Button_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void RadInterventiFilter_CheckedChanged(object sender, EventArgs e)
        {
            if (!loading) UpdateUiState();
        }

        private void RadAzione_CheckedChanged(object sender, EventArgs e)
        {
            if (!loading) UpdateUiState();
        }

        private void CmbMonth_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!loading && RadInterventiMeseAnno.Checked) CalcolaDateDaMeseAnno();
        }

        private void CmbYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!loading && RadInterventiMeseAnno.Checked) CalcolaDateDaMeseAnno();
        }

        private void DtpInterventiDa_ValueChanged(object sender, EventArgs e)
        {
            if (!loading && RadInterventiDateRange.Checked) UpdateDateRange();
        }

        private void DtpInterventiA_ValueChanged(object sender, EventArgs e)
        {
            if (!loading && RadInterventiDateRange.Checked) UpdateDateRange();
        }

        private void ChkFiliale_CheckedChanged(object sender, EventArgs e)
        {
            CmbFiliale.Enabled = ChkFiliale.Checked;
        }

        private void CalcolaDateDaMeseAnno()
        {
            if (CmbYear.SelectedItem == null || CmbMonth.SelectedIndex < 0) return;

            int y = Convert.ToInt32(CmbYear.SelectedItem);
            int m = CmbMonth.SelectedIndex + 1;

            var firstDay = new DateTime(y, m, 1);
            var lastDay = new DateTime(y, m, DateTime.DaysInMonth(y, m));

            DtpInterventiDa.Value = firstDay;
            DtpInterventiA.Value = lastDay;
            DtpDataFattura.Value = lastDay; // default data fattura = fine mese
        }

        private void UpdateDateRange()
        {
            // Quando l'utente modifica manualmente le date da/a, aggiorna data fattura
            if (RadInterventiDateRange.Checked)
            {
                DtpDataFattura.Value = DtpInterventiA.Value;
            }
        }

        private void UpdateUiState()
        {
            // --- Gruppo 1: Filtro Interventi ---
            bool isMeseAnno = RadInterventiMeseAnno.Checked;
            bool isDateRange = RadInterventiDateRange.Checked;
            bool isNoFilter = RadInterventiNoFilter.Checked;

            LblMonth.Enabled = CmbMonth.Enabled = isMeseAnno;
            LblYear.Enabled = CmbYear.Enabled = isMeseAnno;
            LblInterventiDa.Enabled = DtpInterventiDa.Enabled = isDateRange;
            LblInterventiA.Enabled = DtpInterventiA.Enabled = isDateRange;

            // Se NoFilter, disabilita tutto il gruppo date
            if (isNoFilter)
            {
                LblMonth.Enabled = CmbMonth.Enabled = false;
                LblYear.Enabled = CmbYear.Enabled = false;
                LblInterventiDa.Enabled = DtpInterventiDa.Enabled = false;
                LblInterventiA.Enabled = DtpInterventiA.Enabled = false;
            }

            // --- Gruppo 2: Azione ---
            bool isSoloControllo = RadAzioneSoloControllo.Checked;
            bool isControlloFattura = RadAzioneControlloFattura.Checked;
            bool isSoloFattura = RadAzioneSoloFattura.Checked;

            ChkRiassegna.Enabled = isSoloControllo || isControlloFattura;
            if (isSoloFattura) ChkRiassegna.Checked = false;

            // --- Gruppo 3: Data Fattura ---
            bool mostraDataFattura = isControlloFattura || isSoloFattura;
            LblDataFattura.Enabled = DtpDataFattura.Enabled = mostraDataFattura;

            // --- Gruppo 4: Filiale ---
            ChkFiliale.Enabled = true; // sempre abilitato
            CmbFiliale.Enabled = ChkFiliale.Checked;
        }

        /// <summary>
        /// Restituisce l'oggetto DTO FiltriOrdiniConsuntivo popolato con i valori selezionati nella form.
        /// </summary>
        public FiltriOrdiniConsuntivo OttieniFiltri()
        {
            var filtri = new FiltriOrdiniConsuntivo
            {
                // Filtro Interventi
                InterventiFilter = RadInterventiNoFilter.Checked ? InterventiFilterMode.NoFilter
                                    : RadInterventiMeseAnno.Checked ? InterventiFilterMode.MeseAnno
                                    : InterventiFilterMode.DateRange,

                FromInterventiDate = DtpInterventiDa.Value.Date,
                ToInterventiDate = DtpInterventiA.Value.Date.AddDays(1).AddSeconds(-1), // fine giorno inclusivo

                // Azione
                Azione = RadAzioneSoloControllo.Checked ? ActionMode.SoloControllo
                           : RadAzioneControlloFattura.Checked ? ActionMode.ControlloEFattura
                           : ActionMode.SoloFattura,

                // Data Fattura
                DataFattura = DtpDataFattura.Value.Date,

                // Filtri aggiuntivi
                SingolaFiliale = ChkFiliale.Checked,
                Filiale = CmbFiliale.SelectedValue?.ToString() ?? string.Empty,

                // Campi legacy per compatibilità
                NrMese = (CmbMonth.SelectedIndex + 1).ToString(),
                Riassegna = ChkRiassegna.Checked
            };

            filtri.CostruisciFiltriLog();
            return filtri;
        }
    }
}