namespace FatturazioneContratti.Forms
{
    partial class FLogin
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
            this.grpConn = new System.Windows.Forms.GroupBox();
            this.txtServer = new System.Windows.Forms.TextBox();
            this.lblServer = new System.Windows.Forms.Label();
            this.txtUser = new System.Windows.Forms.TextBox();
            this.lblUser = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.lblPsw = new System.Windows.Forms.Label();
            this.txtLoginId = new System.Windows.Forms.TextBox();
            this.lblLoginId = new System.Windows.Forms.Label();
            this.btnTestConnection = new System.Windows.Forms.Button();
            this.grpAzienda = new System.Windows.Forms.GroupBox();
            this.rdoUno = new System.Windows.Forms.RadioButton();
            this.rdoSpa = new System.Windows.Forms.RadioButton();
            this.chkTestDb = new System.Windows.Forms.CheckBox();
            this.lblDbInUso = new System.Windows.Forms.Label();
            this.chkDebug = new System.Windows.Forms.CheckBox();
            this.grpAzioni = new System.Windows.Forms.GroupBox();
            this.btnGeneraOrdini = new System.Windows.Forms.Button();
            this.btnAdeguamentoIstat = new System.Windows.Forms.Button();
            this.btnConsuntivi = new System.Windows.Forms.Button();
            this.btnAllineaFattureOrdini = new System.Windows.Forms.Button();
            this.lstStato = new System.Windows.Forms.ListBox();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnPulisciLog = new System.Windows.Forms.Button();
            this.grpConn.SuspendLayout();
            this.grpAzienda.SuspendLayout();
            this.grpAzioni.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpConn
            // 
            this.grpConn.Controls.Add(this.btnTestConnection);
            this.grpConn.Controls.Add(this.txtLoginId);
            this.grpConn.Controls.Add(this.lblLoginId);
            this.grpConn.Controls.Add(this.txtPassword);
            this.grpConn.Controls.Add(this.lblPsw);
            this.grpConn.Controls.Add(this.txtUser);
            this.grpConn.Controls.Add(this.lblUser);
            this.grpConn.Controls.Add(this.txtServer);
            this.grpConn.Controls.Add(this.lblServer);
            this.grpConn.Location = new System.Drawing.Point(12, 12);
            this.grpConn.Name = "grpConn";
            this.grpConn.Size = new System.Drawing.Size(560, 80);
            this.grpConn.TabIndex = 0;
            this.grpConn.TabStop = false;
            this.grpConn.Text = "Connessione SQL Server";
            // 
            // txtServer
            // 
            this.txtServer.Location = new System.Drawing.Point(60, 20);
            this.txtServer.Name = "txtServer";
            this.txtServer.Size = new System.Drawing.Size(120, 20);
            this.txtServer.TabIndex = 1;
            // 
            // lblServer
            // 
            this.lblServer.AutoSize = true;
            this.lblServer.Location = new System.Drawing.Point(10, 23);
            this.lblServer.Name = "lblServer";
            this.lblServer.Size = new System.Drawing.Size(41, 13);
            this.lblServer.TabIndex = 0;
            this.lblServer.Text = "Server:";
            // 
            // txtUser
            // 
            this.txtUser.Location = new System.Drawing.Point(230, 20);
            this.txtUser.Name = "txtUser";
            this.txtUser.Size = new System.Drawing.Size(80, 20);
            this.txtUser.TabIndex = 3;
            // 
            // lblUser
            // 
            this.lblUser.AutoSize = true;
            this.lblUser.Location = new System.Drawing.Point(190, 23);
            this.lblUser.Name = "lblUser";
            this.lblUser.Size = new System.Drawing.Size(32, 13);
            this.lblUser.TabIndex = 2;
            this.lblUser.Text = "User:";
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(360, 20);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(80, 20);
            this.txtPassword.TabIndex = 5;
            // 
            // lblPsw
            // 
            this.lblPsw.AutoSize = true;
            this.lblPsw.Location = new System.Drawing.Point(325, 23);
            this.lblPsw.Name = "lblPsw";
            this.lblPsw.Size = new System.Drawing.Size(30, 13);
            this.lblPsw.TabIndex = 4;
            this.lblPsw.Text = "Psw:";
            // 
            // txtLoginId
            // 
            this.txtLoginId.Location = new System.Drawing.Point(60, 48);
            this.txtLoginId.Name = "txtLoginId";
            this.txtLoginId.Size = new System.Drawing.Size(50, 20);
            this.txtLoginId.TabIndex = 7;
            // 
            // lblLoginId
            // 
            this.lblLoginId.AutoSize = true;
            this.lblLoginId.Location = new System.Drawing.Point(10, 51);
            this.lblLoginId.Name = "lblLoginId";
            this.lblLoginId.Size = new System.Drawing.Size(47, 13);
            this.lblLoginId.TabIndex = 6;
            this.lblLoginId.Text = "LoginId:";
            // 
            // btnTestConnection
            // 
            this.btnTestConnection.Location = new System.Drawing.Point(455, 18);
            this.btnTestConnection.Name = "btnTestConnection";
            this.btnTestConnection.Size = new System.Drawing.Size(95, 24);
            this.btnTestConnection.TabIndex = 8;
            this.btnTestConnection.Text = "Test Conn.";
            this.btnTestConnection.UseVisualStyleBackColor = true;
            this.btnTestConnection.Click += new System.EventHandler(this.btnTestConnection_Click);
            // 
            // grpAzienda
            // 
            this.grpAzienda.Controls.Add(this.chkDebug);
            this.grpAzienda.Controls.Add(this.lblDbInUso);
            this.grpAzienda.Controls.Add(this.chkTestDb);
            this.grpAzienda.Controls.Add(this.rdoSpa);
            this.grpAzienda.Controls.Add(this.rdoUno);
            this.grpAzienda.Location = new System.Drawing.Point(12, 98);
            this.grpAzienda.Name = "grpAzienda";
            this.grpAzienda.Size = new System.Drawing.Size(560, 60);
            this.grpAzienda.TabIndex = 1;
            this.grpAzienda.TabStop = false;
            this.grpAzienda.Text = "Database & Azienda";
            // 
            // rdoUno
            // 
            this.rdoUno.AutoSize = true;
            this.rdoUno.Checked = true;
            this.rdoUno.Location = new System.Drawing.Point(15, 20);
            this.rdoUno.Name = "rdoUno";
            this.rdoUno.Size = new System.Drawing.Size(89, 17);
            this.rdoUno.TabIndex = 0;
            this.rdoUno.TabStop = true;
            this.rdoUno.Text = "Azienda UNO";
            this.rdoUno.UseVisualStyleBackColor = true;
            this.rdoUno.CheckedChanged += new System.EventHandler(this.rdoAzienda_CheckedChanged);
            // 
            // rdoSpa
            // 
            this.rdoSpa.AutoSize = true;
            this.rdoSpa.Location = new System.Drawing.Point(115, 20);
            this.rdoSpa.Name = "rdoSpa";
            this.rdoSpa.Size = new System.Drawing.Size(87, 17);
            this.rdoSpa.TabIndex = 1;
            this.rdoSpa.Text = "Azienda SPA";
            this.rdoSpa.UseVisualStyleBackColor = true;
            this.rdoSpa.CheckedChanged += new System.EventHandler(this.rdoAzienda_CheckedChanged);
            // 
            // chkTestDb
            // 
            this.chkTestDb.AutoSize = true;
            this.chkTestDb.Location = new System.Drawing.Point(220, 20);
            this.chkTestDb.Name = "chkTestDb";
            this.chkTestDb.Size = new System.Drawing.Size(117, 17);
            this.chkTestDb.TabIndex = 2;
            this.chkTestDb.Text = "Usa Database Test";
            this.chkTestDb.UseVisualStyleBackColor = true;
            this.chkTestDb.CheckedChanged += new System.EventHandler(this.chkTestDb_CheckedChanged);
            // 
            // lblDbInUso
            // 
            this.lblDbInUso.AutoSize = true;
            this.lblDbInUso.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDbInUso.Location = new System.Drawing.Point(12, 40);
            this.lblDbInUso.Name = "lblDbInUso";
            this.lblDbInUso.Size = new System.Drawing.Size(102, 13);
            this.lblDbInUso.TabIndex = 3;
            this.lblDbInUso.Text = "Database: (None)";
            // 
            // chkDebug
            // 
            this.chkDebug.AutoSize = true;
            this.chkDebug.Location = new System.Drawing.Point(450, 20);
            this.chkDebug.Name = "chkDebug";
            this.chkDebug.Size = new System.Drawing.Size(95, 17);
            this.chkDebug.TabIndex = 4;
            this.chkDebug.Text = "Log Debug ON";
            this.chkDebug.UseVisualStyleBackColor = true;
            // 
            // grpAzioni
            // 
            this.grpAzioni.Controls.Add(this.btnAllineaFattureOrdini);
            this.grpAzioni.Controls.Add(this.btnConsuntivi);
            this.grpAzioni.Controls.Add(this.btnAdeguamentoIstat);
            this.grpAzioni.Controls.Add(this.btnGeneraOrdini);
            this.grpAzioni.Location = new System.Drawing.Point(12, 164);
            this.grpAzioni.Name = "grpAzioni";
            this.grpAzioni.Size = new System.Drawing.Size(560, 95);
            this.grpAzioni.TabIndex = 2;
            this.grpAzioni.TabStop = false;
            this.grpAzioni.Text = "Operazioni di Elaborazione";
            // 
            // btnGeneraOrdini
            // 
            this.btnGeneraOrdini.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGeneraOrdini.Location = new System.Drawing.Point(15, 23);
            this.btnGeneraOrdini.Name = "btnGeneraOrdini";
            this.btnGeneraOrdini.Size = new System.Drawing.Size(125, 55);
            this.btnGeneraOrdini.TabIndex = 0;
            this.btnGeneraOrdini.Text = "Genera Canoni da Ordini";
            this.btnGeneraOrdini.UseVisualStyleBackColor = true;
            this.btnGeneraOrdini.Click += new System.EventHandler(this.btnGeneraOrdini_Click);
            // 
            // btnAdeguamentoIstat
            // 
            this.btnAdeguamentoIstat.Location = new System.Drawing.Point(150, 23);
            this.btnAdeguamentoIstat.Name = "btnAdeguamentoIstat";
            this.btnAdeguamentoIstat.Size = new System.Drawing.Size(125, 55);
            this.btnAdeguamentoIstat.TabIndex = 1;
            this.btnAdeguamentoIstat.Text = "Adeguamento ISTAT";
            this.btnAdeguamentoIstat.UseVisualStyleBackColor = true;
            this.btnAdeguamentoIstat.Click += new System.EventHandler(this.btnAdeguamentoIstat_Click);
            // 
            // btnConsuntivi
            // 
            this.btnConsuntivi.Location = new System.Drawing.Point(285, 23);
            this.btnConsuntivi.Name = "btnConsuntivi";
            this.btnConsuntivi.Size = new System.Drawing.Size(125, 55);
            this.btnConsuntivi.TabIndex = 2;
            this.btnConsuntivi.Text = "Consuntivazione / Flusso Integra";
            this.btnConsuntivi.UseVisualStyleBackColor = true;
            this.btnConsuntivi.Click += new System.EventHandler(this.btnConsuntivi_Click);
            // 
            // btnAllineaFattureOrdini
            // 
            this.btnAllineaFattureOrdini.Location = new System.Drawing.Point(420, 23);
            this.btnAllineaFattureOrdini.Name = "btnAllineaFattureOrdini";
            this.btnAllineaFattureOrdini.Size = new System.Drawing.Size(125, 55);
            this.btnAllineaFattureOrdini.TabIndex = 3;
            this.btnAllineaFattureOrdini.Text = "Allinea Canoni Fatture";
            this.btnAllineaFattureOrdini.UseVisualStyleBackColor = true;
            this.btnAllineaFattureOrdini.Click += new System.EventHandler(this.btnAllineaFattureOrdini_Click);
            // 
            // lstStato
            // 
            this.lstStato.FormattingEnabled = true;
            this.lstStato.Location = new System.Drawing.Point(12, 268);
            this.lstStato.Name = "lstStato";
            this.lstStato.Size = new System.Drawing.Size(560, 160);
            this.lstStato.TabIndex = 3;
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(12, 452);
            this.progressBar.Maximum = 100;
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(560, 18);
            this.progressBar.TabIndex = 4;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(12, 434);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(38, 13);
            this.lblStatus.TabIndex = 5;
            this.lblStatus.Text = "Pronto";
            // 
            // btnPulisciLog
            // 
            this.btnPulisciLog.Location = new System.Drawing.Point(482, 431);
            this.btnPulisciLog.Name = "btnPulisciLog";
            this.btnPulisciLog.Size = new System.Drawing.Size(90, 20);
            this.btnPulisciLog.TabIndex = 6;
            this.btnPulisciLog.Text = "Pulisci Log";
            this.btnPulisciLog.UseVisualStyleBackColor = true;
            this.btnPulisciLog.Click += new System.EventHandler(this.btnPulisciLog_Click);
            // 
            // FLogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 481);
            this.Controls.Add(this.btnPulisciLog);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.lstStato);
            this.Controls.Add(this.grpAzioni);
            this.Controls.Add(this.grpAzienda);
            this.Controls.Add(this.grpConn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FLogin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Gestione Contratti & Fatturazione - Mago.NET";
            this.Load += new System.EventHandler(this.FLogin_Load);
            this.grpConn.ResumeLayout(false);
            this.grpConn.PerformLayout();
            this.grpAzienda.ResumeLayout(false);
            this.grpAzienda.PerformLayout();
            this.grpAzioni.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox grpConn;
        private System.Windows.Forms.TextBox txtServer;
        private System.Windows.Forms.Label lblServer;
        private System.Windows.Forms.TextBox txtUser;
        private System.Windows.Forms.Label lblUser;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label lblPsw;
        private System.Windows.Forms.TextBox txtLoginId;
        private System.Windows.Forms.Label lblLoginId;
        private System.Windows.Forms.Button btnTestConnection;
        private System.Windows.Forms.GroupBox grpAzienda;
        private System.Windows.Forms.RadioButton rdoUno;
        private System.Windows.Forms.RadioButton rdoSpa;
        private System.Windows.Forms.CheckBox chkTestDb;
        private System.Windows.Forms.Label lblDbInUso;
        private System.Windows.Forms.CheckBox chkDebug;
        private System.Windows.Forms.GroupBox grpAzioni;
        private System.Windows.Forms.Button btnGeneraOrdini;
        private System.Windows.Forms.Button btnAdeguamentoIstat;
        private System.Windows.Forms.Button btnConsuntivi;
        private System.Windows.Forms.Button btnAllineaFattureOrdini;
        private System.Windows.Forms.ListBox lstStato;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btnPulisciLog;
    }
}