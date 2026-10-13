namespace FatturazioneContratti.Forms
{
    partial class FSettings
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
            this.btnTestConnection = new System.Windows.Forms.Button();
            this.txtLoginId = new System.Windows.Forms.TextBox();
            this.lblLoginId = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.lblPsw = new System.Windows.Forms.Label();
            this.txtUser = new System.Windows.Forms.TextBox();
            this.lblUser = new System.Windows.Forms.Label();
            this.txtServer = new System.Windows.Forms.TextBox();
            this.lblServer = new System.Windows.Forms.Label();
            this.grpAzienda = new System.Windows.Forms.GroupBox();
            this.chkDebug = new System.Windows.Forms.CheckBox();
            this.lblDbInUso = new System.Windows.Forms.Label();
            this.chkTestDb = new System.Windows.Forms.CheckBox();
            this.rdoSpa = new System.Windows.Forms.RadioButton();
            this.rdoUno = new System.Windows.Forms.RadioButton();
            this.grpDatabase = new System.Windows.Forms.GroupBox();
            this.txtTmpDbSpa = new System.Windows.Forms.TextBox();
            this.lblTmpDbSpa = new System.Windows.Forms.Label();
            this.txtDbSpa = new System.Windows.Forms.TextBox();
            this.lblDbSpa = new System.Windows.Forms.Label();
            this.txtTmpDbUno = new System.Windows.Forms.TextBox();
            this.lblTmpDbUno = new System.Windows.Forms.Label();
            this.txtDbUno = new System.Windows.Forms.TextBox();
            this.lblDbUno = new System.Windows.Forms.Label();
            this.grpPercorsi = new System.Windows.Forms.GroupBox();
            this.btnSfogliaPaghe = new System.Windows.Forms.Button();
            this.txtPathPaghe = new System.Windows.Forms.TextBox();
            this.lblPathPaghe = new System.Windows.Forms.Label();
            this.btnSfogliaFox = new System.Windows.Forms.Button();
            this.txtPathFox = new System.Windows.Forms.TextBox();
            this.lblPathFox = new System.Windows.Forms.Label();
            this.btnSfogliaGenerale = new System.Windows.Forms.Button();
            this.txtPathGenerale = new System.Windows.Forms.TextBox();
            this.lblPathGenerale = new System.Windows.Forms.Label();
            this.btnApriJson = new System.Windows.Forms.Button();
            this.btnSalva = new System.Windows.Forms.Button();
            this.btnAnnulla = new System.Windows.Forms.Button();
            this.grpConn.SuspendLayout();
            this.grpAzienda.SuspendLayout();
            this.grpDatabase.SuspendLayout();
            this.grpPercorsi.SuspendLayout();
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
            this.grpConn.Size = new System.Drawing.Size(530, 95);
            this.grpConn.TabIndex = 0;
            this.grpConn.TabStop = false;
            this.grpConn.Text = "Connessione SQL Server";
            // 
            // lblServer
            // 
            this.lblServer.AutoSize = true;
            this.lblServer.Location = new System.Drawing.Point(12, 27);
            this.lblServer.Name = "lblServer";
            this.lblServer.Size = new System.Drawing.Size(42, 15);
            this.lblServer.TabIndex = 0;
            this.lblServer.Text = "Server:";
            // 
            // txtServer
            // 
            this.txtServer.Location = new System.Drawing.Point(60, 24);
            this.txtServer.Name = "txtServer";
            this.txtServer.Size = new System.Drawing.Size(170, 23);
            this.txtServer.TabIndex = 1;
            // 
            // lblUser
            // 
            this.lblUser.AutoSize = true;
            this.lblUser.Location = new System.Drawing.Point(240, 27);
            this.lblUser.Name = "lblUser";
            this.lblUser.Size = new System.Drawing.Size(33, 15);
            this.lblUser.TabIndex = 2;
            this.lblUser.Text = "User:";
            // 
            // txtUser
            // 
            this.txtUser.Location = new System.Drawing.Point(280, 24);
            this.txtUser.Name = "txtUser";
            this.txtUser.Size = new System.Drawing.Size(90, 23);
            this.txtUser.TabIndex = 3;
            // 
            // lblPsw
            // 
            this.lblPsw.AutoSize = true;
            this.lblPsw.Location = new System.Drawing.Point(380, 27);
            this.lblPsw.Name = "lblPsw";
            this.lblPsw.Size = new System.Drawing.Size(31, 15);
            this.lblPsw.TabIndex = 4;
            this.lblPsw.Text = "Psw:";
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(420, 24);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(90, 23);
            this.txtPassword.TabIndex = 5;
            // 
            // btnTestConnection
            // 
            this.btnTestConnection.Location = new System.Drawing.Point(420, 55);
            this.btnTestConnection.Name = "btnTestConnection";
            this.btnTestConnection.Size = new System.Drawing.Size(90, 23);
            this.btnTestConnection.TabIndex = 8;
            this.btnTestConnection.Text = "Test Conn.";
            this.btnTestConnection.UseVisualStyleBackColor = true;
            this.btnTestConnection.Click += new System.EventHandler(this.btnTestConnection_Click);
            // 
            // lblLoginId
            // 
            this.lblLoginId.AutoSize = true;
            this.lblLoginId.Location = new System.Drawing.Point(12, 60);
            this.lblLoginId.Name = "lblLoginId";
            this.lblLoginId.Size = new System.Drawing.Size(50, 15);
            this.lblLoginId.TabIndex = 6;
            this.lblLoginId.Text = "LoginId:";
            // 
            // txtLoginId
            // 
            this.txtLoginId.Location = new System.Drawing.Point(70, 57);
            this.txtLoginId.Name = "txtLoginId";
            this.txtLoginId.Size = new System.Drawing.Size(58, 23);
            this.txtLoginId.TabIndex = 7;
            // 
            // grpAzienda
            // 
            this.grpAzienda.Controls.Add(this.chkDebug);
            this.grpAzienda.Controls.Add(this.lblDbInUso);
            this.grpAzienda.Controls.Add(this.chkTestDb);
            this.grpAzienda.Controls.Add(this.rdoSpa);
            this.grpAzienda.Controls.Add(this.rdoUno);
            this.grpAzienda.Location = new System.Drawing.Point(12, 115);
            this.grpAzienda.Name = "grpAzienda";
            this.grpAzienda.Size = new System.Drawing.Size(530, 69);
            this.grpAzienda.TabIndex = 1;
            this.grpAzienda.TabStop = false;
            this.grpAzienda.Text = "Database & Azienda";
            // 
            // chkDebug
            // 
            this.chkDebug.AutoSize = true;
            this.chkDebug.Location = new System.Drawing.Point(438, 23);
            this.chkDebug.Name = "chkDebug";
            this.chkDebug.Size = new System.Drawing.Size(82, 19);
            this.chkDebug.TabIndex = 4;
            this.chkDebug.Text = "Debug ON";
            this.chkDebug.UseVisualStyleBackColor = true;
            // 
            // lblDbInUso
            // 
            this.lblDbInUso.AutoSize = true;
            this.lblDbInUso.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblDbInUso.Location = new System.Drawing.Point(14, 46);
            this.lblDbInUso.Name = "lblDbInUso";
            this.lblDbInUso.Size = new System.Drawing.Size(107, 13);
            this.lblDbInUso.TabIndex = 3;
            this.lblDbInUso.Text = "Database: (None)";
            // 
            // chkTestDb
            // 
            this.chkTestDb.AutoSize = true;
            this.chkTestDb.Location = new System.Drawing.Point(257, 23);
            this.chkTestDb.Name = "chkTestDb";
            this.chkTestDb.Size = new System.Drawing.Size(119, 19);
            this.chkTestDb.TabIndex = 2;
            this.chkTestDb.Text = "Usa Database Test";
            this.chkTestDb.UseVisualStyleBackColor = true;
            this.chkTestDb.CheckedChanged += new System.EventHandler(this.chkTestDb_CheckedChanged);
            // 
            // rdoSpa
            // 
            this.rdoSpa.AutoSize = true;
            this.rdoSpa.Location = new System.Drawing.Point(134, 23);
            this.rdoSpa.Name = "rdoSpa";
            this.rdoSpa.Size = new System.Drawing.Size(90, 19);
            this.rdoSpa.TabIndex = 1;
            this.rdoSpa.Text = "Azienda SPA";
            this.rdoSpa.UseVisualStyleBackColor = true;
            this.rdoSpa.CheckedChanged += new System.EventHandler(this.rdoAzienda_CheckedChanged);
            // 
            // rdoUno
            // 
            this.rdoUno.AutoSize = true;
            this.rdoUno.Checked = true;
            this.rdoUno.Location = new System.Drawing.Point(18, 23);
            this.rdoUno.Name = "rdoUno";
            this.rdoUno.Size = new System.Drawing.Size(96, 19);
            this.rdoUno.TabIndex = 0;
            this.rdoUno.TabStop = true;
            this.rdoUno.Text = "Azienda UNO";
            this.rdoUno.UseVisualStyleBackColor = true;
            this.rdoUno.CheckedChanged += new System.EventHandler(this.rdoAzienda_CheckedChanged);
            // 
            // grpDatabase
            // 
            this.grpDatabase.Controls.Add(this.txtTmpDbSpa);
            this.grpDatabase.Controls.Add(this.lblTmpDbSpa);
            this.grpDatabase.Controls.Add(this.txtDbSpa);
            this.grpDatabase.Controls.Add(this.lblDbSpa);
            this.grpDatabase.Controls.Add(this.txtTmpDbUno);
            this.grpDatabase.Controls.Add(this.lblTmpDbUno);
            this.grpDatabase.Controls.Add(this.txtDbUno);
            this.grpDatabase.Controls.Add(this.lblDbUno);
            this.grpDatabase.Location = new System.Drawing.Point(12, 190);
            this.grpDatabase.Name = "grpDatabase";
            this.grpDatabase.Size = new System.Drawing.Size(530, 95);
            this.grpDatabase.TabIndex = 2;
            this.grpDatabase.TabStop = false;
            this.grpDatabase.Text = "Database SQL Server";
            // 
            // txtTmpDbSpa
            // 
            this.txtTmpDbSpa.Location = new System.Drawing.Point(375, 54);
            this.txtTmpDbSpa.Name = "txtTmpDbSpa";
            this.txtTmpDbSpa.Size = new System.Drawing.Size(140, 20);
            this.txtTmpDbSpa.TabIndex = 7;
            // 
            // lblTmpDbSpa
            // 
            this.lblTmpDbSpa.AutoSize = true;
            this.lblTmpDbSpa.Location = new System.Drawing.Point(280, 57);
            this.lblTmpDbSpa.Name = "lblTmpDbSpa";
            this.lblTmpDbSpa.Size = new System.Drawing.Size(86, 13);
            this.lblTmpDbSpa.TabIndex = 6;
            this.lblTmpDbSpa.Text = "Test SPA (Tmp):";
            // 
            // txtDbSpa
            // 
            this.txtDbSpa.Location = new System.Drawing.Point(375, 25);
            this.txtDbSpa.Name = "txtDbSpa";
            this.txtDbSpa.Size = new System.Drawing.Size(140, 20);
            this.txtDbSpa.TabIndex = 5;
            // 
            // lblDbSpa
            // 
            this.lblDbSpa.AutoSize = true;
            this.lblDbSpa.Location = new System.Drawing.Point(280, 28);
            this.lblDbSpa.Name = "lblDbSpa";
            this.lblDbSpa.Size = new System.Drawing.Size(83, 13);
            this.lblDbSpa.TabIndex = 4;
            this.lblDbSpa.Text = "Database SPA:";
            // 
            // txtTmpDbUno
            // 
            this.txtTmpDbUno.Location = new System.Drawing.Point(110, 54);
            this.txtTmpDbUno.Name = "txtTmpDbUno";
            this.txtTmpDbUno.Size = new System.Drawing.Size(140, 20);
            this.txtTmpDbUno.TabIndex = 3;
            // 
            // lblTmpDbUno
            // 
            this.lblTmpDbUno.AutoSize = true;
            this.lblTmpDbUno.Location = new System.Drawing.Point(15, 57);
            this.lblTmpDbUno.Name = "lblTmpDbUno";
            this.lblTmpDbUno.Size = new System.Drawing.Size(89, 13);
            this.lblTmpDbUno.TabIndex = 2;
            this.lblTmpDbUno.Text = "Test UNO (Tmp):";
            // 
            // txtDbUno
            // 
            this.txtDbUno.Location = new System.Drawing.Point(110, 25);
            this.txtDbUno.Name = "txtDbUno";
            this.txtDbUno.Size = new System.Drawing.Size(140, 20);
            this.txtDbUno.TabIndex = 1;
            // 
            // lblDbUno
            // 
            this.lblDbUno.AutoSize = true;
            this.lblDbUno.Location = new System.Drawing.Point(15, 28);
            this.lblDbUno.Name = "lblDbUno";
            this.lblDbUno.Size = new System.Drawing.Size(86, 13);
            this.lblDbUno.TabIndex = 0;
            this.lblDbUno.Text = "Database UNO:";
            // 
            // grpPercorsi
            // 
            this.grpPercorsi.Controls.Add(this.btnSfogliaPaghe);
            this.grpPercorsi.Controls.Add(this.txtPathPaghe);
            this.grpPercorsi.Controls.Add(this.lblPathPaghe);
            this.grpPercorsi.Controls.Add(this.btnSfogliaFox);
            this.grpPercorsi.Controls.Add(this.txtPathFox);
            this.grpPercorsi.Controls.Add(this.lblPathFox);
            this.grpPercorsi.Controls.Add(this.btnSfogliaGenerale);
            this.grpPercorsi.Controls.Add(this.txtPathGenerale);
            this.grpPercorsi.Controls.Add(this.lblPathGenerale);
            this.grpPercorsi.Location = new System.Drawing.Point(12, 295);
            this.grpPercorsi.Name = "grpPercorsi";
            this.grpPercorsi.Size = new System.Drawing.Size(530, 125);
            this.grpPercorsi.TabIndex = 3;
            this.grpPercorsi.TabStop = false;
            this.grpPercorsi.Text = "Percorsi Cartelle di Lavoro";
            // 
            // btnSfogliaPaghe
            // 
            this.btnSfogliaPaghe.Location = new System.Drawing.Point(485, 87);
            this.btnSfogliaPaghe.Name = "btnSfogliaPaghe";
            this.btnSfogliaPaghe.Size = new System.Drawing.Size(30, 23);
            this.btnSfogliaPaghe.TabIndex = 8;
            this.btnSfogliaPaghe.Text = "...";
            this.btnSfogliaPaghe.UseVisualStyleBackColor = true;
            this.btnSfogliaPaghe.Click += new System.EventHandler(this.btnSfoglia_Click);
            // 
            // txtPathPaghe
            // 
            this.txtPathPaghe.Location = new System.Drawing.Point(110, 89);
            this.txtPathPaghe.Name = "txtPathPaghe";
            this.txtPathPaghe.Size = new System.Drawing.Size(369, 20);
            this.txtPathPaghe.TabIndex = 7;
            // 
            // lblPathPaghe
            // 
            this.lblPathPaghe.AutoSize = true;
            this.lblPathPaghe.Location = new System.Drawing.Point(15, 92);
            this.lblPathPaghe.Name = "lblPathPaghe";
            this.lblPathPaghe.Size = new System.Drawing.Size(78, 13);
            this.lblPathPaghe.TabIndex = 6;
            this.lblPathPaghe.Text = "Flusso Paghe:";
            // 
            // btnSfogliaFox
            // 
            this.btnSfogliaFox.Location = new System.Drawing.Point(485, 56);
            this.btnSfogliaFox.Name = "btnSfogliaFox";
            this.btnSfogliaFox.Size = new System.Drawing.Size(30, 23);
            this.btnSfogliaFox.TabIndex = 5;
            this.btnSfogliaFox.Text = "...";
            this.btnSfogliaFox.UseVisualStyleBackColor = true;
            this.btnSfogliaFox.Click += new System.EventHandler(this.btnSfoglia_Click);
            // 
            // txtPathFox
            // 
            this.txtPathFox.Location = new System.Drawing.Point(110, 58);
            this.txtPathFox.Name = "txtPathFox";
            this.txtPathFox.Size = new System.Drawing.Size(369, 20);
            this.txtPathFox.TabIndex = 4;
            // 
            // lblPathFox
            // 
            this.lblPathFox.AutoSize = true;
            this.lblPathFox.Location = new System.Drawing.Point(15, 61);
            this.lblPathFox.Name = "lblPathFox";
            this.lblPathFox.Size = new System.Drawing.Size(65, 13);
            this.lblPathFox.TabIndex = 3;
            this.lblPathFox.Text = "Cartella Fox:";
            // 
            // btnSfogliaGenerale
            // 
            this.btnSfogliaGenerale.Location = new System.Drawing.Point(485, 25);
            this.btnSfogliaGenerale.Name = "btnSfogliaGenerale";
            this.btnSfogliaGenerale.Size = new System.Drawing.Size(30, 23);
            this.btnSfogliaGenerale.TabIndex = 2;
            this.btnSfogliaGenerale.Text = "...";
            this.btnSfogliaGenerale.UseVisualStyleBackColor = true;
            this.btnSfogliaGenerale.Click += new System.EventHandler(this.btnSfoglia_Click);
            // 
            // txtPathGenerale
            // 
            this.txtPathGenerale.Location = new System.Drawing.Point(110, 27);
            this.txtPathGenerale.Name = "txtPathGenerale";
            this.txtPathGenerale.Size = new System.Drawing.Size(369, 20);
            this.txtPathGenerale.TabIndex = 1;
            // 
            // lblPathGenerale
            // 
            this.lblPathGenerale.AutoSize = true;
            this.lblPathGenerale.Location = new System.Drawing.Point(15, 30);
            this.lblPathGenerale.Name = "lblPathGenerale";
            this.lblPathGenerale.Size = new System.Drawing.Size(81, 13);
            this.lblPathGenerale.TabIndex = 0;
            this.lblPathGenerale.Text = "Path Generale:";
            // 
            // btnApriJson
            // 
            this.btnApriJson.Location = new System.Drawing.Point(12, 430);
            this.btnApriJson.Name = "btnApriJson";
            this.btnApriJson.Size = new System.Drawing.Size(145, 30);
            this.btnApriJson.TabIndex = 4;
            this.btnApriJson.Text = "Modifica file .json";
            this.btnApriJson.UseVisualStyleBackColor = true;
            this.btnApriJson.Click += new System.EventHandler(this.btnApriJson_Click);
            // 
            // btnSalva
            // 
            this.btnSalva.Location = new System.Drawing.Point(370, 430);
            this.btnSalva.Name = "btnSalva";
            this.btnSalva.Size = new System.Drawing.Size(85, 30);
            this.btnSalva.TabIndex = 5;
            this.btnSalva.Text = "Salva";
            this.btnSalva.UseVisualStyleBackColor = true;
            this.btnSalva.Click += new System.EventHandler(this.btnSalva_Click);
            // 
            // btnAnnulla
            // 
            this.btnAnnulla.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnAnnulla.Location = new System.Drawing.Point(461, 430);
            this.btnAnnulla.Name = "btnAnnulla";
            this.btnAnnulla.Size = new System.Drawing.Size(81, 30);
            this.btnAnnulla.TabIndex = 6;
            this.btnAnnulla.Text = "Annulla";
            this.btnAnnulla.UseVisualStyleBackColor = true;
            // 
            // FSettings
            // 
            this.AcceptButton = this.btnSalva;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnAnnulla;
            this.ClientSize = new System.Drawing.Size(554, 470);
            this.Controls.Add(this.btnAnnulla);
            this.Controls.Add(this.btnSalva);
            this.Controls.Add(this.btnApriJson);
            this.Controls.Add(this.grpPercorsi);
            this.Controls.Add(this.grpDatabase);
            this.Controls.Add(this.grpAzienda);
            this.Controls.Add(this.grpConn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FSettings";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Configurazione Database e Percorsi";
            this.Load += new System.EventHandler(this.FSettings_Load);
            this.grpConn.ResumeLayout(false);
            this.grpConn.PerformLayout();
            this.grpAzienda.ResumeLayout(false);
            this.grpAzienda.PerformLayout();
            this.grpDatabase.ResumeLayout(false);
            this.grpDatabase.PerformLayout();
            this.grpPercorsi.ResumeLayout(false);
            this.grpPercorsi.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpConn;
        private System.Windows.Forms.Button btnTestConnection;
        private System.Windows.Forms.TextBox txtLoginId;
        private System.Windows.Forms.Label lblLoginId;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label lblPsw;
        private System.Windows.Forms.TextBox txtUser;
        private System.Windows.Forms.Label lblUser;
        private System.Windows.Forms.TextBox txtServer;
        private System.Windows.Forms.Label lblServer;
        private System.Windows.Forms.GroupBox grpAzienda;
        private System.Windows.Forms.CheckBox chkDebug;
        private System.Windows.Forms.Label lblDbInUso;
        private System.Windows.Forms.CheckBox chkTestDb;
        private System.Windows.Forms.RadioButton rdoSpa;
        private System.Windows.Forms.RadioButton rdoUno;
        private System.Windows.Forms.GroupBox grpDatabase;
        private System.Windows.Forms.TextBox txtTmpDbSpa;
        private System.Windows.Forms.Label lblTmpDbSpa;
        private System.Windows.Forms.TextBox txtDbSpa;
        private System.Windows.Forms.Label lblDbSpa;
        private System.Windows.Forms.TextBox txtTmpDbUno;
        private System.Windows.Forms.Label lblTmpDbUno;
        private System.Windows.Forms.TextBox txtDbUno;
        private System.Windows.Forms.Label lblDbUno;
        private System.Windows.Forms.GroupBox grpPercorsi;
        private System.Windows.Forms.Button btnSfogliaGenerale;
        private System.Windows.Forms.TextBox txtPathGenerale;
        private System.Windows.Forms.Label lblPathGenerale;
        private System.Windows.Forms.Button btnSfogliaPaghe;
        private System.Windows.Forms.TextBox txtPathPaghe;
        private System.Windows.Forms.Label lblPathPaghe;
        private System.Windows.Forms.Button btnSfogliaFox;
        private System.Windows.Forms.TextBox txtPathFox;
        private System.Windows.Forms.Label lblPathFox;
        private System.Windows.Forms.Button btnApriJson;
        private System.Windows.Forms.Button btnSalva;
        private System.Windows.Forms.Button btnAnnulla;
    }
}