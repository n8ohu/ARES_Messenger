namespace ARES_Messenger
{
    partial class ChatSetupForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.txtCall = new System.Windows.Forms.TextBox();
            this.txtARQID = new System.Windows.Forms.TextBox();
            this.cmbCapture = new System.Windows.Forms.ComboBox();
            this.cmbPlayback = new System.Windows.Forms.ComboBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.btnRadio = new System.Windows.Forms.Button();
            this.btnTwoTone = new System.Windows.Forms.Button();
            this.nudDriveLevel = new System.Windows.Forms.NumericUpDown();
            this.nudTNCTCPIPPort = new System.Windows.Forms.NumericUpDown();
            this.grpInterface = new System.Windows.Forms.GroupBox();
            this.lblPairing = new System.Windows.Forms.Label();
            this.RadioButton1 = new System.Windows.Forms.RadioButton();
            this.txtPairing = new System.Windows.Forms.TextBox();
            this.lblTNCTCPIPPort = new System.Windows.Forms.Label();
            this.rdoTCPIP = new System.Windows.Forms.RadioButton();
            this.lblHostAddress = new System.Windows.Forms.Label();
            this.txtTCPIPAddress = new System.Windows.Forms.TextBox();
            this.chkMorseCodeId = new System.Windows.Forms.CheckBox();
            this.cmbCharacterSet = new System.Windows.Forms.ComboBox();
            this.nudTuning = new System.Windows.Forms.NumericUpDown();
            this.nudARQTimeout = new System.Windows.Forms.NumericUpDown();
            this.chkDebugLog = new System.Windows.Forms.CheckBox();
            this.chkEnbAutoupdate = new System.Windows.Forms.CheckBox();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rdoCR = new System.Windows.Forms.RadioButton();
            this.rdoCtrlCR = new System.Windows.Forms.RadioButton();
            this.rdoSpace = new System.Windows.Forms.RadioButton();
            this.rdoWord = new System.Windows.Forms.RadioButton();
            this.nudFont = new System.Windows.Forms.NumericUpDown();
            this.nudFrontPorch = new System.Windows.Forms.NumericUpDown();
            this.nudBackPorch = new System.Windows.Forms.NumericUpDown();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnHelp = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.nudDriveLevel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTNCTCPIPPort)).BeginInit();
            this.grpInterface.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudTuning)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudARQTimeout)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudFont)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudFrontPorch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudBackPorch)).BeginInit();
            this.SuspendLayout();
            // 
            // txtCall
            // 
            this.txtCall.Location = new System.Drawing.Point(64, 17);
            this.txtCall.Name = "txtCall";
            this.txtCall.Size = new System.Drawing.Size(125, 20);
            this.txtCall.TabIndex = 0;
            // 
            // txtARQID
            // 
            this.txtARQID.Location = new System.Drawing.Point(271, 17);
            this.txtARQID.Name = "txtARQID";
            this.txtARQID.Size = new System.Drawing.Size(166, 20);
            this.txtARQID.TabIndex = 1;
            // 
            // cmbCapture
            // 
            this.cmbCapture.FormattingEnabled = true;
            this.cmbCapture.Location = new System.Drawing.Point(125, 43);
            this.cmbCapture.Name = "cmbCapture";
            this.cmbCapture.Size = new System.Drawing.Size(264, 21);
            this.cmbCapture.TabIndex = 2;
            // 
            // cmbPlayback
            // 
            this.cmbPlayback.FormattingEnabled = true;
            this.cmbPlayback.Location = new System.Drawing.Point(125, 72);
            this.cmbPlayback.Name = "cmbPlayback";
            this.cmbPlayback.Size = new System.Drawing.Size(264, 21);
            this.cmbPlayback.TabIndex = 3;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(537, 287);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnUpdate
            // 
            this.btnUpdate.Location = new System.Drawing.Point(351, 287);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(171, 29);
            this.btnUpdate.TabIndex = 5;
            this.btnUpdate.Text = "Update ARDOP Chat Setup";
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // btnRadio
            // 
            this.btnRadio.Location = new System.Drawing.Point(253, 287);
            this.btnRadio.Name = "btnRadio";
            this.btnRadio.Size = new System.Drawing.Size(75, 21);
            this.btnRadio.TabIndex = 6;
            this.btnRadio.Text = "Radio Setup";
            this.btnRadio.UseVisualStyleBackColor = true;
            this.btnRadio.Click += new System.EventHandler(this.btnRadio_Click);
            // 
            // btnTwoTone
            // 
            this.btnTwoTone.Location = new System.Drawing.Point(143, 291);
            this.btnTwoTone.Name = "btnTwoTone";
            this.btnTwoTone.Size = new System.Drawing.Size(91, 21);
            this.btnTwoTone.TabIndex = 7;
            this.btnTwoTone.Text = "Two Tone Test";
            this.btnTwoTone.UseVisualStyleBackColor = true;
            // 
            // nudDriveLevel
            // 
            this.nudDriveLevel.Location = new System.Drawing.Point(85, 292);
            this.nudDriveLevel.Name = "nudDriveLevel";
            this.nudDriveLevel.Size = new System.Drawing.Size(48, 20);
            this.nudDriveLevel.TabIndex = 8;
            // 
            // nudTNCTCPIPPort
            // 
            this.nudTNCTCPIPPort.Location = new System.Drawing.Point(218, 22);
            this.nudTNCTCPIPPort.Name = "nudTNCTCPIPPort";
            this.nudTNCTCPIPPort.Size = new System.Drawing.Size(73, 20);
            this.nudTNCTCPIPPort.TabIndex = 9;
            // 
            // grpInterface
            // 
            this.grpInterface.Controls.Add(this.lblPairing);
            this.grpInterface.Controls.Add(this.RadioButton1);
            this.grpInterface.Controls.Add(this.txtPairing);
            this.grpInterface.Controls.Add(this.lblTNCTCPIPPort);
            this.grpInterface.Controls.Add(this.rdoTCPIP);
            this.grpInterface.Controls.Add(this.lblHostAddress);
            this.grpInterface.Controls.Add(this.txtTCPIPAddress);
            this.grpInterface.Controls.Add(this.nudTNCTCPIPPort);
            this.grpInterface.Location = new System.Drawing.Point(21, 109);
            this.grpInterface.Name = "grpInterface";
            this.grpInterface.Size = new System.Drawing.Size(367, 98);
            this.grpInterface.TabIndex = 10;
            this.grpInterface.TabStop = false;
            this.grpInterface.Text = "Virtual ARDOP TNC Interface";
            // 
            // lblPairing
            // 
            this.lblPairing.AutoSize = true;
            this.lblPairing.Enabled = false;
            this.lblPairing.Location = new System.Drawing.Point(84, 81);
            this.lblPairing.Name = "lblPairing";
            this.lblPairing.Size = new System.Drawing.Size(42, 13);
            this.lblPairing.TabIndex = 16;
            this.lblPairing.Text = "Pairing:";
            // 
            // RadioButton1
            // 
            this.RadioButton1.AutoSize = true;
            this.RadioButton1.Enabled = false;
            this.RadioButton1.Location = new System.Drawing.Point(7, 75);
            this.RadioButton1.Name = "RadioButton1";
            this.RadioButton1.Size = new System.Drawing.Size(70, 17);
            this.RadioButton1.TabIndex = 15;
            this.RadioButton1.Text = "Bluetooth";
            this.RadioButton1.UseVisualStyleBackColor = true;
            // 
            // txtPairing
            // 
            this.txtPairing.Location = new System.Drawing.Point(218, 75);
            this.txtPairing.Name = "txtPairing";
            this.txtPairing.Size = new System.Drawing.Size(100, 20);
            this.txtPairing.TabIndex = 14;
            // 
            // lblTNCTCPIPPort
            // 
            this.lblTNCTCPIPPort.AutoSize = true;
            this.lblTNCTCPIPPort.Location = new System.Drawing.Point(90, 24);
            this.lblTNCTCPIPPort.Name = "lblTNCTCPIPPort";
            this.lblTNCTCPIPPort.Size = new System.Drawing.Size(88, 13);
            this.lblTNCTCPIPPort.TabIndex = 13;
            this.lblTNCTCPIPPort.Text = "TNC TCPIP Port:";
            // 
            // rdoTCPIP
            // 
            this.rdoTCPIP.AutoSize = true;
            this.rdoTCPIP.Checked = true;
            this.rdoTCPIP.Location = new System.Drawing.Point(6, 22);
            this.rdoTCPIP.Name = "rdoTCPIP";
            this.rdoTCPIP.Size = new System.Drawing.Size(56, 17);
            this.rdoTCPIP.TabIndex = 12;
            this.rdoTCPIP.TabStop = true;
            this.rdoTCPIP.Text = "TCPIP";
            this.rdoTCPIP.UseVisualStyleBackColor = true;
            this.rdoTCPIP.CheckedChanged += new System.EventHandler(this.rdoTCPIP_CheckedChanged);
            // 
            // lblHostAddress
            // 
            this.lblHostAddress.AutoSize = true;
            this.lblHostAddress.Location = new System.Drawing.Point(76, 54);
            this.lblHostAddress.Name = "lblHostAddress";
            this.lblHostAddress.Size = new System.Drawing.Size(103, 13);
            this.lblHostAddress.TabIndex = 11;
            this.lblHostAddress.Text = "Host address/name:";
            // 
            // txtTCPIPAddress
            // 
            this.txtTCPIPAddress.Location = new System.Drawing.Point(218, 48);
            this.txtTCPIPAddress.Name = "txtTCPIPAddress";
            this.txtTCPIPAddress.Size = new System.Drawing.Size(100, 20);
            this.txtTCPIPAddress.TabIndex = 10;
            // 
            // chkMorseCodeId
            // 
            this.chkMorseCodeId.AutoSize = true;
            this.chkMorseCodeId.Location = new System.Drawing.Point(52, 319);
            this.chkMorseCodeId.Name = "chkMorseCodeId";
            this.chkMorseCodeId.Size = new System.Drawing.Size(117, 17);
            this.chkMorseCodeId.TabIndex = 11;
            this.chkMorseCodeId.Text = "Use Morse Code Id";
            this.chkMorseCodeId.UseVisualStyleBackColor = true;
            // 
            // cmbCharacterSet
            // 
            this.cmbCharacterSet.FormattingEnabled = true;
            this.cmbCharacterSet.Location = new System.Drawing.Point(100, 243);
            this.cmbCharacterSet.Name = "cmbCharacterSet";
            this.cmbCharacterSet.Size = new System.Drawing.Size(121, 21);
            this.cmbCharacterSet.TabIndex = 12;
            // 
            // nudTuning
            // 
            this.nudTuning.Location = new System.Drawing.Point(497, 156);
            this.nudTuning.Name = "nudTuning";
            this.nudTuning.Size = new System.Drawing.Size(120, 20);
            this.nudTuning.TabIndex = 13;
            // 
            // nudARQTimeout
            // 
            this.nudARQTimeout.Location = new System.Drawing.Point(517, 184);
            this.nudARQTimeout.Name = "nudARQTimeout";
            this.nudARQTimeout.Size = new System.Drawing.Size(120, 20);
            this.nudARQTimeout.TabIndex = 14;
            // 
            // chkDebugLog
            // 
            this.chkDebugLog.AutoSize = true;
            this.chkDebugLog.Location = new System.Drawing.Point(239, 327);
            this.chkDebugLog.Name = "chkDebugLog";
            this.chkDebugLog.Size = new System.Drawing.Size(80, 17);
            this.chkDebugLog.TabIndex = 15;
            this.chkDebugLog.Text = "checkBox1";
            this.chkDebugLog.UseVisualStyleBackColor = true;
            // 
            // chkEnbAutoupdate
            // 
            this.chkEnbAutoupdate.AutoSize = true;
            this.chkEnbAutoupdate.Location = new System.Drawing.Point(52, 214);
            this.chkEnbAutoupdate.Name = "chkEnbAutoupdate";
            this.chkEnbAutoupdate.Size = new System.Drawing.Size(80, 17);
            this.chkEnbAutoupdate.TabIndex = 16;
            this.chkEnbAutoupdate.Text = "checkBox2";
            this.chkEnbAutoupdate.UseVisualStyleBackColor = true;
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.Location = new System.Drawing.Point(154, 220);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(80, 17);
            this.checkBox3.TabIndex = 17;
            this.checkBox3.Text = "checkBox3";
            this.checkBox3.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Location = new System.Drawing.Point(497, -37);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 100);
            this.groupBox1.TabIndex = 18;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "groupBox1";
            // 
            // rdoCR
            // 
            this.rdoCR.AutoSize = true;
            this.rdoCR.Location = new System.Drawing.Point(451, 240);
            this.rdoCR.Name = "rdoCR";
            this.rdoCR.Size = new System.Drawing.Size(85, 17);
            this.rdoCR.TabIndex = 19;
            this.rdoCR.TabStop = true;
            this.rdoCR.Text = "radioButton2";
            this.rdoCR.UseVisualStyleBackColor = true;
            // 
            // rdoCtrlCR
            // 
            this.rdoCtrlCR.AutoSize = true;
            this.rdoCtrlCR.Location = new System.Drawing.Point(517, 326);
            this.rdoCtrlCR.Name = "rdoCtrlCR";
            this.rdoCtrlCR.Size = new System.Drawing.Size(67, 17);
            this.rdoCtrlCR.TabIndex = 20;
            this.rdoCtrlCR.TabStop = true;
            this.rdoCtrlCR.Text = "rdoCtlCR";
            this.rdoCtrlCR.UseVisualStyleBackColor = true;
            // 
            // rdoSpace
            // 
            this.rdoSpace.AutoSize = true;
            this.rdoSpace.Location = new System.Drawing.Point(394, 160);
            this.rdoSpace.Name = "rdoSpace";
            this.rdoSpace.Size = new System.Drawing.Size(85, 17);
            this.rdoSpace.TabIndex = 21;
            this.rdoSpace.TabStop = true;
            this.rdoSpace.Text = "radioButton2";
            this.rdoSpace.UseVisualStyleBackColor = true;
            // 
            // rdoWord
            // 
            this.rdoWord.AutoSize = true;
            this.rdoWord.Location = new System.Drawing.Point(375, 327);
            this.rdoWord.Name = "rdoWord";
            this.rdoWord.Size = new System.Drawing.Size(85, 17);
            this.rdoWord.TabIndex = 22;
            this.rdoWord.TabStop = true;
            this.rdoWord.Text = "radioButton2";
            this.rdoWord.UseVisualStyleBackColor = true;
            // 
            // nudFont
            // 
            this.nudFont.Location = new System.Drawing.Point(253, 243);
            this.nudFont.Name = "nudFont";
            this.nudFont.Size = new System.Drawing.Size(120, 20);
            this.nudFont.TabIndex = 23;
            // 
            // nudFrontPorch
            // 
            this.nudFrontPorch.Location = new System.Drawing.Point(497, 93);
            this.nudFrontPorch.Maximum = new decimal(new int[] {
            128,
            0,
            0,
            0});
            this.nudFrontPorch.Minimum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.nudFrontPorch.Name = "nudFrontPorch";
            this.nudFrontPorch.Size = new System.Drawing.Size(120, 20);
            this.nudFrontPorch.TabIndex = 24;
            this.nudFrontPorch.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            // 
            // nudBackPorch
            // 
            this.nudBackPorch.Location = new System.Drawing.Point(340, 214);
            this.nudBackPorch.Maximum = new decimal(new int[] {
            256,
            0,
            0,
            0});
            this.nudBackPorch.Name = "nudBackPorch";
            this.nudBackPorch.Size = new System.Drawing.Size(120, 20);
            this.nudBackPorch.TabIndex = 25;
            // 
            // btnHelp
            // 
            this.btnHelp.Location = new System.Drawing.Point(537, 227);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(79, 29);
            this.btnHelp.TabIndex = 26;
            this.btnHelp.Text = "Setup Help";
            this.btnHelp.UseVisualStyleBackColor = true;
            // 
            // ChatSetupForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(629, 356);
            this.Controls.Add(this.btnHelp);
            this.Controls.Add(this.nudBackPorch);
            this.Controls.Add(this.nudFrontPorch);
            this.Controls.Add(this.nudFont);
            this.Controls.Add(this.rdoWord);
            this.Controls.Add(this.rdoSpace);
            this.Controls.Add(this.rdoCtrlCR);
            this.Controls.Add(this.rdoCR);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.checkBox3);
            this.Controls.Add(this.chkEnbAutoupdate);
            this.Controls.Add(this.chkDebugLog);
            this.Controls.Add(this.nudARQTimeout);
            this.Controls.Add(this.nudTuning);
            this.Controls.Add(this.cmbCharacterSet);
            this.Controls.Add(this.chkMorseCodeId);
            this.Controls.Add(this.grpInterface);
            this.Controls.Add(this.nudDriveLevel);
            this.Controls.Add(this.btnTwoTone);
            this.Controls.Add(this.btnRadio);
            this.Controls.Add(this.btnUpdate);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.cmbPlayback);
            this.Controls.Add(this.cmbCapture);
            this.Controls.Add(this.txtARQID);
            this.Controls.Add(this.txtCall);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ChatSetupForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Chat Setup";
            this.Load += new System.EventHandler(this.H4Setup_Load);
            ((System.ComponentModel.ISupportInitialize)(this.nudDriveLevel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTNCTCPIPPort)).EndInit();
            this.grpInterface.ResumeLayout(false);
            this.grpInterface.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudTuning)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudARQTimeout)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudFont)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudFrontPorch)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudBackPorch)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtCall;
        private System.Windows.Forms.TextBox txtARQID;
        private System.Windows.Forms.ComboBox cmbCapture;
        private System.Windows.Forms.ComboBox cmbPlayback;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Button btnRadio;
        private System.Windows.Forms.Button btnTwoTone;
        private System.Windows.Forms.NumericUpDown nudDriveLevel;
        private System.Windows.Forms.NumericUpDown nudTNCTCPIPPort;
        private System.Windows.Forms.GroupBox grpInterface;
        private System.Windows.Forms.Label lblHostAddress;
        private System.Windows.Forms.TextBox txtTCPIPAddress;
        private System.Windows.Forms.CheckBox chkMorseCodeId;
        private System.Windows.Forms.RadioButton rdoTCPIP;
        private System.Windows.Forms.Label lblTNCTCPIPPort;
        private System.Windows.Forms.Label lblPairing;
        private System.Windows.Forms.RadioButton RadioButton1;
        private System.Windows.Forms.TextBox txtPairing;
        private System.Windows.Forms.ComboBox cmbCharacterSet;
        private System.Windows.Forms.NumericUpDown nudTuning;
        private System.Windows.Forms.NumericUpDown nudARQTimeout;
        private System.Windows.Forms.CheckBox chkDebugLog;
        private System.Windows.Forms.CheckBox chkEnbAutoupdate;
        private System.Windows.Forms.CheckBox checkBox3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rdoCR;
        private System.Windows.Forms.RadioButton rdoCtrlCR;
        private System.Windows.Forms.RadioButton rdoSpace;
        private System.Windows.Forms.RadioButton rdoWord;
        private System.Windows.Forms.NumericUpDown nudFont;
        private System.Windows.Forms.NumericUpDown nudFrontPorch;
        private System.Windows.Forms.NumericUpDown nudBackPorch;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button btnHelp;
    }
}