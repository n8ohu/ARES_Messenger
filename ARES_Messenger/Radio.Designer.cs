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
            this.btnUpdate = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.grpRadioSettings = new System.Windows.Forms.GroupBox();
            this.grpPTTPort = new System.Windows.Forms.GroupBox();
            this.cmbAntenna = new System.Windows.Forms.ComboBox();
            this.cmbModel = new System.Windows.Forms.ComboBox();
            this.cmbControlPort = new System.Windows.Forms.ComboBox();
            this.cmbControlBaud = new System.Windows.Forms.ComboBox();
            this.cmbPTTPort = new System.Windows.Forms.ComboBox();
            this.cmbPTTBaud = new System.Windows.Forms.ComboBox();
            this.grpRadioControlPort.SuspendLayout();
            this.grpRadioSettings.SuspendLayout();
            this.grpPTTPort.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpRadioControlPort
            // 
            this.grpRadioControlPort.Controls.Add(this.cmbControlBaud);
            this.grpRadioControlPort.Controls.Add(this.cmbControlPort);
            this.grpRadioControlPort.Location = new System.Drawing.Point(12, 96);
            this.grpRadioControlPort.Name = "grpRadioControlPort";
            this.grpRadioControlPort.Size = new System.Drawing.Size(583, 47);
            this.grpRadioControlPort.TabIndex = 0;
            this.grpRadioControlPort.TabStop = false;
            this.grpRadioControlPort.Text = "Radio Control Port";
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
            this.grpRadioSettings.Controls.Add(this.cmbModel);
            this.grpRadioSettings.Controls.Add(this.cmbAntenna);
            this.grpRadioSettings.Location = new System.Drawing.Point(12, 12);
            this.grpRadioSettings.Name = "grpRadioSettings";
            this.grpRadioSettings.Size = new System.Drawing.Size(583, 75);
            this.grpRadioSettings.TabIndex = 3;
            this.grpRadioSettings.TabStop = false;
            this.grpRadioSettings.Text = "Radio Selection";
            // 
            // grpPTTPort
            // 
            this.grpPTTPort.Controls.Add(this.cmbPTTBaud);
            this.grpPTTPort.Controls.Add(this.cmbPTTPort);
            this.grpPTTPort.Location = new System.Drawing.Point(12, 153);
            this.grpPTTPort.Name = "grpPTTPort";
            this.grpPTTPort.Size = new System.Drawing.Size(583, 47);
            this.grpPTTPort.TabIndex = 4;
            this.grpPTTPort.TabStop = false;
            this.grpPTTPort.Text = "PTT Port (Optional)";
            // 
            // cmbAntenna
            // 
            this.cmbAntenna.FormattingEnabled = true;
            this.cmbAntenna.Location = new System.Drawing.Point(285, 20);
            this.cmbAntenna.Name = "cmbAntenna";
            this.cmbAntenna.Size = new System.Drawing.Size(121, 21);
            this.cmbAntenna.TabIndex = 0;
            // 
            // cmbModel
            // 
            this.cmbModel.FormattingEnabled = true;
            this.cmbModel.Location = new System.Drawing.Point(141, 19);
            this.cmbModel.Name = "cmbModel";
            this.cmbModel.Size = new System.Drawing.Size(121, 21);
            this.cmbModel.TabIndex = 1;
            // 
            // cmbControlPort
            // 
            this.cmbControlPort.FormattingEnabled = true;
            this.cmbControlPort.Location = new System.Drawing.Point(154, 20);
            this.cmbControlPort.Name = "cmbControlPort";
            this.cmbControlPort.Size = new System.Drawing.Size(121, 21);
            this.cmbControlPort.TabIndex = 0;
            // 
            // cmbControlBaud
            // 
            this.cmbControlBaud.FormattingEnabled = true;
            this.cmbControlBaud.Location = new System.Drawing.Point(327, 20);
            this.cmbControlBaud.Name = "cmbControlBaud";
            this.cmbControlBaud.Size = new System.Drawing.Size(121, 21);
            this.cmbControlBaud.TabIndex = 1;
            // 
            // cmbPTTPort
            // 
            this.cmbPTTPort.FormattingEnabled = true;
            this.cmbPTTPort.Location = new System.Drawing.Point(154, 20);
            this.cmbPTTPort.Name = "cmbPTTPort";
            this.cmbPTTPort.Size = new System.Drawing.Size(121, 21);
            this.cmbPTTPort.TabIndex = 0;
            // 
            // cmbPTTBaud
            // 
            this.cmbPTTBaud.FormattingEnabled = true;
            this.cmbPTTBaud.Location = new System.Drawing.Point(327, 20);
            this.cmbPTTBaud.Name = "cmbPTTBaud";
            this.cmbPTTBaud.Size = new System.Drawing.Size(121, 21);
            this.cmbPTTBaud.TabIndex = 1;
            // 
            // Radio
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(613, 261);
            this.Controls.Add(this.grpPTTPort);
            this.Controls.Add(this.grpRadioSettings);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnUpdate);
            this.Controls.Add(this.grpRadioControlPort);
            this.Name = "Radio";
            this.Text = "Radio";
            this.grpRadioControlPort.ResumeLayout(false);
            this.grpRadioSettings.ResumeLayout(false);
            this.grpPTTPort.ResumeLayout(false);
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
    }
}