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
            aiutoToolStripMenuItem = new ToolStripMenuItem();
            informazioniToolStripMenuItem = new ToolStripMenuItem();
            grpAzioni = new GroupBox();
            btnAllineaFattureOrdini = new Button();
            btnConsuntivi = new Button();
            btnAdeguamentoIstat = new Button();
            btnGeneraOrdini = new Button();
            lstStato = new ListBox();
            progressBar = new ProgressBar();
            lblStatus = new Label();
            lblDbInUso = new Label();
            btnPulisciStato = new Button();
            subProgressBar = new ProgressBar();
            lnkApriLog = new LinkLabel();
            menuStripPrincipal.SuspendLayout();
            grpAzioni.SuspendLayout();
            SuspendLayout();
            // 
            // menuStripPrincipal
            // 
            menuStripPrincipal.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, strumentiToolStripMenuItem, moduliToolStripMenuItem, aiutoToolStripMenuItem });
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
            esciToolStripMenuItem.Click += EsciToolStripMenuItem_Click;
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
            configurazioneAvanzataToolStripMenuItem.Click += ConfigurazioneAvanzataToolStripMenuItem_Click;
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
            apriJsonToolStripMenuItem.Click += ApriJsonToolStripMenuItem_Click;
            // 
            // apriCartellaSettingsToolStripMenuItem
            // 
            apriCartellaSettingsToolStripMenuItem.Name = "apriCartellaSettingsToolStripMenuItem";
            apriCartellaSettingsToolStripMenuItem.Size = new Size(236, 22);
            apriCartellaSettingsToolStripMenuItem.Text = "Apri Cartella &Impostazioni";
            apriCartellaSettingsToolStripMenuItem.Click += ApriCartellaSettingsToolStripMenuItem_Click;
            // 
            // apriCartellaLogToolStripMenuItem
            // 
            apriCartellaLogToolStripMenuItem.Name = "apriCartellaLogToolStripMenuItem";
            apriCartellaLogToolStripMenuItem.Size = new Size(236, 22);
            apriCartellaLogToolStripMenuItem.Text = "Apri Cartella &Log";
            apriCartellaLogToolStripMenuItem.Click += ApriCartellaLogToolStripMenuItem_Click;
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
            pagheToolStripMenuItem.Click += PagheToolStripMenuItem_Click;
            // 
            // aiutoToolStripMenuItem
            // 
            aiutoToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { informazioniToolStripMenuItem });
            aiutoToolStripMenuItem.Name = "aiutoToolStripMenuItem";
            aiutoToolStripMenuItem.Size = new Size(24, 20);
            aiutoToolStripMenuItem.Text = "?";
            // 
            // informazioniToolStripMenuItem
            // 
            informazioniToolStripMenuItem.Name = "informazioniToolStripMenuItem";
            informazioniToolStripMenuItem.Size = new Size(232, 22);
            informazioniToolStripMenuItem.Text = "&Informazioni su FatturazioneContratti...";
            informazioniToolStripMenuItem.Click += InformazioniToolStripMenuItem_Click;
            // 
            // grpAzioni
            // 
            grpAzioni.Controls.Add(btnAllineaFattureOrdini);
            grpAzioni.Controls.Add(btnConsuntivi);
            grpAzioni.Controls.Add(btnAdeguamentoIstat);
            grpAzioni.Controls.Add(btnGeneraOrdini);
            grpAzioni.Location = new Point(14, 37);
            grpAzioni.Margin = new Padding(4, 3, 4, 3);
            grpAzioni.Name = "grpAzioni";
            grpAzioni.Padding = new Padding(4, 3, 4, 3);
            grpAzioni.Size = new Size(653, 110);
            grpAzioni.TabIndex = 1;
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
            btnAllineaFattureOrdini.Click += BtnAllineaFattureOrdini_Click;
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
            btnConsuntivi.Click += BtnConsuntivi_Click;
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
            btnAdeguamentoIstat.Click += BtnAdeguamentoIstat_Click;
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
            btnGeneraOrdini.Click += BtnGeneraOrdini_Click;
            // 
            // lstStato
            // 
            lstStato.FormattingEnabled = true;
            lstStato.ItemHeight = 15;
            lstStato.Location = new Point(14, 153);
            lstStato.Margin = new Padding(4, 3, 4, 3);
            lstStato.Name = "lstStato";
            lstStato.Size = new Size(653, 259);
            lstStato.TabIndex = 2;
            // 
            // progressBar
            // 
            progressBar.Location = new Point(14, 460);
            progressBar.Margin = new Padding(4, 3, 4, 3);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(653, 21);
            progressBar.Step = 1;
            progressBar.TabIndex = 7;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(14, 440);
            lblStatus.Margin = new Padding(4, 0, 4, 0);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(43, 15);
            lblStatus.TabIndex = 5;
            lblStatus.Text = "Pronto";
            // 
            // lblDbInUso
            // 
            lblDbInUso.AutoSize = true;
            lblDbInUso.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold);
            lblDbInUso.Location = new Point(14, 415);
            lblDbInUso.Margin = new Padding(4, 0, 4, 0);
            lblDbInUso.Name = "lblDbInUso";
            lblDbInUso.Size = new Size(107, 13);
            lblDbInUso.TabIndex = 9;
            lblDbInUso.Text = "Database: (None)";
            // 
            // btnPulisciStato
            // 
            btnPulisciStato.Location = new Point(562, 430);
            btnPulisciStato.Margin = new Padding(4, 3, 4, 3);
            btnPulisciStato.Name = "btnPulisciStato";
            btnPulisciStato.Size = new Size(105, 25);
            btnPulisciStato.TabIndex = 6;
            btnPulisciStato.Text = "Pulisci";
            btnPulisciStato.UseVisualStyleBackColor = true;
            btnPulisciStato.Click += BtnPulisciStato_Click;
            // 
            // subProgressBar
            // 
            subProgressBar.Location = new Point(346, 435);
            subProgressBar.Margin = new Padding(4, 3, 4, 3);
            subProgressBar.Name = "subProgressBar";
            subProgressBar.Size = new Size(208, 21);
            subProgressBar.Step = 1;
            subProgressBar.TabIndex = 8;
            subProgressBar.Visible = false;
            // 
            // lnkApriLog
            // 
            lnkApriLog.Location = new Point(14, 487);
            lnkApriLog.Name = "lnkApriLog";
            lnkApriLog.Size = new Size(120, 17);
            lnkApriLog.TabIndex = 10;
            lnkApriLog.TabStop = true;
            lnkApriLog.Text = "Apri file di log";
            lnkApriLog.Visible = false;
            lnkApriLog.LinkClicked += LnkApriLog_LinkClicked;
            // 
            // FLogin
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(681, 520);
            Controls.Add(subProgressBar);
            Controls.Add(lnkApriLog);
            Controls.Add(btnPulisciStato);
            Controls.Add(lblDbInUso);
            Controls.Add(lblStatus);
            Controls.Add(progressBar);
            Controls.Add(lstStato);
            Controls.Add(grpAzioni);
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
        private System.Windows.Forms.ToolStripMenuItem aiutoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem informazioniToolStripMenuItem;
        private System.Windows.Forms.GroupBox grpAzioni;
        private System.Windows.Forms.Button btnAllineaFattureOrdini;
        private System.Windows.Forms.Button btnConsuntivi;
        private System.Windows.Forms.Button btnAdeguamentoIstat;
        private System.Windows.Forms.Button btnGeneraOrdini;
        private System.Windows.Forms.ListBox lstStato;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblDbInUso;
        private System.Windows.Forms.Button btnPulisciStato;
        private ProgressBar subProgressBar;
        private System.Windows.Forms.LinkLabel lnkApriLog;
    }
}