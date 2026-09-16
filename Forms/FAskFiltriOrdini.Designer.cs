namespace FatturazioneContratti.Forms
{
    partial class FAskFiltriOrdini
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.TableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.OK_Button = new System.Windows.Forms.Button();
            this.Cancel_Button = new System.Windows.Forms.Button();
            this.DtaOrdineDA = new System.Windows.Forms.DateTimePicker();
            this.DtaOrdineA = new System.Windows.Forms.DateTimePicker();
            this.LblDallaData = new System.Windows.Forms.Label();
            this.LblAlladata = new System.Windows.Forms.Label();
            this.GroupOrdini = new System.Windows.Forms.GroupBox();
            this.TxtordPriorita = new System.Windows.Forms.TextBox();
            this.ChkPriorita = new System.Windows.Forms.CheckBox();
            this.TxtOrdFiliale = new System.Windows.Forms.TextBox();
            this.ChkFiliale = new System.Windows.Forms.CheckBox();
            this.ChkCliente = new System.Windows.Forms.CheckBox();
            this.ChkNrOrdine = new System.Windows.Forms.CheckBox();
            this.RadFinoAllaData = new System.Windows.Forms.RadioButton();
            this.RadDalAl = new System.Windows.Forms.RadioButton();
            this.TxtOrdCliente = new System.Windows.Forms.TextBox();
            this.TxtNrOrdine = new System.Windows.Forms.TextBox();
            this.DtaFattA = new System.Windows.Forms.DateTimePicker();
            this.GroupDataFatt = new System.Windows.Forms.GroupBox();
            this.Label6 = new System.Windows.Forms.Label();
            this.Label7 = new System.Windows.Forms.Label();
            this.DtaFattDa = new System.Windows.Forms.DateTimePicker();
            this.GroupPeriodo = new System.Windows.Forms.GroupBox();
            this.ChkP_Tutti = new System.Windows.Forms.CheckBox();
            this.ChkP_Mensili = new System.Windows.Forms.CheckBox();
            this.ChkP_Bimestrali = new System.Windows.Forms.CheckBox();
            this.ChkP_Trimestrali = new System.Windows.Forms.CheckBox();
            this.ChkP_Quadrimestrali = new System.Windows.Forms.CheckBox();
            this.ChkP_Semestrali = new System.Windows.Forms.CheckBox();
            this.ChkP_Annuali = new System.Windows.Forms.CheckBox();
            this.GroupIstat = new System.Windows.Forms.GroupBox();
            this.LblAnnoAdeguamento = new System.Windows.Forms.Label();
            this.TxtAnnoAdeguamento = new System.Windows.Forms.TextBox();
            this.Label4 = new System.Windows.Forms.Label();
            this.TxtIstatTesto = new System.Windows.Forms.TextBox();
            this.Label2 = new System.Windows.Forms.Label();
            this.TxtIstatAttivita = new System.Windows.Forms.TextBox();
            this.Label1 = new System.Windows.Forms.Label();
            this.TxtPercIstat = new System.Windows.Forms.TextBox();
            this.ToolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.GroupDecorrenza = new System.Windows.Forms.GroupBox();
            this.DtaDecorrenza = new System.Windows.Forms.DateTimePicker();
            this.GroupMomento = new System.Windows.Forms.GroupBox();
            this.RadioTipoServAgg = new System.Windows.Forms.RadioButton();
            this.RadioTipoCanoni = new System.Windows.Forms.RadioButton();
            this.GroupDivisione = new System.Windows.Forms.GroupBox();
            this.RadioDivTecnologia = new System.Windows.Forms.RadioButton();
            this.RadioDivTutti = new System.Windows.Forms.RadioButton();
            this.RadioDivVigilanza = new System.Windows.Forms.RadioButton();
            this.LblAvvisiServAgg = new System.Windows.Forms.Label();
            this.TableLayoutPanel1.SuspendLayout();
            this.GroupOrdini.SuspendLayout();
            this.GroupDataFatt.SuspendLayout();
            this.GroupPeriodo.SuspendLayout();
            this.GroupIstat.SuspendLayout();
            this.GroupDecorrenza.SuspendLayout();
            this.GroupMomento.SuspendLayout();
            this.GroupDivisione.SuspendLayout();
            this.SuspendLayout();
            // 
            // TableLayoutPanel1
            // 
            this.TableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.TableLayoutPanel1.ColumnCount = 2;
            this.TableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableLayoutPanel1.Controls.Add(this.OK_Button, 0, 0);
            this.TableLayoutPanel1.Controls.Add(this.Cancel_Button, 1, 0);
            this.TableLayoutPanel1.Location = new System.Drawing.Point(330, 467);
            this.TableLayoutPanel1.Name = "TableLayoutPanel1";
            this.TableLayoutPanel1.RowCount = 1;
            this.TableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableLayoutPanel1.Size = new System.Drawing.Size(146, 29);
            this.TableLayoutPanel1.TabIndex = 20;
            // 
            // OK_Button
            // 
            this.OK_Button.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.OK_Button.Location = new System.Drawing.Point(3, 3);
            this.OK_Button.Name = "OK_Button";
            this.OK_Button.Size = new System.Drawing.Size(67, 23);
            this.OK_Button.TabIndex = 22;
            this.OK_Button.Text = "OK";
            this.OK_Button.Click += new System.EventHandler(this.OK_Button_Click);
            // 
            // Cancel_Button
            // 
            this.Cancel_Button.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Cancel_Button.Location = new System.Drawing.Point(76, 3);
            this.Cancel_Button.Name = "Cancel_Button";
            this.Cancel_Button.Size = new System.Drawing.Size(67, 23);
            this.Cancel_Button.TabIndex = 23;
            this.Cancel_Button.Text = "Cancel";
            this.Cancel_Button.Click += new System.EventHandler(this.Cancel_Button_Click);
            // 
            // DtaOrdineDA
            // 
            this.DtaOrdineDA.Enabled = false;
            this.DtaOrdineDA.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.DtaOrdineDA.Location = new System.Drawing.Point(96, 65);
            this.DtaOrdineDA.Name = "DtaOrdineDA";
            this.DtaOrdineDA.Size = new System.Drawing.Size(96, 20);
            this.DtaOrdineDA.TabIndex = 3;
            // 
            // DtaOrdineA
            // 
            this.DtaOrdineA.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.DtaOrdineA.Location = new System.Drawing.Point(96, 94);
            this.DtaOrdineA.Name = "DtaOrdineA";
            this.DtaOrdineA.Size = new System.Drawing.Size(96, 20);
            this.DtaOrdineA.TabIndex = 4;
            // 
            // LblDallaData
            // 
            this.LblDallaData.AutoSize = true;
            this.LblDallaData.Location = new System.Drawing.Point(35, 65);
            this.LblDallaData.Name = "LblDallaData";
            this.LblDallaData.Size = new System.Drawing.Size(55, 13);
            this.LblDallaData.TabIndex = 3;
            this.LblDallaData.Text = "Dalla data";
            // 
            // LblAlladata
            // 
            this.LblAlladata.AutoSize = true;
            this.LblAlladata.Location = new System.Drawing.Point(35, 94);
            this.LblAlladata.Name = "LblAlladata";
            this.LblAlladata.Size = new System.Drawing.Size(48, 13);
            this.LblAlladata.TabIndex = 4;
            this.LblAlladata.Text = "Alla data";
            // 
            // GroupOrdini
            // 
            this.GroupOrdini.Controls.Add(this.TxtordPriorita);
            this.GroupOrdini.Controls.Add(this.ChkPriorita);
            this.GroupOrdini.Controls.Add(this.TxtOrdFiliale);
            this.GroupOrdini.Controls.Add(this.ChkFiliale);
            this.GroupOrdini.Controls.Add(this.ChkCliente);
            this.GroupOrdini.Controls.Add(this.ChkNrOrdine);
            this.GroupOrdini.Controls.Add(this.RadFinoAllaData);
            this.GroupOrdini.Controls.Add(this.RadDalAl);
            this.GroupOrdini.Controls.Add(this.TxtOrdCliente);
            this.GroupOrdini.Controls.Add(this.TxtNrOrdine);
            this.GroupOrdini.Controls.Add(this.DtaOrdineA);
            this.GroupOrdini.Controls.Add(this.DtaOrdineDA);
            this.GroupOrdini.Controls.Add(this.LblAlladata);
            this.GroupOrdini.Controls.Add(this.LblDallaData);
            this.GroupOrdini.Location = new System.Drawing.Point(6, 3);
            this.GroupOrdini.Name = "GroupOrdini";
            this.GroupOrdini.Size = new System.Drawing.Size(228, 221);
            this.GroupOrdini.TabIndex = 7;
            this.GroupOrdini.TabStop = false;
            this.GroupOrdini.Text = "Ordini Ciente";
            // 
            // TxtordPriorita
            // 
            this.TxtordPriorita.Enabled = false;
            this.TxtordPriorita.Location = new System.Drawing.Point(110, 196);
            this.TxtordPriorita.Name = "TxtordPriorita";
            this.TxtordPriorita.Size = new System.Drawing.Size(96, 20);
            this.TxtordPriorita.TabIndex = 12;
            // 
            // ChkPriorita
            // 
            this.ChkPriorita.AutoSize = true;
            this.ChkPriorita.Location = new System.Drawing.Point(9, 196);
            this.ChkPriorita.Name = "ChkPriorita";
            this.ChkPriorita.Size = new System.Drawing.Size(58, 17);
            this.ChkPriorita.TabIndex = 11;
            this.ChkPriorita.Text = "Priorità";
            this.ChkPriorita.UseVisualStyleBackColor = true;
            this.ChkPriorita.CheckedChanged += new System.EventHandler(this.ChkPriorita_CheckedChanged);
            // 
            // TxtOrdFiliale
            // 
            this.TxtOrdFiliale.Enabled = false;
            this.TxtOrdFiliale.Location = new System.Drawing.Point(110, 171);
            this.TxtOrdFiliale.Name = "TxtOrdFiliale";
            this.TxtOrdFiliale.Size = new System.Drawing.Size(96, 20);
            this.TxtOrdFiliale.TabIndex = 10;
            // 
            // ChkFiliale
            // 
            this.ChkFiliale.AutoSize = true;
            this.ChkFiliale.Location = new System.Drawing.Point(9, 171);
            this.ChkFiliale.Name = "ChkFiliale";
            this.ChkFiliale.Size = new System.Drawing.Size(90, 17);
            this.ChkFiliale.TabIndex = 9;
            this.ChkFiliale.Text = "Singola Filiale";
            this.ChkFiliale.UseVisualStyleBackColor = true;
            this.ChkFiliale.CheckedChanged += new System.EventHandler(this.ChkFiliale_CheckedChanged);
            // 
            // ChkCliente
            // 
            this.ChkCliente.AutoSize = true;
            this.ChkCliente.Location = new System.Drawing.Point(9, 146);
            this.ChkCliente.Name = "ChkCliente";
            this.ChkCliente.Size = new System.Drawing.Size(96, 17);
            this.ChkCliente.TabIndex = 7;
            this.ChkCliente.Text = "Singolo Cliente";
            this.ChkCliente.UseVisualStyleBackColor = true;
            this.ChkCliente.CheckedChanged += new System.EventHandler(this.ChkCliente_CheckedChanged);
            // 
            // ChkNrOrdine
            // 
            this.ChkNrOrdine.AutoSize = true;
            this.ChkNrOrdine.Location = new System.Drawing.Point(9, 123);
            this.ChkNrOrdine.Name = "ChkNrOrdine";
            this.ChkNrOrdine.Size = new System.Drawing.Size(95, 17);
            this.ChkNrOrdine.TabIndex = 5;
            this.ChkNrOrdine.Text = "Singolo Ordine";
            this.ChkNrOrdine.UseVisualStyleBackColor = true;
            this.ChkNrOrdine.CheckedChanged += new System.EventHandler(this.ChkNrOrdine_CheckedChanged);
            // 
            // RadFinoAllaData
            // 
            this.RadFinoAllaData.AutoSize = true;
            this.RadFinoAllaData.Checked = true;
            this.RadFinoAllaData.Location = new System.Drawing.Point(9, 19);
            this.RadFinoAllaData.Name = "RadFinoAllaData";
            this.RadFinoAllaData.Size = new System.Drawing.Size(100, 17);
            this.RadFinoAllaData.TabIndex = 1;
            this.RadFinoAllaData.TabStop = true;
            this.RadFinoAllaData.Text = "Fino alla data ...";
            this.RadFinoAllaData.UseVisualStyleBackColor = true;
            this.RadFinoAllaData.CheckedChanged += new System.EventHandler(this.RadFinoAllaData_CheckedChanged);
            // 
            // RadDalAl
            // 
            this.RadDalAl.AutoSize = true;
            this.RadDalAl.Location = new System.Drawing.Point(9, 42);
            this.RadDalAl.Name = "RadDalAl";
            this.RadDalAl.Size = new System.Drawing.Size(140, 17);
            this.RadDalAl.TabIndex = 2;
            this.RadDalAl.Text = "Dalla data ... alla data ...";
            this.RadDalAl.UseVisualStyleBackColor = true;
            // 
            // TxtOrdCliente
            // 
            this.TxtOrdCliente.Enabled = false;
            this.TxtOrdCliente.Location = new System.Drawing.Point(110, 146);
            this.TxtOrdCliente.Name = "TxtOrdCliente";
            this.TxtOrdCliente.Size = new System.Drawing.Size(96, 20);
            this.TxtOrdCliente.TabIndex = 8;
            // 
            // TxtNrOrdine
            // 
            this.TxtNrOrdine.Enabled = false;
            this.TxtNrOrdine.Location = new System.Drawing.Point(110, 120);
            this.TxtNrOrdine.Name = "TxtNrOrdine";
            this.TxtNrOrdine.Size = new System.Drawing.Size(96, 20);
            this.TxtNrOrdine.TabIndex = 6;
            // 
            // DtaFattA
            // 
            this.DtaFattA.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.DtaFattA.Location = new System.Drawing.Point(110, 45);
            this.DtaFattA.Name = "DtaFattA";
            this.DtaFattA.Size = new System.Drawing.Size(96, 20);
            this.DtaFattA.TabIndex = 12;
            // 
            // GroupDataFatt
            // 
            this.GroupDataFatt.Controls.Add(this.Label6);
            this.GroupDataFatt.Controls.Add(this.Label7);
            this.GroupDataFatt.Controls.Add(this.DtaFattDa);
            this.GroupDataFatt.Controls.Add(this.DtaFattA);
            this.GroupDataFatt.Location = new System.Drawing.Point(249, 3);
            this.GroupDataFatt.Name = "GroupDataFatt";
            this.GroupDataFatt.Size = new System.Drawing.Size(228, 81);
            this.GroupDataFatt.TabIndex = 14;
            this.GroupDataFatt.TabStop = false;
            this.GroupDataFatt.Text = "Data prossima fattura";
            // 
            // Label6
            // 
            this.Label6.AutoSize = true;
            this.Label6.Location = new System.Drawing.Point(44, 45);
            this.Label6.Name = "Label6";
            this.Label6.Size = new System.Drawing.Size(48, 13);
            this.Label6.TabIndex = 14;
            this.Label6.Text = "Alla data";
            // 
            // Label7
            // 
            this.Label7.AutoSize = true;
            this.Label7.Location = new System.Drawing.Point(44, 19);
            this.Label7.Name = "Label7";
            this.Label7.Size = new System.Drawing.Size(55, 13);
            this.Label7.TabIndex = 13;
            this.Label7.Text = "Dalla data";
            // 
            // DtaFattDa
            // 
            this.DtaFattDa.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.DtaFattDa.Location = new System.Drawing.Point(110, 19);
            this.DtaFattDa.Name = "DtaFattDa";
            this.DtaFattDa.Size = new System.Drawing.Size(96, 20);
            this.DtaFattDa.TabIndex = 11;
            this.DtaFattDa.ValueChanged += new System.EventHandler(this.DtaFattDa_ValueChanged);
            // 
            // GroupPeriodo
            // 
            this.GroupPeriodo.Controls.Add(this.ChkP_Tutti);
            this.GroupPeriodo.Controls.Add(this.ChkP_Mensili);
            this.GroupPeriodo.Controls.Add(this.ChkP_Bimestrali);
            this.GroupPeriodo.Controls.Add(this.ChkP_Trimestrali);
            this.GroupPeriodo.Controls.Add(this.ChkP_Quadrimestrali);
            this.GroupPeriodo.Controls.Add(this.ChkP_Semestrali);
            this.GroupPeriodo.Controls.Add(this.ChkP_Annuali);
            this.GroupPeriodo.Location = new System.Drawing.Point(249, 90);
            this.GroupPeriodo.Name = "GroupPeriodo";
            this.GroupPeriodo.Size = new System.Drawing.Size(228, 114);
            this.GroupPeriodo.TabIndex = 15;
            this.GroupPeriodo.TabStop = false;
            this.GroupPeriodo.Text = "Periodo";
            // 
            // ChkP_Tutti
            // 
            this.ChkP_Tutti.AutoSize = true;
            this.ChkP_Tutti.Checked = true;
            this.ChkP_Tutti.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ChkP_Tutti.Location = new System.Drawing.Point(9, 20);
            this.ChkP_Tutti.Name = "ChkP_Tutti";
            this.ChkP_Tutti.Size = new System.Drawing.Size(47, 17);
            this.ChkP_Tutti.TabIndex = 11;
            this.ChkP_Tutti.Text = "Tutti";
            this.ChkP_Tutti.UseVisualStyleBackColor = true;
            this.ChkP_Tutti.CheckedChanged += new System.EventHandler(this.ChkP_Tutti_CheckedChanged);
            // 
            // ChkP_Mensili
            // 
            this.ChkP_Mensili.AutoSize = true;
            this.ChkP_Mensili.Enabled = false;
            this.ChkP_Mensili.Location = new System.Drawing.Point(110, 89);
            this.ChkP_Mensili.Name = "ChkP_Mensili";
            this.ChkP_Mensili.Size = new System.Drawing.Size(58, 17);
            this.ChkP_Mensili.TabIndex = 17;
            this.ChkP_Mensili.Text = "Mensili";
            this.ChkP_Mensili.UseVisualStyleBackColor = true;
            // 
            // ChkP_Bimestrali
            // 
            this.ChkP_Bimestrali.AutoSize = true;
            this.ChkP_Bimestrali.Enabled = false;
            this.ChkP_Bimestrali.Location = new System.Drawing.Point(110, 66);
            this.ChkP_Bimestrali.Name = "ChkP_Bimestrali";
            this.ChkP_Bimestrali.Size = new System.Drawing.Size(70, 17);
            this.ChkP_Bimestrali.TabIndex = 16;
            this.ChkP_Bimestrali.Text = "Bimestrali";
            this.ChkP_Bimestrali.UseVisualStyleBackColor = true;
            // 
            // ChkP_Trimestrali
            // 
            this.ChkP_Trimestrali.AutoSize = true;
            this.ChkP_Trimestrali.Enabled = false;
            this.ChkP_Trimestrali.Location = new System.Drawing.Point(110, 43);
            this.ChkP_Trimestrali.Name = "ChkP_Trimestrali";
            this.ChkP_Trimestrali.Size = new System.Drawing.Size(73, 17);
            this.ChkP_Trimestrali.TabIndex = 15;
            this.ChkP_Trimestrali.Text = "Trimestrali";
            this.ChkP_Trimestrali.UseVisualStyleBackColor = true;
            // 
            // ChkP_Quadrimestrali
            // 
            this.ChkP_Quadrimestrali.AutoSize = true;
            this.ChkP_Quadrimestrali.Enabled = false;
            this.ChkP_Quadrimestrali.Location = new System.Drawing.Point(9, 89);
            this.ChkP_Quadrimestrali.Name = "ChkP_Quadrimestrali";
            this.ChkP_Quadrimestrali.Size = new System.Drawing.Size(92, 17);
            this.ChkP_Quadrimestrali.TabIndex = 14;
            this.ChkP_Quadrimestrali.Text = "Quadrimestrali";
            this.ChkP_Quadrimestrali.UseVisualStyleBackColor = true;
            // 
            // ChkP_Semestrali
            // 
            this.ChkP_Semestrali.AutoSize = true;
            this.ChkP_Semestrali.Enabled = false;
            this.ChkP_Semestrali.Location = new System.Drawing.Point(9, 66);
            this.ChkP_Semestrali.Name = "ChkP_Semestrali";
            this.ChkP_Semestrali.Size = new System.Drawing.Size(74, 17);
            this.ChkP_Semestrali.TabIndex = 13;
            this.ChkP_Semestrali.Text = "Semestrali";
            this.ChkP_Semestrali.UseVisualStyleBackColor = true;
            // 
            // ChkP_Annuali
            // 
            this.ChkP_Annuali.AutoSize = true;
            this.ChkP_Annuali.Enabled = false;
            this.ChkP_Annuali.Location = new System.Drawing.Point(9, 43);
            this.ChkP_Annuali.Name = "ChkP_Annuali";
            this.ChkP_Annuali.Size = new System.Drawing.Size(61, 17);
            this.ChkP_Annuali.TabIndex = 12;
            this.ChkP_Annuali.Text = "Annuali";
            this.ChkP_Annuali.UseVisualStyleBackColor = true;
            // 
            // GroupIstat
            // 
            this.GroupIstat.Controls.Add(this.LblAnnoAdeguamento);
            this.GroupIstat.Controls.Add(this.TxtAnnoAdeguamento);
            this.GroupIstat.Controls.Add(this.Label4);
            this.GroupIstat.Controls.Add(this.TxtIstatTesto);
            this.GroupIstat.Controls.Add(this.Label2);
            this.GroupIstat.Controls.Add(this.TxtIstatAttivita);
            this.GroupIstat.Controls.Add(this.Label1);
            this.GroupIstat.Controls.Add(this.TxtPercIstat);
            this.GroupIstat.Enabled = false;
            this.GroupIstat.Location = new System.Drawing.Point(44, 326);
            this.GroupIstat.Name = "GroupIstat";
            this.GroupIstat.Size = new System.Drawing.Size(228, 131);
            this.GroupIstat.TabIndex = 15;
            this.GroupIstat.TabStop = false;
            this.GroupIstat.Text = "ISTAT";
            this.GroupIstat.Visible = false;
            // 
            // LblAnnoAdeguamento
            // 
            this.LblAnnoAdeguamento.AutoSize = true;
            this.LblAnnoAdeguamento.Location = new System.Drawing.Point(43, 43);
            this.LblAnnoAdeguamento.Name = "LblAnnoAdeguamento";
            this.LblAnnoAdeguamento.Size = new System.Drawing.Size(134, 13);
            this.LblAnnoAdeguamento.TabIndex = 28;
            this.LblAnnoAdeguamento.Text = "Anno adeguamento ISTAT";
            // 
            // TxtAnnoAdeguamento
            // 
            this.TxtAnnoAdeguamento.Location = new System.Drawing.Point(182, 40);
            this.TxtAnnoAdeguamento.Name = "TxtAnnoAdeguamento";
            this.TxtAnnoAdeguamento.Size = new System.Drawing.Size(40, 20);
            this.TxtAnnoAdeguamento.TabIndex = 20;
            // 
            // Label4
            // 
            this.Label4.AutoSize = true;
            this.Label4.Location = new System.Drawing.Point(6, 72);
            this.Label4.Name = "Label4";
            this.Label4.Size = new System.Drawing.Size(34, 13);
            this.Label4.TabIndex = 26;
            this.Label4.Text = "Testo";
            // 
            // TxtIstatTesto
            // 
            this.TxtIstatTesto.Location = new System.Drawing.Point(46, 72);
            this.TxtIstatTesto.Multiline = true;
            this.TxtIstatTesto.Name = "TxtIstatTesto";
            this.TxtIstatTesto.Size = new System.Drawing.Size(176, 44);
            this.TxtIstatTesto.TabIndex = 21;
            this.TxtIstatTesto.Text = "Il canone è aggiornato sulla base dell\'indice ISTAT relativo al mese di novembre 2021";
            // 
            // Label2
            // 
            this.Label2.AutoSize = true;
            this.Label2.Location = new System.Drawing.Point(137, 16);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(39, 13);
            this.Label2.TabIndex = 22;
            this.Label2.Text = "Attività";
            // 
            // TxtIstatAttivita
            // 
            this.TxtIstatAttivita.Location = new System.Drawing.Point(182, 13);
            this.TxtIstatAttivita.Name = "TxtIstatAttivita";
            this.TxtIstatAttivita.Size = new System.Drawing.Size(40, 20);
            this.TxtIstatAttivita.TabIndex = 19;
            this.TxtIstatAttivita.Text = "ISTAT";
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Location = new System.Drawing.Point(6, 16);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(64, 13);
            this.Label1.TabIndex = 1;
            this.Label1.Text = "Percentuale";
            // 
            // TxtPercIstat
            // 
            this.TxtPercIstat.Location = new System.Drawing.Point(85, 13);
            this.TxtPercIstat.Name = "TxtPercIstat";
            this.TxtPercIstat.Size = new System.Drawing.Size(36, 20);
            this.TxtPercIstat.TabIndex = 18;
            this.TxtPercIstat.TextChanged += new System.EventHandler(this.TxtPercIstat_TextChanged);
            // 
            // ToolTip1
            // 
            this.ToolTip1.AutomaticDelay = 80;
            // 
            // GroupDecorrenza
            // 
            this.GroupDecorrenza.Controls.Add(this.DtaDecorrenza);
            this.GroupDecorrenza.Enabled = false;
            this.GroupDecorrenza.Location = new System.Drawing.Point(6, 433);
            this.GroupDecorrenza.Name = "GroupDecorrenza";
            this.GroupDecorrenza.Size = new System.Drawing.Size(228, 49);
            this.GroupDecorrenza.TabIndex = 25;
            this.GroupDecorrenza.TabStop = false;
            this.GroupDecorrenza.Text = "Decorrenza inferiore a";
            this.GroupDecorrenza.Visible = false;
            // 
            // DtaDecorrenza
            // 
            this.DtaDecorrenza.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.DtaDecorrenza.Location = new System.Drawing.Point(110, 19);
            this.DtaDecorrenza.Name = "DtaDecorrenza";
            this.DtaDecorrenza.Size = new System.Drawing.Size(96, 20);
            this.DtaDecorrenza.TabIndex = 13;
            this.DtaDecorrenza.ValueChanged += new System.EventHandler(this.DtaDecorrenza_ValueChanged);
            // 
            // GroupMomento
            // 
            this.GroupMomento.Controls.Add(this.RadioTipoServAgg);
            this.GroupMomento.Controls.Add(this.RadioTipoCanoni);
            this.GroupMomento.Location = new System.Drawing.Point(6, 224);
            this.GroupMomento.Name = "GroupMomento";
            this.GroupMomento.Size = new System.Drawing.Size(228, 74);
            this.GroupMomento.TabIndex = 26;
            this.GroupMomento.TabStop = false;
            this.GroupMomento.Text = "Tipo";
            // 
            // RadioTipoServAgg
            // 
            this.RadioTipoServAgg.AutoSize = true;
            this.RadioTipoServAgg.Location = new System.Drawing.Point(7, 42);
            this.RadioTipoServAgg.Name = "RadioTipoServAgg";
            this.RadioTipoServAgg.Size = new System.Drawing.Size(105, 17);
            this.RadioTipoServAgg.TabIndex = 1;
            this.RadioTipoServAgg.Text = "Servizi Aggiuntivi";
            this.RadioTipoServAgg.UseVisualStyleBackColor = true;
            this.RadioTipoServAgg.CheckedChanged += new System.EventHandler(this.RadioTipoServAgg_CheckedChanged);
            // 
            // RadioTipoCanoni
            // 
            this.RadioTipoCanoni.AutoSize = true;
            this.RadioTipoCanoni.Checked = true;
            this.RadioTipoCanoni.Location = new System.Drawing.Point(7, 20);
            this.RadioTipoCanoni.Name = "RadioTipoCanoni";
            this.RadioTipoCanoni.Size = new System.Drawing.Size(58, 17);
            this.RadioTipoCanoni.TabIndex = 0;
            this.RadioTipoCanoni.TabStop = true;
            this.RadioTipoCanoni.Text = "Canoni";
            this.RadioTipoCanoni.UseVisualStyleBackColor = true;
            // 
            // GroupDivisione
            // 
            this.GroupDivisione.Controls.Add(this.RadioDivVigilanza);
            this.GroupDivisione.Controls.Add(this.RadioDivTecnologia);
            this.GroupDivisione.Controls.Add(this.RadioDivTutti);
            this.GroupDivisione.Location = new System.Drawing.Point(249, 210);
            this.GroupDivisione.Name = "GroupDivisione";
            this.GroupDivisione.Size = new System.Drawing.Size(228, 88);
            this.GroupDivisione.TabIndex = 27;
            this.GroupDivisione.TabStop = false;
            this.GroupDivisione.Text = "Divisione";
            // 
            // RadioDivTecnologia
            // 
            this.RadioDivTecnologia.AutoSize = true;
            this.RadioDivTecnologia.Location = new System.Drawing.Point(7, 42);
            this.RadioDivTecnologia.Name = "RadioDivTecnologia";
            this.RadioDivTecnologia.Size = new System.Drawing.Size(78, 17);
            this.RadioDivTecnologia.TabIndex = 1;
            this.RadioDivTecnologia.Text = "Tecnologia";
            this.RadioDivTecnologia.UseVisualStyleBackColor = true;
            // 
            // RadioDivTutti
            // 
            this.RadioDivTutti.AutoSize = true;
            this.RadioDivTutti.Checked = true;
            this.RadioDivTutti.Location = new System.Drawing.Point(7, 20);
            this.RadioDivTutti.Name = "RadioDivTutti";
            this.RadioDivTutti.Size = new System.Drawing.Size(46, 17);
            this.RadioDivTutti.TabIndex = 0;
            this.RadioDivTutti.TabStop = true;
            this.RadioDivTutti.Text = "Tutti";
            this.RadioDivTutti.UseVisualStyleBackColor = true;
            // 
            // RadioDivVigilanza
            // 
            this.RadioDivVigilanza.AutoSize = true;
            this.RadioDivVigilanza.Location = new System.Drawing.Point(7, 65);
            this.RadioDivVigilanza.Name = "RadioDivVigilanza";
            this.RadioDivVigilanza.Size = new System.Drawing.Size(67, 17);
            this.RadioDivVigilanza.TabIndex = 2;
            this.RadioDivVigilanza.Text = "Vigilanza";
            this.RadioDivVigilanza.UseVisualStyleBackColor = true;
            // 
            // LblAvvisiServAgg
            // 
            this.LblAvvisiServAgg.ForeColor = System.Drawing.Color.Blue;
            this.LblAvvisiServAgg.Location = new System.Drawing.Point(3, 301);
            this.LblAvvisiServAgg.Name = "LblAvvisiServAgg";
            this.LblAvvisiServAgg.Size = new System.Drawing.Size(329, 62);
            this.LblAvvisiServAgg.TabIndex = 28;
            this.LblAvvisiServAgg.Text = "ATTENZIONE VERRANNO GENERATE DELLE FATTURE\r\nLa data di fatturazione sarà sempre f" +
    "ine mese rispetto alla data impostata sopra.";
            this.LblAvvisiServAgg.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.LblAvvisiServAgg.Visible = false;
            // 
            // FAskFiltriOrdini
            // 
            this.AcceptButton = this.OK_Button;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.Cancel_Button;
            this.ClientSize = new System.Drawing.Size(488, 508);
            this.Controls.Add(this.LblAvvisiServAgg);
            this.Controls.Add(this.GroupDivisione);
            this.Controls.Add(this.GroupMomento);
            this.Controls.Add(this.GroupDecorrenza);
            this.Controls.Add(this.GroupIstat);
            this.Controls.Add(this.GroupPeriodo);
            this.Controls.Add(this.GroupDataFatt);
            this.Controls.Add(this.TableLayoutPanel1);
            this.Controls.Add(this.GroupOrdini);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FAskFiltriOrdini";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Filtri";
            this.Load += new System.EventHandler(this.FAskFiltriOrdini_Load);
            this.TableLayoutPanel1.ResumeLayout(false);
            this.GroupOrdini.ResumeLayout(false);
            this.GroupOrdini.PerformLayout();
            this.GroupDataFatt.ResumeLayout(false);
            this.GroupDataFatt.PerformLayout();
            this.GroupPeriodo.ResumeLayout(false);
            this.GroupPeriodo.PerformLayout();
            this.GroupIstat.ResumeLayout(false);
            this.GroupIstat.PerformLayout();
            this.GroupDecorrenza.ResumeLayout(false);
            this.GroupMomento.ResumeLayout(false);
            this.GroupMomento.PerformLayout();
            this.GroupDivisione.ResumeLayout(false);
            this.GroupDivisione.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.TableLayoutPanel TableLayoutPanel1;
        internal System.Windows.Forms.Button OK_Button;
        internal System.Windows.Forms.Button Cancel_Button;
        internal System.Windows.Forms.DateTimePicker DtaOrdineDA;
        internal System.Windows.Forms.DateTimePicker DtaOrdineA;
        internal System.Windows.Forms.Label LblDallaData;
        internal System.Windows.Forms.Label LblAlladata;
        internal System.Windows.Forms.GroupBox GroupOrdini;
        internal System.Windows.Forms.TextBox TxtOrdCliente;
        internal System.Windows.Forms.TextBox TxtNrOrdine;
        internal System.Windows.Forms.DateTimePicker DtaFattA;
        internal System.Windows.Forms.RadioButton RadDalAl;
        internal System.Windows.Forms.RadioButton RadFinoAllaData;
        internal System.Windows.Forms.CheckBox ChkCliente;
        internal System.Windows.Forms.CheckBox ChkNrOrdine;
        internal System.Windows.Forms.GroupBox GroupDataFatt;
        internal System.Windows.Forms.GroupBox GroupPeriodo;
        internal System.Windows.Forms.CheckBox ChkP_Tutti;
        internal System.Windows.Forms.CheckBox ChkP_Mensili;
        internal System.Windows.Forms.CheckBox ChkP_Bimestrali;
        internal System.Windows.Forms.CheckBox ChkP_Trimestrali;
        internal System.Windows.Forms.CheckBox ChkP_Quadrimestrali;
        internal System.Windows.Forms.CheckBox ChkP_Semestrali;
        internal System.Windows.Forms.CheckBox ChkP_Annuali;
        internal System.Windows.Forms.TextBox TxtOrdFiliale;
        internal System.Windows.Forms.CheckBox ChkFiliale;
        internal System.Windows.Forms.GroupBox GroupIstat;
        internal System.Windows.Forms.Label Label1;
        internal System.Windows.Forms.TextBox TxtPercIstat;
        internal System.Windows.Forms.Label Label2;
        internal System.Windows.Forms.TextBox TxtIstatAttivita;
        internal System.Windows.Forms.ToolTip ToolTip1;
        internal System.Windows.Forms.Label Label4;
        internal System.Windows.Forms.TextBox TxtIstatTesto;
        internal System.Windows.Forms.Label LblAnnoAdeguamento;
        internal System.Windows.Forms.TextBox TxtAnnoAdeguamento;
        internal System.Windows.Forms.DateTimePicker DtaFattDa;
        internal System.Windows.Forms.Label Label6;
        internal System.Windows.Forms.Label Label7;
        internal System.Windows.Forms.GroupBox GroupDecorrenza;
        internal System.Windows.Forms.DateTimePicker DtaDecorrenza;
        internal System.Windows.Forms.GroupBox GroupMomento;
        internal System.Windows.Forms.RadioButton RadioTipoServAgg;
        internal System.Windows.Forms.RadioButton RadioTipoCanoni;
        internal System.Windows.Forms.TextBox TxtordPriorita;
        internal System.Windows.Forms.CheckBox ChkPriorita;
        internal System.Windows.Forms.GroupBox GroupDivisione;
        internal System.Windows.Forms.RadioButton RadioDivVigilanza;
        internal System.Windows.Forms.RadioButton RadioDivTecnologia;
        internal System.Windows.Forms.RadioButton RadioDivTutti;
        internal System.Windows.Forms.Label LblAvvisiServAgg;
    }
}