namespace ARES_Messenger
{
    partial class ContactList
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
            this.ToolStrip1 = new System.Windows.Forms.ToolStrip();
            this.grdContacts = new System.Windows.Forms.DataGridView();
            this.Contact = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Callsign = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Frequency = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Bandwidth = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.QTH = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Comment = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.grdContacts)).BeginInit();
            this.SuspendLayout();
            // 
            // ToolStrip1
            // 
            this.ToolStrip1.Location = new System.Drawing.Point(0, 0);
            this.ToolStrip1.Name = "ToolStrip1";
            this.ToolStrip1.Size = new System.Drawing.Size(640, 25);
            this.ToolStrip1.TabIndex = 1;
            this.ToolStrip1.Text = "toolStrip1";
            // 
            // grdContacts
            // 
            this.grdContacts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdContacts.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Contact,
            this.Callsign,
            this.Frequency,
            this.Bandwidth,
            this.QTH,
            this.Comment});
            this.grdContacts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grdContacts.Location = new System.Drawing.Point(0, 25);
            this.grdContacts.Name = "grdContacts";
            this.grdContacts.Size = new System.Drawing.Size(640, 172);
            this.grdContacts.TabIndex = 2;
            // 
            // Contact
            // 
            this.Contact.HeaderText = "Contact";
            this.Contact.Name = "Contact";
            // 
            // Callsign
            // 
            this.Callsign.HeaderText = "Callsign";
            this.Callsign.Name = "Callsign";
            // 
            // Frequency
            // 
            this.Frequency.HeaderText = "Freq (kHz)";
            this.Frequency.Name = "Frequency";
            // 
            // Bandwidth
            // 
            this.Bandwidth.HeaderText = "Bandwidth (Hz)";
            this.Bandwidth.Name = "Bandwidth";
            // 
            // QTH
            // 
            this.QTH.HeaderText = "Grid Square/QTH";
            this.QTH.Name = "QTH";
            // 
            // Comment
            // 
            this.Comment.HeaderText = "Comment";
            this.Comment.Name = "Comment";
            // 
            // ContactList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(640, 197);
            this.Controls.Add(this.grdContacts);
            this.Controls.Add(this.ToolStrip1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ContactList";
            this.Text = "Mini Log";
            ((System.ComponentModel.ISupportInitialize)(this.grdContacts)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip ToolStrip1;
        private System.Windows.Forms.DataGridView grdContacts;
        private System.Windows.Forms.DataGridViewTextBoxColumn Contact;
        private System.Windows.Forms.DataGridViewTextBoxColumn Callsign;
        private System.Windows.Forms.DataGridViewTextBoxColumn Frequency;
        private System.Windows.Forms.DataGridViewTextBoxColumn Bandwidth;
        private System.Windows.Forms.DataGridViewTextBoxColumn QTH;
        private System.Windows.Forms.DataGridViewTextBoxColumn Comment;
    }
}