namespace FatturazioneContratti.Forms
{
    partial class FAskFiltriAllineamentoFatture
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
            lblDa = new Label();
            dtpDa = new DateTimePicker();
            lblA = new Label();
            dtpA = new DateTimePicker();
            grpNumeri = new GroupBox();
            radTuttiNumeri = new RadioButton();
            radIntervalloNumeri = new RadioButton();
            lblNrFirst = new Label();
            txtNrFirst = new TextBox();
            lblNrLast = new Label();
            txtNrLast = new TextBox();
            btnOk = new Button();
            btnCancel = new Button();
            grpNumeri.SuspendLayout();
            SuspendLayout();
            // 
            // lblDa
            // 
            lblDa.AutoSize = true;
            lblDa.Location = new Point(18, 21);
            lblDa.Margin = new Padding(4, 0, 4, 0);
            lblDa.Name = "lblDa";
            lblDa.Size = new Size(88, 15);
            lblDa.TabIndex = 0;
            lblDa.Text = "Data fattura da:";
            // 
            // dtpDa
            // 
            dtpDa.Format = DateTimePickerFormat.Short;
            dtpDa.Location = new Point(140, 16);
            dtpDa.Margin = new Padding(4, 3, 4, 3);
            dtpDa.Name = "dtpDa";
            dtpDa.Size = new Size(111, 23);
            dtpDa.TabIndex = 1;
            dtpDa.ValueChanged += DtpDa_ValueChanged;
            // 
            // lblA
            // 
            lblA.AutoSize = true;
            lblA.Location = new Point(18, 55);
            lblA.Margin = new Padding(4, 0, 4, 0);
            lblA.Name = "lblA";
            lblA.Size = new Size(81, 15);
            lblA.TabIndex = 2;
            lblA.Text = "Data fattura a:";
            // 
            // dtpA
            // 
            dtpA.Format = DateTimePickerFormat.Short;
            dtpA.Location = new Point(140, 51);
            dtpA.Margin = new Padding(4, 3, 4, 3);
            dtpA.Name = "dtpA";
            dtpA.Size = new Size(111, 23);
            dtpA.TabIndex = 3;
            // 
            // grpNumeri
            // 
            grpNumeri.Controls.Add(radTuttiNumeri);
            grpNumeri.Controls.Add(radIntervalloNumeri);
            grpNumeri.Controls.Add(lblNrFirst);
            grpNumeri.Controls.Add(txtNrFirst);
            grpNumeri.Controls.Add(lblNrLast);
            grpNumeri.Controls.Add(txtNrLast);
            grpNumeri.Location = new Point(18, 87);
            grpNumeri.Margin = new Padding(4, 3, 4, 3);
            grpNumeri.Name = "grpNumeri";
            grpNumeri.Padding = new Padding(4, 3, 4, 3);
            grpNumeri.Size = new Size(257, 144);
            grpNumeri.TabIndex = 4;
            grpNumeri.TabStop = false;
            grpNumeri.Text = "Numeri documento";
            // 
            // radTuttiNumeri
            // 
            radTuttiNumeri.AutoSize = true;
            radTuttiNumeri.Checked = true;
            radTuttiNumeri.Location = new Point(12, 28);
            radTuttiNumeri.Margin = new Padding(4, 3, 4, 3);
            radTuttiNumeri.Name = "radTuttiNumeri";
            radTuttiNumeri.Size = new Size(96, 19);
            radTuttiNumeri.TabIndex = 0;
            radTuttiNumeri.TabStop = true;
            radTuttiNumeri.Text = "Tutti i numeri";
            radTuttiNumeri.UseVisualStyleBackColor = true;
            radTuttiNumeri.CheckedChanged += RadTuttiNumeri_CheckedChanged;
            // 
            // radIntervalloNumeri
            // 
            radIntervalloNumeri.AutoSize = true;
            radIntervalloNumeri.Location = new Point(12, 55);
            radIntervalloNumeri.Margin = new Padding(4, 3, 4, 3);
            radIntervalloNumeri.Name = "radIntervalloNumeri";
            radIntervalloNumeri.Size = new Size(149, 19);
            radIntervalloNumeri.TabIndex = 1;
            radIntervalloNumeri.Text = "Intervallo primo/ultimo";
            radIntervalloNumeri.UseVisualStyleBackColor = true;
            radIntervalloNumeri.CheckedChanged += RadIntervalloNumeri_CheckedChanged;
            // 
            // lblNrFirst
            // 
            lblNrFirst.AutoSize = true;
            lblNrFirst.Enabled = false;
            lblNrFirst.Location = new Point(12, 88);
            lblNrFirst.Margin = new Padding(4, 0, 4, 0);
            lblNrFirst.Name = "lblNrFirst";
            lblNrFirst.Size = new Size(87, 15);
            lblNrFirst.TabIndex = 2;
            lblNrFirst.Text = "Primo numero:";
            // 
            // txtNrFirst
            // 
            txtNrFirst.Enabled = false;
            txtNrFirst.Location = new Point(111, 84);
            txtNrFirst.Margin = new Padding(4, 3, 4, 3);
            txtNrFirst.Name = "txtNrFirst";
            txtNrFirst.Size = new Size(111, 23);
            txtNrFirst.TabIndex = 3;
            // 
            // lblNrLast
            // 
            lblNrLast.AutoSize = true;
            lblNrLast.Enabled = false;
            lblNrLast.Location = new Point(12, 115);
            lblNrLast.Margin = new Padding(4, 0, 4, 0);
            lblNrLast.Name = "lblNrLast";
            lblNrLast.Size = new Size(91, 15);
            lblNrLast.TabIndex = 4;
            lblNrLast.Text = "Ultimo numero:";
            // 
            // txtNrLast
            // 
            txtNrLast.Enabled = false;
            txtNrLast.Location = new Point(111, 112);
            txtNrLast.Margin = new Padding(4, 3, 4, 3);
            txtNrLast.Name = "txtNrLast";
            txtNrLast.Size = new Size(111, 23);
            txtNrLast.TabIndex = 5;
            // 
            // btnOk
            // 
            btnOk.DialogResult = DialogResult.OK;
            btnOk.Location = new Point(93, 237);
            btnOk.Margin = new Padding(4, 3, 4, 3);
            btnOk.Name = "btnOk";
            btnOk.Size = new Size(88, 30);
            btnOk.TabIndex = 5;
            btnOk.Text = "OK";
            btnOk.UseVisualStyleBackColor = true;
            btnOk.Click += BtnOk_Click;
            // 
            // btnCancel
            // 
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new Point(187, 237);
            btnCancel.Margin = new Padding(4, 3, 4, 3);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(88, 30);
            btnCancel.TabIndex = 6;
            btnCancel.Text = "Annulla";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // FAskFiltriAllineamentoFatture
            // 
            AcceptButton = btnOk;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = btnCancel;
            ClientSize = new Size(292, 283);
            Controls.Add(lblDa);
            Controls.Add(dtpDa);
            Controls.Add(lblA);
            Controls.Add(dtpA);
            Controls.Add(grpNumeri);
            Controls.Add(btnOk);
            Controls.Add(btnCancel);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Margin = new Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FAskFiltriAllineamentoFatture";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Allineamento canoni fatture";
            grpNumeri.ResumeLayout(false);
            grpNumeri.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblDa;
        private System.Windows.Forms.DateTimePicker dtpDa;
        private System.Windows.Forms.Label lblA;
        private System.Windows.Forms.DateTimePicker dtpA;
        private System.Windows.Forms.GroupBox grpNumeri;
        private System.Windows.Forms.RadioButton radTuttiNumeri;
        private System.Windows.Forms.RadioButton radIntervalloNumeri;
        private System.Windows.Forms.Label lblNrFirst;
        private System.Windows.Forms.TextBox txtNrFirst;
        private System.Windows.Forms.Label lblNrLast;
        private System.Windows.Forms.TextBox txtNrLast;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
    }
}