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
        //private bool blnInitializingGrid = false;
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
        // capture rtbSend Foucus status upon entry
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
                            //mnuCall.Enabled = true;
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
                            //mnuCall.Enabled = true;
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
                        //rtbSession.SelectedText = Strings.Chr(0) + Strings.Chr(0);
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

       

        private void StartARDOPWinTNC()
        {
            if (Microsoft.VisualBasic.FileIO.FileSystem.FileExists(Globals.strExecutionDirectory + "ARDOP_Win.exe") == false)
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
                    System.Threading.Thread.Sleep(100);
                    objTNCTCPIPPort.Linger = true;
                    objTNCTCPIPPort.KeepAlive = true;
                    objTNCTCPIPPort.AcceptData = true;
                    objTNCTCPIPPort.RemoteHost = Globals.strTCPIPAddress;
                    objTNCTCPIPPort.RemotePort = Globals.intTCPIPPort;
                    blnRDY = false;
                    blnData = false;
                    objTNCTCPIPPort.Connected = true;
                    System.Threading.Thread.Sleep(1000);
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
            //grdContacts.Font = new System.Drawing.Font("Microsoft Sans Serif", Globals.intFontSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, Convert.ToByte(0));
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
                    //mnuCall.Enabled = true;
                    break;
                case "FEC RBST":
                    mnuMode.Text = "Mode: FEC RBST";
                    break;
            }
            this.rtbSend.Enabled = true;
            intHorizSplitDst = Globals.objINIFile.GetInteger("ARDOP Chat", "Horiz Splitter", 200);
            //intVertSplitDst = Globals.objINIFile.GetInteger("ARDOP Chat", "Vert Splitter", 200);
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
                //mnuItemAutoID.Font = new System.Drawing.Font("Microsoft Sans Serif", 11, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, Convert.ToByte(0));
                //mnuItemAutoID.Text = "Auto ID is Off";
                Globals.queSessionEvents.Enqueue("P  *** Auto ID is OFF" + Constants.vbCrLf);
            }
            else
            {
                //mnuItemAutoID.Font = new System.Drawing.Font("Microsoft Sans Serif", 11, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, Convert.ToByte(0));
                //mnuItemAutoID.Text = "Auto ID is On";
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
                //Globals.objRadio.SetFrequency(0);
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
                //btnScrollLock.Visible = true;
                rtbSession.SuspendLayout();
            }
        }


        private void rtbSend_GotFocus(object sender, System.EventArgs e)
        {
            blnRTBSendHasFocus = true;
            if (blnScrollLocked)
            {
                //btnScrollLock.PerformClick();
                blnScrollLocked = false;
            }
        }

        /*private void rtbSend_KeyDown1(object sender, KeyEventArgs e)
        {
            
            if (e.KeyCode == 17)
            {
                blnCtrlKey = true;
            }
        }


        private void rtbSend_KeyPress1(object sender, KeyPressEventArgs e)
        {
            
            strLastSendKey = e.KeyChar;
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

        private void rtbSend_KeyUp1(object sender, KeyEventArgs e)
        {
            
            if (e.KeyCode == 17)
            {
                blnCtrlKey = false;
            }
        }*/

        
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


        public Main()
        {
            InitializeComponent();
        }
    }
}