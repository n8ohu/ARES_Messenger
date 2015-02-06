namespace ARES_Messenger
{
    partial class DialogAutoupdate
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
            this.lblCV = new System.Windows.Forms.Label();
            this.lblNV = new System.Windows.Forms.Label();
            this.lblTR = new System.Windows.Forms.Label();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.btnAbort = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblCV
            // 
            this.lblCV.AutoSize = true;
            this.lblCV.Location = new System.Drawing.Point(186, 120);
            this.lblCV.Name = "lblCV";
            this.lblCV.Size = new System.Drawing.Size(16, 13);
            this.lblCV.TabIndex = 0;
            this.lblCV.Text = "---";
            // 
            // lblNV
            // 
            this.lblNV.AutoSize = true;
            this.lblNV.Location = new System.Drawing.Point(186, 140);
            this.lblNV.Name = "lblNV";
            this.lblNV.Size = new System.Drawing.Size(16, 13);
            this.lblNV.TabIndex = 1;
            this.lblNV.Text = "---";
            // 
            // lblTR
            // 
            this.lblTR.AutoSize = true;
            this.lblTR.Location = new System.Drawing.Point(186, 159);
            this.lblTR.Name = "lblTR";
            this.lblTR.Size = new System.Drawing.Size(16, 13);
            this.lblTR.TabIndex = 2;
            this.lblTR.Text = "---";
            // 
            // btnUpdate
            // 
            this.btnUpdate.Location = new System.Drawing.Point(226, 191);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(106, 29);
            this.btnUpdate.TabIndex = 3;
            this.btnUpdate.Text = "Update Now";
            this.btnUpdate.UseVisualStyleBackColor = true;
            // 
            // btnAbort
            // 
            this.btnAbort.Location = new System.Drawing.Point(12, 192);
            this.btnAbort.Name = "btnAbort";
            this.btnAbort.Size = new System.Drawing.Size(113, 28);
            this.btnAbort.TabIndex = 4;
            this.btnAbort.Text = "Remind Me Later";
            this.btnAbort.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(254, 52);
            this.label1.TabIndex = 5;
            this.label1.Text = "A new version of this program has been released. \r\nClick \'Update Now\' to install " +
    "the update and restart.\r\nClick \'Remind Me Later\' if you do not wish to update \r\n" +
    "at this time.\r\n";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(95, 120);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(85, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Current Version: ";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(95, 138);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(70, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "New Version:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(98, 159);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(86, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Time Remaining:";
            // 
            // DialogAutoupdate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(346, 232);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnAbort);
            this.Controls.Add(this.btnUpdate);
            this.Controls.Add(this.lblTR);
            this.Controls.Add(this.lblNV);
            this.Controls.Add(this.lblCV);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DialogAutoupdate";
            this.Text = "Autoupdate";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblCV;
        private System.Windows.Forms.Label lblNV;
        private System.Windows.Forms.Label lblTR;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Button btnAbort;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
    }
}