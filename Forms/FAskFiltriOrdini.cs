using FatturazioneContratti.Orders;
using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatturazioneContratti.Forms
{
    public partial class FAskFiltriOrdini : Form
    {
        public bool IsIstat { get; set; }
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

        public FAskFiltriOrdini()
        {
            InitializeComponent();

            DtaFattA.Value = DateTime.Now;
            TxtAnnoAdeguamento.Text = DateTime.Now.Year.ToString();
        }

        private void FAskFiltriOrdini_Load(object sender, EventArgs e)
        {
            ToolTip1 = new ToolTip();

            // Popola combo filiale
            CmbFiliale.DisplayMember = "Value";
            CmbFiliale.ValueMember = "Key";
            CmbFiliale.DataSource = _filiali.ToList();
            CmbFiliale.SelectedIndex = -1;
            CmbFiliale.Enabled = false;

            if (IsIstat)
            {
                this.Text = "Filtri adeguamento ISTAT";

                // Nascondo quelli non utili
                GroupDataFatt.Visible = false;
                GroupPeriodo.Visible = false;
                GroupMomento.Visible = false;

                // Mostro gli altri
                GroupDecorrenza.Enabled = true;
                GroupDecorrenza.Location = new Point(6, 224);
                GroupDecorrenza.Visible = true;

                GroupIstat.Enabled = true;
                GroupIstat.Location = GroupDataFatt.Location;
                GroupIstat.Visible = true;

                // Tooltip esplicativi
                string s = "Contratti con data decorrenza minore a questa data";
                ToolTip1.SetToolTip(this.DtaFattA, s);

                s = "Valorizzato con Anno della 'Data decorrenza' -1." + Environment.NewLine +
                    "Se per l'anno indicato e' già stata eseguita" + Environment.NewLine +
                    "una rivalutazione la riga non verrà processata.";
                ToolTip1.SetToolTip(this.TxtAnnoAdeguamento, s);
                ToolTip1.SetToolTip(this.LblAnnoAdeguamento, s);

                s = "Campo 'Centro di Costo' di Mago";
                ToolTip1.SetToolTip(this.ChkFiliale, s);
                ToolTip1.SetToolTip(this.CmbFiliale, s);
            }
        }

        private void OK_Button_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void Cancel_Button_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void RadFinoAllaData_CheckedChanged(object sender, EventArgs e)
        {
            DtaOrdineDA.Enabled = !RadFinoAllaData.Checked;
            DtaOrdineDA.Value = new DateTime(DtaOrdineA.Value.Year, 1, 1);
        }

        private void ChkNrOrdine_CheckedChanged(object sender, EventArgs e)
        {
            TxtNrOrdine.Enabled = ChkNrOrdine.Checked;
        }

        private void ChkCliente_CheckedChanged(object sender, EventArgs e)
        {
            TxtOrdCliente.Enabled = ChkCliente.Checked;
        }

        private void ChkFiliale_CheckedChanged(object sender, EventArgs e)
        {
            CmbFiliale.Enabled = ChkFiliale.Checked;
        }

        private void ChkPriorita_CheckedChanged(object sender, EventArgs e)
        {
            TxtordPriorita.Enabled = ChkPriorita.Checked;
        }

        private void ChkP_Tutti_CheckedChanged(object sender, EventArgs e)
        {
            bool abilitaSingoli = !ChkP_Tutti.Checked;
            ChkP_Annuali.Enabled = abilitaSingoli;
            ChkP_Semestrali.Enabled = abilitaSingoli;
            ChkP_Quadrimestrali.Enabled = abilitaSingoli;
            ChkP_Trimestrali.Enabled = abilitaSingoli;
            ChkP_Bimestrali.Enabled = abilitaSingoli;
            ChkP_Mensili.Enabled = abilitaSingoli;
        }

        private void TxtPercIstat_TextChanged(object sender, EventArgs e)
        {
            // Nessuno stato da mantenere: la percentuale viene riletta in OttieniFiltri().
        }

        private void DtaDecorrenza_ValueChanged(object sender, EventArgs e)
        {
            DateTime dtaIstatRifatt = DtaDecorrenza.Value.AddYears(1);
            string sAnnoIstat = dtaIstatRifatt.AddMonths(-2).Year.ToString();
            TxtAnnoAdeguamento.Text = dtaIstatRifatt.Year.ToString();

            string meseIstat = DtaDecorrenza.Value.AddMonths(-2).ToString("MMMM", CultureInfo.CreateSpecificCulture("it-IT"));
            TxtIstatTesto.Text = $"Il canone è aggiornato sulla base dell'indice ISTAT relativo al mese di {meseIstat} {sAnnoIstat}";
        }

        private void DtaFattDa_ValueChanged(object sender, EventArgs e)
        {
            DtaFattA.Value = DtaFattDa.Value;
        }

        private void RadioTipoServAgg_CheckedChanged(object sender, EventArgs e)
        {
            bool isServAgg = RadioTipoServAgg.Checked;
            GroupDataFatt.Enabled = !isServAgg;
            LblAvvisiServAgg.Visible = isServAgg;
            RadioDivTutti.Enabled = !isServAgg;
            RadioDivTecnologia.Enabled = !isServAgg;
            RadioDivTecnologia.Checked = !isServAgg;
            RadioDivVigilanza.Checked = isServAgg;
            RadioDivTutti.Checked = !isServAgg;
        }

        /// <summary>
        /// Popola e restituisce l'oggetto DTO FiltriOrdini direttamente dallo stato corrente del form.
        /// </summary>
        public FiltriOrdini OttieniFiltri()
        {
            var f = new FiltriOrdini
            {
                FiltroDateOrdini = RadDalAl.Checked,
                FromOrdDate = DtaOrdineDA.Value.Date,
                ToOrdDate = DtaOrdineA.Value.Date,
                SingoloOrdine = ChkNrOrdine.Checked,
                NrOrd = TxtNrOrdine.Text,
                SingoloCliente = ChkCliente.Checked,
                Cliente = TxtOrdCliente.Text,
                SingolaFiliale = ChkFiliale.Checked,
                Filiale = CmbFiliale.SelectedValue?.ToString() ?? string.Empty,
                SingolaPriorita = ChkPriorita.Checked,
                Priorita = TxtordPriorita.Text,
                DataFattDa = DtaFattDa.Value.Date,
                DataFattA = DtaFattA.Value.Date,
                TipoCanoni = RadioTipoCanoni.Checked,
                TipoServiziAggiuntivi = RadioTipoServAgg.Checked,
                PeriodoTutti = ChkP_Tutti.Checked,
                PeriodoAnnuali = ChkP_Annuali.Checked,
                PeriodoSemestrali = ChkP_Semestrali.Checked,
                PeriodoQuadrimestrali = ChkP_Quadrimestrali.Checked,
                PeriodoTrimestrali = ChkP_Trimestrali.Checked,
                PeriodoBimestrali = ChkP_Bimestrali.Checked,
                PeriodoMensili = ChkP_Mensili.Checked,
                DivisioneTutti = RadioDivTutti.Checked,
                DivisioneTecnologia = RadioDivTecnologia.Checked,
                DivisioneVigilanza = RadioDivVigilanza.Checked,
                IsIstat = this.IsIstat,
                DataDecorrenza = DtaDecorrenza.Value.Date,
                CausaleAttivita = TxtIstatAttivita.Text,
                AnnoAdeguamento = int.TryParse(TxtAnnoAdeguamento.Text, out int anno) ? anno : 0,
                TestoFattura = TxtIstatTesto.Text
            };

            double.TryParse(TxtPercIstat.Text, out double pIstat);
            f.PercIstat = pIstat;

            f.Periodi[0] = f.PeriodoTutti;
            f.Periodi[1] = f.PeriodoMensili;
            f.Periodi[2] = f.PeriodoBimestrali;
            f.Periodi[3] = f.PeriodoTrimestrali;
            f.Periodi[4] = f.PeriodoQuadrimestrali;
            f.Periodi[5] = f.PeriodoSemestrali;
            f.Periodi[6] = f.PeriodoAnnuali;

            f.CostruisciFiltriLog();
            return f;
        }
    }
}