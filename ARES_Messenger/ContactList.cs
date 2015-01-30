using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace ARES_Messenger
{   
    public partial class ContactList : Form
    {

        private bool blnInitializingGrid = false;

        private void GetQSONameQTHFromMiniLog(ref string strName, ref string strQTH, string strCallSign)
        {
            DataGridViewRow objRow = new DataGridViewRow();
            int intRows = grdContacts.RowCount;

            for (int i = 0; i <= intRows - 1; i++)
            {
                objRow = grdContacts.Rows[i];
                if ((objRow.Cells[1].Value != null))
                {
                    if (strCallSign.Trim().ToUpper() == objRow.Cells[1].Value.ToString().Trim().ToUpper())
                    {
                        strName = objRow.Cells[3].Value.ToString().Trim();
                        strQTH = objRow.Cells[4].Value.ToString().Trim();
                        return;
                    }
                }
            }
            strName = "";
            strQTH = "";
        }
        
        private void InitializeContactGrid()
        {
            DataGridViewRow objRow = default(DataGridViewRow);
            blnInitializingGrid = true;
            grdContacts.Rows.Clear();
            if (Microsoft.VisualBasic.FileIO.FileSystem.FileExists(Globals.strDataDirectory + "H4 Contacts.dat"))
            {
                try
                {
                    string strContactList = Microsoft.VisualBasic.FileIO.FileSystem.ReadAllText(Globals.strDataDirectory + "H4 Contacts.dat");
                    StringReader objStringReader = new StringReader(strContactList);
                    do
                    {
                        string strLine = objStringReader.ReadLine();
                        if (strLine == null)
                            break; // TODO: might not be correct. Was : Exit Do
                        string[] strTokens = strLine.Split('|');
                        //strTokens(0) - Contact Date/time (UTC)
                        //strTokens(1) - Call sign
                        //strTokens(2) - Frequency (kHz)
                        objRow = new DataGridViewRow();
                        objRow.CreateCells(grdContacts);
                        for (int i = 0; i <= objRow.Cells.Count - 1; i++)
                        {
                            objRow.Cells[i].Value = strTokens[i];
                        }
                        grdContacts.Rows.Add(objRow);
                        objRow = null;
                    } while (true);
                }
                catch (Exception ex)
                {
                    //Globals.Exceptions("[Main.InitializeContactGrid] Err: " + Err.Number + " Exception: " + ex.ToString);
                }

            }
            //grdContacts.Sort(Time, System.ComponentModel.ListSortDirection.Descending);
            blnInitializingGrid = false;
        }

        private void UpdateContacts()
        {
            DataGridViewRow objRow = new DataGridViewRow();
            string strContacts = "";
            try
            {
                //grdContacts.Sort(Time, System.ComponentModel.ListSortDirection.Descending);
                for (int i = 0; i <= grdContacts.RowCount - 2; i++)
                {
                    objRow = grdContacts.Rows[i];

                    for (int j = 0; j <= objRow.Cells.Count - 1; j++)
                    {
                        if ((objRow.Cells[j].Value != null))
                        {
                            strContacts += objRow.Cells[j].Value.ToString() + "|";
                        }
                        else
                        {
                            strContacts += "|";
                        }
                    }
                    strContacts += Constants.vbCrLf;
                }
                Microsoft.VisualBasic.FileIO.FileSystem.WriteAllText(Globals.strDataDirectory + "H4 Contacts.dat", strContacts, false);
            }
            catch (Exception ex)
            {
                //Globals.Exception("[UpdateContacts] Err:" + Err.Number.ToString() + " Exception: " + ex.ToString() + Constants.vbCr + "strContacts:" + strContacts);
            }

        }

        
        public ContactList()
        {
            InitializeComponent();
        }
    }
}
