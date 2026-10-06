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
            menuStripPrincipal = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            esciToolStripMenuItem = new ToolStripMenuItem();
            strumentiToolStripMenuItem = new ToolStripMenuItem();
            configurazioneAvanzataToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            apriJsonToolStripMenuItem = new ToolStripMenuItem();
            apriCartellaSettingsToolStripMenuItem = new ToolStripMenuItem();
            apriCartellaLogToolStripMenuItem = new ToolStripMenuItem();
            moduliToolStripMenuItem = new ToolStripMenuItem();
            contrattiFatturazioneToolStripMenuItem = new ToolStripMenuItem();
            pagheToolStripMenuItem = new ToolStripMenuItem();
            grpConn = new GroupBox();
            btnTestConnection = new Button();
            txtLoginId = new TextBox();
            lblLoginId = new Label();
            txtPassword = new TextBox();
            lblPsw = new Label();
            txtUser = new TextBox();
            lblUser = new Label();
            txtServer = new TextBox();
            lblServer = new Label();
            grpAzienda = new GroupBox();
            chkDebug = new CheckBox();
            lblDbInUso = new Label();
            chkTestDb = new CheckBox();
            rdoSpa = new RadioButton();
            rdoUno = new RadioButton();
            grpAzioni = new GroupBox();
            btnAllineaFattureOrdini = new Button();
            btnConsuntivi = new Button();
            btnAdeguamentoIstat = new Button();
            btnGeneraOrdini = new Button();
            lstStato = new ListBox();
            progressBar = new ProgressBar();
            lblStatus = new Label();
            btnPulisciLog = new Button();
            subProgressBar = new ProgressBar();
            menuStripPrincipal.SuspendLayout();
            grpConn.SuspendLayout();
            grpAzienda.SuspendLayout();
            grpAzioni.SuspendLayout();
            SuspendLayout();
            // 
            // menuStripPrincipal
            // 
            menuStripPrincipal.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, strumentiToolStripMenuItem, moduliToolStripMenuItem });
            menuStripPrincipal.Location = new Point(0, 0);
            menuStripPrincipal.Name = "menuStripPrincipal";
            menuStripPrincipal.Padding = new Padding(7, 2, 0, 2);
            menuStripPrincipal.Size = new Size(681, 24);
            menuStripPrincipal.TabIndex = 0;
            menuStripPrincipal.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { esciToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(37, 20);
            fileToolStripMenuItem.Text = "&File";
            // 
            // esciToolStripMenuItem
            // 
            esciToolStripMenuItem.Name = "esciToolStripMenuItem";
            esciToolStripMenuItem.ShortcutKeys = Keys.Alt | Keys.F4;
            esciToolStripMenuItem.Size = new Size(136, 22);
            esciToolStripMenuItem.Text = "&Esci";
            esciToolStripMenuItem.Click += esciToolStripMenuItem_Click;
            // 
            // strumentiToolStripMenuItem
            // 
            strumentiToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { configurazioneAvanzataToolStripMenuItem, toolStripSeparator1, apriJsonToolStripMenuItem, apriCartellaSettingsToolStripMenuItem, apriCartellaLogToolStripMenuItem });
            strumentiToolStripMenuItem.Name = "strumentiToolStripMenuItem";
            strumentiToolStripMenuItem.Size = new Size(71, 20);
            strumentiToolStripMenuItem.Text = "&Strumenti";
            // 
            // configurazioneAvanzataToolStripMenuItem
            // 
            configurazioneAvanzataToolStripMenuItem.Name = "configurazioneAvanzataToolStripMenuItem";
            configurazioneAvanzataToolStripMenuItem.Size = new Size(236, 22);
            configurazioneAvanzataToolStripMenuItem.Text = "&Configurazione DB e Percorsi...";
            configurazioneAvanzataToolStripMenuItem.Click += configurazioneAvanzataToolStripMenuItem_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(233, 6);
            // 
            // apriJsonToolStripMenuItem
            // 
            apriJsonToolStripMenuItem.Name = "apriJsonToolStripMenuItem";
            apriJsonToolStripMenuItem.Size = new Size(236, 22);
            apriJsonToolStripMenuItem.Text = "Modifica file &settings.json";
            apriJsonToolStripMenuItem.Click += apriJsonToolStripMenuItem_Click;
            // 
            // apriCartellaSettingsToolStripMenuItem
            // 
            apriCartellaSettingsToolStripMenuItem.Name = "apriCartellaSettingsToolStripMenuItem";
            apriCartellaSettingsToolStripMenuItem.Size = new Size(236, 22);
            apriCartellaSettingsToolStripMenuItem.Text = "Apri Cartella &Impostazioni";
            apriCartellaSettingsToolStripMenuItem.Click += apriCartellaSettingsToolStripMenuItem_Click;
            // 
            // apriCartellaLogToolStripMenuItem
            // 
            apriCartellaLogToolStripMenuItem.Name = "apriCartellaLogToolStripMenuItem";
            apriCartellaLogToolStripMenuItem.Size = new Size(236, 22);
            apriCartellaLogToolStripMenuItem.Text = "Apri Cartella &Log";
            apriCartellaLogToolStripMenuItem.Click += apriCartellaLogToolStripMenuItem_Click;
            // 
            // moduliToolStripMenuItem
            // 
            moduliToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { contrattiFatturazioneToolStripMenuItem, pagheToolStripMenuItem });
            moduliToolStripMenuItem.Name = "moduliToolStripMenuItem";
            moduliToolStripMenuItem.Size = new Size(57, 20);
            moduliToolStripMenuItem.Text = "&Moduli";
            // 
            // contrattiFatturazioneToolStripMenuItem
            // 
            contrattiFatturazioneToolStripMenuItem.Checked = true;
            contrattiFatturazioneToolStripMenuItem.CheckState = CheckState.Checked;
            contrattiFatturazioneToolStripMenuItem.Name = "contrattiFatturazioneToolStripMenuItem";
            contrattiFatturazioneToolStripMenuItem.Size = new Size(192, 22);
            contrattiFatturazioneToolStripMenuItem.Text = "Contratti & Fatturazione";
            // 
            // pagheToolStripMenuItem
            // 
            pagheToolStripMenuItem.Name = "pagheToolStripMenuItem";
            pagheToolStripMenuItem.Size = new Size(192, 22);
            pagheToolStripMenuItem.Text = "Flusso &Paghe";
            pagheToolStripMenuItem.Click += pagheToolStripMenuItem_Click;
            // 
            // grpConn
            // 
            grpConn.Controls.Add(btnTestConnection);
            grpConn.Controls.Add(txtLoginId);
            grpConn.Controls.Add(lblLoginId);
            grpConn.Controls.Add(txtPassword);
            grpConn.Controls.Add(lblPsw);
            grpConn.Controls.Add(txtUser);
            grpConn.Controls.Add(lblUser);
            grpConn.Controls.Add(txtServer);
            grpConn.Controls.Add(lblServer);
            grpConn.Location = new Point(14, 37);
            grpConn.Margin = new Padding(4, 3, 4, 3);
            grpConn.Name = "grpConn";
            grpConn.Padding = new Padding(4, 3, 4, 3);
            grpConn.Size = new Size(653, 92);
            grpConn.TabIndex = 1;
            grpConn.TabStop = false;
            grpConn.Text = "Connessione SQL Server";
            // 
            // btnTestConnection
            // 
            btnTestConnection.Location = new Point(531, 21);
            btnTestConnection.Margin = new Padding(4, 3, 4, 3);
            btnTestConnection.Name = "btnTestConnection";
            btnTestConnection.Size = new Size(111, 28);
            btnTestConnection.TabIndex = 8;
            btnTestConnection.Text = "Test Conn.";
            btnTestConnection.UseVisualStyleBackColor = true;
            btnTestConnection.Click += btnTestConnection_Click;
            // 
            // txtLoginId
            // 
            txtLoginId.Location = new Point(70, 55);
            txtLoginId.Margin = new Padding(4, 3, 4, 3);
            txtLoginId.Name = "txtLoginId";
            txtLoginId.Size = new Size(58, 23);
            txtLoginId.TabIndex = 7;
            // 
            // lblLoginId
            // 
            lblLoginId.AutoSize = true;
            lblLoginId.Location = new Point(12, 59);
            lblLoginId.Margin = new Padding(4, 0, 4, 0);
            lblLoginId.Name = "lblLoginId";
            lblLoginId.Size = new Size(50, 15);
            lblLoginId.TabIndex = 6;
            lblLoginId.Text = "LoginId:";
            // 
            // txtPassword
            // 
            txtPassword.Location = new Point(420, 23);
            txtPassword.Margin = new Padding(4, 3, 4, 3);
            txtPassword.Name = "txtPassword";
            txtPassword.PasswordChar = '*';
            txtPassword.Size = new Size(93, 23);
            txtPassword.TabIndex = 5;
            // 
            // lblPsw
            // 
            lblPsw.AutoSize = true;
            lblPsw.Location = new Point(379, 27);
            lblPsw.Margin = new Padding(4, 0, 4, 0);
            lblPsw.Name = "lblPsw";
            lblPsw.Size = new Size(31, 15);
            lblPsw.TabIndex = 4;
            lblPsw.Text = "Psw:";
            // 
            // txtUser
            // 
            txtUser.Location = new Point(268, 23);
            txtUser.Margin = new Padding(4, 3, 4, 3);
            txtUser.Name = "txtUser";
            txtUser.Size = new Size(93, 23);
            txtUser.TabIndex = 3;
            // 
            // lblUser
            // 
            lblUser.AutoSize = true;
            lblUser.Location = new Point(222, 27);
            lblUser.Margin = new Padding(4, 0, 4, 0);
            lblUser.Name = "lblUser";
            lblUser.Size = new Size(33, 15);
            lblUser.TabIndex = 2;
            lblUser.Text = "User:";
            // 
            // txtServer
            // 
            txtServer.Location = new Point(70, 23);
            txtServer.Margin = new Padding(4, 3, 4, 3);
            txtServer.Name = "txtServer";
            txtServer.Size = new Size(139, 23);
            txtServer.TabIndex = 1;
            // 
            // lblServer
            // 
            lblServer.AutoSize = true;
            lblServer.Location = new Point(12, 27);
            lblServer.Margin = new Padding(4, 0, 4, 0);
            lblServer.Name = "lblServer";
            lblServer.Size = new Size(42, 15);
            lblServer.TabIndex = 0;
            lblServer.Text = "Server:";
            // 
            // grpAzienda
            // 
            grpAzienda.Controls.Add(chkDebug);
            grpAzienda.Controls.Add(lblDbInUso);
            grpAzienda.Controls.Add(chkTestDb);
            grpAzienda.Controls.Add(rdoSpa);
            grpAzienda.Controls.Add(rdoUno);
            grpAzienda.Location = new Point(14, 136);
            grpAzienda.Margin = new Padding(4, 3, 4, 3);
            grpAzienda.Name = "grpAzienda";
            grpAzienda.Padding = new Padding(4, 3, 4, 3);
            grpAzienda.Size = new Size(653, 69);
            grpAzienda.TabIndex = 2;
            grpAzienda.TabStop = false;
            grpAzienda.Text = "Database & Azienda";
            // 
            // chkDebug
            // 
            chkDebug.AutoSize = true;
            chkDebug.Location = new Point(525, 23);
            chkDebug.Margin = new Padding(4, 3, 4, 3);
            chkDebug.Name = "chkDebug";
            chkDebug.Size = new Size(82, 19);
            chkDebug.TabIndex = 4;
            chkDebug.Text = "Debug ON";
            chkDebug.UseVisualStyleBackColor = true;
            // 
            // lblDbInUso
            // 
            lblDbInUso.AutoSize = true;
            lblDbInUso.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblDbInUso.Location = new Point(14, 46);
            lblDbInUso.Margin = new Padding(4, 0, 4, 0);
            lblDbInUso.Name = "lblDbInUso";
            lblDbInUso.Size = new Size(107, 13);
            lblDbInUso.TabIndex = 3;
            lblDbInUso.Text = "Database: (None)";
            // 
            // chkTestDb
            // 
            chkTestDb.AutoSize = true;
            chkTestDb.Location = new Point(257, 23);
            chkTestDb.Margin = new Padding(4, 3, 4, 3);
            chkTestDb.Name = "chkTestDb";
            chkTestDb.Size = new Size(119, 19);
            chkTestDb.TabIndex = 2;
            chkTestDb.Text = "Usa Database Test";
            chkTestDb.UseVisualStyleBackColor = true;
            chkTestDb.CheckedChanged += chkTestDb_CheckedChanged;
            // 
            // rdoSpa
            // 
            rdoSpa.AutoSize = true;
            rdoSpa.Location = new Point(134, 23);
            rdoSpa.Margin = new Padding(4, 3, 4, 3);
            rdoSpa.Name = "rdoSpa";
            rdoSpa.Size = new Size(90, 19);
            rdoSpa.TabIndex = 1;
            rdoSpa.Text = "Azienda SPA";
            rdoSpa.UseVisualStyleBackColor = true;
            rdoSpa.CheckedChanged += rdoAzienda_CheckedChanged;
            // 
            // rdoUno
            // 
            rdoUno.AutoSize = true;
            rdoUno.Checked = true;
            rdoUno.Location = new Point(18, 23);
            rdoUno.Margin = new Padding(4, 3, 4, 3);
            rdoUno.Name = "rdoUno";
            rdoUno.Size = new Size(96, 19);
            rdoUno.TabIndex = 0;
            rdoUno.TabStop = true;
            rdoUno.Text = "Azienda UNO";
            rdoUno.UseVisualStyleBackColor = true;
            rdoUno.CheckedChanged += rdoAzienda_CheckedChanged;
            // 
            // grpAzioni
            // 
            grpAzioni.Controls.Add(btnAllineaFattureOrdini);
            grpAzioni.Controls.Add(btnConsuntivi);
            grpAzioni.Controls.Add(btnAdeguamentoIstat);
            grpAzioni.Controls.Add(btnGeneraOrdini);
            grpAzioni.Location = new Point(14, 212);
            grpAzioni.Margin = new Padding(4, 3, 4, 3);
            grpAzioni.Name = "grpAzioni";
            grpAzioni.Padding = new Padding(4, 3, 4, 3);
            grpAzioni.Size = new Size(653, 110);
            grpAzioni.TabIndex = 3;
            grpAzioni.TabStop = false;
            grpAzioni.Text = "Operazioni di Elaborazione";
            // 
            // btnAllineaFattureOrdini
            // 
            btnAllineaFattureOrdini.Location = new Point(490, 27);
            btnAllineaFattureOrdini.Margin = new Padding(4, 3, 4, 3);
            btnAllineaFattureOrdini.Name = "btnAllineaFattureOrdini";
            btnAllineaFattureOrdini.Size = new Size(146, 63);
            btnAllineaFattureOrdini.TabIndex = 3;
            btnAllineaFattureOrdini.Text = "Allinea Canoni Fatture";
            btnAllineaFattureOrdini.UseVisualStyleBackColor = true;
            btnAllineaFattureOrdini.Click += btnAllineaFattureOrdini_Click;
            // 
            // btnConsuntivi
            // 
            btnConsuntivi.Location = new Point(332, 27);
            btnConsuntivi.Margin = new Padding(4, 3, 4, 3);
            btnConsuntivi.Name = "btnConsuntivi";
            btnConsuntivi.Size = new Size(146, 63);
            btnConsuntivi.TabIndex = 2;
            btnConsuntivi.Text = "Consuntivazione / Flusso Integra";
            btnConsuntivi.UseVisualStyleBackColor = true;
            btnConsuntivi.Click += btnConsuntivi_Click;
            // 
            // btnAdeguamentoIstat
            // 
            btnAdeguamentoIstat.Location = new Point(175, 27);
            btnAdeguamentoIstat.Margin = new Padding(4, 3, 4, 3);
            btnAdeguamentoIstat.Name = "btnAdeguamentoIstat";
            btnAdeguamentoIstat.Size = new Size(146, 63);
            btnAdeguamentoIstat.TabIndex = 1;
            btnAdeguamentoIstat.Text = "Adeguamento ISTAT";
            btnAdeguamentoIstat.UseVisualStyleBackColor = true;
            btnAdeguamentoIstat.Click += btnAdeguamentoIstat_Click;
            // 
            // btnGeneraOrdini
            // 
            btnGeneraOrdini.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnGeneraOrdini.Location = new Point(18, 27);
            btnGeneraOrdini.Margin = new Padding(4, 3, 4, 3);
            btnGeneraOrdini.Name = "btnGeneraOrdini";
            btnGeneraOrdini.Size = new Size(146, 63);
            btnGeneraOrdini.TabIndex = 0;
            btnGeneraOrdini.Text = "Genera Canoni da Ordini";
            btnGeneraOrdini.UseVisualStyleBackColor = true;
            btnGeneraOrdini.Click += btnGeneraOrdini_Click;
            // 
            // lstStato
            // 
            lstStato.FormattingEnabled = true;
            lstStato.ItemHeight = 15;
            lstStato.Location = new Point(14, 329);
            lstStato.Margin = new Padding(4, 3, 4, 3);
            lstStato.Name = "lstStato";
            lstStato.Size = new Size(653, 184);
            lstStato.TabIndex = 4;
            // 
            // progressBar
            // 
            progressBar.Location = new Point(14, 547);
            progressBar.Margin = new Padding(4, 3, 4, 3);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(653, 21);
            progressBar.Step = 1;
            progressBar.TabIndex = 7;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(14, 522);
            lblStatus.Margin = new Padding(4, 0, 4, 0);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(43, 15);
            lblStatus.TabIndex = 5;
            lblStatus.Text = "Pronto";
            // 
            // btnPulisciLog
            // 
            btnPulisciLog.Location = new Point(562, 517);
            btnPulisciLog.Margin = new Padding(4, 3, 4, 3);
            btnPulisciLog.Name = "btnPulisciLog";
            btnPulisciLog.Size = new Size(105, 25);
            btnPulisciLog.TabIndex = 6;
            btnPulisciLog.Text = "Pulisci Log";
            btnPulisciLog.UseVisualStyleBackColor = true;
            btnPulisciLog.Click += btnPulisciLog_Click;
            // 
            // subProgressBar
            // 
            subProgressBar.Location = new Point(346, 519);
            subProgressBar.Margin = new Padding(4, 3, 4, 3);
            subProgressBar.Name = "subProgressBar";
            subProgressBar.Size = new Size(208, 21);
            subProgressBar.Step = 1;
            subProgressBar.TabIndex = 8;
            subProgressBar.Visible = false;
            // 
            // FLogin
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(681, 586);
            Controls.Add(subProgressBar);
            Controls.Add(btnPulisciLog);
            Controls.Add(lblStatus);
            Controls.Add(progressBar);
            Controls.Add(lstStato);
            Controls.Add(grpAzioni);
            Controls.Add(grpAzienda);
            Controls.Add(grpConn);
            Controls.Add(menuStripPrincipal);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MainMenuStrip = menuStripPrincipal;
            Margin = new Padding(4, 3, 4, 3);
            MaximizeBox = false;
            Name = "FLogin";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Gestione Contratti & Fatturazione - Mago.NET";
            Load += FLogin_Load;
            menuStripPrincipal.ResumeLayout(false);
            menuStripPrincipal.PerformLayout();
            grpConn.ResumeLayout(false);
            grpConn.PerformLayout();
            grpAzienda.ResumeLayout(false);
            grpAzienda.PerformLayout();
            grpAzioni.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStripPrincipal;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem esciToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem strumentiToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem configurazioneAvanzataToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem apriJsonToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem apriCartellaSettingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem apriCartellaLogToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem moduliToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem contrattiFatturazioneToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pagheToolStripMenuItem;
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
        private ProgressBar subProgressBar;
    }
}