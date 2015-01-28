namespace ARES_Messenger
{
    partial class Main
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.rtbSession = new System.Windows.Forms.RichTextBox();
            this.rtbSend = new System.Windows.Forms.RichTextBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.ipdaemon1 = new nsoftware.IPWorks.Ipdaemon(this.components);
            this.ftp1 = new nsoftware.IPWorks.Ftp(this.components);
            this.http1 = new nsoftware.IPWorks.Http(this.components);
            this.SuspendLayout();
            // 
            // rtbSession
            // 
            this.rtbSession.Location = new System.Drawing.Point(27, 27);
            this.rtbSession.Name = "rtbSession";
            this.rtbSession.Size = new System.Drawing.Size(725, 202);
            this.rtbSession.TabIndex = 0;
            this.rtbSession.Text = "";
            // 
            // rtbSend
            // 
            this.rtbSend.Location = new System.Drawing.Point(27, 235);
            this.rtbSend.Name = "rtbSend";
            this.rtbSend.Size = new System.Drawing.Size(725, 97);
            this.rtbSend.TabIndex = 1;
            this.rtbSend.Text = "";
            // 
            // toolStrip1
            // 
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(784, 25);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // ipdaemon1
            // 
            this.ipdaemon1.About = "IP*Works! V9 [Build 5414]";
            // 
            // ftp1
            // 
            this.ftp1.About = "IP*Works! V9 [Build 5414]";
            // 
            // http1
            // 
            this.http1.About = "IP*Works! V9 [Build 5414]";
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 361);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.rtbSend);
            this.Controls.Add(this.rtbSession);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Main";
            this.Text = "ARES Messenger Chat";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox rtbSession;
        private System.Windows.Forms.RichTextBox rtbSend;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private nsoftware.IPWorks.Ipdaemon ipdaemon1;
        private nsoftware.IPWorks.Ftp ftp1;
        private nsoftware.IPWorks.Http http1;
    }
}

