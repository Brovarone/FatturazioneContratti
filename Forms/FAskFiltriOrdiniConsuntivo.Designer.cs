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
            this.TableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.OK_Button = new System.Windows.Forms.Button();
            this.Cancel_Button = new System.Windows.Forms.Button();
            this.LblMonth = new System.Windows.Forms.Label();
            this.RadOnlyCheck = new System.Windows.Forms.RadioButton();
            this.RadCheckAndFatt = new System.Windows.Forms.RadioButton();
            this.RadOnlyFatt = new System.Windows.Forms.RadioButton();
            this.Label1 = new System.Windows.Forms.Label();
            this.DtaFatt = new System.Windows.Forms.DateTimePicker();
            this.CmbMonth = new System.Windows.Forms.ComboBox();
            this.TxtOrdFiliale = new System.Windows.Forms.TextBox();
            this.ChkFiliale = new System.Windows.Forms.CheckBox();
            this.firstLogDate = new System.Windows.Forms.DateTimePicker();
            this.lastLogDate = new System.Windows.Forms.DateTimePicker();
            this.ChkNoFilter = new System.Windows.Forms.CheckBox();
            this.CmbYear = new System.Windows.Forms.ComboBox();
            this.LblYear = new System.Windows.Forms.Label();
            this.ChkRiassegna = new System.Windows.Forms.CheckBox();
            this.GroupBoxCheck = new System.Windows.Forms.GroupBox();
            this.TableLayoutPanel1.SuspendLayout();
            this.GroupBoxCheck.SuspendLayout();
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
            this.TableLayoutPanel1.Location = new System.Drawing.Point(154, 291);
            this.TableLayoutPanel1.Name = "TableLayoutPanel1";
            this.TableLayoutPanel1.RowCount = 1;
            this.TableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableLayoutPanel1.Size = new System.Drawing.Size(146, 29);
            this.TableLayoutPanel1.TabIndex = 0;
            // 
            // OK_Button
            // 
            this.OK_Button.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.OK_Button.Location = new System.Drawing.Point(3, 3);
            this.OK_Button.Name = "OK_Button";
            this.OK_Button.Size = new System.Drawing.Size(67, 23);
            this.OK_Button.TabIndex = 0;
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
            this.Cancel_Button.TabIndex = 1;
            this.Cancel_Button.Text = "Annulla";
            this.Cancel_Button.Click += new System.EventHandler(this.Cancel_Button_Click);
            // 
            // LblMonth
            // 
            this.LblMonth.AutoSize = true;
            this.LblMonth.Location = new System.Drawing.Point(12, 36);
            this.LblMonth.Name = "LblMonth";
            this.LblMonth.Size = new System.Drawing.Size(33, 13);
            this.LblMonth.TabIndex = 2;
            this.LblMonth.Text = "Mese";
            // 
            // RadOnlyCheck
            // 
            this.RadOnlyCheck.AutoSize = true;
            this.RadOnlyCheck.Checked = true;
            this.RadOnlyCheck.Enabled = false;
            this.RadOnlyCheck.Location = new System.Drawing.Point(6, 19);
            this.RadOnlyCheck.Name = "RadOnlyCheck";
            this.RadOnlyCheck.Size = new System.Drawing.Size(90, 17);
            this.RadOnlyCheck.TabIndex = 3;
            this.RadOnlyCheck.TabStop = true;
            this.RadOnlyCheck.Text = "Solo Controllo";
            this.RadOnlyCheck.UseVisualStyleBackColor = true;
            // 
            // RadCheckAndFatt
            // 
            this.RadCheckAndFatt.AutoSize = true;
            this.RadCheckAndFatt.Enabled = false;
            this.RadCheckAndFatt.Location = new System.Drawing.Point(6, 42);
            this.RadCheckAndFatt.Name = "RadCheckAndFatt";
            this.RadCheckAndFatt.Size = new System.Drawing.Size(136, 17);
            this.RadCheckAndFatt.TabIndex = 4;
            this.RadCheckAndFatt.Text = "Controllo + Fatturazione";
            this.RadCheckAndFatt.UseVisualStyleBackColor = true;
            // 
            // RadOnlyFatt
            // 
            this.RadOnlyFatt.AutoSize = true;
            this.RadOnlyFatt.Enabled = false;
            this.RadOnlyFatt.Location = new System.Drawing.Point(6, 87);
            this.RadOnlyFatt.Name = "RadOnlyFatt";
            this.RadOnlyFatt.Size = new System.Drawing.Size(107, 17);
            this.RadOnlyFatt.TabIndex = 5;
            this.RadOnlyFatt.Text = "Solo Fatturazione";
            this.RadOnlyFatt.UseVisualStyleBackColor = true;
            this.RadOnlyFatt.CheckedChanged += new System.EventHandler(this.RadOnlyFatt_CheckedChanged);
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Location = new System.Drawing.Point(12, 223);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(54, 13);
            this.Label1.TabIndex = 7;
            this.Label1.Text = "Data Fatt.";
            // 
            // DtaFatt
            // 
            this.DtaFatt.Enabled = false;
            this.DtaFatt.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.DtaFatt.Location = new System.Drawing.Point(72, 220);
            this.DtaFatt.Name = "DtaFatt";
            this.DtaFatt.Size = new System.Drawing.Size(96, 20);
            this.DtaFatt.TabIndex = 8;
            // 
            // CmbMonth
            // 
            this.CmbMonth.Enabled = false;
            this.CmbMonth.FormattingEnabled = true;
            this.CmbMonth.Location = new System.Drawing.Point(72, 33);
            this.CmbMonth.Name = "CmbMonth";
            this.CmbMonth.Size = new System.Drawing.Size(106, 21);
            this.CmbMonth.TabIndex = 9;
            this.CmbMonth.SelectedIndexChanged += new System.EventHandler(this.CmbMonth_SelectedIndexChanged);
            // 
            // TxtOrdFiliale
            // 
            this.TxtOrdFiliale.Enabled = false;
            this.TxtOrdFiliale.Location = new System.Drawing.Point(107, 246);
            this.TxtOrdFiliale.Name = "TxtOrdFiliale";
            this.TxtOrdFiliale.Size = new System.Drawing.Size(96, 20);
            this.TxtOrdFiliale.TabIndex = 12;
            // 
            // ChkFiliale
            // 
            this.ChkFiliale.AutoSize = true;
            this.ChkFiliale.Enabled = false;
            this.ChkFiliale.Location = new System.Drawing.Point(6, 246);
            this.ChkFiliale.Name = "ChkFiliale";
            this.ChkFiliale.Size = new System.Drawing.Size(90, 17);
            this.ChkFiliale.TabIndex = 11;
            this.ChkFiliale.Text = "Singola Filiale";
            this.ChkFiliale.UseVisualStyleBackColor = true;
            this.ChkFiliale.CheckedChanged += new System.EventHandler(this.ChkFiliale_CheckedChanged);
            // 
            // firstLogDate
            // 
            this.firstLogDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.firstLogDate.Location = new System.Drawing.Point(201, 33);
            this.firstLogDate.Name = "firstLogDate";
            this.firstLogDate.Size = new System.Drawing.Size(96, 20);
            this.firstLogDate.TabIndex = 13;
            // 
            // lastLogDate
            // 
            this.lastLogDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.lastLogDate.Location = new System.Drawing.Point(201, 59);
            this.lastLogDate.Name = "lastLogDate";
            this.lastLogDate.Size = new System.Drawing.Size(96, 20);
            this.lastLogDate.TabIndex = 14;
            // 
            // ChkNoFilter
            // 
            this.ChkNoFilter.AutoSize = true;
            this.ChkNoFilter.Location = new System.Drawing.Point(15, 10);
            this.ChkNoFilter.Name = "ChkNoFilter";
            this.ChkNoFilter.Size = new System.Drawing.Size(48, 17);
            this.ChkNoFilter.TabIndex = 15;
            this.ChkNoFilter.Text = "Filtra";
            this.ChkNoFilter.UseVisualStyleBackColor = true;
            this.ChkNoFilter.CheckedChanged += new System.EventHandler(this.ChkNoFilter_CheckedChanged);
            // 
            // CmbYear
            // 
            this.CmbYear.Enabled = false;
            this.CmbYear.FormattingEnabled = true;
            this.CmbYear.Location = new System.Drawing.Point(72, 62);
            this.CmbYear.Name = "CmbYear";
            this.CmbYear.Size = new System.Drawing.Size(106, 21);
            this.CmbYear.TabIndex = 17;
            this.CmbYear.SelectedIndexChanged += new System.EventHandler(this.CmbYear_SelectedIndexChanged);
            // 
            // LblYear
            // 
            this.LblYear.AutoSize = true;
            this.LblYear.Location = new System.Drawing.Point(12, 65);
            this.LblYear.Name = "LblYear";
            this.LblYear.Size = new System.Drawing.Size(32, 13);
            this.LblYear.TabIndex = 16;
            this.LblYear.Text = "Anno";
            // 
            // ChkRiassegna
            // 
            this.ChkRiassegna.AutoSize = true;
            this.ChkRiassegna.Enabled = false;
            this.ChkRiassegna.Location = new System.Drawing.Point(6, 65);
            this.ChkRiassegna.Name = "ChkRiassegna";
            this.ChkRiassegna.Size = new System.Drawing.Size(76, 17);
            this.ChkRiassegna.TabIndex = 18;
            this.ChkRiassegna.Text = "Riassegna";
            this.ChkRiassegna.UseVisualStyleBackColor = true;
            // 
            // GroupBoxCheck
            // 
            this.GroupBoxCheck.Controls.Add(this.RadOnlyCheck);
            this.GroupBoxCheck.Controls.Add(this.ChkRiassegna);
            this.GroupBoxCheck.Controls.Add(this.RadCheckAndFatt);
            this.GroupBoxCheck.Controls.Add(this.RadOnlyFatt);
            this.GroupBoxCheck.Location = new System.Drawing.Point(72, 89);
            this.GroupBoxCheck.Name = "GroupBoxCheck";
            this.GroupBoxCheck.Size = new System.Drawing.Size(200, 114);
            this.GroupBoxCheck.TabIndex = 19;
            this.GroupBoxCheck.TabStop = false;
            // 
            // FAskFiltriOrdiniConsuntivo
            // 
            this.AcceptButton = this.OK_Button;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.Cancel_Button;
            this.ClientSize = new System.Drawing.Size(312, 332);
            this.Controls.Add(this.GroupBoxCheck);
            this.Controls.Add(this.CmbYear);
            this.Controls.Add(this.LblYear);
            this.Controls.Add(this.ChkNoFilter);
            this.Controls.Add(this.lastLogDate);
            this.Controls.Add(this.firstLogDate);
            this.Controls.Add(this.TxtOrdFiliale);
            this.Controls.Add(this.ChkFiliale);
            this.Controls.Add(this.CmbMonth);
            this.Controls.Add(this.DtaFatt);
            this.Controls.Add(this.Label1);
            this.Controls.Add(this.LblMonth);
            this.Controls.Add(this.TableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FAskFiltriOrdiniConsuntivo";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Consuntivazione";
            this.Load += new System.EventHandler(this.FAskFiltriOrdiniConsuntivo_Load);
            this.TableLayoutPanel1.ResumeLayout(false);
            this.GroupBoxCheck.ResumeLayout(false);
            this.GroupBoxCheck.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.TableLayoutPanel TableLayoutPanel1;
        internal System.Windows.Forms.Button OK_Button;
        internal System.Windows.Forms.Button Cancel_Button;
        internal System.Windows.Forms.Label LblMonth;
        internal System.Windows.Forms.RadioButton RadOnlyCheck;
        internal System.Windows.Forms.RadioButton RadCheckAndFatt;
        internal System.Windows.Forms.RadioButton RadOnlyFatt;
        internal System.Windows.Forms.Label Label1;
        internal System.Windows.Forms.DateTimePicker DtaFatt;
        internal System.Windows.Forms.ComboBox CmbMonth;
        internal System.Windows.Forms.TextBox TxtOrdFiliale;
        internal System.Windows.Forms.CheckBox ChkFiliale;
        internal System.Windows.Forms.DateTimePicker firstLogDate;
        internal System.Windows.Forms.DateTimePicker lastLogDate;
        internal System.Windows.Forms.CheckBox ChkNoFilter;
        internal System.Windows.Forms.ComboBox CmbYear;
        internal System.Windows.Forms.Label LblYear;
        internal System.Windows.Forms.CheckBox ChkRiassegna;
        internal System.Windows.Forms.GroupBox GroupBoxCheck;
    }
}