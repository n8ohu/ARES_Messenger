namespace ARES_Messenger
{
    partial class Radio
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
            this.grpRadioControlPort = new System.Windows.Forms.GroupBox();
            this.chkControlDTR = new System.Windows.Forms.CheckBox();
            this.chkControlRTS = new System.Windows.Forms.CheckBox();
            this.cmbControlBaud = new System.Windows.Forms.ComboBox();
            this.cmbControlPort = new System.Windows.Forms.ComboBox();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.grpRadioSettings = new System.Windows.Forms.GroupBox();
            this.Label5 = new System.Windows.Forms.Label();
            this.Label2 = new System.Windows.Forms.Label();
            this.lblIcomAddress = new System.Windows.Forms.Label();
            this.txtIcomAddress = new System.Windows.Forms.TextBox();
            this.rdoFM = new System.Windows.Forms.RadioButton();
            this.rdoUSBDigital = new System.Windows.Forms.RadioButton();
            this.chkInternalTuner = new System.Windows.Forms.CheckBox();
            this.rdoUSB = new System.Windows.Forms.RadioButton();
            this.cmbModel = new System.Windows.Forms.ComboBox();
            this.cmbAntenna = new System.Windows.Forms.ComboBox();
            this.grpPTTPort = new System.Windows.Forms.GroupBox();
            this.chkPTTRTS = new System.Windows.Forms.CheckBox();
            this.chkPTTDTR = new System.Windows.Forms.CheckBox();
            this.cmbPTTBaud = new System.Windows.Forms.ComboBox();
            this.cmbPTTPort = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.grpRadioControlPort.SuspendLayout();
            this.grpRadioSettings.SuspendLayout();
            this.grpPTTPort.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpRadioControlPort
            // 
            this.grpRadioControlPort.Controls.Add(this.label6);
            this.grpRadioControlPort.Controls.Add(this.label1);
            this.grpRadioControlPort.Controls.Add(this.chkControlDTR);
            this.grpRadioControlPort.Controls.Add(this.chkControlRTS);
            this.grpRadioControlPort.Controls.Add(this.cmbControlBaud);
            this.grpRadioControlPort.Controls.Add(this.cmbControlPort);
            this.grpRadioControlPort.Location = new System.Drawing.Point(12, 96);
            this.grpRadioControlPort.Name = "grpRadioControlPort";
            this.grpRadioControlPort.Size = new System.Drawing.Size(583, 47);
            this.grpRadioControlPort.TabIndex = 0;
            this.grpRadioControlPort.TabStop = false;
            this.grpRadioControlPort.Text = "Radio Control Port";
            // 
            // chkControlDTR
            // 
            this.chkControlDTR.AutoSize = true;
            this.chkControlDTR.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkControlDTR.Location = new System.Drawing.Point(545, 20);
            this.chkControlDTR.Name = "chkControlDTR";
            this.chkControlDTR.Size = new System.Drawing.Size(85, 17);
            this.chkControlDTR.TabIndex = 3;
            this.chkControlDTR.Text = "Enable DTR";
            this.chkControlDTR.UseVisualStyleBackColor = true;
            // 
            // chkControlRTS
            // 
            this.chkControlRTS.AutoSize = true;
            this.chkControlRTS.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkControlRTS.Location = new System.Drawing.Point(467, 20);
            this.chkControlRTS.Name = "chkControlRTS";
            this.chkControlRTS.Size = new System.Drawing.Size(84, 17);
            this.chkControlRTS.TabIndex = 2;
            this.chkControlRTS.Text = "Enable RTS";
            this.chkControlRTS.UseVisualStyleBackColor = true;
            // 
            // cmbControlBaud
            // 
            this.cmbControlBaud.FormattingEnabled = true;
            this.cmbControlBaud.Location = new System.Drawing.Point(327, 20);
            this.cmbControlBaud.Name = "cmbControlBaud";
            this.cmbControlBaud.Size = new System.Drawing.Size(121, 21);
            this.cmbControlBaud.TabIndex = 1;
            // 
            // cmbControlPort
            // 
            this.cmbControlPort.FormattingEnabled = true;
            this.cmbControlPort.Location = new System.Drawing.Point(154, 20);
            this.cmbControlPort.Name = "cmbControlPort";
            this.cmbControlPort.Size = new System.Drawing.Size(121, 21);
            this.cmbControlPort.TabIndex = 0;
            // 
            // btnUpdate
            // 
            this.btnUpdate.Location = new System.Drawing.Point(248, 218);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(150, 30);
            this.btnUpdate.TabIndex = 1;
            this.btnUpdate.Text = "Save Radio Settings";
            this.btnUpdate.UseVisualStyleBackColor = true;
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(409, 218);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(150, 30);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            // 
            // grpRadioSettings
            // 
            this.grpRadioSettings.Controls.Add(this.Label5);
            this.grpRadioSettings.Controls.Add(this.Label2);
            this.grpRadioSettings.Controls.Add(this.lblIcomAddress);
            this.grpRadioSettings.Controls.Add(this.txtIcomAddress);
            this.grpRadioSettings.Controls.Add(this.rdoFM);
            this.grpRadioSettings.Controls.Add(this.rdoUSBDigital);
            this.grpRadioSettings.Controls.Add(this.chkInternalTuner);
            this.grpRadioSettings.Controls.Add(this.rdoUSB);
            this.grpRadioSettings.Controls.Add(this.cmbModel);
            this.grpRadioSettings.Controls.Add(this.cmbAntenna);
            this.grpRadioSettings.Location = new System.Drawing.Point(12, 12);
            this.grpRadioSettings.Name = "grpRadioSettings";
            this.grpRadioSettings.Size = new System.Drawing.Size(583, 75);
            this.grpRadioSettings.TabIndex = 3;
            this.grpRadioSettings.TabStop = false;
            this.grpRadioSettings.Text = "Radio Selection";
            // 
            // Label5
            // 
            this.Label5.AutoSize = true;
            this.Label5.Location = new System.Drawing.Point(312, 22);
            this.Label5.Name = "Label5";
            this.Label5.Size = new System.Drawing.Size(94, 13);
            this.Label5.TabIndex = 9;
            this.Label5.Text = "Antenna Selection";
            // 
            // Label2
            // 
            this.Label2.AutoSize = true;
            this.Label2.Location = new System.Drawing.Point(20, 22);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(100, 13);
            this.Label2.TabIndex = 8;
            this.Label2.Text = "Select Radio Model";
            // 
            // lblIcomAddress
            // 
            this.lblIcomAddress.AutoSize = true;
            this.lblIcomAddress.Location = new System.Drawing.Point(36, 53);
            this.lblIcomAddress.Name = "lblIcomAddress";
            this.lblIcomAddress.Size = new System.Drawing.Size(71, 13);
            this.lblIcomAddress.TabIndex = 7;
            this.lblIcomAddress.Text = "Icom Address";
            // 
            // txtIcomAddress
            // 
            this.txtIcomAddress.Location = new System.Drawing.Point(108, 50);
            this.txtIcomAddress.Name = "txtIcomAddress";
            this.txtIcomAddress.Size = new System.Drawing.Size(27, 20);
            this.txtIcomAddress.TabIndex = 6;
            // 
            // rdoFM
            // 
            this.rdoFM.AutoSize = true;
            this.rdoFM.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.rdoFM.Location = new System.Drawing.Point(342, 53);
            this.rdoFM.Name = "rdoFM";
            this.rdoFM.Size = new System.Drawing.Size(64, 17);
            this.rdoFM.TabIndex = 5;
            this.rdoFM.TabStop = true;
            this.rdoFM.Text = "VHF FM";
            this.rdoFM.UseVisualStyleBackColor = true;
            // 
            // rdoUSBDigital
            // 
            this.rdoUSBDigital.AutoSize = true;
            this.rdoUSBDigital.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.rdoUSBDigital.Location = new System.Drawing.Point(236, 51);
            this.rdoUSBDigital.Name = "rdoUSBDigital";
            this.rdoUSBDigital.Size = new System.Drawing.Size(79, 17);
            this.rdoUSBDigital.TabIndex = 4;
            this.rdoUSBDigital.TabStop = true;
            this.rdoUSBDigital.Text = "USB Digital";
            this.rdoUSBDigital.UseVisualStyleBackColor = true;
            // 
            // chkInternalTuner
            // 
            this.chkInternalTuner.AutoSize = true;
            this.chkInternalTuner.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkInternalTuner.Location = new System.Drawing.Point(455, 52);
            this.chkInternalTuner.Name = "chkInternalTuner";
            this.chkInternalTuner.Size = new System.Drawing.Size(128, 17);
            this.chkInternalTuner.TabIndex = 3;
            this.chkInternalTuner.Text = "Enable Internal Tuner";
            this.chkInternalTuner.UseVisualStyleBackColor = true;
            // 
            // rdoUSB
            // 
            this.rdoUSB.AutoSize = true;
            this.rdoUSB.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.rdoUSB.Checked = true;
            this.rdoUSB.Location = new System.Drawing.Point(162, 53);
            this.rdoUSB.Name = "rdoUSB";
            this.rdoUSB.Size = new System.Drawing.Size(47, 17);
            this.rdoUSB.TabIndex = 2;
            this.rdoUSB.TabStop = true;
            this.rdoUSB.Text = "USB";
            this.rdoUSB.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.rdoUSB.UseVisualStyleBackColor = true;
            // 
            // cmbModel
            // 
            this.cmbModel.FormattingEnabled = true;
            this.cmbModel.Location = new System.Drawing.Point(126, 19);
            this.cmbModel.Name = "cmbModel";
            this.cmbModel.Size = new System.Drawing.Size(163, 21);
            this.cmbModel.TabIndex = 1;
            // 
            // cmbAntenna
            // 
            this.cmbAntenna.FormattingEnabled = true;
            this.cmbAntenna.Items.AddRange(new object[] {
            "Default",
            "Internal 1",
            "Internal 2"});
            this.cmbAntenna.Location = new System.Drawing.Point(412, 19);
            this.cmbAntenna.Name = "cmbAntenna";
            this.cmbAntenna.Size = new System.Drawing.Size(163, 21);
            this.cmbAntenna.TabIndex = 0;
            this.cmbAntenna.Text = "Default";
            // 
            // grpPTTPort
            // 
            this.grpPTTPort.Controls.Add(this.label4);
            this.grpPTTPort.Controls.Add(this.label3);
            this.grpPTTPort.Controls.Add(this.chkPTTRTS);
            this.grpPTTPort.Controls.Add(this.chkPTTDTR);
            this.grpPTTPort.Controls.Add(this.cmbPTTBaud);
            this.grpPTTPort.Controls.Add(this.cmbPTTPort);
            this.grpPTTPort.Location = new System.Drawing.Point(12, 153);
            this.grpPTTPort.Name = "grpPTTPort";
            this.grpPTTPort.Size = new System.Drawing.Size(583, 47);
            this.grpPTTPort.TabIndex = 4;
            this.grpPTTPort.TabStop = false;
            this.grpPTTPort.Text = "PTT Port (Optional)";
            // 
            // chkPTTRTS
            // 
            this.chkPTTRTS.AutoSize = true;
            this.chkPTTRTS.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkPTTRTS.Location = new System.Drawing.Point(545, 20);
            this.chkPTTRTS.Name = "chkPTTRTS";
            this.chkPTTRTS.Size = new System.Drawing.Size(85, 17);
            this.chkPTTRTS.TabIndex = 3;
            this.chkPTTRTS.Text = "Enable DTR";
            this.chkPTTRTS.UseVisualStyleBackColor = true;
            // 
            // chkPTTDTR
            // 
            this.chkPTTDTR.AutoSize = true;
            this.chkPTTDTR.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkPTTDTR.Location = new System.Drawing.Point(467, 20);
            this.chkPTTDTR.Name = "chkPTTDTR";
            this.chkPTTDTR.Size = new System.Drawing.Size(84, 17);
            this.chkPTTDTR.TabIndex = 2;
            this.chkPTTDTR.Text = "Enable RTS";
            this.chkPTTDTR.UseVisualStyleBackColor = true;
            // 
            // cmbPTTBaud
            // 
            this.cmbPTTBaud.FormattingEnabled = true;
            this.cmbPTTBaud.Location = new System.Drawing.Point(327, 20);
            this.cmbPTTBaud.Name = "cmbPTTBaud";
            this.cmbPTTBaud.Size = new System.Drawing.Size(121, 21);
            this.cmbPTTBaud.TabIndex = 1;
            // 
            // cmbPTTPort
            // 
            this.cmbPTTPort.FormattingEnabled = true;
            this.cmbPTTPort.Location = new System.Drawing.Point(154, 20);
            this.cmbPTTPort.Name = "cmbPTTPort";
            this.cmbPTTPort.Size = new System.Drawing.Size(121, 21);
            this.cmbPTTPort.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(57, 218);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 5;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(23, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Serial Port to Use";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(26, 20);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Serial Port to Use";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(282, 27);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(32, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Baud";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(282, 28);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(32, 13);
            this.label6.TabIndex = 5;
            this.label6.Text = "Baud";
            // 
            // Radio
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(613, 261);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.grpPTTPort);
            this.Controls.Add(this.grpRadioSettings);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnUpdate);
            this.Controls.Add(this.grpRadioControlPort);
            this.Name = "Radio";
            this.Text = "Radio";
            this.grpRadioControlPort.ResumeLayout(false);
            this.grpRadioControlPort.PerformLayout();
            this.grpRadioSettings.ResumeLayout(false);
            this.grpRadioSettings.PerformLayout();
            this.grpPTTPort.ResumeLayout(false);
            this.grpPTTPort.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpRadioControlPort;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.GroupBox grpRadioSettings;
        private System.Windows.Forms.GroupBox grpPTTPort;
        private System.Windows.Forms.ComboBox cmbControlBaud;
        private System.Windows.Forms.ComboBox cmbControlPort;
        private System.Windows.Forms.ComboBox cmbModel;
        private System.Windows.Forms.ComboBox cmbAntenna;
        private System.Windows.Forms.ComboBox cmbPTTBaud;
        private System.Windows.Forms.ComboBox cmbPTTPort;
        private System.Windows.Forms.RadioButton rdoUSB;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox chkControlDTR;
        private System.Windows.Forms.CheckBox chkControlRTS;
        private System.Windows.Forms.CheckBox chkInternalTuner;
        private System.Windows.Forms.CheckBox chkPTTRTS;
        private System.Windows.Forms.CheckBox chkPTTDTR;
        private System.Windows.Forms.RadioButton rdoFM;
        private System.Windows.Forms.RadioButton rdoUSBDigital;
        private System.Windows.Forms.TextBox txtIcomAddress;
        private System.Windows.Forms.Label Label5;
        private System.Windows.Forms.Label Label2;
        private System.Windows.Forms.Label lblIcomAddress;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
    }
}