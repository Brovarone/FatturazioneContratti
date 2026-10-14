namespace FatturazioneContratti.Forms
{
    partial class FAskFiltriOrdiniConsuntivo
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
            TableLayoutPanelButtons = new TableLayoutPanel();
            OK_Button = new Button();
            Cancel_Button = new Button();
            GrpInterventiFilter = new GroupBox();
            RadInterventiNoFilter = new RadioButton();
            RadInterventiMeseAnno = new RadioButton();
            LblMonth = new Label();
            CmbMonth = new ComboBox();
            LblYear = new Label();
            CmbYear = new ComboBox();
            RadInterventiDateRange = new RadioButton();
            LblInterventiDa = new Label();
            DtpInterventiDa = new DateTimePicker();
            LblInterventiA = new Label();
            DtpInterventiA = new DateTimePicker();
            GrpAzione = new GroupBox();
            RadAzioneSoloControllo = new RadioButton();
            RadAzioneControlloFattura = new RadioButton();
            RadAzioneSoloFattura = new RadioButton();
            ChkRiassegna = new CheckBox();
            LblDataFattura = new Label();
            DtpDataFattura = new DateTimePicker();
            ChkFiliale = new CheckBox();
            CmbFiliale = new ComboBox();
            ChkSingoloOrdine = new CheckBox();
            TxtNrOrdine = new TextBox();
            TableLayoutPanelButtons.SuspendLayout();
            GrpInterventiFilter.SuspendLayout();
            GrpAzione.SuspendLayout();
            SuspendLayout();
            // 
            // TableLayoutPanelButtons
            // 
            TableLayoutPanelButtons.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            TableLayoutPanelButtons.ColumnCount = 2;
            TableLayoutPanelButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            TableLayoutPanelButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            TableLayoutPanelButtons.Controls.Add(OK_Button, 0, 0);
            TableLayoutPanelButtons.Controls.Add(Cancel_Button, 1, 0);
            TableLayoutPanelButtons.Location = new Point(220, 448);
            TableLayoutPanelButtons.Margin = new Padding(4, 3, 4, 3);
            TableLayoutPanelButtons.Name = "TableLayoutPanelButtons";
            TableLayoutPanelButtons.RowCount = 1;
            TableLayoutPanelButtons.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            TableLayoutPanelButtons.Size = new Size(170, 33);
            TableLayoutPanelButtons.TabIndex = 0;
            // 
            // OK_Button
            // 
            OK_Button.Anchor = AnchorStyles.None;
            OK_Button.Location = new Point(4, 3);
            OK_Button.Margin = new Padding(4, 3, 4, 3);
            OK_Button.Name = "OK_Button";
            OK_Button.Size = new Size(77, 27);
            OK_Button.TabIndex = 0;
            OK_Button.Text = "OK";
            OK_Button.UseVisualStyleBackColor = true;
            OK_Button.Click += OK_Button_Click;
            // 
            // Cancel_Button
            // 
            Cancel_Button.Anchor = AnchorStyles.None;
            Cancel_Button.DialogResult = DialogResult.Cancel;
            Cancel_Button.Location = new Point(89, 3);
            Cancel_Button.Margin = new Padding(4, 3, 4, 3);
            Cancel_Button.Name = "Cancel_Button";
            Cancel_Button.Size = new Size(77, 27);
            Cancel_Button.TabIndex = 1;
            Cancel_Button.Text = "Annulla";
            Cancel_Button.UseVisualStyleBackColor = true;
            Cancel_Button.Click += Cancel_Button_Click;
            // 
            // GrpInterventiFilter
            // 
            GrpInterventiFilter.Controls.Add(RadInterventiNoFilter);
            GrpInterventiFilter.Controls.Add(RadInterventiMeseAnno);
            GrpInterventiFilter.Controls.Add(LblMonth);
            GrpInterventiFilter.Controls.Add(CmbMonth);
            GrpInterventiFilter.Controls.Add(LblYear);
            GrpInterventiFilter.Controls.Add(CmbYear);
            GrpInterventiFilter.Controls.Add(RadInterventiDateRange);
            GrpInterventiFilter.Controls.Add(LblInterventiDa);
            GrpInterventiFilter.Controls.Add(DtpInterventiDa);
            GrpInterventiFilter.Controls.Add(LblInterventiA);
            GrpInterventiFilter.Controls.Add(DtpInterventiA);
            GrpInterventiFilter.Location = new Point(12, 12);
            GrpInterventiFilter.Name = "GrpInterventiFilter";
            GrpInterventiFilter.Size = new Size(370, 162);
            GrpInterventiFilter.TabIndex = 1;
            GrpInterventiFilter.TabStop = false;
            GrpInterventiFilter.Text = "Filtro Interventi (IntegraInterventi)";
            // 
            // RadInterventiNoFilter
            // 
            RadInterventiNoFilter.AutoSize = true;
            RadInterventiNoFilter.Location = new Point(10, 22);
            RadInterventiNoFilter.Name = "RadInterventiNoFilter";
            RadInterventiNoFilter.Size = new Size(68, 19);
            RadInterventiNoFilter.TabIndex = 0;
            RadInterventiNoFilter.TabStop = true;
            RadInterventiNoFilter.Text = "No filter";
            RadInterventiNoFilter.UseVisualStyleBackColor = true;
            RadInterventiNoFilter.CheckedChanged += RadInterventiFilter_CheckedChanged;
            // 
            // RadInterventiMeseAnno
            // 
            RadInterventiMeseAnno.AutoSize = true;
            RadInterventiMeseAnno.Location = new Point(10, 47);
            RadInterventiMeseAnno.Name = "RadInterventiMeseAnno";
            RadInterventiMeseAnno.Size = new Size(93, 19);
            RadInterventiMeseAnno.TabIndex = 1;
            RadInterventiMeseAnno.TabStop = true;
            RadInterventiMeseAnno.Text = "Mese / Anno";
            RadInterventiMeseAnno.UseVisualStyleBackColor = true;
            RadInterventiMeseAnno.CheckedChanged += RadInterventiFilter_CheckedChanged;
            // 
            // LblMonth
            // 
            LblMonth.AutoSize = true;
            LblMonth.Location = new Point(30, 74);
            LblMonth.Name = "LblMonth";
            LblMonth.Size = new Size(35, 15);
            LblMonth.TabIndex = 3;
            LblMonth.Text = "Mese";
            // 
            // CmbMonth
            // 
            CmbMonth.DropDownStyle = ComboBoxStyle.DropDownList;
            CmbMonth.FormattingEnabled = true;
            CmbMonth.Location = new Point(75, 71);
            CmbMonth.Name = "CmbMonth";
            CmbMonth.Size = new Size(123, 23);
            CmbMonth.TabIndex = 4;
            CmbMonth.SelectedIndexChanged += CmbMonth_SelectedIndexChanged;
            // 
            // LblYear
            // 
            LblYear.AutoSize = true;
            LblYear.Location = new Point(210, 74);
            LblYear.Name = "LblYear";
            LblYear.Size = new Size(36, 15);
            LblYear.TabIndex = 5;
            LblYear.Text = "Anno";
            // 
            // CmbYear
            // 
            CmbYear.DropDownStyle = ComboBoxStyle.DropDownList;
            CmbYear.FormattingEnabled = true;
            CmbYear.Location = new Point(255, 71);
            CmbYear.Name = "CmbYear";
            CmbYear.Size = new Size(80, 23);
            CmbYear.TabIndex = 6;
            CmbYear.SelectedIndexChanged += CmbYear_SelectedIndexChanged;
            // 
            // RadInterventiDateRange
            // 
            RadInterventiDateRange.AutoSize = true;
            RadInterventiDateRange.Location = new Point(10, 100);
            RadInterventiDateRange.Name = "RadInterventiDateRange";
            RadInterventiDateRange.Size = new Size(85, 19);
            RadInterventiDateRange.TabIndex = 2;
            RadInterventiDateRange.TabStop = true;
            RadInterventiDateRange.Text = "Date Da / A";
            RadInterventiDateRange.UseVisualStyleBackColor = true;
            RadInterventiDateRange.CheckedChanged += RadInterventiFilter_CheckedChanged;
            // 
            // LblInterventiDa
            // 
            LblInterventiDa.AutoSize = true;
            LblInterventiDa.Location = new Point(30, 127);
            LblInterventiDa.Name = "LblInterventiDa";
            LblInterventiDa.Size = new Size(21, 15);
            LblInterventiDa.TabIndex = 7;
            LblInterventiDa.Text = "Da";
            // 
            // DtpInterventiDa
            // 
            DtpInterventiDa.Format = DateTimePickerFormat.Short;
            DtpInterventiDa.Location = new Point(75, 123);
            DtpInterventiDa.Name = "DtpInterventiDa";
            DtpInterventiDa.Size = new Size(111, 23);
            DtpInterventiDa.TabIndex = 8;
            DtpInterventiDa.ValueChanged += DtpInterventiDa_ValueChanged;
            // 
            // LblInterventiA
            // 
            LblInterventiA.AutoSize = true;
            LblInterventiA.Location = new Point(210, 127);
            LblInterventiA.Name = "LblInterventiA";
            LblInterventiA.Size = new Size(15, 15);
            LblInterventiA.TabIndex = 9;
            LblInterventiA.Text = "A";
            // 
            // DtpInterventiA
            // 
            DtpInterventiA.Format = DateTimePickerFormat.Short;
            DtpInterventiA.Location = new Point(255, 123);
            DtpInterventiA.Name = "DtpInterventiA";
            DtpInterventiA.Size = new Size(111, 23);
            DtpInterventiA.TabIndex = 10;
            DtpInterventiA.ValueChanged += DtpInterventiA_ValueChanged;
            // 
            // GrpAzione
            // 
            GrpAzione.Controls.Add(RadAzioneSoloControllo);
            GrpAzione.Controls.Add(RadAzioneControlloFattura);
            GrpAzione.Controls.Add(RadAzioneSoloFattura);
            GrpAzione.Controls.Add(ChkRiassegna);
            GrpAzione.Controls.Add(LblDataFattura);
            GrpAzione.Controls.Add(DtpDataFattura);
            GrpAzione.Location = new Point(12, 180);
            GrpAzione.Name = "GrpAzione";
            GrpAzione.Size = new Size(370, 130);
            GrpAzione.TabIndex = 2;
            GrpAzione.TabStop = false;
            GrpAzione.Text = "Cosa fare";
            // 
            // RadAzioneSoloControllo
            // 
            RadAzioneSoloControllo.AutoSize = true;
            RadAzioneSoloControllo.Location = new Point(10, 22);
            RadAzioneSoloControllo.Name = "RadAzioneSoloControllo";
            RadAzioneSoloControllo.Size = new Size(99, 19);
            RadAzioneSoloControllo.TabIndex = 0;
            RadAzioneSoloControllo.TabStop = true;
            RadAzioneSoloControllo.Text = "Solo controllo";
            RadAzioneSoloControllo.UseVisualStyleBackColor = true;
            RadAzioneSoloControllo.CheckedChanged += RadAzione_CheckedChanged;
            // 
            // RadAzioneControlloFattura
            // 
            RadAzioneControlloFattura.AutoSize = true;
            RadAzioneControlloFattura.Location = new Point(10, 47);
            RadAzioneControlloFattura.Name = "RadAzioneControlloFattura";
            RadAzioneControlloFattura.Size = new Size(154, 19);
            RadAzioneControlloFattura.TabIndex = 1;
            RadAzioneControlloFattura.TabStop = true;
            RadAzioneControlloFattura.Text = "Controllo + Fatturazione";
            RadAzioneControlloFattura.UseVisualStyleBackColor = true;
            RadAzioneControlloFattura.CheckedChanged += RadAzione_CheckedChanged;
            // 
            // RadAzioneSoloFattura
            // 
            RadAzioneSoloFattura.AutoSize = true;
            RadAzioneSoloFattura.Location = new Point(10, 72);
            RadAzioneSoloFattura.Name = "RadAzioneSoloFattura";
            RadAzioneSoloFattura.Size = new Size(114, 19);
            RadAzioneSoloFattura.TabIndex = 2;
            RadAzioneSoloFattura.TabStop = true;
            RadAzioneSoloFattura.Text = "Solo fatturazione";
            RadAzioneSoloFattura.UseVisualStyleBackColor = true;
            RadAzioneSoloFattura.CheckedChanged += RadAzione_CheckedChanged;
            // 
            // ChkRiassegna
            // 
            ChkRiassegna.AutoSize = true;
            ChkRiassegna.Location = new Point(180, 22);
            ChkRiassegna.Name = "ChkRiassegna";
            ChkRiassegna.Size = new Size(78, 19);
            ChkRiassegna.TabIndex = 3;
            ChkRiassegna.Text = "Riassegna";
            ChkRiassegna.UseVisualStyleBackColor = true;
            // 
            // LblDataFattura
            // 
            LblDataFattura.AutoSize = true;
            LblDataFattura.Location = new Point(180, 74);
            LblDataFattura.Name = "LblDataFattura";
            LblDataFattura.Size = new Size(57, 15);
            LblDataFattura.TabIndex = 4;
            LblDataFattura.Text = "Data Fatt.";
            // 
            // DtpDataFattura
            // 
            DtpDataFattura.Format = DateTimePickerFormat.Short;
            DtpDataFattura.Location = new Point(250, 70);
            DtpDataFattura.Name = "DtpDataFattura";
            DtpDataFattura.Size = new Size(111, 23);
            DtpDataFattura.TabIndex = 5;
            // 
            // ChkFiliale
            // 
            ChkFiliale.AutoSize = true;
            ChkFiliale.Location = new Point(12, 322);
            ChkFiliale.Name = "ChkFiliale";
            ChkFiliale.Size = new Size(98, 19);
            ChkFiliale.TabIndex = 3;
            ChkFiliale.Text = "Singola Filiale";
            ChkFiliale.UseVisualStyleBackColor = true;
            ChkFiliale.CheckedChanged += ChkFiliale_CheckedChanged;
            // 
            // CmbFiliale
            // 
            CmbFiliale.DropDownStyle = ComboBoxStyle.DropDownList;
            CmbFiliale.FormattingEnabled = true;
            CmbFiliale.Location = new Point(120, 320);
            CmbFiliale.Name = "CmbFiliale";
            CmbFiliale.Size = new Size(200, 23);
            CmbFiliale.TabIndex = 4;
            // 
            // ChkSingoloOrdine
            // 
            ChkSingoloOrdine.AutoSize = true;
            ChkSingoloOrdine.Location = new Point(12, 351);
            ChkSingoloOrdine.Name = "ChkSingoloOrdine";
            ChkSingoloOrdine.Size = new Size(105, 19);
            ChkSingoloOrdine.TabIndex = 5;
            ChkSingoloOrdine.Text = "Singolo Ordine";
            ChkSingoloOrdine.UseVisualStyleBackColor = true;
            ChkSingoloOrdine.CheckedChanged += ChkSingoloOrdine_CheckedChanged;
            // 
            // TxtNrOrdine
            // 
            TxtNrOrdine.Enabled = false;
            TxtNrOrdine.Location = new Point(120, 349);
            TxtNrOrdine.Name = "TxtNrOrdine";
            TxtNrOrdine.Size = new Size(96, 23);
            TxtNrOrdine.TabIndex = 6;
            // 
            // FAskFiltriOrdiniConsuntivo
            // 
            AcceptButton = OK_Button;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = Cancel_Button;
            ClientSize = new Size(394, 493);
            Controls.Add(TxtNrOrdine);
            Controls.Add(ChkSingoloOrdine);
            Controls.Add(CmbFiliale);
            Controls.Add(ChkFiliale);
            Controls.Add(GrpAzione);
            Controls.Add(GrpInterventiFilter);
            Controls.Add(TableLayoutPanelButtons);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Margin = new Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FAskFiltriOrdiniConsuntivo";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Consuntivazione";
            Load += FAskFiltriOrdiniConsuntivo_Load;
            TableLayoutPanelButtons.ResumeLayout(false);
            GrpInterventiFilter.ResumeLayout(false);
            GrpInterventiFilter.PerformLayout();
            GrpAzione.ResumeLayout(false);
            GrpAzione.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        internal TableLayoutPanel TableLayoutPanelButtons;
        internal Button OK_Button;
        internal Button Cancel_Button;
        internal GroupBox GrpInterventiFilter;
        internal RadioButton RadInterventiNoFilter;
        internal RadioButton RadInterventiMeseAnno;
        internal RadioButton RadInterventiDateRange;
        internal Label LblMonth;
        internal ComboBox CmbMonth;
        internal Label LblYear;
        internal ComboBox CmbYear;
        internal Label LblInterventiDa;
        internal DateTimePicker DtpInterventiDa;
        internal Label LblInterventiA;
        internal DateTimePicker DtpInterventiA;
        internal GroupBox GrpAzione;
        internal RadioButton RadAzioneSoloControllo;
        internal RadioButton RadAzioneControlloFattura;
        internal RadioButton RadAzioneSoloFattura;
        internal CheckBox ChkRiassegna;
        internal Label LblDataFattura;
        internal DateTimePicker DtpDataFattura;
        internal CheckBox ChkFiliale;
        internal ComboBox CmbFiliale;
        internal CheckBox ChkSingoloOrdine;
        internal TextBox TxtNrOrdine;
    }
}