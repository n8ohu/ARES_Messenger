using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ARES_Messenger
{
    public partial class DialogAutoupdate : Form
    {
        private System.Timers.Timer tmrTimeout;

        private int intTimeout = 30;
        private void DialogAutoupdate_Load(System.Object sender, System.EventArgs e)
        {
            lblCV.Text = Application.ProductVersion;
            lblNV.Text = Globals.strNewAUVersion;
            CheckForIllegalCrossThreadCalls = false;
            tmrTimeout = new System.Timers.Timer(1000);

            int tmpTop = Globals.objMain.Top + (Globals.objMain.Height / 2);
            int tmpLeft = Globals.objMain.Left + (Globals.objMain.Width / 2);
            this.Top = tmpTop - (this.Height / 2);
            this.Left = tmpLeft - (this.Width / 2);

            tmrTimeout.Elapsed += UpdateTimeout;
            tmrTimeout.AutoReset = true;
            lblTR.Text = "30";
            btnUpdate.Focus();
            this.Activate();
            this.TopLevel = true;
            //
            // Don't timeout the UI for now
            //
            //tmrTimeout.Start();
        }

        private void UpdateTimeout(object s, System.Timers.ElapsedEventArgs e)
        {
            intTimeout = intTimeout - 1;
            lblTR.Text = intTimeout.ToString();
            if (intTimeout <= 0)
            {
                Close();
            }
        }

        private void btnAbort_Click(System.Object sender, System.EventArgs e)
        {
            //blnAbortAU = True
            Close();
        }

        private void BtnUpdate_Click(System.Object sender, System.EventArgs e)
        {
            Globals.blnAbortAU = false;
            Close();
        }

        private void DialogAutoupdate_FormClosing(System.Object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            tmrTimeout.Stop();
        }
        public DialogAutoupdate()
        {
            FormClosing += DialogAutoupdate_FormClosing;
            Load += DialogAutoupdate_Load;
        }

    }
}
