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
            this.rdoUSBDigital = new System.Windows.Forms.RadioButton();
            this.rdoFM = new System.Windows.Forms.RadioButton();
            this.txtIcomAddress = new System.Windows.Forms.TextBox();
            this.grpRadioControlPort.SuspendLayout();
            this.grpRadioSettings.SuspendLayout();
            this.grpPTTPort.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpRadioControlPort
            // 
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
            this.cmbModel.Location = new System.Drawing.Point(141, 19);
            this.cmbModel.Name = "cmbModel";
            this.cmbModel.Size = new System.Drawing.Size(121, 21);
            this.cmbModel.TabIndex = 1;
            // 
            // cmbAntenna
            // 
            this.cmbAntenna.FormattingEnabled = true;
            this.cmbAntenna.Location = new System.Drawing.Point(285, 20);
            this.cmbAntenna.Name = "cmbAntenna";
            this.cmbAntenna.Size = new System.Drawing.Size(121, 21);
            this.cmbAntenna.TabIndex = 0;
            // 
            // grpPTTPort
            // 
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
            // rdoUSBDigital
            // 
            this.rdoUSBDigital.AutoSize = true;
            this.rdoUSBDigital.Location = new System.Drawing.Point(236, 52);
            this.rdoUSBDigital.Name = "rdoUSBDigital";
            this.rdoUSBDigital.Size = new System.Drawing.Size(85, 17);
            this.rdoUSBDigital.TabIndex = 4;
            this.rdoUSBDigital.TabStop = true;
            this.rdoUSBDigital.Text = "radioButton1";
            this.rdoUSBDigital.UseVisualStyleBackColor = true;
            // 
            // rdoFM
            // 
            this.rdoFM.AutoSize = true;
            this.rdoFM.Location = new System.Drawing.Point(327, 52);
            this.rdoFM.Name = "rdoFM";
            this.rdoFM.Size = new System.Drawing.Size(85, 17);
            this.rdoFM.TabIndex = 5;
            this.rdoFM.TabStop = true;
            this.rdoFM.Text = "radioButton1";
            this.rdoFM.UseVisualStyleBackColor = true;
            // 
            // txtIcomAddress
            // 
            this.txtIcomAddress.Location = new System.Drawing.Point(84, 52);
            this.txtIcomAddress.Name = "txtIcomAddress";
            this.txtIcomAddress.Size = new System.Drawing.Size(100, 20);
            this.txtIcomAddress.TabIndex = 6;
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
    }
}