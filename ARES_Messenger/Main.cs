using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Windows.Forms;

namespace ARES_Messenger
{
    public partial class Main : Form
    {

        // Integers
        private int intNewLineIndex;
        private int intARDOP_WinProcessID = -1;
        private int intHorizSplitDst;
        private int intVertSplitDst;
        private int intOBQueueValue;
        private int intThroughput;
        private int intDisplayedThroughput;
        private int intRtbSendPtr = 0;
        private int intFrequency = 0;
        private int intContactsSelectedRow;
        private int intQSOFreqHz;

        private Int32 intTNCIBData_CmdPtr = 0;
        // Doubles

        private double dblLogQueueBase2;
        // Boolean--

        private bool blnRTBSendHasFocus;
        private bool blnRDY;
        private bool blnData;
        private bool blnTNCIsOpen = false;
        private bool blnRtbSessionKeyPressed = false;
        private bool blnSendKeyPressed = false;
        private bool blnRestartTNC = false;
        private bool blnClearingTextBoxes = false;
        private bool blnCtrlKey = false;
        private bool blnInitializingGrid = false;
        private bool blnBtnEndVisible = false;
        private bool blnRequestCWID = false;
        private bool blnIgnoreManualSessionScroll = false;
        private bool blnScrollLocked = false;
        private bool blnUpdateTNCdisplay = false;
        private bool blnStartingARDOPTNC = false;
        private bool blnRadioModeInitialized = false;
        private bool blnRestartNotification = false;
        private bool blnPTT = false;
        private bool blnAnsweringCQ = false;

        private bool blnMenuModeEnabled = false;
        //Strings
        private string strLastSendKey = "";
        private string strSentText = "";
        private string strThruPutDirection = "Rx: ";
        private string strConnectedCallSign = "";
        private string strQSOCallSign;
        private string strADIFComment;
        private string strQSOName;
        private string strQSOQTH;
        private string strAVGSN;
        private string strRemoteGS = "";
        private string strSearch4GS = "";
        private UTF8Encoding objUTF8 = new UTF8Encoding();
        private string strAnswerCQNewText = "";
        // used for temporary mode saving during FEC Robust Beacon
        private string strSaveMode = "";

        //arrays

        private byte[] bytTNCIBData_CmdBuffer = new byte[-1 + 1];
        private struct SessionUpdates
        {
            public string Text;
            public Font Font;
            public Color Color;
            public bool ScrollToCaret;
        }

        private struct ThroughputData
        {
            // the date/time the item was inserted
            public System.DateTime Arrival;
            // The actual character count received or sent (adjusted for ASCII 7 bit if needed) 
            public int CharCnt;
        }

        private Collection cllInbound = new Collection();

        private Collection cllOutbound = new Collection();

        // object to buffer Session rtb inputs
        SessionUpdates objSessionUpdate;
        // Collection of Session updates to allow holding off auto scrolling
        private Collection cllPendingUpdates = new Collection();

        private System.DateTime dttScrollLocked;
        private System.DateTime dttQSOStart;
        private System.DateTime dttQSOEnd;
        private System.DateTime dttLastFECSend;
        private System.DateTime dttLastRxSpeedUpdate;

        private System.DateTime dttLastCQReceived;


        /*private void UpdateContacts()
        {
            DataGridViewRow objRow = new DataGridViewRow();
            string strContacts = "";
            try
            {
                grdContacts.Sort(Time, System.ComponentModel.ListSortDirection.Descending);
                for (int i = 0; i <= grdContacts.RowCount - 2; i++)
                {
                    objRow = grdContacts.Rows(i);

                    for (int j = 0; j <= objRow.Cells.Count - 1; j++)
                    {
                        if ((objRow.Cells(j).Value != null))
                        {
                            strContacts += objRow.Cells(j).Value.ToString + "|";
                        }
                        else
                        {
                            strContacts += "|";
                        }
                    }
                    strContacts += Constants.vbCrLf;
                }
                My.Computer.FileSystem.WriteAllText(strDataDirectory + "H4 Contacts.dat", strContacts, false);
            }
            catch (Exception ex)
            {
                Exception("[UpdateContacts] Err:" + Err.Number.ToString + " Exception: " + ex.ToString + Constants.vbCr + "strContacts:" + strContacts);
            }

        }*/

        private void InitializeContactGrid()
        {
            DataGridViewRow objRow = default(DataGridViewRow);
            blnInitializingGrid = true;
            grdContacts.Rows.Clear();
            if (IO.File.Exists(strDataDirectory + "H4 Contacts.dat"))
            {
                try
                {
                    string strContactList = My.Computer.FileSystem.ReadAllText(strDataDirectory + "H4 Contacts.dat");
                    StringReader objStringReader = new StringReader(strContactList);
                    do
                    {
                        string strLine = objStringReader.ReadLine;
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
                            objRow.Cells(i).Value = strTokens(i);
                        }
                        grdContacts.Rows.Add(objRow);
                        objRow = null;
                    } while (true);
                }
                catch (Exception ex)
                {
                    Exceptions("[Main.InitializeContactGrid] Err: " + Err.Number + " Exception: " + ex.ToString);
                }

            }
            grdContacts.Sort(Time, System.ComponentModel.ListSortDirection.Descending);
            blnInitializingGrid = false;
        }




        public Main()
        {
            InitializeComponent();
        }
    }
}