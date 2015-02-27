using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Windows.Forms;
using nsoftware.IPWorks;

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





        // subroutine to update the Session rich text box from the queSessionEvents
        // capture rtbSend Focus status upon entry
        //InitializeComponent()
        bool static_UpdateSessionDisplay_blnLFAppended;
        readonly Microsoft.VisualBasic.CompilerServices.StaticLocalInitFlag static_UpdateSessionDisplay_strRcvTxtFECLogBuffer_Init = new Microsoft.VisualBasic.CompilerServices.StaticLocalInitFlag();
        string static_UpdateSessionDisplay_strRcvTxtFECLogBuffer;
        private void UpdateSessionDisplay()
        {
            bool blnRTBSendHadFocus = blnRTBSendHasFocus;
            string strText = null;
            bool blnSRTBSendFocus = blnRTBSendHasFocus;
            bool blnLog = false;
            bool blnScrollToCaret = false;
            Font objFont = new System.Drawing.Font("Microsoft Sans Serif", Globals.intFontSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, Convert.ToByte(0));
            Color objColor = default(Color);
            lock (static_UpdateSessionDisplay_strRcvTxtFECLogBuffer_Init)
            {
                try
                {
                    if (InitStaticVariableHelper(static_UpdateSessionDisplay_strRcvTxtFECLogBuffer_Init))
                    {
                        static_UpdateSessionDisplay_strRcvTxtFECLogBuffer = "";
                    }
                }
                finally
                {
                    static_UpdateSessionDisplay_strRcvTxtFECLogBuffer_Init.State = 1;
                }
            }
            // To buffer individual characters for logging
            string strPrefix = "";
            // used to insure status on a clean line

            while (Globals.queSessionEvents.Count > 0)
            {
                strText = Globals.queSessionEvents.Dequeue().ToString();
                blnLog = true;
                switch (strText[0])
                {

                    case 'A':
                        objColor = Color.Navy;
                        objFont = new System.Drawing.Font("Microsoft Sans Serif", Globals.intFontSize, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, Convert.ToByte(0));
                        if (!string.IsNullOrEmpty(static_UpdateSessionDisplay_strRcvTxtFECLogBuffer))
                        {
                            Globals.Log(static_UpdateSessionDisplay_strRcvTxtFECLogBuffer);
                            static_UpdateSessionDisplay_strRcvTxtFECLogBuffer = "";
                        }
                        Globals.Log("{Info} " + strText.Substring(2));
                        if (static_UpdateSessionDisplay_blnLFAppended)
                            strText = "  " + Constants.vbLf + strText.Substring(2);

                        break;
                    case 'B':
                        objColor = Color.Black;
                        objFont = new System.Drawing.Font("Microsoft Sans Serif", Globals.intFontSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, Convert.ToByte(0));
                        if (string.IsNullOrEmpty(static_UpdateSessionDisplay_strRcvTxtFECLogBuffer))
                        {
                            static_UpdateSessionDisplay_strRcvTxtFECLogBuffer = "{Rcv Txt FEC} " + strText.Substring(2);
                        }
                        else
                        {
                            static_UpdateSessionDisplay_strRcvTxtFECLogBuffer += strText.Substring(2);
                        }
                        // Wordwrap over 80 characters
                        if (static_UpdateSessionDisplay_strRcvTxtFECLogBuffer.Length > 80 & static_UpdateSessionDisplay_strRcvTxtFECLogBuffer.EndsWith(" "))
                        {
                            Globals.Log(static_UpdateSessionDisplay_strRcvTxtFECLogBuffer);
                            static_UpdateSessionDisplay_strRcvTxtFECLogBuffer = "";
                        }

                        break;
                    case 'C':
                        if (!string.IsNullOrEmpty(static_UpdateSessionDisplay_strRcvTxtFECLogBuffer))
                        {
                            Globals.Log(static_UpdateSessionDisplay_strRcvTxtFECLogBuffer);
                            static_UpdateSessionDisplay_strRcvTxtFECLogBuffer = "";
                        }
                        rtbSession.Clear();

                        continue;
                    case 'G':
                        objColor = Color.DarkGreen;
                        objFont = new System.Drawing.Font("Microsoft Sans Serif", Globals.intFontSize, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, Convert.ToByte(0));
                        if (!string.IsNullOrEmpty(static_UpdateSessionDisplay_strRcvTxtFECLogBuffer))
                        {
                            Globals.Log(static_UpdateSessionDisplay_strRcvTxtFECLogBuffer);
                            static_UpdateSessionDisplay_strRcvTxtFECLogBuffer = "";
                        }
                        Globals.Log("{Sent Txt} " + strText.Substring(2));

                        break;
                    case 'P':
                        objColor = Color.Purple;
                        objFont = new System.Drawing.Font("Microsoft Sans Serif", Globals.intFontSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, Convert.ToByte(0));
                        if (!string.IsNullOrEmpty(static_UpdateSessionDisplay_strRcvTxtFECLogBuffer))
                        {
                            Globals.Log(static_UpdateSessionDisplay_strRcvTxtFECLogBuffer);
                            static_UpdateSessionDisplay_strRcvTxtFECLogBuffer = "";
                        }
                        Globals.Log("{Status} " + strText.Substring(3));
                        if (Globals.strMode == "ARQ" & (strText.IndexOf("DISCONNECTED") != -1))
                        {
                            mnuCall.Enabled = true;
                            Globals.blnARQCalling = false;
                        }

                        // This insures Status messages in purple are always on a clean line
                        if ((rtbSession.Text.EndsWith(Constants.vbCrLf) | rtbSession.Text.EndsWith(Constants.vbCr) | rtbSession.Text.EndsWith(Constants.vbLf)))
                        {
                            strPrefix = "";
                        }
                        else
                        {
                            strPrefix = Constants.vbLf;
                        }
                        strText = "P " + strPrefix + strText.Substring(2);

                        break;
                    case 'R':
                        objColor = Color.DarkRed;
                        objFont = new System.Drawing.Font("Microsoft Sans Serif", Globals.intFontSize, System.Drawing.FontStyle.Strikeout, System.Drawing.GraphicsUnit.Point, Convert.ToByte(0));
                        if (!string.IsNullOrEmpty(static_UpdateSessionDisplay_strRcvTxtFECLogBuffer))
                        {
                            Globals.Log(static_UpdateSessionDisplay_strRcvTxtFECLogBuffer);
                            static_UpdateSessionDisplay_strRcvTxtFECLogBuffer = "";
                        }
                        Globals.Log("{Rcv Txt Err} " + strText.Substring(2));

                        break;
                    case 'F':
                        objColor = Color.Red;
                        objFont = new System.Drawing.Font("Microsoft Sans Serif", Globals.intFontSize, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, Convert.ToByte(0));
                        if (!string.IsNullOrEmpty(static_UpdateSessionDisplay_strRcvTxtFECLogBuffer))
                        {
                            Globals.Log(static_UpdateSessionDisplay_strRcvTxtFECLogBuffer);
                            static_UpdateSessionDisplay_strRcvTxtFECLogBuffer = "";
                        }
                        Globals.Log("{Alert} " + strText.Substring(4));
                        if (Globals.strMode == "ARQ")
                        {
                            mnuCall.Enabled = true;
                            Globals.blnARQCalling = false;
                        }
                        // This insures Alert messages in Red Bold are always on a clean line
                        if ((rtbSession.Text.EndsWith(Constants.vbCrLf) | rtbSession.Text.EndsWith(Constants.vbCr) | rtbSession.Text.EndsWith(Constants.vbLf)))
                        {
                            strPrefix = "";
                        }
                        else
                        {
                            strPrefix = Constants.vbLf;
                        }
                        strText = "F " + strPrefix + strText.Substring(2);

                        break;
                    case 'Q':
                        objColor = Color.Black;
                        objFont = new System.Drawing.Font("Microsoft Sans Serif", Globals.intFontSize, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, Convert.ToByte(0));
                        if (!string.IsNullOrEmpty(static_UpdateSessionDisplay_strRcvTxtFECLogBuffer))
                        {
                            Globals.Log(static_UpdateSessionDisplay_strRcvTxtFECLogBuffer);
                            static_UpdateSessionDisplay_strRcvTxtFECLogBuffer = "";
                        }
                        Globals.Log("{Rcv Txt ARQ} " + strText.Substring(2));
                        break;
                    // If blnLFAppended Then strText = "  " & vbLf & strText.Substring(2)

                    case 'T':
                        // Robust FEC Text 
                        objColor = Color.Navy;
                        objFont = new System.Drawing.Font("Microsoft Sans Serif", Globals.intFontSize, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, Convert.ToByte(0));
                        if (string.IsNullOrEmpty(static_UpdateSessionDisplay_strRcvTxtFECLogBuffer))
                        {
                            static_UpdateSessionDisplay_strRcvTxtFECLogBuffer = "{Rcv Txt FEC} " + strText.Substring(2);
                        }
                        else
                        {
                            static_UpdateSessionDisplay_strRcvTxtFECLogBuffer += strText.Substring(2);
                        }
                        // Wordwrap over 80 characters
                        if (static_UpdateSessionDisplay_strRcvTxtFECLogBuffer.Length > 80 & static_UpdateSessionDisplay_strRcvTxtFECLogBuffer.EndsWith(" "))
                        {
                            Globals.Log(static_UpdateSessionDisplay_strRcvTxtFECLogBuffer);
                            static_UpdateSessionDisplay_strRcvTxtFECLogBuffer = "";
                        }
                        break;
                }
                try
                {
                    objSessionUpdate.Color = objColor;
                    objSessionUpdate.Font = objFont;

                    //objSessionUpdate.ScrollToCaret = False
                    objSessionUpdate.ScrollToCaret = objSessionUpdate.Color != Color.Black;
                    // Changed from False 0.1.6.1 to improve readability (keep bottom line visible) 
                    string strToAppend = strText.Substring(2).Replace(Constants.vbCrLf, Constants.vbLf);
                    strToAppend = strToAppend.Replace(Constants.vbCr, Constants.vbLf);


                    if (strToAppend.IndexOf(Constants.vbLf) != -1)
                    {
                        objSessionUpdate.ScrollToCaret = true;
                    }

                    objSessionUpdate.Text = strToAppend;
                    cllPendingUpdates.Add(objSessionUpdate);
                    if (blnScrollLocked)
                        return;

                    // This scheme keeps the bottom line of the Session rich text box visible but it still "jumps" areound up or down a line????
                    blnIgnoreManualSessionScroll = true;
                    // this keeps the auto scroll process from displaying the Scroll Locked button
                    //If blnLFAppended And rtbSession.TextLength > 0 Then
                    if (static_UpdateSessionDisplay_blnLFAppended & rtbSession.TextLength > 1)
                    {
                        // modified 0.1.6.1 to add two LF to improve last line readability
                        // rtbSession.SelectionStart = rtbSession.Text.Length - 1
                        //rtbSession.SelectionLength = 1
                        //rtbSession.SelectedText = Chr(0) ' This replaces the Appended LF with a null essentially deleting it
                        rtbSession.SelectionStart = rtbSession.Text.Length - 2;
                        rtbSession.SelectionLength = 2;
                        rtbSession.SelectedText = Strings.Chr(0) + Strings.Chr(0);
                        static_UpdateSessionDisplay_blnLFAppended = false;
                    }
                    foreach (SessionUpdates Update in cllPendingUpdates)
                    {
                        rtbSession.SelectionColor = Update.Color;
                        //If rtbSession.SelectionColor = Color.DarkGreen Then
                        //    Debug.WriteLine("UpdateText G=" & Update.Text.Replace(vbLf, "[Lf]"))
                        //End If
                        //If rtbSession.SelectionColor = Color.Black Then
                        //    rtbSession.SelectionColor = Color.Black
                        //    Debug.WriteLine("UpdateText B=" & Update.Text.Replace(vbLf, "[Lf]"))

                        //End If
                        rtbSession.SelectionFont = Update.Font;
                        rtbSession.SelectionStart = rtbSession.Text.Length;
                        rtbSession.AppendText(Update.Text);
                        rtbSession.SelectionLength = Update.Text.Length;
                        rtbSession.Select();
                        blnScrollToCaret = blnScrollToCaret | Update.ScrollToCaret;
                    }
                    cllPendingUpdates.Clear();
                    if (!rtbSession.Text.EndsWith(Constants.vbLf))
                    {
                        rtbSession.AppendText(Constants.vbLf + Constants.vbLf);
                        static_UpdateSessionDisplay_blnLFAppended = true;
                    }
                    rtbSession.SelectionStart = rtbSession.Text.Length;
                    rtbSession.Select();
                    if (blnScrollToCaret)
                    {
                        rtbSession.ScrollToCaret();
                        blnScrollToCaret = false;
                    }

                    blnIgnoreManualSessionScroll = false;
                }
                catch (Exception ex)
                {
                    Globals.Exception("[Main.UpdateSessionDisplay] Err: " + ex.ToString());
                }
            }
            if (blnRTBSendHadFocus & !blnRTBSendHasFocus)
            {
                rtbSend.Focus();
                //Debug.WriteLine("UpdateSessionDisplay, rtbSend Set focus")
            }
        }

        private void CloseTNC()
        {
            try
            {
                if (objTNCTCPIPPort.Connected == true)
                {
                    SendCommandToTNC("CLOSE");
                    Thread.Sleep(1000);
                }
                else
                {
                    return;
                }

                if (objTCPData.Connected)
                {
                    objTNCTCPIPPort.Linger = false;
                    objTCPData.Disconnect();
                }
                objTNCTCPIPPort.Dispose();
                objTCPData = null;
                bool blnRunning = false;
                Process[] Processes = Process.GetProcesses();
                foreach (Process objProcess in Processes)
                {
                    if (objProcess.Id == intARDOP_WinProcessID)
                    {
                        blnRunning = true; break; // TODO: might not be correct. Was : Exit For
                    }
                }

                if (blnRunning)
                {
                    try
                    {
                        Process objProcess = Process.GetProcessById(intARDOP_WinProcessID);
                        objProcess.Kill();
                        int intLoops = 0;
                        do
                        {
                            Thread.Sleep(100);
                            if (objProcess.HasExited)
                            {
                                intARDOP_WinProcessID = -1;
                                blnTNCIsOpen = false;
                                break; // TODO: might not be correct. Was : Exit Do
                            }
                            intLoops += 1;
                            if (intLoops > 30)
                            {
                                break; // TODO: might not be correct. Was : Exit Do
                            }
                        } while (true);
                    }
                    catch
                    {
                    }
                }
                else
                {
                    blnTNCIsOpen = false;
                }
                Globals.objINIFile.WriteInteger("H4_TNC", "Process Id", -1);
                Globals.objINIFile.Flush();
                Thread.Sleep(200);
            }
            catch (Exception ex)
            {
                Globals.Exceptions("Main.CloseTNC: " + ex.ToString());
            }

        }

        private void Main_FormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            //UpdateContacts();
            Globals.objINIFile.WriteString("Main", "Startup Mode", Globals.strMode);
            if (intARDOP_WinProcessID != -1)
            {
                try
                {
                    Process objProcess = Process.GetProcessById(intARDOP_WinProcessID);
                    objProcess.Kill();
                    Globals.Log("*** Existing ARDOP_Win process " + intARDOP_WinProcessID.ToString() + " killed." + Constants.vbCr);
                    blnTNCIsOpen = false;
                    // Thread.Sleep(1000)
                }
                catch
                {
                }
            }

            if (this.WindowState == FormWindowState.Normal)
            {
                try
                {
                    // objINIFile.WriteInteger("Chat", "Horiz Splitter", CInt(100 * rtbSession.Height / (rtbSession.Height + rtbSend.Height)))
                    Globals.objINIFile.WriteInteger("ARDOP Chat", "Horiz Splitter", splHorizontal.SplitterDistance);
                    //objINIFile.WriteInteger("ARDOP Chat", "Vert Splitter", splVertical.SplitterDistance);
                    Globals.objINIFile.WriteInteger("ARDOP Chat", "Top", this.Top);
                    Globals.objINIFile.WriteInteger("ARDOP Chat", "Left", this.Left);
                    Globals.objINIFile.WriteInteger("ARDOP Chat", "Width", this.Width);
                    Globals.objINIFile.WriteInteger("ARDOP Chat", "Height", this.Height);

                }
                catch (Exception ex)
                {
                    Globals.Exceptions("ARDOP Chat main form Closed: " + ex.ToString());
                }

            }
            Globals.objINIFile.Flush();
            Thread.Sleep(200);
        }
        private void Main_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            if (blnStartingARDOPTNC)
            {
                e.Cancel = true;
                return;
            }

            if (Globals.blnAutoupdateInProgress)
            {
                if (Interaction.MsgBox("Autoupdate is in progress! ... Do you want to force close and cancel Autoupdate?", MsgBoxStyle.YesNo, "Autoupdate in Progress!") == MsgBoxResult.No)
                {
                    e.Cancel = true;
                    return;
                }
            }
            if (Globals.objRadio != null)
                Globals.objRadio.CloseRadio();
            if (blnTNCIsOpen)
            {
                CloseTNC();
            }
            if (Globals.objAutoupdate != null)
            {
                Globals.objAutoupdate.Close();
            }
            Globals.objINIFile.WriteString("Main", "Test Autoupdate", "False");
            Globals.objINIFile.WriteString("ARDOP Chat", "AutoID", Globals.blnAutoID.ToString());
        }

        private void Main_Load(object sender, System.EventArgs e)
        {
            Globals.strExecutionDirectory = Application.StartupPath + "\\";
            Globals.strLogsDirectory = Globals.strExecutionDirectory + "Logs\\";
            if (!Directory.Exists(Globals.strLogsDirectory))
                Directory.CreateDirectory(Globals.strLogsDirectory);
            Globals.strDataDirectory = Globals.strExecutionDirectory + "Data\\";
            if (!Directory.Exists(Globals.strDataDirectory))
                Directory.CreateDirectory(Globals.strDataDirectory);

            Globals.objINIFile = new INIFile(Globals.strExecutionDirectory + "ARES_Messenger.ini");
            tmrStartup.Start();
            Globals.SetMainReference(this);
        }


        int static_tmrPoll_Tick_intTicks;
        private void tmrPoll_Tick(object sender, System.EventArgs e)
        {
            tmrPoll.Stop();
            UpdateSessionDisplay();
            if (Globals.blnAborting)
            {
                SendCommandToTNC("ABORT");
                Globals.blnAborting = false;
            }
            btnEnd.Visible = blnBtnEndVisible;
            if (blnRestartTNC)
            {
                RestartTNC();
                Thread.Sleep(1000);
            }
            if (blnUpdateTNCdisplay)
            {
                if ((Globals.objRadio != null))
                {
                    Globals.queSessionEvents.Enqueue("G *** Freq Changed: " + Strings.Format(intFrequency / 1000, "##0000.000") + " KHz Dial" + Constants.vbCrLf);
                    Globals.objRadio.SetFrequency(intFrequency);
                    SendCommandToTNC("DISPLAY " + "USB Dial:" + Strings.Format((intFrequency) / 1000, "##0000.000"));
                }
                blnUpdateTNCdisplay = false;
            }
            if (blnRequestCWID)
            {
                Thread.Sleep(100);
                blnRequestCWID = false;
                SendCommandToTNC("CWID DE " + Globals.strMyCallsign);
            }
            // only log on changes in blnAutoUpdateInProgress
            if (Globals.blnAutoupdateInProgress & !Globals.blnAutoUpdateInProgressLogged)
            {
                Globals.queSessionEvents.Enqueue("P " + "*** Autoupdate in Process... @ " + Globals.TimestampEx() + Constants.vbCrLf);
                Globals.blnAutoUpdateInProgressLogged = true;
            }
            else if (Globals.blnAutoUpdateInProgressLogged & !Globals.blnAutoupdateInProgress)
            {
                Globals.queSessionEvents.Enqueue("P " + "*** Autoupdate Complete @ " + Globals.TimestampEx() + "   Restart Chat to run new version." + Constants.vbCrLf);
                Globals.blnAutoUpdateInProgressLogged = false;
            }
            if (lblQueue.Text != "OB Queue: " + intOBQueueValue.ToString())
            {
                lblQueue.Text = "OB Queue: " + intOBQueueValue.ToString();
            }
            if (intDisplayedThroughput != intThroughput)
            {
                prgThroughput.Value = Math.Min(intThroughput, prgThroughput.Maximum);
                intDisplayedThroughput = intThroughput;
                lblThroughput.Text = strThruPutDirection + intThroughput.ToString();
            }

            if (!string.IsNullOrEmpty(strAnswerCQNewText))
            {
                mnuAnswerCQ.Text = strAnswerCQNewText;
                strAnswerCQNewText = "";
                mnuAnswerCQ.Enabled = false;
                //  True  'Disable
            }

            if (DateTime.Now.Subtract(dttLastCQReceived).TotalSeconds > 60)
            {
                blnAnsweringCQ = false;
                mnuAnswerCQ.Enabled = false;
                mnuAnswerCQ.Text = "ANSWER CQ";
            }
            if (blnScrollLocked & DateTime.Now.Subtract(dttScrollLocked).TotalSeconds > 10)
                btnScrollLock.PerformClick();
            if (!(Globals.blnARQConnected | Globals.blnARQCalling | blnPTT))
            {
                static_tmrPoll_Tick_intTicks += 1;
                // only request frequency every 2 seconds when not trconnected or calling or transmitting 
                if (static_tmrPoll_Tick_intTicks >= 20)
                {
                    static_tmrPoll_Tick_intTicks = 0;
                    if ((Globals.objRadio != null))
                    {
                        int intReportedFreq = 0;
                        intReportedFreq = Globals.objRadio.GetFrequency();
                        if (intFrequency != intReportedFreq & intReportedFreq != 0)
                        {
                            SendCommandToTNC("DISPLAY " + "USB dial: " + Strings.Format((intReportedFreq) / 1000, "##0000.000"));
                            intFrequency = intReportedFreq;
                            Globals.queSessionEvents.Enqueue("A  *** Freq Changed: " + Strings.Format(intFrequency / 1000, "##0000.000") + " KHz Dial" + Constants.vbCrLf);
                            if (!blnRadioModeInitialized)
                            {
                                Globals.objRadio.SetFrequency(intFrequency);
                                blnRadioModeInitialized = true;
                            }
                        }
                    }
                }
            }
            // Only send beacon if no ARQ connection and outbound Queue = 0 
            if (Globals.blnEnableBeacon & DateTime.Now.Subtract(Globals.dttNextBeacon).TotalSeconds > 0 & intOBQueueValue == 0 & !Globals.blnARQConnected)
            {
                if (Globals.strMode == "FEC RBST")
                {
                    strSaveMode = "";
                }
                else
                {
                    strSaveMode = Globals.strMode;
                    SendCommandToTNC("MODE FEC RBST");
                    Globals.strMode = "FEC RBST";
                }
                rtbSend.Text = rtbSend.Text + Constants.vbCrLf + Globals.strFECBeaconText + Constants.vbCrLf;
                Globals.queSessionEvents.Enqueue("P  *** Robust FEC Beacon sent @ " + Globals.TimestampEx() + " UTC" + Constants.vbCrLf);
                btnQueueData.PerformClick();
                Globals.dttNextBeacon = DateTime.Now.AddMinutes((0.9 + 0.2 * VBMath.Rnd(1)) * Globals.intBeaconInterval);
                // schedule in an average time of intBeaconInterval
            }
            if (mnuMode.Text != "Mode: " + Globals.strMode)
            {
                mnuMode.Text = "Mode: " + Globals.strMode;
                Globals.queSessionEvents.Enqueue("P  " + Constants.vbCrLf + "*** Mode: " + Globals.strMode + Constants.vbCrLf);
                mnuCall.Enabled = (Globals.strMode == "ARQ");
            }

            if (DateTime.Now.Subtract(dttLastRxSpeedUpdate).TotalSeconds > 10 & !blnPTT)
            {
                UpdateInboundThroughput(0);
            }
            if (Globals.blnAutoupdateRestart & !blnRestartNotification)
            {
                if (Interaction.MsgBox("Autoupdate completed!  Restart application?", MsgBoxStyle.YesNo, "Auto Update Successful") == Constants.vbYes)
                {
                    Application.Restart();
                }
                blnRestartNotification = true;
            }
            if (mnuMode.Enabled != blnMenuModeEnabled)
                mnuMode.Enabled = blnMenuModeEnabled;
            tmrPoll.Start();
        }



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

        }
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
        }*/

        private void StartARDOPWinTNC()
        {
            if (File.Exists(Globals.strExecutionDirectory + "ARDOP_Win.exe") == false)
            {
                Globals.Exceptions("*** ARDOP_Win.exe is not found in the program directory..." + Constants.vbCr);
                Interaction.MsgBox("ARDOP_Win.exe is not found in the program directory...", MsgBoxStyle.Information);
                return;
            }

            if (string.IsNullOrEmpty(Globals.strCaptureDevice) | string.IsNullOrEmpty(Globals.strPlaybackDevice))
            {
                ChatSetupForm dlgSetup = new ChatSetupForm();
                if (dlgSetup.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    Interaction.MsgBox("Close ARDOP Chat and restart to use any updated values in Chat setup", MsgBoxStyle.Information);
                    return;
                }
            }
            Application.DoEvents();
            do
            {
                intARDOP_WinProcessID = Globals.objINIFile.GetInteger("ARDOP_Win", "Process Id", -1);
                if (intARDOP_WinProcessID != -1)
                {
                    try
                    {
                        Process objProcess = Process.GetProcessById(intARDOP_WinProcessID);
                        objProcess.Kill();
                        Globals.Log("*** Existing ARDOP_Win process " + intARDOP_WinProcessID.ToString() + " killed." + Constants.vbCr);
                        blnTNCIsOpen = false;
                        Thread.Sleep(1000);
                    }
                    catch
                    {
                    }
                }

                intARDOP_WinProcessID = -1;
                try
                {
                    intARDOP_WinProcessID = Interaction.Shell(Globals.strExecutionDirectory + "ARDOP_Win.exe TCPIP " + Globals.intTCPIPPort.ToString() + " " + Globals.strTCPIPAddress, AppWinStyle.NormalFocus);
                }
                catch (Exception ex)
                {
                    Globals.Exceptions("StartARDOPWinTNC] Err: " + ex.ToString());
                    break; // TODO: might not be correct. Was : Exit Do
                }

                if (intARDOP_WinProcessID <= 0)
                {
                    Globals.queSessionEvents.Enqueue("F *** Failure to start ARDOP_Win.exe virtual TNC!" + Constants.vbCr);
                    break; // TODO: might not be correct. Was : Exit Do
                }
                else
                {
                    Globals.objINIFile.WriteInteger("ARDOP_Win", "Process Id", intARDOP_WinProcessID);
                    Globals.objINIFile.Flush();
                    blnTNCIsOpen = true;
                }
                if (OpenTNC_TCPIPPort() == false)
                {
                    Globals.queSessionEvents.Enqueue("F *** Failure to open TCPIP Port " + Globals.intTCPIPPort.ToString() + " to ARDOP_Win.exe" + Constants.vbCr + "Check and make sure port is unused." + Constants.vbCr);
                    break; // TODO: might not be correct. Was : Exit Do
                }
                blnTNCIsOpen = true;
                if (SendCommandToTNC("VERSION") == false)
                    break; // TODO: might not be correct. Was : Exit Do
                //If SendCommandToTNC("CODEC") = False Then Exit Do
                if (SendCommandToTNC("MYCALL " + Globals.strMyCallsign) == false)
                    break; // TODO: might not be correct. Was : Exit Do
                blnTNCIsOpen = true;
                //If SendCommandToTNC("DEBUGLOG " & blnEnableDebugLogs.ToString) = False Then Exit Do
                //If SendCommandToTNC("CODEC False") = False Then Exit Do
                //If SendCommandToTNC("ProcessId") = False Then Exit Do
                //If SendCommandToTNC("MYCALL " & strMyCallsign) = False Then Exit Do
                //If SendCommandToTNC("Capture " & strCaptureDevice) = False Then Exit Do
                //If SendCommandToTNC("Playback " & strPlaybackDevice) = False Then Exit Do
                //If SendCommandToTNC("MODE " & strMode) = False Then Exit Do
                //If SendCommandToTNC("TIMEOUT " & intARQTimeout.ToString) = False Then Exit Do
                //If SendCommandToTNC("DRIVELEVEL " & intH4DriveLevel.ToString) = False Then Exit Do
                //If SendCommandToTNC("AUTOID " & blnAutoID.ToString) = False Then Exit Do
                //If SendCommandToTNC("TUNING " & intTuning.ToString) = False Then Exit Do
                //If SendCommandToTNC("ENCODING " & strCharacterSet) = False Then Exit Do
                //If SendCommandToTNC("FRONTPORCH " & intFrontPorch.ToString) = False Then Exit Do
                //If SendCommandToTNC("BACKPORCH " & intBackPorch.ToString) = False Then Exit Do
                //Dim strPTTPort As String = objINIFile.GetString("ARDOP Chat", "PTT Port", "External")
                //If SendCommandToTNC("VOX " & (strPTTPort = "External").ToString) = False Then Exit Do
                //If SendCommandToTNC("CWONARQID " & blnMorseId.ToString) = False Then Exit Do
                //If SendCommandToTNC("BUFFER") = False Then Exit Do
                //blnTNCIsOpen = SendCommandToTNC("CODEC True")
                Globals.blnARQCalling = false;
                break; // TODO: might not be correct. Was : Exit Do
            } while (true);
            if (!blnTNCIsOpen)
                Globals.queSessionEvents.Enqueue("F *** Failure to open Open ARDOP Win TNC." + Constants.vbCr);
            this.Cursor = Cursors.Default;
        }

        // Function to open the command port to the TNC
        private bool OpenTNC_TCPIPPort()
        {
            // Open the TCPIP port to the TNC...
            if (objTNCTCPIPPort != null)
            {
                objTNCTCPIPPort.Linger = false;
                objTNCTCPIPPort.Connected = false;
                objTNCTCPIPPort.Dispose();
                objTNCTCPIPPort = null;
            }
            objTNCTCPIPPort = new nsoftware.IPWorks.Ipport();
            System.DateTime dttStartOpen = DateTime.Now;
            while (DateTime.Now.Subtract(dttStartOpen).TotalSeconds < 10)
            {
                try
                {
                    objTNCTCPIPPort.Connected = false;
                    Thread.Sleep(100);
                    objTNCTCPIPPort.Linger = true;
                    objTNCTCPIPPort.KeepAlive = true;
                    objTNCTCPIPPort.AcceptData = true;
                    objTNCTCPIPPort.RemoteHost = Globals.strTCPIPAddress;
                    objTNCTCPIPPort.RemotePort = Globals.intTCPIPPort;
                    blnRDY = false;
                    blnData = false;
                    objTNCTCPIPPort.Connected = true;
                    Thread.Sleep(1000);
                }
                catch
                {
                    // Do nothing... TNC may still be starting up.
                }

                // Wait for CMD response from the TNC. Restart if not seen within 10 seconds...

                // once connected wait for CMD response
                while (objTNCTCPIPPort.Connected)
                {
                    if (DateTime.Now.Subtract(dttStartOpen).TotalSeconds > 10)
                    {
                        Globals.Exceptions("[Main.OpenTNC_TCPIPPort] RDY Response 10 sec Timeout");
                        return false;
                    }
                    if (blnRDY == true)
                    {
                        Debug.WriteLine("[OpenTNC_TCPIPPort] Reply from TNC: RDY");
                        return true;
                    }

                    Thread.Sleep(100);
                    // wait 100ms and try again
                }
                if (!objTNCTCPIPPort.Connected)
                {
                    objTNCTCPIPPort.Linger = false;
                    Thread.Sleep(100);
                    // wait 100ms
                }
            }
            Globals.Exceptions("[Main.OpenTNC_TCPIPPort] 10 sec Timeout. Could not connect to TNC TCPIP Port on port " + Globals.intTCPIPPort.ToString());
            return false;
        }



        public bool SendCommandToTNC(string strCommand)
        {
            //  This is from Host side as identified by the leading "C:"   TNC side sends "c:"
            // Subroutine to send a line of text (terminated with <Cr>) on the command port... All commands beging with "C:" and end with <Cr>
            // A two byte CRC appended following the <Cr>
            // The strText cannot contain a "C:" sequence or a <Cr>
            // Returns TRUE if command sent successfully.
            // Form byt array to send with CRC
            // TODO:  Complete for Serial and BlueTooth

            if (Globals.blnRestarting)
                return false;
            // True
            if (objTNCTCPIPPort == null)
                return false;
            //True
            if (objTNCTCPIPPort.Connected == false)
                return false;
            System.DateTime dttStartWait = DateTime.Now;
            while (DateTime.Now.Subtract(dttStartWait).TotalSeconds < 5 & (!blnRDY))
            {
                Thread.Sleep(50);
            }
            if (!blnRDY)
            {
                Globals.Exception("[SendCommandToTNC] Err: Timeout waiting for RDY");
                return false;
            }
            blnRDY = false;
            try
            {
                byte[] bytToSend = Globals.GetBytes("C:" + strCommand.Trim().ToUpper() + Constants.vbCr);
                Array.Resize(ref bytToSend, bytToSend.Length + 2);
                // resize 2 bytes larger for CRC
                Globals.GenCRC16(ref bytToSend, 2, bytToSend.Length - 3, 0xffff);
                // Generate CRC starting after "c:"  
                try
                {
                    objTNCTCPIPPort.Send(bytToSend);
                    return true;
                }
                catch
                {
                    //Globals.Exception("[SendCommandToTNC] Err: " + Err.Description);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Globals.Exception("[SendCommandToTNC] Err: " + ex.ToString());
            }
            return false;
        }

        private void AppendDataToBuffer(ref byte[] bytNewData, ref byte[] bytBuffer)
        {
            if (bytNewData.Length == 0)
                return;
            int intStartPtr = bytBuffer.Length;
            Array.Resize(ref bytBuffer, bytBuffer.Length + bytNewData.Length);
            Array.Copy(bytNewData, 0, bytBuffer, intStartPtr, bytNewData.Length);
        }
        readonly Microsoft.VisualBasic.CompilerServices.StaticLocalInitFlag static_objTNCTCPIPPort_OnDataIn_strCommandFromTNC_Init = new Microsoft.VisualBasic.CompilerServices.StaticLocalInitFlag();


        // This creates a first in first out buffer and pointers to handle receiving commands or data.
        // Data may be received on NON CMD or Data frame boundaries. (handles buffer and latency issues)
        string static_objTNCTCPIPPort_OnDataIn_strCommandFromTNC;
        readonly Microsoft.VisualBasic.CompilerServices.StaticLocalInitFlag static_objTNCTCPIPPort_OnDataIn_bytDataFromTNC_Init = new Microsoft.VisualBasic.CompilerServices.StaticLocalInitFlag();
        byte[] static_objTNCTCPIPPort_OnDataIn_bytDataFromTNC;
        readonly Microsoft.VisualBasic.CompilerServices.StaticLocalInitFlag static_objTNCTCPIPPort_OnDataIn_intDataBytesToReceive_Init = new Microsoft.VisualBasic.CompilerServices.StaticLocalInitFlag();
        Int32 static_objTNCTCPIPPort_OnDataIn_intDataBytesToReceive;
        readonly Microsoft.VisualBasic.CompilerServices.StaticLocalInitFlag static_objTNCTCPIPPort_OnDataIn_blnReceivingCMD_Init = new Microsoft.VisualBasic.CompilerServices.StaticLocalInitFlag();
        bool static_objTNCTCPIPPort_OnDataIn_blnReceivingCMD;
        readonly Microsoft.VisualBasic.CompilerServices.StaticLocalInitFlag static_objTNCTCPIPPort_OnDataIn_blnReceivingData_Init = new Microsoft.VisualBasic.CompilerServices.StaticLocalInitFlag();
        bool static_objTNCTCPIPPort_OnDataIn_blnReceivingData;
        readonly Microsoft.VisualBasic.CompilerServices.StaticLocalInitFlag static_objTNCTCPIPPort_OnDataIn_intDataBytePtr_Init = new Microsoft.VisualBasic.CompilerServices.StaticLocalInitFlag();
        Int32 static_objTNCTCPIPPort_OnDataIn_intDataBytePtr;
        readonly Microsoft.VisualBasic.CompilerServices.StaticLocalInitFlag static_objTNCTCPIPPort_OnDataIn_intCMDStartPtr_Init = new Microsoft.VisualBasic.CompilerServices.StaticLocalInitFlag();
        Int32 static_objTNCTCPIPPort_OnDataIn_intCMDStartPtr;
        readonly Microsoft.VisualBasic.CompilerServices.StaticLocalInitFlag static_objTNCTCPIPPort_OnDataIn_intDataStartPtr_Init = new Microsoft.VisualBasic.CompilerServices.StaticLocalInitFlag();
        Int32 static_objTNCTCPIPPort_OnDataIn_intDataStartPtr;
        private void objTNCTCPIPPort_OnDataIn(object sender, nsoftware.IPWorks.IpportDataInEventArgs e)
        {
            lock (static_objTNCTCPIPPort_OnDataIn_strCommandFromTNC_Init)
            {
                try
                {
                    if (InitStaticVariableHelper(static_objTNCTCPIPPort_OnDataIn_strCommandFromTNC_Init))
                    {
                        static_objTNCTCPIPPort_OnDataIn_strCommandFromTNC = "";
                    }
                }
                finally
                {
                    static_objTNCTCPIPPort_OnDataIn_strCommandFromTNC_Init.State = 1;
                }
            }
            lock (static_objTNCTCPIPPort_OnDataIn_bytDataFromTNC_Init)
            {
                try
                {
                    if (InitStaticVariableHelper(static_objTNCTCPIPPort_OnDataIn_bytDataFromTNC_Init))
                    {
                        static_objTNCTCPIPPort_OnDataIn_bytDataFromTNC = null;
                    }
                }
                finally
                {
                    static_objTNCTCPIPPort_OnDataIn_bytDataFromTNC_Init.State = 1;
                }
            }
            lock (static_objTNCTCPIPPort_OnDataIn_intDataBytesToReceive_Init)
            {
                try
                {
                    if (InitStaticVariableHelper(static_objTNCTCPIPPort_OnDataIn_intDataBytesToReceive_Init))
                    {
                        static_objTNCTCPIPPort_OnDataIn_intDataBytesToReceive = 0;
                    }
                }
                finally
                {
                    static_objTNCTCPIPPort_OnDataIn_intDataBytesToReceive_Init.State = 1;
                }
            }
            lock (static_objTNCTCPIPPort_OnDataIn_blnReceivingCMD_Init)
            {
                try
                {
                    if (InitStaticVariableHelper(static_objTNCTCPIPPort_OnDataIn_blnReceivingCMD_Init))
                    {
                        static_objTNCTCPIPPort_OnDataIn_blnReceivingCMD = false;
                    }
                }
                finally
                {
                    static_objTNCTCPIPPort_OnDataIn_blnReceivingCMD_Init.State = 1;
                }
            }
            lock (static_objTNCTCPIPPort_OnDataIn_blnReceivingData_Init)
            {
                try
                {
                    if (InitStaticVariableHelper(static_objTNCTCPIPPort_OnDataIn_blnReceivingData_Init))
                    {
                        static_objTNCTCPIPPort_OnDataIn_blnReceivingData = false;
                    }
                }
                finally
                {
                    static_objTNCTCPIPPort_OnDataIn_blnReceivingData_Init.State = 1;
                }
            }
            lock (static_objTNCTCPIPPort_OnDataIn_intDataBytePtr_Init)
            {
                try
                {
                    if (InitStaticVariableHelper(static_objTNCTCPIPPort_OnDataIn_intDataBytePtr_Init))
                    {
                        static_objTNCTCPIPPort_OnDataIn_intDataBytePtr = 0;
                    }
                }
                finally
                {
                    static_objTNCTCPIPPort_OnDataIn_intDataBytePtr_Init.State = 1;
                }
            }
            lock (static_objTNCTCPIPPort_OnDataIn_intCMDStartPtr_Init)
            {
                try
                {
                    if (InitStaticVariableHelper(static_objTNCTCPIPPort_OnDataIn_intCMDStartPtr_Init))
                    {
                        static_objTNCTCPIPPort_OnDataIn_intCMDStartPtr = 0;
                    }
                }
                finally
                {
                    static_objTNCTCPIPPort_OnDataIn_intCMDStartPtr_Init.State = 1;
                }
            }
            lock (static_objTNCTCPIPPort_OnDataIn_intDataStartPtr_Init)
            {
                try
                {
                    if (InitStaticVariableHelper(static_objTNCTCPIPPort_OnDataIn_intDataStartPtr_Init))
                    {
                        static_objTNCTCPIPPort_OnDataIn_intDataStartPtr = 0;
                    }
                }
                finally
                {
                    static_objTNCTCPIPPort_OnDataIn_intDataStartPtr_Init.State = 1;
                }
            }

            //Logs.WriteDebug("[Main.objTCPIP.OnDataIn] TCPIP OnDataIn from host: " & e.Text)
            AppendDataToBuffer(ref e.TextB, ref bytTNCIBData_CmdBuffer);
        SearchForStart:

            // look for start of Command ("C:")  or Data (D:") and establish start pointer (Capital C or D indicates from Host)
            if (!(static_objTNCTCPIPPort_OnDataIn_blnReceivingCMD | static_objTNCTCPIPPort_OnDataIn_blnReceivingData))
            {
                for (int i = intTNCIBData_CmdPtr; i <= bytTNCIBData_CmdBuffer.Length - 2; i++)
                {
                    // search for ASCII "c:"
                    if (bytTNCIBData_CmdBuffer[i] == 0x63 & bytTNCIBData_CmdBuffer[i + 1] == 0x3a)
                    {
                        // start of command.
                        static_objTNCTCPIPPort_OnDataIn_intCMDStartPtr = i;
                        static_objTNCTCPIPPort_OnDataIn_blnReceivingCMD = true;
                        static_objTNCTCPIPPort_OnDataIn_blnReceivingData = false;
                        break; // TODO: might not be correct. Was : Exit For
                        // search for ASCII "d:"
                    }
                    else if (bytTNCIBData_CmdBuffer[i] == 0x64 & bytTNCIBData_CmdBuffer[i + 1] == 0x3a)
                    {
                        // start of Data
                        static_objTNCTCPIPPort_OnDataIn_intDataStartPtr = i;
                        static_objTNCTCPIPPort_OnDataIn_blnReceivingCMD = false;
                        static_objTNCTCPIPPort_OnDataIn_blnReceivingData = true;
                        static_objTNCTCPIPPort_OnDataIn_intDataBytesToReceive = 0;
                        break; // TODO: might not be correct. Was : Exit For
                    }
                }
            }

            if (static_objTNCTCPIPPort_OnDataIn_blnReceivingCMD)
            {
                // Look for <Cr> with room for 2 byte CRC
                for (int i = static_objTNCTCPIPPort_OnDataIn_intCMDStartPtr; i <= bytTNCIBData_CmdBuffer.Length - 3; i++)
                {
                    // search for Carriage Return which ends Command (note 2 CRC bytes to follow)
                    if (bytTNCIBData_CmdBuffer[i] == 0xd)
                    {
                        byte[] bytCmd = new byte[i - static_objTNCTCPIPPort_OnDataIn_intCMDStartPtr + 1];
                        // 2 bytes added for CRC and "c:" skipped
                        Array.Copy(bytTNCIBData_CmdBuffer, static_objTNCTCPIPPort_OnDataIn_intCMDStartPtr + 2, bytCmd, 0, bytCmd.Length);
                        //copy over the Command (less "c:") and the 2 byte CRC
                        // check the CRC
                        if (Globals.CheckCRC16(ref bytCmd, 0xffff))
                        {
                            Array.Resize(ref bytCmd, bytCmd.Length - 2);
                            // Drop off the CRC
                            static_objTNCTCPIPPort_OnDataIn_strCommandFromTNC = Globals.GetString(bytCmd);
                            Debug.WriteLine("[OnDataIn] Command Received: " + static_objTNCTCPIPPort_OnDataIn_strCommandFromTNC);

                            // Process the received and CRC checked command here:
                            ProcessCmdFromTNC(static_objTNCTCPIPPort_OnDataIn_strCommandFromTNC.Trim().ToUpper());
                        }
                        else
                        {
                            SendCommandToTNC("CRCFAULT");
                        }
                        // resize buffer and reset pointer
                        static_objTNCTCPIPPort_OnDataIn_blnReceivingCMD = false;
                        byte[] bytTemp = new byte[bytTNCIBData_CmdBuffer.Length - i - 3];
                        //skip past the 2 byte CRC
                        if (bytTemp.Length > 0)
                            Array.Copy(bytTNCIBData_CmdBuffer, i + 3, bytTemp, 0, bytTemp.Length);
                        bytTNCIBData_CmdBuffer = bytTemp;
                        intTNCIBData_CmdPtr = 0;
                        if (bytTNCIBData_CmdBuffer.Length > 0)
                            goto SearchForStart;
                    }
                }
            }

            if (static_objTNCTCPIPPort_OnDataIn_blnReceivingData)
            {
                // Data lenght must always be >0 for a legitimate data frame:
                if (static_objTNCTCPIPPort_OnDataIn_intDataBytesToReceive == 0)
                {
                    if (bytTNCIBData_CmdBuffer.Length - static_objTNCTCPIPPort_OnDataIn_intDataStartPtr >= 4)
                    {
                        // Compute the byte count to receive plus 2 additional bytes for the 16 bit CRC
                        static_objTNCTCPIPPort_OnDataIn_intDataBytesToReceive = (bytTNCIBData_CmdBuffer[intTNCIBData_CmdPtr + 2] << 8) + bytTNCIBData_CmdBuffer[intTNCIBData_CmdPtr + 3] + 2;
                        // includes 2 byte CRC
                        intTNCIBData_CmdPtr = intTNCIBData_CmdPtr + 4;
                        // advance pointer past "d:" and byte count
                        static_objTNCTCPIPPort_OnDataIn_bytDataFromTNC = new byte[static_objTNCTCPIPPort_OnDataIn_intDataBytesToReceive];
                        static_objTNCTCPIPPort_OnDataIn_intDataBytePtr = 0;
                    }
                }
                if (static_objTNCTCPIPPort_OnDataIn_intDataBytesToReceive > 0 & (intTNCIBData_CmdPtr < bytTNCIBData_CmdBuffer.Length))
                {
                    for (int i = 0; i <= bytTNCIBData_CmdBuffer.Length - intTNCIBData_CmdPtr - 3; i++)
                    {
                        static_objTNCTCPIPPort_OnDataIn_bytDataFromTNC[static_objTNCTCPIPPort_OnDataIn_intDataBytePtr] = bytTNCIBData_CmdBuffer[intTNCIBData_CmdPtr];
                        static_objTNCTCPIPPort_OnDataIn_intDataBytePtr += 1;
                        intTNCIBData_CmdPtr += 1;
                        static_objTNCTCPIPPort_OnDataIn_intDataBytesToReceive -= 1;
                        if (static_objTNCTCPIPPort_OnDataIn_intDataBytesToReceive == 0)
                            break; // TODO: might not be correct. Was : Exit For
                    }
                    if (static_objTNCTCPIPPort_OnDataIn_intDataBytesToReceive == 0)
                    {
                        static_objTNCTCPIPPort_OnDataIn_blnReceivingData = false;
                        // Process bytDataFromHost here (check CRC etc) 
                        //Logs.WriteDebug("[Main.objTCPIP.OnDataIn] Data received from host: " & bytDataFromHost.Length.ToString & " bytes")
                        // resize the bufffer, and set pointer to it's start.
                        if (intTNCIBData_CmdPtr >= bytTNCIBData_CmdBuffer.Length - 1)
                        {
                            bytTNCIBData_CmdBuffer = new byte[-1 + 1];
                            // clear the buffer and zero the pointer
                            intTNCIBData_CmdPtr = 0;
                            // resize the bufffer, and set pointer to it's start.
                        }
                        else
                        {
                            byte[] bytTemp = new byte[bytTNCIBData_CmdBuffer.Length - intTNCIBData_CmdPtr - 1];
                            Array.Copy(bytTNCIBData_CmdBuffer, intTNCIBData_CmdPtr, bytTemp, 0, bytTemp.Length);
                            bytTNCIBData_CmdBuffer = bytTemp;
                            intTNCIBData_CmdPtr = 0;
                        }
                    }
                }
            }
        }

        private bool ProcessCmdFromTNC(string strCMD)
        {

            if (strCMD == "RDY")
            {
                blnRDY = true;
            }
            if (strCMD.StartsWith("VERSION"))
            {
                Globals.queSessionEvents.Enqueue("P  *** TNC " + strCMD + Constants.vbLf);
            }
        }
        private void ARQToolStripMenuItem_Click(System.Object sender, System.EventArgs e)
        {
            //MsgBox("ARQ Mode is in development and not fully implemented. Do not report issues at this time.", MsgBoxStyle.Information, "ARQ...work in process...")

            mnuMode.Text = "Mode: ARQ";
            SendCommandToTNC("MODE ARQ");
            Globals.strMode = "ARQ";
            Globals.queSessionEvents.Enqueue("P  " + Constants.vbCrLf + "*** Mode: ARQ" + Constants.vbCrLf);
            this.rtbSend.Enabled = true;
            mnuPasteAndSend.Enabled = true;
            mnuPaste.Enabled = true;
            mnuCall.Enabled = true;
            Globals.blnARQCalling = false;
        }



        private void FECToolStripMenuItem_Click(System.Object sender, System.EventArgs e)
        {
            SendCommandToTNC("MODE FEC");
            mnuMode.Text = "Mode: FEC";
            Globals.strMode = "FEC";
            Globals.queSessionEvents.Enqueue("P  " + Constants.vbCrLf + "*** Mode: FEC" + Constants.vbCrLf);
            mnuCall.Enabled = false;
            this.rtbSend.Enabled = true;
            mnuPasteAndSend.Enabled = true;
            mnuPaste.Enabled = true;
        }


        private void mnuLogs_Click(System.Object sender, System.EventArgs e)
        {
            try
            {
                OpenFileDialog dlgViewLog = new OpenFileDialog();
                dlgViewLog.Multiselect = true;
                dlgViewLog.Title = "Select a Log File to View...";
                dlgViewLog.InitialDirectory = Globals.strLogsDirectory;
                dlgViewLog.Filter = "Log File(.log;.adi)|*.log;*.adi";
                // Image Files(*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF|All files (*.*)|*.* 
                dlgViewLog.RestoreDirectory = true;
                if (dlgViewLog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    try
                    {
                        Process.Start(dlgViewLog.FileName);
                    }
                    catch
                    {
                        //Interaction.MsgBox(Err.Description, MsgBoxStyle.Information);
                    }
                }
            }
            catch
            {
                //Globals.Exceptions("[Main.Log View Click] " + Err.Description);
            }
        }

        /*protected override void Finalize()
        {
            base.Finalize();
        }*/


        private void objTCPData_OnDataIn(object sender, nsoftware.IPWorks.IpportDataInEventArgs e)
        {
            Globals.DataType enmDataType = default(Globals.DataType);

            byte[] bytReceived = Globals.StripTypeFromData(e.TextB, ref enmDataType);
            string strReceived = "";
            if (Globals.strCharacterSet == "UTF-8")
            {
                UTF8Encoding objUTF8 = new UTF8Encoding();
                strReceived = objUTF8.GetString(bytReceived);
            }
            else
            {
                //Dim objASCII As New u
                strReceived = Globals.GetString(bytReceived);
            }
            switch (enmDataType)
            {
                case Globals.DataType.RcvdFEC:
                    Globals.queSessionEvents.Enqueue("B " + strReceived);
                    // Black normal text 
                    UpdateInboundThroughput(bytReceived.Length);
                    // Only compute inbound throughput on good data
                    break;
                case Globals.DataType.SentARQ:
                case Globals.DataType.SentFEC:
                    Globals.queSessionEvents.Enqueue("G " + strReceived);
                    // Green text 
                    break;
                case Globals.DataType.RcvdARQ:
                    Globals.queSessionEvents.Enqueue("Q " + strReceived);
                    // Black Bold text (ARQ) 
                    UpdateInboundThroughput(bytReceived.Length);
                    // Only compute inbound throughput on good data
                    break;
                case Globals.DataType.StrikeThru:
                    Globals.queSessionEvents.Enqueue("R " + strReceived);
                    // Red strike through text
                    break;
                case Globals.DataType.RcvdFECRbst:
                    Globals.queSessionEvents.Enqueue("T " + strReceived);
                    // Navy Bold text (FEC Robust) 
                    UpdateInboundThroughput(bytReceived.Length);
                    // Only compute inbound throughput on good data
                    break;
            }

        }


        private void Main_LocationChanged(object sender, System.EventArgs e)
        {
        }

        private void Main_Resize(object sender, System.EventArgs e)
        {
            if (this.WindowState != FormWindowState.Minimized)
            {
                if (this.Width < 480)
                    this.Width = 480;
                if (this.Height < 200)
                    this.Height = 200;
            }

        }


        private void DeleteToolStripMenuItem_Click(System.Object sender, System.EventArgs e)
        {
            try
            {
                OpenFileDialog dlgFile = new OpenFileDialog();
                dlgFile.Multiselect = true;
                dlgFile.InitialDirectory = Globals.strUserFilesDirectory;
                dlgFile.Title = "Select a File(s) to Delete...";
                dlgFile.Filter = "Text File(.txt)|*.txt";
                dlgFile.RestoreDirectory = true;
                if (dlgFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    for (int i = 0; i <= dlgFile.FileNames.Length - 1; i++)
                    {
                        try
                        {
                            if (File.Exists(dlgFile.FileName))
                            {
                                if (Interaction.MsgBox("Confirm Delete of file " + dlgFile.FileNames[i], MsgBoxStyle.OkCancel) == MsgBoxResult.Ok)
                                {
                                    File.Delete(dlgFile.FileNames[i]);
                                }
                            }
                            else
                            {
                                Interaction.MsgBox("File " + dlgFile.FileName + " does not exist!");
                                return;
                            }
                        }
                        catch
                        {
                            //Interaction.MsgBox(Err.Description, MsgBoxStyle.Information);
                        }
                    }
                }
            }
            catch
            {
                //Globals.Exceptions("[Main.mnuUserFiles.Click] " + Err.Description);
            }
        }


        private void SendToolStripMenuItem_Click(System.Object sender, System.EventArgs e)
        {
            try
            {
                OpenFileDialog dlgFile = new OpenFileDialog();
                dlgFile.Multiselect = false;
                dlgFile.CheckFileExists = true;
                dlgFile.Title = "Select a File to Paste to Kbd Text and send ...";
                dlgFile.InitialDirectory = Globals.strUserFilesDirectory;
                dlgFile.Filter = "Text File(.txt)|*.txt";
                dlgFile.RestoreDirectory = true;
                if (dlgFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string strFileData = File.ReadAllText(dlgFile.FileName);
                    rtbSend.Text = rtbSend.Text + strFileData;
                    // Trim of FileData removed rev 0.1.7.2
                    // This positions the cursor at the end of the added text from file.
                    rtbSend.AppendText(" ");
                    rtbSend.Undo();
                    Globals.strUserFilesDirectory = dlgFile.FileName.Substring(0, 1 + dlgFile.FileName.LastIndexOf("\\"));
                    btnQueueData.PerformClick();
                }
                rtbSend.Focus();
            }
            catch (Exception ex)
            {
                //Globals.Exceptions("[Main.mnuPaste.Click] Err: " + Err.Number.ToString() + " Exception: " + ex.ToString());
            }
        }

        private void OpenCreateToolStripMenuItem_Click(System.Object sender, System.EventArgs e)
        {
            try
            {
                OpenFileDialog dlgFile = new OpenFileDialog();
                string strFileName = "";
                dlgFile.Multiselect = false;
                dlgFile.Title = "Enter file to Creat...";
                dlgFile.InitialDirectory = Globals.strUserFilesDirectory;
                dlgFile.Filter = "Text File(.txt)|*.txt";
                dlgFile.CheckFileExists = false;
                dlgFile.RestoreDirectory = true;
                if (dlgFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    if (File.Exists(dlgFile.FileName))
                    {
                        Interaction.MsgBox("File " + dlgFile.FileName + " already exists!");
                        return;
                    }
                    else
                    {
                        File.Create(dlgFile.FileName);
                        strFileName = dlgFile.FileName;
                    }
                    System.DateTime dttStartWait = DateTime.Now;
                    Globals.strUserFilesDirectory = dlgFile.FileName.Substring(0, 1 + dlgFile.FileName.LastIndexOf("\\"));
                }
                //  If strFileName <> "" Then Process.Start(strFileName)
            }
            catch
            {
                //Globals.Exceptions("[Main.mnuUserFiles.Click] " + Err.Description);
            }
        }

        private void AboutToolStripMenuItem_Click(System.Object sender, System.EventArgs e)
        {
            About dlgAbout = new About();
            dlgAbout.Show();
        }




        /*private void grdContacts_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == Windows.Forms.MouseButtons.Right)
            {
                DataGridViewRow objRow = new DataGridViewRow();
                if (grdContacts.SelectedRows.Count > 0)
                {
                    objRow = grdContacts.SelectedRows(0);
                }
                else
                {
                    Interaction.MsgBox("No grid row selected!" + Constants.vbCrLf + "Left click to select a row, right click to add/edit.", MsgBoxStyle.Information, "No grid row selected!");
                    return;
                }


                if ((objRow.Cells(0).Value == null) & intFrequency == 0)
                {
                    //strContact = FormatDate(Date.UtcNow) & "|||||"
                    strContact = FormatDate(System.DateTime.UtcNow) + "|" + strConnectedCallSign + "|||";

                }
                else if ((objRow.Cells(0).Value == null) & intFrequency > 0)
                {
                    strContact = FormatDate(System.DateTime.UtcNow) + "|" + strConnectedCallSign + "|" + Strings.Format((intFrequency) / 1000, "##0000.000") + "||";
                }
                else
                {
                    strContact = objRow.Cells(0).Value.ToString + "|";
                    for (int i = 1; i <= objRow.Cells.Count - 1; i++)
                    {
                        if ((objRow.Cells(i).Value == null))
                        {
                            strContact += "|";
                        }
                        else
                        {
                            strContact += objRow.Cells(i).Value.ToString + "|";
                        }
                    }
                }

                Contact dlgContact = new Contact();
                dlgContact.ShowDialog();
                if (dlgContact.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    if (strContact.StartsWith("X"))
                    {
                        if (grdContacts.SelectedRows.Count == 1 && grdContacts.SelectedRows(0).Index < grdContacts.RowCount - 1)
                        {
                            grdContacts.Rows.RemoveAt(grdContacts.SelectedRows(0).Index);
                            grdContacts.Refresh();
                        }
                        // this edits the row by a remove and add
                    }
                    else if (strContact.StartsWith("E"))
                    {
                        if (grdContacts.SelectedRows.Count == 1 && grdContacts.SelectedRows(0).Index < grdContacts.RowCount - 1)
                        {
                            grdContacts.Rows.RemoveAt(grdContacts.SelectedRows(0).Index);
                        }
                        string[] strTokens = strContact.Split('|');
                        objRow = new DataGridViewRow();
                        objRow.CreateCells(grdContacts);
                        objRow.Cells(0).Value = strTokens(0).Substring(1);
                        objRow.Cells(1).Value = strTokens(1);
                        objRow.Cells(2).Value = strTokens(2);
                        objRow.Cells(3).Value = strTokens(3);
                        objRow.Cells(4).Value = strTokens(4);
                        grdContacts.Rows.Add(objRow);
                        grdContacts.Refresh();
                        // this adds the row
                    }
                    else
                    {
                        string[] strTokens = strContact.Split('|');
                        objRow = new DataGridViewRow();
                        objRow.CreateCells(grdContacts);
                        objRow.Cells(0).Value = strTokens(0);
                        objRow.Cells(1).Value = strTokens(1);
                        objRow.Cells(2).Value = strTokens(2);
                        objRow.Cells(3).Value = strTokens(3);
                        objRow.Cells(4).Value = strTokens(4);
                        grdContacts.Rows.Add(objRow);
                        grdContacts.Refresh();
                    }

                }
            }
        }*/

        private void splHorizontal_SplitterMoved(object sender, System.Windows.Forms.SplitterEventArgs e)
        {
            Debug.WriteLine("Horz Splitter Distance = " + splHorizontal.SplitterDistance.ToString());
            Debug.WriteLine("RTB Session Height:" + rtbSession.Height.ToString() + "  Preferred Height: " + rtbSession.PreferredHeight.ToString());
            int intLines = Convert.ToInt32(rtbSession.Height / rtbSession.PreferredHeight);
            Debug.WriteLine("RTB Session Lines:" + intLines.ToString());
            //rtbSession.Height = intLines * rtbSession.PreferredHeight
        }

        private void ToolStripLabel1_Click(System.Object sender, System.EventArgs e)
        {
            if (!Globals.blnAborting)
            {
                Globals.queSessionEvents.Enqueue("F " + Constants.vbCrLf + "*** Manual Abort Request" + Constants.vbCrLf);
                Globals.blnAborting = true;
            }
        }

        private void tmrStartup_Tick(object sender, System.EventArgs e)
        {
            tmrStartup.Stop();
            tmrPoll.Stop();
            //objTNCTCPIPPort = Nothing
            //objTNCTCPIPPort = New nsoftware.IPWorks.Ipport
            Globals.blnMorseId = Convert.ToBoolean(Globals.objINIFile.GetString("ARDOP Chat", "MorseID", "True"));
            Globals.strCaptureDevice = Globals.objINIFile.GetString("ARDOP Chat", "CaptureDevice", "");
            Globals.strPlaybackDevice = Globals.objINIFile.GetString("ARDOP Chat", "PlaybackDevice", "");
            Globals.intTCPIPPort = Globals.objINIFile.GetInteger("ARDOP Chat", "TCPIPPort", 8515);
            Globals.intFontSize = Globals.objINIFile.GetInteger("ARDOP Chat", "TextBoxFontSize", 10);
            //grdContacts.Font = new System.Drawing.Font("Microsoft Sans Serif", intFontSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, Convert.ToByte(0));
            rtbSend.Font = new System.Drawing.Font("Microsoft Sans Serif", Globals.intFontSize, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, Convert.ToByte(0));
            Globals.intARQTimeout = Globals.objINIFile.GetInteger("ARDOP Chat", "ARQTimeout", 90);
            Globals.intH4DriveLevel = Globals.objINIFile.GetInteger("ARDOP Chat", "DriveLevel", 100);
            Globals.intTuning = Globals.objINIFile.GetInteger("ARDOP Chat", "Tuning", 100);
            Globals.intCallRepeats = Globals.objINIFile.GetInteger("ARDOP Chat", "Call Repeats", 5);
            if (Globals.intTuning > 200)
                Globals.intTuning = 200;
            // (to accomodate any older ini files with tuning values over 200) 
            Globals.strTCPIPAddress = Globals.objINIFile.GetString("ARDOP Chat", "TCPIPAddress", "127.0.0.1");
            Globals.strCharacterSet = Globals.objINIFile.GetString("ARDOP Chat", "CharacterSet", "ASCII(7BIT)");
            Globals.strUserFilesDirectory = Globals.objINIFile.GetString("ARDOP Chat", "UserFileDirectory", Globals.strExecutionDirectory + "UserFiles\\");
            if (!Directory.Exists(Globals.strUserFilesDirectory))
                Directory.CreateDirectory(Globals.strUserFilesDirectory);
            Globals.blnAutoID = Convert.ToBoolean(Globals.objINIFile.GetString("ARDOP Chat", "AutoID", "False"));
            Globals.strMyCallsign = Globals.objINIFile.GetString("ARDOP Chat", "MyCallSign", "");
            Globals.strMyARQID = Globals.objINIFile.GetString("ARDOP Chat", "MyARQID", Globals.strMyCallsign);
            Globals.strCQText = Globals.objINIFile.GetString("ARDOP Chat", "CQ Text", "CQ ARQ");
            Globals.blnSendCR = Convert.ToBoolean(Globals.objINIFile.GetString("ARDOP Chat", "SendCR", "False"));
            Globals.blnSendSpace = Convert.ToBoolean(Globals.objINIFile.GetString("ARDOP Chat", "SendSpace", "False"));
            Globals.blnSendWord = Convert.ToBoolean(Globals.objINIFile.GetString("ARDOP Chat", "SendWord", "False"));
            Globals.blnSendCtrlCr = Convert.ToBoolean(Globals.objINIFile.GetString("ARDOP Chat", "SendCtrlCR", "False"));
            Globals.blnEnableDebugLogs = Convert.ToBoolean(Globals.objINIFile.GetString("ARDOP Chat", "EnableDebugLogs", "True"));
            Globals.blnEnableAutoupdate = Convert.ToBoolean(Globals.objINIFile.GetString("ARDOP Chat", "EnableAutoUpdate", "True"));
            Globals.blnAutoUpdateTest = Convert.ToBoolean(Globals.objINIFile.GetString("Main", "Test Autoupdate", "False"));
            Globals.blnEnableBeacon = Convert.ToBoolean(Globals.objINIFile.GetString("Main", "Enable Beacon", "False"));
            Globals.strFECBeaconText = Globals.objINIFile.GetString("Main", "FEC Beacon Text", "DE " + Globals.strMyCallsign);
            Globals.strARQBeaconText = Globals.objINIFile.GetString("Main", "ARQ Beacon Text", "H4 ARQ <" + Globals.strMyCallsign);
            Globals.intBeaconInterval = Globals.objINIFile.GetInteger("Main", "Beacon Interval", 20);
            if (Globals.blnEnableBeacon)
                Globals.dttNextBeacon = DateTime.Now.AddMinutes(Globals.intBeaconInterval);
            Globals.intFrontPorch = Globals.objINIFile.GetInteger("ARDOP Chat", "FrontPorch", 125);
            Globals.intBackPorch = Globals.objINIFile.GetInteger("ARDOP Chat", "BackPorch", 0);
            Globals.strMode = Globals.objINIFile.GetString("Main", "Startup Mode", "FEC");
            switch (Globals.strMode)
            {
                case "FEC":
                    mnuMode.Text = "Mode: FEC";
                    break;
                case "FEC FD":
                    mnuMode.Text = "Mode: FEC FD";
                    break;
                case "ARQ":
                    mnuMode.Text = "Mode: ARQ";
                    mnuCall.Enabled = true;
                    break;
                case "FEC RBST":
                    mnuMode.Text = "Mode: FEC RBST";
                    break;
            }
            this.rtbSend.Enabled = true;
            intHorizSplitDst = Globals.objINIFile.GetInteger("ARDOP Chat", "Horiz Splitter", 200);
            //intVertSplitDst = objINIFile.GetInteger("ARDOP Chat", "Vert Splitter", 200);
            // This insures window placement is on screen independent of .ini values
            System.Windows.Forms.Screen[] screen = System.Windows.Forms.Screen.AllScreens;
            bool blnLocOK = false;
            Int32 intTop = default(Int32);
            Int32 intLeft = default(Int32);
            Int32 intWidth = default(Int32);
            Int32 intHeight = default(Int32);
            // Set inital window position and size...
            intTop = Globals.objINIFile.GetInteger("ARDOP Chat", "Top", 100);
            intLeft = Globals.objINIFile.GetInteger("ARDOP Chat", "Left", 100);
            intWidth = Globals.objINIFile.GetInteger("ARDOP Chat", "Width", 680);
            intHeight = Globals.objINIFile.GetInteger("ARDOP Chat", "Height", 360);
            for (int i = 0; i <= screen.Length - 1; i++)
            {
                if (screen[i].Bounds.Top <= intTop & screen[i].Bounds.Bottom >= (intTop + intHeight) & screen[i].Bounds.Left <= intLeft & screen[i].Bounds.Right >= (intLeft + intWidth))
                {
                    // Position window in its last location only if it is within the bounds of the screen
                    this.Top = intTop;
                    this.Left = intLeft;
                    this.Width = intWidth;
                    this.Height = intHeight;
                    blnLocOK = true;
                    break; // TODO: might not be correct. Was : Exit For
                }
            }
            if (!blnLocOK)
            {
                // Det default window position, Height and Width
                this.Top = 100;
                this.Left = 100;
                this.Width = 680;
                this.Height = 360;
            }

            string strBaseCallsign = Globals.GetBaseCallsign(Globals.strMyCallsign);
            //InitializeContactGrid();
            this.Text = "ARDOP Chat " + Application.ProductVersion;
            Globals.queSessionEvents.Enqueue("P " + Constants.vbLf);
            if (Globals.blnEnableAutoupdate)
            {
                Globals.queSessionEvents.Enqueue("P  *** Startup of ARDOP Chat Ver " + Application.ProductVersion + " ; Auto Update Enabled using HTTP port 8776" + Constants.vbLf);
            }
            else
            {
                Globals.queSessionEvents.Enqueue("P  *** Startup of ARDOP Chat Ver " + Application.ProductVersion + " ; Auto Update Disabled" + Constants.vbLf);
            }
            if (Globals.blnAutoUpdateTest)
                Globals.queSessionEvents.Enqueue("P  *** Start Autoupdate Test in approx 20 sec ***" + Constants.vbLf);
            Globals.queSessionEvents.Enqueue("P  " + Constants.vbCrLf + "*** Mode: " + Globals.strMode + Constants.vbCrLf);
            if (!Globals.blnAutoID)
            {
                mnuItemAutoID.Font = new System.Drawing.Font("Microsoft Sans Serif", 11, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, Convert.ToByte(0));
                mnuItemAutoID.Text = "Auto ID is Off";
                Globals.queSessionEvents.Enqueue("P  *** Auto ID is OFF" + Constants.vbCrLf);
            }
            else
            {
                mnuItemAutoID.Font = new System.Drawing.Font("Microsoft Sans Serif", 11, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, Convert.ToByte(0));
                mnuItemAutoID.Text = "Auto ID is On";
                Globals.queSessionEvents.Enqueue("P  *** Auto ID is ON" + Constants.vbCrLf);
            }
            try
            {
                splHorizontal.SplitterDistance = intHorizSplitDst;
                //splVertical.SplitterDistance = intVertSplitDst;
            }
            catch
            {
            }


            Globals.Log("BLANK");
            // Setup auto update use test mode if in Source code directory
            Globals.objAutoupdate = new Autoupdate(Globals.blnAutoUpdateTest, (!Globals.blnEnableAutoupdate));
            Application.DoEvents();
            Globals.Log("*** Startup of ARDOP Chat Ver " + Application.ProductVersion + " *** " + Constants.vbCrLf);
            if ((Globals.objRadio == null))
            {
                Globals.objRadio = new Radio();
                Globals.objRadio.SetFrequency(0);
                // this initializes the radio control ports but will not actually set the freq to 0.
            }
            blnStartingARDOPTNC = true;
            StartARDOPWinTNC();
            if (blnTNCIsOpen)
            {
                Globals.queSessionEvents.Enqueue("P  *** ARDOP Win TNC TCPIP OK on port " + Globals.intTCPIPPort.ToString() + Constants.vbLf);
            }
            blnStartingARDOPTNC = false;

            //queSessionEvents.Enqueue("P " & vbLf)
            Globals.Log("BLANK");
            tmrPoll.Start();
            this.Focus();
        }

        private void rtbSession_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            blnRtbSessionKeyPressed = true;
        }


        private void rtbSession_LostFocus(object sender, System.EventArgs e)
        {
        }


        private void rtbSession_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
        }

        private void rtbSession_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                ContextMenurtbSession.Visible = true;
                ContextMenurtbSession.Show(this.Left + this.Width - ContextMenurtbSession.Width, this.Top + 50);

            }
        }

        private void rtbSession_SizeChanged(object sender, System.EventArgs e)
        {
            //Dim intLines As Integer = CInt(rtbSend.Height / rtbSend.PreferredHeight)
            //rtbSession.Height = intLines * rtbSend.PreferredHeight
        }


        private void rtbSession_TextChanged(System.Object sender, System.EventArgs e)
        {
            if (blnClearingTextBoxes)
                return;
            if (blnRtbSessionKeyPressed)
            {
                rtbSession.Undo();
                blnRtbSessionKeyPressed = false;
            }

        }

        private void mnuUserFiles_Click(System.Object sender, System.EventArgs e)
        {
            if (Globals.strMode == "ARQ" & !Globals.blnARQConnected)
            {
                mnuPasteAndSend.Enabled = false;
            }
            else
            {
                mnuPasteAndSend.Enabled = true;
            }
        }

        private void RestartTNC()
        {
            blnRestartTNC = false;
            CloseTNC();
            Thread.Sleep(2000);
            StartARDOPWinTNC();
        }

        private void EditToolStripMenuItem_Click(System.Object sender, System.EventArgs e)
        {
            try
            {
                OpenFileDialog dlgFile = new OpenFileDialog();
                dlgFile.Multiselect = false;
                dlgFile.Title = "Enter file to Edit ...";
                dlgFile.InitialDirectory = Globals.strUserFilesDirectory;
                dlgFile.Filter = "Text File(.txt)|*.txt";
                dlgFile.CheckFileExists = false;
                dlgFile.RestoreDirectory = true;
                if (dlgFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    if (!File.Exists(dlgFile.FileName))
                    {
                        Interaction.MsgBox("File " + dlgFile.FileName + " does not exist!");
                        return;
                    }
                    else
                    {
                        Process.Start(dlgFile.FileName);
                    }
                    Globals.strUserFilesDirectory = dlgFile.FileName.Substring(0, 1 + dlgFile.FileName.LastIndexOf("\\"));
                }
            }
            catch
            {
                //Globals.Exceptions("[Main.mnuUserFiles.Click] " + Err.Description);
            }
        }

        private void Main_ResizeEnd(object sender, System.EventArgs e)
        {
            dttScrollLocked = DateTime.Now.AddSeconds(-30);
        }


        private void Main_VisibleChanged(object sender, System.EventArgs e)
        {
        }

        private void mnuMode_Click(System.Object sender, System.EventArgs e)
        {
            FECToolStripMenuItem.Text = "FEC";
        }

        private void ToolStripMenuItem1_Click(System.Object sender, System.EventArgs e)
        {
            try
            {
                OpenFileDialog dlgFile = new OpenFileDialog();
                dlgFile.Multiselect = false;
                dlgFile.CheckFileExists = true;
                dlgFile.Title = "Select a File to Paste to Kbd Text ...";
                dlgFile.InitialDirectory = Globals.strUserFilesDirectory;
                dlgFile.Filter = "Text File(.txt)|*.txt";
                dlgFile.RestoreDirectory = true;
                if (dlgFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string strFileData = File.ReadAllText(dlgFile.FileName);
                    rtbSend.Text = rtbSend.Text + strFileData.Trim();
                    // This positions the cursor at the end of the added text from file.
                    rtbSend.AppendText(" ");
                    rtbSend.Undo();
                    Globals.strUserFilesDirectory = dlgFile.FileName.Substring(0, 1 + dlgFile.FileName.LastIndexOf("\\"));
                }
                rtbSend.Focus();
            }
            catch (Exception ex)
            {
                //Globals.Exceptions("[Main.mnuPaste.Click] Err: " + Err.Number.ToString + " Exception: " + ex.ToString);
            }
        }



        private void btnEnd_Click(System.Object sender, System.EventArgs e)
        {
            SendCommandToTNC("BUFFER 0");
            // clear the buffer
            SendCommandToTNC("ARQEND");
            Globals.
                queSessionEvents.Enqueue("P " + Constants.vbCrLf + "*** Disconnect request " + Constants.vbCrLf);

        }

        private void btnQueueData_Click(System.Object sender, System.EventArgs e)
        {
            if (Globals.strMode == "ARQ" & !Globals.blnARQConnected)
            {
                Interaction.MsgBox("Text may not be sent in ARQ mode until CONNECTED!", MsgBoxStyle.Information, "No ARQ Connection");
                return;
            }
            string strTextToSend = null;
            strTextToSend = rtbSend.Text.Substring(intRtbSendPtr);
            intRtbSendPtr = rtbSend.Text.Length;

            if (strTextToSend.Length > 0)
            {
                objTCPData.DataToSend = strTextToSend;
                if (Globals.strMode.IndexOf("FEC") != -1 & DateTime.Now.Subtract(dttLastFECSend).TotalSeconds > 120)
                {
                    LogADIFQSO("", System.DateTime.UtcNow, System.DateTime.UtcNow.AddHours(-1), "", "", "H4", intFrequency, "", strRemoteGS, "Transmit H4 FEC");
                    dttLastFECSend = System.DateTime.UtcNow;
                }
            }
            RepaintRTBSend(ref intRtbSendPtr);
        }

        private void RepaintRTBSend(ref int intPtr)
        {
            string strDataToDisplay = rtbSend.Text;
            blnClearingTextBoxes = true;
            rtbSend.Clear();
            rtbSend.Select(0, 0);
            rtbSend.SelectionColor = Color.DimGray;
            rtbSend.AppendText(strDataToDisplay);
            rtbSend.Select(intPtr, 0);
            rtbSend.SelectionColor = Color.Navy;
            blnClearingTextBoxes = false;

        }

        private void rtbSession_VScroll(object sender, System.EventArgs e)
        {
            if (!blnIgnoreManualSessionScroll)
            {
                dttScrollLocked = DateTime.Now;
                blnScrollLocked = true;
                btnScrollLock.Visible = true;
                rtbSession.SuspendLayout();
            }
        }

        /*private void splVertical_SplitterMoved(object sender, System.Windows.Forms.SplitterEventArgs e)
        {
            Debug.WriteLine("Vert Splitter Distance = " + splVertical.SplitterDistance.ToString);
            //If splVertical.SplitterDistance > 480 Then splVertical.SplitterDistance = 480
            //If splVertical.SplitterDistance > grdContacts.Width + 20 Then splVertical.SplitterDistance = grdContacts.Width + 20
        }*/

        private void rtbSend_GotFocus(object sender, System.EventArgs e)
        {
            blnRTBSendHasFocus = true;
            if (blnScrollLocked)
            {
                btnScrollLock.PerformClick();
                blnScrollLocked = false;
            }
        }


        private void rtbSend_KeyDown1(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ControlKey)
            {
                blnCtrlKey = true;
            }
        }


        private void rtbSend_KeyPress1(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            strLastSendKey = Convert.ToString(e.KeyChar);
            if (((strLastSendKey == Constants.vbLf) & blnCtrlKey & Globals.blnSendCtrlCr) | ((strLastSendKey == Constants.vbCr) & (Globals.blnSendCR | Globals.blnSendSpace)))
            {
                if (Globals.strMode == "ARQ" & !Globals.blnARQConnected)
                {
                    Interaction.MsgBox("Text may not be sent in ARQ mode until CONNECTED!", MsgBoxStyle.Information, "No ARQ Connection");
                    return;
                }
                string strTextToSend = null;

                strTextToSend = rtbSend.Text.Substring(intRtbSendPtr);

                if (strTextToSend.Length > 0)
                {
                    objTCPData.DataToSend = strTextToSend;
                    Globals.dttNextBeacon = DateTime.Now.AddMinutes(Globals.intBeaconInterval);
                    // hold off any beacons for the beacon interval
                    if (Globals.strMode.IndexOf("FEC") != -1 & DateTime.Now.Subtract(dttLastFECSend).TotalSeconds > 120)
                    {
                        LogADIFQSO("", System.DateTime.UtcNow, System.DateTime.UtcNow.AddHours(-1), "", "", "H4", intFrequency, "", strRemoteGS, "Transmit H4 FEC");
                        dttLastFECSend = System.DateTime.UtcNow;
                    }
                }
                intRtbSendPtr = rtbSend.Text.Length;
                RepaintRTBSend(ref intRtbSendPtr);
            }
            else if (strLastSendKey == " " & rtbSend.Text.EndsWith(" ") & Globals.blnSendSpace)
            {
                if (Globals.strMode == "ARQ" & !Globals.blnARQConnected)
                {
                    Interaction.MsgBox("Text may not be sent in ARQ mode until CONNECTED!", MsgBoxStyle.Information, "No ARQ Connection");
                    return;
                }
                string strTextToSend = rtbSend.Text.Substring(intRtbSendPtr);

                if (strTextToSend.Length > 0)
                {
                    objTCPData.DataToSend = strTextToSend;
                    if (Globals.strMode.IndexOf("FEC") != -1 & DateTime.Now.Subtract(dttLastFECSend).TotalSeconds > 120)
                    {
                        LogADIFQSO("", System.DateTime.UtcNow, System.DateTime.UtcNow.AddHours(-1), "", "", "H4", intFrequency, "", strRemoteGS, "Transmit H4 FEC");
                        dttLastFECSend = System.DateTime.UtcNow;
                    }
                }
                intRtbSendPtr = rtbSend.Text.Length;
                RepaintRTBSend(ref intRtbSendPtr);
            }
            else if ((strLastSendKey == " " | strLastSendKey == Constants.vbCr) & Globals.blnSendWord)
            {
                if (Globals.strMode == "ARQ" & !Globals.blnARQConnected)
                {
                    Interaction.MsgBox("Text may not be sent in ARQ mode until CONNECTED!", MsgBoxStyle.Information, "No ARQ Connection");
                    return;
                }
                string strTextToSend = rtbSend.Text.Substring(intRtbSendPtr);

                if (strTextToSend.Length > 0)
                {
                    objTCPData.DataToSend = strTextToSend;
                    if (Globals.strMode.IndexOf("FEC") != -1 & DateTime.Now.Subtract(dttLastFECSend).TotalSeconds > 120)
                    {
                        LogADIFQSO("", System.DateTime.UtcNow, System.DateTime.UtcNow.AddHours(-1), "", "", "H4", intFrequency, "", strRemoteGS, "Transmit H4 FEC");
                        dttLastFECSend = System.DateTime.UtcNow;
                    }
                }
                intRtbSendPtr = rtbSend.Text.Length;
                RepaintRTBSend(ref intRtbSendPtr);
            }
            else if (rtbSend.Text.Length < intRtbSendPtr)
            {
                intRtbSendPtr += 0;
            }
        }

        private void rtbSend_KeyUp1(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ControlKey)
            {
                blnCtrlKey = false;
            }
        }

        private void rtbSend_LostFocus(object sender, System.EventArgs e)
        {
            blnRTBSendHasFocus = false;
            //Debug.WriteLine("rtbSend_LostFocus")
        }

        private void rtbSend_MouseDown1(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                ContextMenurtbSend.Visible = true;
                ContextMenurtbSend.Show(this.Left + this.Width - ContextMenurtbSend.Width, this.Top + this.Height - ContextMenurtbSend.Height);

                //If MsgBox("Do you want to clear all text in the Keyboard text box?" & vbCr & _
                //          "Prior sent has been logged to " & Application.ProductName & _
                //       " " & Format(Date.UtcNow, "yyyyMMdd") & ".log", MsgBoxStyle.YesNo, "Clear Keyboard Entry Text Box?") = MsgBoxResult.Yes Then
                //    blnClearingTextBoxes = True
                //    rtbSend.Clear()
                //    blnClearingTextBoxes = False
                //    intRtbSendPtr = 0
                //End If
            }
        }


        private void rtbSend_SizeChanged(object sender, System.EventArgs e)
        {
        }

        private void rtbSend_TextChanged(object sender, System.EventArgs e)
        {
            intNewLineIndex = rtbSend.GetFirstCharIndexOfCurrentLine();
            if (rtbSend.Text.Length < intRtbSendPtr & !blnClearingTextBoxes)
            {
                rtbSend.Undo();
            }
        }

        private void btnScrollLock_Click(System.Object sender, System.EventArgs e)
        {
            bool blnRTBSendHadFocus = blnRTBSendHasFocus;
            foreach (SessionUpdates Update in cllPendingUpdates)
            {
                Debug.WriteLine("[btnScrollLock.Click] Update added");
                rtbSession.SelectionColor = Update.Color;
                rtbSession.SelectionFont = Update.Font;
                rtbSession.SelectionStart = rtbSession.Text.Length;
                rtbSession.AppendText(Update.Text);
                rtbSession.SelectionLength = Update.Text.Length;
                rtbSession.Select();
            }
            cllPendingUpdates.Clear();
            Debug.WriteLine("[btnScrollLock.Click] Scroll to Caret");
            rtbSession.ScrollToCaret();
            blnScrollLocked = false;
            btnScrollLock.Visible = false;
            if (blnRTBSendHadFocus)
            {
                rtbSend.Focus();
            }
        }

        private void ExitToolStripMenuItem1_Click(System.Object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void SetupToolStripMenuItem_Click(System.Object sender, System.EventArgs e)
        {
            ChatSetupForm dlgSetup = new ChatSetupForm();
            if (dlgSetup.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Globals.objINIFile.WriteInteger("ARDOP Chat", "Horiz Splitter", Convert.ToInt32(100 * rtbSession.Height / (rtbSession.Height + rtbSend.Height)));
                Globals.objINIFile.WriteInteger("ARDOP Chat", "Top", this.Top);
                Globals.objINIFile.WriteInteger("ARDOP Chat", "Left", this.Left);
                Globals.objINIFile.WriteInteger("ARDOP Chat", "Width", this.Width);
                Globals.objINIFile.WriteInteger("ARDOP Chat", "Height", this.Height);
                Globals.objINIFile.Flush();
                Globals.queSessionEvents.Enqueue("A  *** ARDOP Win TNC restarted with updated parameters." + Constants.vbCrLf);
                Application.DoEvents();
                tmrPoll.Stop();
                tmrStartup.Start();
                if (mnuMode.Text.IndexOf("FEC") != -1)
                {
                    mnuMode.Text = "Mode: FEC";
                }
                //grdContacts.Font = new System.Drawing.Font("Microsoft Sans Serif", intFontSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, Convert.ToByte(0));
                rtbSend.Font = new System.Drawing.Font("Microsoft Sans Serif", Globals.intFontSize, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, Convert.ToByte(0));
            }
            if (Globals.blnAutoID)
            {
                mnuItemAutoID.Font = new System.Drawing.Font("Microsoft Sans Serif", 11, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, Convert.ToByte(0));
            }
            else
            {
                mnuItemAutoID.Font = new System.Drawing.Font("Microsoft Sans Serif", 11, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, Convert.ToByte(0));
            }
        }

        private void HelpContentsToolStripMenuItem_Click(System.Object sender, System.EventArgs e)
        {
            try
            {
                Help.ShowHelp(this, Globals.strExecutionDirectory + "Chat Help.chm", HelpNavigator.TableOfContents);
            }
            catch
            {
                //Globals.Exception("[Chat Main mnuHelp_Click] " + Err.Description);
            }
        }

        private void ToolStripMenuItem1_Click_1(System.Object sender, System.EventArgs e)
        {
            try
            {
                Help.ShowHelp(this, Globals.strExecutionDirectory + "Chat Help.chm", HelpNavigator.Index);
            }
            catch
            {
                //Globals.Exception("[Chat Main mnuHelp_Click] " + Err.Description);
            }
        }

        private void ToolStripMenuItem2_Click(System.Object sender, System.EventArgs e)
        {
            if (Globals.blnAutoID)
            {
                Globals.blnAutoID = false;
                mnuItemAutoID.Font = new System.Drawing.Font("Microsoft Sans Serif", 11, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, Convert.ToByte(0));
                mnuItemAutoID.Text = "Auto ID is Off";
                Globals.queSessionEvents.Enqueue("P  " + Constants.vbCrLf + "*** Auto ID is OFF" + Constants.vbCrLf);
            }
            else
            {
                Globals.blnAutoID = true;
                mnuItemAutoID.Font = new System.Drawing.Font("Microsoft Sans Serif", 11, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, Convert.ToByte(0));
                mnuItemAutoID.Text = "Auto ID is On";
                Globals.queSessionEvents.Enqueue("P  " + Constants.vbCrLf + "*** Auto ID is ON" + Constants.vbCrLf);
            }
            SendCommandToTNC("AUTOID " + Globals.blnAutoID.ToString());

        }

        // This calculates the 30 second rolling throughput of received data in bytes(characters)/minute
        private void UpdateInboundThroughput(int intCharCnt)
        {
            ThroughputData objInbound = default(ThroughputData);
            objInbound.Arrival = DateTime.Now;
            objInbound.CharCnt = intCharCnt;
            cllInbound.Add(objInbound);
            int intTotal1MinCharacters = 0;
            System.DateTime dttOldest = DateTime.Now;
            int intCntAtOldest = 0;
            int int1MinInboundChar = 0;
            int intCllPtr = 1;
            if (intCharCnt == 0)
            {
                cllInbound.Clear();
                intThroughput = 0;
                strThruPutDirection = "Rx: ";
                return;
            }
            for (int i = 1; i <= cllInbound.Count; i++)
            {
                objInbound = (ThroughputData)cllInbound.Item(intCllPtr);
                if (DateTime.Now.Subtract(objInbound.Arrival).TotalSeconds > 30)
                {
                    // Purge this old data from the collection
                    cllInbound.Remove(intCllPtr);
                    // Don't increment the pointer...it now points to the next entry
                }
                else if (dttOldest.Subtract(objInbound.Arrival).TotalSeconds > 0)
                {
                    intCllPtr += 1;
                    dttOldest = objInbound.Arrival;
                    intCntAtOldest = objInbound.CharCnt;
                    intTotal1MinCharacters += objInbound.CharCnt;
                }
                else
                {
                    intCllPtr += 1;
                    intTotal1MinCharacters += objInbound.CharCnt;
                }
            }
            if (cllInbound.Count == 1)
            {
                // Estimate the average of 4 seconds/frame
                objInbound = (ThroughputData)cllInbound.Item(1);
                //intThroughput = 15 * objInbound.CharCnt
                intThroughput = 0;
            }
            else
            {
                if (DateTime.Now.Subtract(dttOldest).TotalSeconds < 0.5)
                    return;
                // require a minimum of .5 sec data to compute throughput
                intThroughput = Convert.ToInt32((intTotal1MinCharacters - intCntAtOldest) * 60 / (DateTime.Now.Subtract(dttOldest).TotalSeconds));
            }
            strThruPutDirection = "Rx: ";
            dttLastRxSpeedUpdate = DateTime.Now;
        }

        // This calculates the 1 minute rolling throughput of transmitted data in characters/minute
        private void UpdateOutboundThroughputARQ(int intCharCnt)
        {
            ThroughputData objOutbound = default(ThroughputData);
            objOutbound.Arrival = DateTime.Now;
            objOutbound.CharCnt = intCharCnt;
            cllOutbound.Add(objOutbound);
            int intTotal1MinCharacters = 0;
            System.DateTime dttOldest = DateTime.Now;
            int intCntAtOldest = 0;
            int int1MinInboundChar = 0;
            int intCllPtr = 1;

            for (int i = 1; i <= cllOutbound.Count; i++)
            {
                objOutbound = (ThroughputData)cllOutbound.Item(intCllPtr);
                if (DateTime.Now.Subtract(objOutbound.Arrival).TotalSeconds > 60)
                {
                    // Purge this old data from the collection
                    cllOutbound.Remove(intCllPtr);
                    // Don't increment the pointer...it now points to the next entry
                }
                else if (dttOldest.Subtract(objOutbound.Arrival).TotalSeconds > 0)
                {
                    // This entry is older
                    intCllPtr += 1;
                    dttOldest = objOutbound.Arrival;
                    intCntAtOldest = objOutbound.CharCnt;
                    // capture this so we can subtract it at the end
                    intTotal1MinCharacters += objOutbound.CharCnt;
                }
                else
                {
                    intCllPtr += 1;
                    intTotal1MinCharacters += objOutbound.CharCnt;
                }
            }
            if (cllOutbound.Count == 1)
            {
                // Estimate the average of 4 seconds/frame
                objOutbound = (ThroughputData)cllOutbound.Item(1);
                intThroughput = 15 * objOutbound.CharCnt;
            }
            else
            {
                if (DateTime.Now.Subtract(dttOldest).TotalSeconds < 2)
                    return;
                intThroughput = Convert.ToInt32((intTotal1MinCharacters - intCntAtOldest) * 60 / (Now.Subtract(dttOldest).TotalSeconds));
            }
            strThruPutDirection = "Tx: ";
        }

        private void UpdateOutboundThroughputFEC()
        {
            strThruPutDirection = "Tx: ";
            intThroughput = 410;
            // Outbound FEC throughput is constant at 7 characters per 1024 ms.
        }
        private void mnuRtbSendClear_Click(object sender, System.EventArgs e)
        {
            if (Interaction.MsgBox("Do you want to clear all text in the Keyboard text box?", MsgBoxStyle.YesNo, "Clear Keyboard Entry Text Box?") == MsgBoxResult.Yes)
            {
                blnClearingTextBoxes = true;
                rtbSend.Clear();
                blnClearingTextBoxes = false;
                intRtbSendPtr = 0;
            }
        }


        private void mnuRtbSessionClear_Click(object sender, System.EventArgs e)
        {
            if (Interaction.MsgBox("Do you want to clear all text in the Session text box?" + Constants.vbCr + "Prior Session text has been logged to Chat <date>.log", MsgBoxStyle.YesNo, "Clear Session Text Box?") == MsgBoxResult.Yes)
            {
                blnClearingTextBoxes = true;
                rtbSession.Clear();
                blnClearingTextBoxes = false;
            }
        }

        private void mnuRtbSendCopy_Click(object sender, System.EventArgs e)
        {
            if (!string.IsNullOrEmpty(rtbSend.SelectedText))
                Clipboard.SetText(rtbSend.SelectedText);
        }

        private void mnuRtbSessionCopy_Click(object sender, System.EventArgs e)
        {
            if (!string.IsNullOrEmpty(rtbSession.SelectedText))
                Clipboard.SetText(rtbSession.SelectedText);
        }

        private void mnuRtbSendPaste_Click(object sender, System.EventArgs e)
        {
            if (Clipboard.ContainsText())
                rtbSend.Paste();
        }

        private void mnuRtbSendCut_Click(object sender, System.EventArgs e)
        {
            if (!string.IsNullOrEmpty(rtbSend.SelectedText))
                Clipboard.SetText(rtbSend.SelectedText);
            rtbSend.SelectedText = "";
        }

        private void mnuRtbSessionCut_Click(System.Object sender, System.EventArgs e)
        {
            if (!string.IsNullOrEmpty(rtbSession.SelectedText))
                Clipboard.SetText(rtbSession.SelectedText);
            rtbSession.SelectedText = "";
            Interaction.MsgBox("Cut Session text was logged to Chat <date>.log", MsgBoxStyle.Information);
        }


        /*private void grdContacts_CellContentClick(System.Object sender, System.Windows.Forms.DataGridViewCellEventArgs e)
        {
            if (blnInitializingGrid)
                return;
            if (!Information.IsNumeric(e.RowIndex))
                return;
            Debug.WriteLine("grdContacts_CellContentClick; Row Index: " + e.RowIndex.ToString);
            DataGridViewRow objRow = new DataGridViewRow();

            try
            {
                objRow = grdContacts.Rows(e.RowIndex);
                if ((objRow.Cells(1).Value != null) && IsValidCallsign(objRow.Cells(1).Value.ToString.Trim.ToUpper))
                {
                    strLastCallsignCalled = objRow.Cells(1).Value.ToString.Trim.ToUpper;
                }
                if ((objRow.Cells(2).Value == null))
                    return;
                if (!Information.IsNumeric(objRow.Cells(2).Value))
                    return;
                intFrequency = Convert.ToInt32(1000 * Convert.ToDouble(objRow.Cells(2).Value));
                if (intFrequency < 1800000 | intFrequency > 148000000 | blnARQConnected)
                    return;
                // don't allow frequency change when connected.
                if ((objRadio != null))
                {
                    queSessionEvents.Enqueue("A  *** Freq Changed: " + Strings.Format(intFrequency / 1000, "##0000.000") + " KHz Dial" + Constants.vbCrLf);
                    objRadio.SetFrequency(intFrequency);
                    SendCommandToTNC("DISPLAY " + "USB Dial:" + Strings.Format((intFrequency) / 1000, "##0000.000"));
                }

            }
            catch (Exception ex)
            {
                Exceptions("grdContacts_CellContentClick] Err: " + ex.ToString);
            }
        }*/

        public void LogADIFQSO(string strCall, System.DateTime dttStartUTCDate, System.DateTime dttEndUTCDate, string strName, string strQTH, string strMode, int intFreqHz, string strAVGSN, string strRemoteGS, string strComment = "")
        {
            string strLine = "<station_callsign:" + Globals.strMyCallsign.Length.ToString() + ">" + Globals.strMyCallsign;
            if (!string.IsNullOrEmpty(strCall))
            {
                strLine += "<call:" + strCall.Length.ToString() + ">" + strCall.ToUpper();
            }
            strLine += "<qso_date:8>" + Strings.Format(dttStartUTCDate, "yyyyMMdd");
            strLine += "<time_on:6>" + Strings.Format(dttStartUTCDate, "HHmmss");
            if (dttStartUTCDate.Subtract(dttEndUTCDate).TotalSeconds < 0)
            {
                strLine += "<time_off:6>" + Strings.Format(dttEndUTCDate, "HHmmss");
            }
            if (!string.IsNullOrEmpty(strName))
            {
                strLine += "<name:" + strName.Length + ">" + strName;
            }
            if (!string.IsNullOrEmpty(strQTH))
            {
                strLine += "<qth:" + strQTH.Length + ">" + strQTH;
            }
            strLine += "<mode:" + strMode.Length.ToString() + ">" + strMode;
            if (intFreqHz >= 1800000 & intFreqHz <= 2000000)
            {
                strLine += "<band:4>160M";
            }
            else if (intFreqHz >= 3500000 & intFreqHz <= 4000000)
            {
                strLine += "<band:3>80M";
            }
            else if (intFreqHz >= 7000000 & intFreqHz <= 7300000)
            {
                strLine += "<band:3>40M";
            }
            else if (intFreqHz >= 10100000 & intFreqHz <= 10150000)
            {
                strLine += "<band:3>30M";
            }
            else if (intFreqHz >= 14000000 & intFreqHz <= 14350000)
            {
                strLine += "<band:3>20M";
            }
            else if (intFreqHz >= 18068000 & intFreqHz <= 18168000)
            {
                strLine += "<band:3>17M";
            }
            else if (intFreqHz >= 21000000 & intFreqHz <= 21450000)
            {
                strLine += "<band:3>15M";
            }
            else if (intFreqHz >= 24890000 & intFreqHz <= 24990000)
            {
                strLine += "<band:3>12M";
            }
            else if (intFreqHz >= 28000000 & intFreqHz <= 29700000)
            {
                strLine += "<band:3>10M";
            }
            else if (intFreqHz >= 50000000 & intFreqHz <= 54000000)
            {
                strLine += "<band:2>6M";
            }
            else if (intFreqHz >= 144000000 & intFreqHz <= 148000000)
            {
                strLine += "<band:2>2M";
            }
            else if (intFreqHz >= 420000000 & intFreqHz <= 450000000)
            {
                strLine += "<band:4>70CM";
            }
            if (intFreqHz > 1800000 & intFreqHz < 144000000)
            {
                string strMHz = Strings.Format(intFreqHz / 1000000, "##0.0000");
                strLine += "<freq:" + strMHz.Length + ">" + strMHz;
            }
            else if (intFreqHz >= 144000000)
            {
                string strMHz = Strings.Format(intFreqHz / 1000000, "##0.000");
                strLine += "<freq:" + strMHz.Length + ">" + strMHz;
            }
            if (!string.IsNullOrEmpty(strRemoteGS))
            {
                strLine += "<gridsquare:" + strRemoteGS.Length + ">" + strRemoteGS;
            }
            if (!string.IsNullOrEmpty(strAVGSN))
            {
                strComment = strComment + "; AvgS/N:" + strAVGSN + " dB";
                // add into comment field 
            }

            if (!string.IsNullOrEmpty(strComment))
            {
                strLine += "<comment:" + strComment.Length + ">" + strComment;
            }
            try
            {
                //If Not IO.File.Exists(strLogsDirectory & "Chat_ADIF_" & strMyCallsign & ".adi") Then ' Add header if new file
                //    My.Computer.FileSystem.WriteAllText(strLogsDirectory & "Chat_ADIF_" & strMyCallsign & ".adi", _
                //    "<adif_ver:4>3.00" & "<programid:" & Application.ProductName.Length.ToString & ">" & Application.ProductName & _
                //    "<programversion:" & Application.ProductVersion.Length.ToString & ">" & Application.ProductVersion & "<eoh>" & vbCrLf, True)
                //    ' Add first record showing My call sign and Grid square
                //    My.Computer.FileSystem.WriteAllText(strLogsDirectory & "Chat_ADIF_" & strMyCallsign & ".adi", _
                //  "<my_callsign:" & strMyCallsign.Length.ToString & ">" & strMyCallsign & "my_gridsquare:" & strMyGridSquare.Length.ToString & ">" & strMyGridSquare & "<eor>" & vbCrLf, True)
                //End If
                //My.Computer.FileSystem.WriteAllText(strLogsDirectory & "Chat_ADIF_" & strMyCallsign & ".adi", strLine & "<eor>" & vbCrLf, True)
            }
            catch (Exception ex)
            {
                //Globals.Exceptions("[Main.LogADIFQSO]  Err :" + Err.Number.ToString + "  Exception: " + ex.ToString);
            }

        }

        /*private void GetQSONameQTHFromMiniLog(ref string strName, ref string strQTH, string strCallSign)
        {
            DataGridViewRow objRow = new DataGridViewRow();
            int intRows = grdContacts.RowCount;

            for (int i = 0; i <= intRows - 1; i++)
            {
                objRow = grdContacts.Rows(i);
                if ((objRow.Cells(1).Value != null))
                {
                    if (strCallSign.Trim.ToUpper == objRow.Cells(1).Value.ToString.Trim.ToUpper)
                    {
                        strName = objRow.Cells(3).Value.ToString.Trim;
                        strQTH = objRow.Cells(4).Value.ToString.Trim;
                        return;
                    }
                }
            }
            strName = "";
            strQTH = "";
        }*/

        private void ToolStripMenuItem2_Click_1(System.Object sender, System.EventArgs e)
        {
            BeaconSetup dlgBeaconSetup = new BeaconSetup();
            dlgBeaconSetup.ShowDialog();
        }


        private void mnuFile_Click(System.Object sender, System.EventArgs e)
        {
        }

        private void ARQCQToolStripMenuItem_Click(System.Object sender, System.EventArgs e)
        {
            ARQCall frmCall = new ARQCall();
            Globals.blnARQCalling = false;
            frmCall.ShowDialog();
            if (Globals.blnARQCalling & Globals.blnBusy)
            {
                if (Interaction.MsgBox("The channel appears busy!  Continue with call?", MsgBoxStyle.YesNo, "Channel Busy") == MsgBoxResult.No)
                {
                    Globals.blnARQCalling = false;
                    return;
                }
            }
            if (Globals.blnARQCalling)
            {
                if (SendCommandToTNC("ARQCONNECT " + Globals.strTargetARQCallsign))
                {
                    strQSOCallSign = Globals.strTargetARQCallsign;
                    strADIFComment = "H4 ARQ Call";
                    dttQSOStart = System.DateTime.UtcNow;
                    Globals.queSessionEvents.Enqueue("P " + Constants.vbCrLf + "*** Calling " + Globals.strTargetARQCallsign + " de " + Globals.strMyCallsign + " @ " + Globals.TimestampEx() + Constants.vbCrLf);
                }
            }
        }

        private void ARQCallToolStripMenuItem_Click(System.Object sender, System.EventArgs e)
        {
            ARQCall frmCall = new ARQCall();
            Globals.blnARQCalling = false;
            frmCall.ShowDialog();
            if (Globals.blnARQCalling & Globals.blnBusy)
            {
                if (Interaction.MsgBox("The channel appears busy!  Continue with call?", MsgBoxStyle.YesNo, "Channel Busy") == MsgBoxResult.No)
                {
                    Globals.blnARQCalling = false;
                    Globals.blnARQCQ = false;
                    return;
                }
            }
            if (Globals.blnARQCalling)
            {
                if (SendCommandToTNC("ARQCALL " + Globals.strTargetARQCallsign + " " + Globals.intCallRepeats.ToString()))
                {
                    strQSOCallSign = Globals.strTargetARQCallsign;
                    strADIFComment = "H4 ARQ Call";
                    dttQSOStart = System.DateTime.UtcNow;
                    Globals.queSessionEvents.Enqueue("P " + Constants.vbCrLf + "*** Calling " + Globals.strTargetARQCallsign + "<" + Globals.strMyCallsign + " @ " + Globals.TimestampEx() + Constants.vbCrLf);
                }
            }
            else if (Globals.blnARQCQ)
            {
                if (SendCommandToTNC("ARQCQ " + Globals.strCQText + "< " + Globals.intCallRepeats.ToString()))
                {
                    strQSOCallSign = Globals.strTargetARQCallsign;
                    strADIFComment = "H4 ARQ Call";
                    dttQSOStart = System.DateTime.UtcNow;
                    Globals.queSessionEvents.Enqueue("P " + Constants.vbCrLf + "*** Calling " + Globals.strCQText + "<" + Globals.strMyCallsign + " @ " + Globals.TimestampEx() + Constants.vbCrLf);
                }
            }
        }

        private void H4FECFDToolStripMenuItem_Click(System.Object sender, System.EventArgs e)
        {
            if (Interaction.MsgBox("Normally FEC Full Duplex is used only in testing and for demo work." + Constants.vbCrLf + "Most radio configurations will not support a full duplex connection." + Constants.vbCrLf + "Continue?", MsgBoxStyle.YesNo, "FEC Full Duplex Notice") == Constants.vbNo)
                return;
            SendCommandToTNC("MODE FEC FD");
            mnuMode.Text = "Mode: FEC FD";
            Globals.strMode = "FEC FD";
            Globals.queSessionEvents.Enqueue("P  " + Constants.vbCrLf + "*** Mode: FEC FD" + Constants.vbCrLf);
            mnuCall.Enabled = false;
            this.rtbSend.Enabled = true;
            mnuPasteAndSend.Enabled = true;
            mnuPaste.Enabled = true;
        }

        private void ARQIDToolStripMenuItem_Click(System.Object sender, System.EventArgs e)
        {
            SendCommandToTNC("ARQID " + Globals.strMyARQID);
        }


        private void mnuCall_Click(System.Object sender, System.EventArgs e)
        {
        }

        private void mnuAnswerCQ_Click(object sender, System.EventArgs e)
        {
            if (mnuAnswerCQ.Text.IndexOf("<") != -1)
            {
                Globals.strTargetARQCallsign = mnuAnswerCQ.Text.Substring(1 + mnuAnswerCQ.Text.IndexOf("<")).Trim().ToUpper();
                blnAnsweringCQ = true;
                mnuAnswerCQ.Enabled = false;
            }
        }

        private void ToolStripMenuItem3_Click(System.Object sender, System.EventArgs e)
        {
            SendCommandToTNC("MODE FEC RBST");
            mnuMode.Text = "Mode: FEC RBST";
            Globals.strMode = "FEC RBST";
            Globals.queSessionEvents.Enqueue("P  " + Constants.vbCrLf + "*** Mode: FEC RBST" + Constants.vbCrLf);
            mnuCall.Enabled = false;
            this.rtbSend.Enabled = true;
            mnuPasteAndSend.Enabled = true;
            mnuPaste.Enabled = true;
        }
        public Main()
        {
            VisibleChanged += Main_VisibleChanged;
            ResizeEnd += Main_ResizeEnd;
            Resize += Main_Resize;
            LocationChanged += Main_LocationChanged;
            Load += Main_Load;
            FormClosing += Main_FormClosing;
            FormClosed += Main_FormClosed;
        }
        static bool InitStaticVariableHelper(Microsoft.VisualBasic.CompilerServices.StaticLocalInitFlag flag)
        {
            if (flag.State == 0)
            {
                flag.State = 2;
                return true;
            }
            else if (flag.State == 2)
            {
                throw new Microsoft.VisualBasic.CompilerServices.IncompleteInitialization();
            }
            else
            {
                return false;
            }
        }


    }

    

}