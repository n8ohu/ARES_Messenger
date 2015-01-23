using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;

namespace ARES_Messenger
{
    public class About : System.Windows.Forms.Form
    {

        #region " Windows Form Designer generated code "

        public About()
            : base()
        {
            Load += About_Load;

            //This call is required by the Windows Form Designer.
            InitializeComponent();

            //Add any initialization after the InitializeComponent() call

        }

        //Form overrides dispose to clean up the component list.
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if ((components != null))
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        //Required by the Windows Form Designer

        private System.ComponentModel.IContainer components;
        //NOTE: The following procedure is required by the Windows Form Designer
        //It can be modified using the Windows Form Designer.  
        //Do not modify it using the code editor.
        public System.Windows.Forms.Label lblDescription;
        public System.Windows.Forms.Label lblCopyright;
        internal System.Windows.Forms.Label Label1;
        internal System.Windows.Forms.PictureBox PictureBox1;
        private System.Windows.Forms.LinkLabel withEventsField_lnkARSFI;
        internal System.Windows.Forms.LinkLabel lnkARSFI
        {
            get { return withEventsField_lnkARSFI; }
            set
            {
                if (withEventsField_lnkARSFI != null)
                {
                    withEventsField_lnkARSFI.LinkClicked -= lnkARSFI_LinkClicked;
                }
                withEventsField_lnkARSFI = value;
                if (withEventsField_lnkARSFI != null)
                {
                    withEventsField_lnkARSFI.LinkClicked += lnkARSFI_LinkClicked;
                }
            }
        }
        public System.Windows.Forms.Label lblVersion;
        [System.Diagnostics.DebuggerStepThrough()]
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(About));
            this.lblDescription = new System.Windows.Forms.Label();
            this.lblCopyright = new System.Windows.Forms.Label();
            this.lblVersion = new System.Windows.Forms.Label();
            this.Label1 = new System.Windows.Forms.Label();
            this.PictureBox1 = new System.Windows.Forms.PictureBox();
            this.lnkARSFI = new System.Windows.Forms.LinkLabel();
            ((System.ComponentModel.ISupportInitialize)this.PictureBox1).BeginInit();
            this.SuspendLayout();
            //
            //lblDescription
            //
            this.lblDescription.Location = new System.Drawing.Point(4, 33);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(378, 39);
            this.lblDescription.TabIndex = 8;
            this.lblDescription.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            //lblCopyright
            //
            this.lblCopyright.Location = new System.Drawing.Point(72, 92);
            this.lblCopyright.Name = "lblCopyright";
            this.lblCopyright.Size = new System.Drawing.Size(232, 44);
            this.lblCopyright.TabIndex = 6;
            this.lblCopyright.Text = " Copyright 2013 Rick Muething, KN6KB   ";
            this.lblCopyright.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            //lblVersion
            //
            this.lblVersion.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, Convert.ToByte(0));
            this.lblVersion.Location = new System.Drawing.Point(105, 9);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(176, 24);
            this.lblVersion.TabIndex = 5;
            this.lblVersion.Text = "Version: 0.0.0.0";
            this.lblVersion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            //Label1
            //
            this.Label1.Location = new System.Drawing.Point(12, 136);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(354, 47);
            this.Label1.TabIndex = 9;
            this.Label1.Text = "ARDOP Chat is made possible through the Amateur Radio Safety Foundation Inc.  You" + "r membership in and support for the ARSF make programs like ARDOP Chat possible." + " ";
            //
            //PictureBox1
            //
            this.PictureBox1.ErrorImage = (System.Drawing.Image)resources.GetObject("PictureBox1.ErrorImage");
            this.PictureBox1.Image = (System.Drawing.Image)resources.GetObject("PictureBox1.Image");
            this.PictureBox1.Location = new System.Drawing.Point(35, 214);
            this.PictureBox1.Name = "PictureBox1";
            this.PictureBox1.Size = new System.Drawing.Size(320, 293);
            this.PictureBox1.TabIndex = 10;
            this.PictureBox1.TabStop = false;
            //
            //lnkARSFI
            //
            this.lnkARSFI.AutoSize = true;
            this.lnkARSFI.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lnkARSFI.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, Convert.ToByte(0));
            this.lnkARSFI.Location = new System.Drawing.Point(133, 183);
            this.lnkARSFI.Name = "lnkARSFI";
            this.lnkARSFI.Size = new System.Drawing.Size(120, 18);
            this.lnkARSFI.TabIndex = 12;
            this.lnkARSFI.TabStop = true;
            this.lnkARSFI.Text = "http://www.arsfi.org";
            //
            //About
            //
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(380, 507);
            this.Controls.Add(this.lnkARSFI);
            this.Controls.Add(this.PictureBox1);
            this.Controls.Add(this.Label1);
            this.Controls.Add(this.lblDescription);
            this.Controls.Add(this.lblCopyright);
            this.Controls.Add(this.lblVersion);
            this.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "About";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = " About ARDOP Chat";
            ((System.ComponentModel.ISupportInitialize)this.PictureBox1).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        // Shows about box for this application...
        private void About_Load(System.Object sender, System.EventArgs e)
        {
            Text = "About " + Application.ProductName;
            lblVersion.Text = "Version: " + Application.ProductVersion;
            lblDescription.Text = "ARDOP Chat is a simple keyboard client program for " + "use with the ARDIO Win virtual TNC.";
            Label1.Text = "ARDOP Chat is made possible through the Amateur Radio Safety Foundation Inc. " + "Your membership in and support for the ARSF make programs like ARDOP Chat possible.";
            Label1.Enabled = true;
        }
        // About_Load

        // Close the about box...
        private void btnClose_Click(System.Object sender, System.EventArgs e)
        {
            Close();
        }
        // btnClose_Click

        private void lnkARSFI_LinkClicked(System.Object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(lnkARSFI.Text);
        }
        // lnkARSFI_LinkClicked

    }

}
