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
            grpDatabase = new GroupBox();
            txtTmpDbSpa = new TextBox();
            lblTmpDbSpa = new Label();
            txtDbSpa = new TextBox();
            lblDbSpa = new Label();
            txtTmpDbUno = new TextBox();
            lblTmpDbUno = new Label();
            txtDbUno = new TextBox();
            lblDbUno = new Label();
            grpPercorsi = new GroupBox();
            btnSfogliaPaghe = new Button();
            txtPathPaghe = new TextBox();
            lblPathPaghe = new Label();
            btnSfogliaFox = new Button();
            txtPathFox = new TextBox();
            lblPathFox = new Label();
            cboLogMode = new ComboBox();
            lblLogMode = new Label();
            btnSfogliaLastLog = new Button();
            txtLastLogPath = new TextBox();
            lblLastLogPath = new Label();
            btnSfogliaGenerale = new Button();
            txtPathGenerale = new TextBox();
            lblPathGenerale = new Label();
            btnApriJson = new Button();
            btnSalva = new Button();
            btnAnnulla = new Button();
            grpConn.SuspendLayout();
            grpAzienda.SuspendLayout();
            grpDatabase.SuspendLayout();
            grpPercorsi.SuspendLayout();
            SuspendLayout();
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
            grpConn.Location = new Point(14, 14);
            grpConn.Margin = new Padding(4, 3, 4, 3);
            grpConn.Name = "grpConn";
            grpConn.Padding = new Padding(4, 3, 4, 3);
            grpConn.Size = new Size(618, 110);
            grpConn.TabIndex = 0;
            grpConn.TabStop = false;
            grpConn.Text = "Connessione SQL Server";
            // 
            // btnTestConnection
            // 
            btnTestConnection.Location = new Point(490, 63);
            btnTestConnection.Margin = new Padding(4, 3, 4, 3);
            btnTestConnection.Name = "btnTestConnection";
            btnTestConnection.Size = new Size(105, 27);
            btnTestConnection.TabIndex = 8;
            btnTestConnection.Text = "Test Conn.";
            btnTestConnection.UseVisualStyleBackColor = true;
            btnTestConnection.Click += BtnTestConnection_Click;
            // 
            // txtLoginId
            // 
            txtLoginId.Location = new Point(82, 66);
            txtLoginId.Margin = new Padding(4, 3, 4, 3);
            txtLoginId.Name = "txtLoginId";
            txtLoginId.Size = new Size(67, 23);
            txtLoginId.TabIndex = 7;
            // 
            // lblLoginId
            // 
            lblLoginId.AutoSize = true;
            lblLoginId.Location = new Point(14, 69);
            lblLoginId.Margin = new Padding(4, 0, 4, 0);
            lblLoginId.Name = "lblLoginId";
            lblLoginId.Size = new Size(50, 15);
            lblLoginId.TabIndex = 6;
            lblLoginId.Text = "LoginId:";
            // 
            // txtPassword
            // 
            txtPassword.Location = new Point(490, 28);
            txtPassword.Margin = new Padding(4, 3, 4, 3);
            txtPassword.Name = "txtPassword";
            txtPassword.PasswordChar = '*';
            txtPassword.Size = new Size(104, 23);
            txtPassword.TabIndex = 5;
            // 
            // lblPsw
            // 
            lblPsw.AutoSize = true;
            lblPsw.Location = new Point(443, 31);
            lblPsw.Margin = new Padding(4, 0, 4, 0);
            lblPsw.Name = "lblPsw";
            lblPsw.Size = new Size(31, 15);
            lblPsw.TabIndex = 4;
            lblPsw.Text = "Psw:";
            // 
            // txtUser
            // 
            txtUser.Location = new Point(327, 28);
            txtUser.Margin = new Padding(4, 3, 4, 3);
            txtUser.Name = "txtUser";
            txtUser.Size = new Size(104, 23);
            txtUser.TabIndex = 3;
            // 
            // lblUser
            // 
            lblUser.AutoSize = true;
            lblUser.Location = new Point(280, 31);
            lblUser.Margin = new Padding(4, 0, 4, 0);
            lblUser.Name = "lblUser";
            lblUser.Size = new Size(33, 15);
            lblUser.TabIndex = 2;
            lblUser.Text = "User:";
            // 
            // txtServer
            // 
            txtServer.Location = new Point(70, 28);
            txtServer.Margin = new Padding(4, 3, 4, 3);
            txtServer.Name = "txtServer";
            txtServer.Size = new Size(198, 23);
            txtServer.TabIndex = 1;
            // 
            // lblServer
            // 
            lblServer.AutoSize = true;
            lblServer.Location = new Point(14, 31);
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
            grpAzienda.Location = new Point(14, 133);
            grpAzienda.Margin = new Padding(4, 3, 4, 3);
            grpAzienda.Name = "grpAzienda";
            grpAzienda.Padding = new Padding(4, 3, 4, 3);
            grpAzienda.Size = new Size(618, 80);
            grpAzienda.TabIndex = 1;
            grpAzienda.TabStop = false;
            grpAzienda.Text = "Database & Azienda";
            // 
            // chkDebug
            // 
            chkDebug.AutoSize = true;
            chkDebug.Location = new Point(511, 27);
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
            lblDbInUso.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold);
            lblDbInUso.Location = new Point(16, 53);
            lblDbInUso.Margin = new Padding(4, 0, 4, 0);
            lblDbInUso.Name = "lblDbInUso";
            lblDbInUso.Size = new Size(107, 13);
            lblDbInUso.TabIndex = 3;
            lblDbInUso.Text = "Database: (None)";
            // 
            // chkTestDb
            // 
            chkTestDb.AutoSize = true;
            chkTestDb.Location = new Point(300, 27);
            chkTestDb.Margin = new Padding(4, 3, 4, 3);
            chkTestDb.Name = "chkTestDb";
            chkTestDb.Size = new Size(119, 19);
            chkTestDb.TabIndex = 2;
            chkTestDb.Text = "Usa Database Test";
            chkTestDb.UseVisualStyleBackColor = true;
            chkTestDb.CheckedChanged += ChkTestDb_CheckedChanged;
            // 
            // rdoSpa
            // 
            rdoSpa.AutoSize = true;
            rdoSpa.Location = new Point(156, 27);
            rdoSpa.Margin = new Padding(4, 3, 4, 3);
            rdoSpa.Name = "rdoSpa";
            rdoSpa.Size = new Size(90, 19);
            rdoSpa.TabIndex = 1;
            rdoSpa.Text = "Azienda SPA";
            rdoSpa.UseVisualStyleBackColor = true;
            rdoSpa.CheckedChanged += RdoAzienda_CheckedChanged;
            // 
            // rdoUno
            // 
            rdoUno.AutoSize = true;
            rdoUno.Checked = true;
            rdoUno.Location = new Point(21, 27);
            rdoUno.Margin = new Padding(4, 3, 4, 3);
            rdoUno.Name = "rdoUno";
            rdoUno.Size = new Size(96, 19);
            rdoUno.TabIndex = 0;
            rdoUno.TabStop = true;
            rdoUno.Text = "Azienda UNO";
            rdoUno.UseVisualStyleBackColor = true;
            rdoUno.CheckedChanged += RdoAzienda_CheckedChanged;
            // 
            // grpDatabase
            // 
            grpDatabase.Controls.Add(txtTmpDbSpa);
            grpDatabase.Controls.Add(lblTmpDbSpa);
            grpDatabase.Controls.Add(txtDbSpa);
            grpDatabase.Controls.Add(lblDbSpa);
            grpDatabase.Controls.Add(txtTmpDbUno);
            grpDatabase.Controls.Add(lblTmpDbUno);
            grpDatabase.Controls.Add(txtDbUno);
            grpDatabase.Controls.Add(lblDbUno);
            grpDatabase.Location = new Point(14, 219);
            grpDatabase.Margin = new Padding(4, 3, 4, 3);
            grpDatabase.Name = "grpDatabase";
            grpDatabase.Padding = new Padding(4, 3, 4, 3);
            grpDatabase.Size = new Size(618, 110);
            grpDatabase.TabIndex = 2;
            grpDatabase.TabStop = false;
            grpDatabase.Text = "Database SQL Server";
            // 
            // txtTmpDbSpa
            // 
            txtTmpDbSpa.Location = new Point(438, 62);
            txtTmpDbSpa.Margin = new Padding(4, 3, 4, 3);
            txtTmpDbSpa.Name = "txtTmpDbSpa";
            txtTmpDbSpa.Size = new Size(163, 23);
            txtTmpDbSpa.TabIndex = 7;
            // 
            // lblTmpDbSpa
            // 
            lblTmpDbSpa.AutoSize = true;
            lblTmpDbSpa.Location = new Point(327, 66);
            lblTmpDbSpa.Margin = new Padding(4, 0, 4, 0);
            lblTmpDbSpa.Name = "lblTmpDbSpa";
            lblTmpDbSpa.Size = new Size(87, 15);
            lblTmpDbSpa.TabIndex = 6;
            lblTmpDbSpa.Text = "Test SPA (Tmp):";
            // 
            // txtDbSpa
            // 
            txtDbSpa.Location = new Point(438, 29);
            txtDbSpa.Margin = new Padding(4, 3, 4, 3);
            txtDbSpa.Name = "txtDbSpa";
            txtDbSpa.Size = new Size(163, 23);
            txtDbSpa.TabIndex = 5;
            // 
            // lblDbSpa
            // 
            lblDbSpa.AutoSize = true;
            lblDbSpa.Location = new Point(327, 32);
            lblDbSpa.Margin = new Padding(4, 0, 4, 0);
            lblDbSpa.Name = "lblDbSpa";
            lblDbSpa.Size = new Size(81, 15);
            lblDbSpa.TabIndex = 4;
            lblDbSpa.Text = "Database SPA:";
            // 
            // txtTmpDbUno
            // 
            txtTmpDbUno.Location = new Point(128, 62);
            txtTmpDbUno.Margin = new Padding(4, 3, 4, 3);
            txtTmpDbUno.Name = "txtTmpDbUno";
            txtTmpDbUno.Size = new Size(163, 23);
            txtTmpDbUno.TabIndex = 3;
            // 
            // lblTmpDbUno
            // 
            lblTmpDbUno.AutoSize = true;
            lblTmpDbUno.Location = new Point(18, 66);
            lblTmpDbUno.Margin = new Padding(4, 0, 4, 0);
            lblTmpDbUno.Name = "lblTmpDbUno";
            lblTmpDbUno.Size = new Size(93, 15);
            lblTmpDbUno.TabIndex = 2;
            lblTmpDbUno.Text = "Test UNO (Tmp):";
            // 
            // txtDbUno
            // 
            txtDbUno.Location = new Point(128, 29);
            txtDbUno.Margin = new Padding(4, 3, 4, 3);
            txtDbUno.Name = "txtDbUno";
            txtDbUno.Size = new Size(163, 23);
            txtDbUno.TabIndex = 1;
            // 
            // lblDbUno
            // 
            lblDbUno.AutoSize = true;
            lblDbUno.Location = new Point(18, 32);
            lblDbUno.Margin = new Padding(4, 0, 4, 0);
            lblDbUno.Name = "lblDbUno";
            lblDbUno.Size = new Size(87, 15);
            lblDbUno.TabIndex = 0;
            lblDbUno.Text = "Database UNO:";
            // 
            // grpPercorsi
            // 
            grpPercorsi.Controls.Add(btnSfogliaPaghe);
            grpPercorsi.Controls.Add(txtPathPaghe);
            grpPercorsi.Controls.Add(lblPathPaghe);
            grpPercorsi.Controls.Add(btnSfogliaFox);
            grpPercorsi.Controls.Add(txtPathFox);
            grpPercorsi.Controls.Add(lblPathFox);
            grpPercorsi.Controls.Add(cboLogMode);
            grpPercorsi.Controls.Add(lblLogMode);
            grpPercorsi.Controls.Add(btnSfogliaLastLog);
            grpPercorsi.Controls.Add(txtLastLogPath);
            grpPercorsi.Controls.Add(lblLastLogPath);
            grpPercorsi.Controls.Add(btnSfogliaGenerale);
            grpPercorsi.Controls.Add(txtPathGenerale);
            grpPercorsi.Controls.Add(lblPathGenerale);
            grpPercorsi.Location = new Point(14, 340);
            grpPercorsi.Margin = new Padding(4, 3, 4, 3);
            grpPercorsi.Name = "grpPercorsi";
            grpPercorsi.Padding = new Padding(4, 3, 4, 3);
            grpPercorsi.Size = new Size(618, 205);
            grpPercorsi.TabIndex = 3;
            grpPercorsi.TabStop = false;
            grpPercorsi.Text = "Percorsi Cartelle di Lavoro";
            // 
            // btnSfogliaPaghe
            // 
            btnSfogliaPaghe.Location = new Point(566, 100);
            btnSfogliaPaghe.Margin = new Padding(4, 3, 4, 3);
            btnSfogliaPaghe.Name = "btnSfogliaPaghe";
            btnSfogliaPaghe.Size = new Size(35, 27);
            btnSfogliaPaghe.TabIndex = 8;
            btnSfogliaPaghe.Text = "...";
            btnSfogliaPaghe.UseVisualStyleBackColor = true;
            btnSfogliaPaghe.Click += BtnSfoglia_Click;
            // 
            // txtPathPaghe
            // 
            txtPathPaghe.Location = new Point(128, 103);
            txtPathPaghe.Margin = new Padding(4, 3, 4, 3);
            txtPathPaghe.Name = "txtPathPaghe";
            txtPathPaghe.Size = new Size(430, 23);
            txtPathPaghe.TabIndex = 7;
            // 
            // lblPathPaghe
            // 
            lblPathPaghe.AutoSize = true;
            lblPathPaghe.Location = new Point(18, 106);
            lblPathPaghe.Margin = new Padding(4, 0, 4, 0);
            lblPathPaghe.Name = "lblPathPaghe";
            lblPathPaghe.Size = new Size(79, 15);
            lblPathPaghe.TabIndex = 6;
            lblPathPaghe.Text = "Flusso Paghe:";
            // 
            // btnSfogliaFox
            // 
            btnSfogliaFox.Location = new Point(566, 65);
            btnSfogliaFox.Margin = new Padding(4, 3, 4, 3);
            btnSfogliaFox.Name = "btnSfogliaFox";
            btnSfogliaFox.Size = new Size(35, 27);
            btnSfogliaFox.TabIndex = 5;
            btnSfogliaFox.Text = "...";
            btnSfogliaFox.UseVisualStyleBackColor = true;
            btnSfogliaFox.Click += BtnSfoglia_Click;
            // 
            // txtPathFox
            // 
            txtPathFox.Location = new Point(128, 67);
            txtPathFox.Margin = new Padding(4, 3, 4, 3);
            txtPathFox.Name = "txtPathFox";
            txtPathFox.Size = new Size(430, 23);
            txtPathFox.TabIndex = 4;
            // 
            // lblPathFox
            // 
            lblPathFox.AutoSize = true;
            lblPathFox.Location = new Point(18, 70);
            lblPathFox.Margin = new Padding(4, 0, 4, 0);
            lblPathFox.Name = "lblPathFox";
            lblPathFox.Size = new Size(72, 15);
            lblPathFox.TabIndex = 3;
            lblPathFox.Text = "Cartella Fox:";
            // 
            // cboLogMode
            // 
            cboLogMode.DropDownStyle = ComboBoxStyle.DropDownList;
            cboLogMode.Location = new Point(128, 171);
            cboLogMode.Margin = new Padding(4, 3, 4, 3);
            cboLogMode.Name = "cboLogMode";
            cboLogMode.Size = new Size(163, 23);
            cboLogMode.TabIndex = 13;
            // 
            // lblLogMode
            // 
            lblLogMode.AutoSize = true;
            lblLogMode.Location = new Point(18, 174);
            lblLogMode.Margin = new Padding(4, 0, 4, 0);
            lblLogMode.Name = "lblLogMode";
            lblLogMode.Size = new Size(80, 15);
            lblLogMode.TabIndex = 12;
            lblLogMode.Text = "Modalità Log:";
            // 
            // btnSfogliaLastLog
            // 
            btnSfogliaLastLog.Location = new Point(566, 137);
            btnSfogliaLastLog.Margin = new Padding(4, 3, 4, 3);
            btnSfogliaLastLog.Name = "btnSfogliaLastLog";
            btnSfogliaLastLog.Size = new Size(35, 27);
            btnSfogliaLastLog.TabIndex = 11;
            btnSfogliaLastLog.Text = "...";
            btnSfogliaLastLog.UseVisualStyleBackColor = true;
            btnSfogliaLastLog.Click += BtnSfoglia_Click;
            // 
            // txtLastLogPath
            // 
            txtLastLogPath.Location = new Point(128, 138);
            txtLastLogPath.Margin = new Padding(4, 3, 4, 3);
            txtLastLogPath.Name = "txtLastLogPath";
            txtLastLogPath.Size = new Size(430, 23);
            txtLastLogPath.TabIndex = 10;
            // 
            // lblLastLogPath
            // 
            lblLastLogPath.AutoSize = true;
            lblLastLogPath.Location = new Point(18, 142);
            lblLastLogPath.Margin = new Padding(4, 0, 4, 0);
            lblLastLogPath.Name = "lblLastLogPath";
            lblLastLogPath.Size = new Size(73, 15);
            lblLastLogPath.TabIndex = 9;
            lblLastLogPath.Text = "Cartella Log:";
            // 
            // btnSfogliaGenerale
            // 
            btnSfogliaGenerale.Location = new Point(566, 29);
            btnSfogliaGenerale.Margin = new Padding(4, 3, 4, 3);
            btnSfogliaGenerale.Name = "btnSfogliaGenerale";
            btnSfogliaGenerale.Size = new Size(35, 27);
            btnSfogliaGenerale.TabIndex = 2;
            btnSfogliaGenerale.Text = "...";
            btnSfogliaGenerale.UseVisualStyleBackColor = true;
            btnSfogliaGenerale.Click += BtnSfoglia_Click;
            // 
            // txtPathGenerale
            // 
            txtPathGenerale.Location = new Point(128, 31);
            txtPathGenerale.Margin = new Padding(4, 3, 4, 3);
            txtPathGenerale.Name = "txtPathGenerale";
            txtPathGenerale.Size = new Size(430, 23);
            txtPathGenerale.TabIndex = 1;
            // 
            // lblPathGenerale
            // 
            lblPathGenerale.AutoSize = true;
            lblPathGenerale.Location = new Point(18, 35);
            lblPathGenerale.Margin = new Padding(4, 0, 4, 0);
            lblPathGenerale.Name = "lblPathGenerale";
            lblPathGenerale.Size = new Size(83, 15);
            lblPathGenerale.TabIndex = 0;
            lblPathGenerale.Text = "Path Generale:";
            // 
            // btnApriJson
            // 
            btnApriJson.Location = new Point(14, 563);
            btnApriJson.Margin = new Padding(4, 3, 4, 3);
            btnApriJson.Name = "btnApriJson";
            btnApriJson.Size = new Size(169, 35);
            btnApriJson.TabIndex = 4;
            btnApriJson.Text = "Modifica file .json";
            btnApriJson.UseVisualStyleBackColor = true;
            btnApriJson.Click += BtnApriJson_Click;
            // 
            // btnSalva
            // 
            btnSalva.Location = new Point(432, 563);
            btnSalva.Margin = new Padding(4, 3, 4, 3);
            btnSalva.Name = "btnSalva";
            btnSalva.Size = new Size(99, 35);
            btnSalva.TabIndex = 5;
            btnSalva.Text = "Salva";
            btnSalva.UseVisualStyleBackColor = true;
            btnSalva.Click += BtnSalva_Click;
            // 
            // btnAnnulla
            // 
            btnAnnulla.DialogResult = DialogResult.Cancel;
            btnAnnulla.Location = new Point(538, 563);
            btnAnnulla.Margin = new Padding(4, 3, 4, 3);
            btnAnnulla.Name = "btnAnnulla";
            btnAnnulla.Size = new Size(94, 35);
            btnAnnulla.TabIndex = 6;
            btnAnnulla.Text = "Annulla";
            btnAnnulla.UseVisualStyleBackColor = true;
            // 
            // FSettings
            // 
            AcceptButton = btnSalva;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = btnAnnulla;
            ClientSize = new Size(646, 609);
            Controls.Add(btnAnnulla);
            Controls.Add(btnSalva);
            Controls.Add(btnApriJson);
            Controls.Add(grpPercorsi);
            Controls.Add(grpDatabase);
            Controls.Add(grpAzienda);
            Controls.Add(grpConn);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Margin = new Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FSettings";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Configurazione Database e Percorsi";
            Load += FSettings_Load;
            grpConn.ResumeLayout(false);
            grpConn.PerformLayout();
            grpAzienda.ResumeLayout(false);
            grpAzienda.PerformLayout();
            grpDatabase.ResumeLayout(false);
            grpDatabase.PerformLayout();
            grpPercorsi.ResumeLayout(false);
            grpPercorsi.PerformLayout();
            ResumeLayout(false);

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
        private System.Windows.Forms.Label lblLastLogPath;
        private System.Windows.Forms.TextBox txtLastLogPath;
        private System.Windows.Forms.Button btnSfogliaLastLog;
        private System.Windows.Forms.Label lblLogMode;
        private System.Windows.Forms.ComboBox cboLogMode;
        private System.Windows.Forms.Button btnApriJson;
        private System.Windows.Forms.Button btnSalva;
        private System.Windows.Forms.Button btnAnnulla;
    }
}