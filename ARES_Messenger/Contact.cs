using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ARES_Messenger
{
    public partial class Contact : Form
    {
       
        private void OK_Button_Click(System.Object sender, System.EventArgs e)
	    {
		// TODO: Check for proper call sign syntax and frequency including foreign settings
		if (!string.IsNullOrEmpty(txtFreq.Text.Trim())) {
			if (!Information.IsNumeric(txtFreq.Text)) {
				Interaction.MsgBox("Frequency must be in KHz (1800 - 148000).",  MsgBoxStyle.ApplicationModal , "Improper Frequency Value");
				return;
			} else if (Convert.ToInt32(txtFreq.Text) > 148000 | Convert.ToInt32(txtFreq.Text) < 1800) {
				Interaction.MsgBox("Frequency must be in KHz (1800 - 148000).", MsgBoxStyle.ApplicationModal , "Improper Frequency Value");
				return;
			}
		}

		System.DateTime dttTestDate = default(System.DateTime);
		try {
			dttTestDate = Convert.ToDateTime(txtDate.Text);
		} catch (Exception ex) {
            Interaction.MsgBox("Improper UTC Date format: yyyy/mm/dd/HH:mm ", MsgBoxStyle.ApplicationModal, "Improper Date Format");
			return;
		}

		this.DialogResult = System.Windows.Forms.DialogResult.OK;
		Globals.strContact = txtDate.Text + "|" + txtCall.Text.Trim().ToUpper() + "|" + txtFreq.Text.Trim() + "|" + txtName.Text.Trim() + "|" + txtGSQTH.Text.Trim() + "|";
		if (OK_Button.Text.IndexOf("Edit") != -1)
			Globals.strContact = "E" + Globals.strContact;
		this.Close();
	}

        private void Cancel_Button_Click(System.Object sender, System.EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void Contact_Load(object sender, System.EventArgs e)
        {
            string[] strInfo = Globals.strContact.Split('|');
            txtDate.Text = strInfo[0];
            txtCall.Text = strInfo[1];
            txtFreq.Text = strInfo[2];
            txtName.Text = strInfo[3];
            txtGSQTH.Text = strInfo[4];
            /*if (ContactList.grdContacts.SelectedRows(0).Index < ContactList.grdContacts.RowCount - 1)
            {
                btnDelete.Enabled = true;
                OK_Button.Text = "Edit Contact";
            }
            else
            {
                btnDelete.Enabled = false;
                OK_Button.Text = "Add Contact";
            }*/

        }

        private void btnDelete_Click(System.Object sender, System.EventArgs e)
        {
            if (Interaction.MsgBox("Delete this contact?", MsgBoxStyle.YesNo, "Confirm Contact Deletion") == MsgBoxResult.No)
                return;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            Globals.strContact = "X" + txtDate.Text + "|" + txtCall.Text.Trim().ToUpper() + "|" + txtFreq.Text.Trim() + "|" + txtName.Text.Trim() + "|" + txtGSQTH.Text.Trim() + "|";
            this.Close();
        }

        private void btnNow_Click(System.Object sender, System.EventArgs e)
        {
            txtDate.Text = Globals.FormatDate(System.DateTime.UtcNow);
        }

        
        public Contact()
        {
            InitializeComponent();
        }
    }
}
