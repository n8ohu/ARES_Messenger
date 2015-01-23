using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.IO;
using System.IO.Ports;
using System.Threading;
using System.Windows.Forms;
using nsoftware.IPWorks;

namespace ARES_Messenger
{
    public static class Globals
    {


        public struct Channel
        {
            public string Callsign;
            public int Frequency;
            public string Mode;
            public string GridSquare;
            public int Range;
            public int Bearing;
            public int PathQuality;
        }

        // Booleans...
        public static bool blnAutoupdateRestart;
        public static bool blnAutoupdateInProgress;
        public static bool blnAutoUpdateInProgressLogged = false;
        public static bool blnEnableAutoupdate;
        public static bool blnAutoUpdateTest;
        public static bool blnClosing;
        public static bool blnDownloadingUpdates;
        public static bool blnEnableDebugLogs;
        public static bool blnInVSDevelopment;
        public static bool blnMorseId;
        public static bool blnBusy;
        public static bool blnProgramClosing;
        public static bool blnRefreshRequest;
        public static bool blnRestarting;
        public static bool blnShowTNC;
        public static bool blnAutoID;
        public static bool blnAborting;
        public static bool blnARQConnected;
        public static bool blnARQCalling;
        public static bool blnARQCQ;
        public static bool blnSendCR;
        public static bool blnSendCtrlCr;
        public static bool blnSendSpace;
        public static bool blnSendWord;
        public static bool blnAbortAU;

        public static bool blnEnableBeacon;

        // DateTime...
        public static System.DateTime dttChannelsUpdated;

        public static System.DateTime dttNextBeacon;

        // Integers...
        public static int intLogsTTLWeeks = 2;
        public static int intSessionHeight;
        public static int intSessionLeft;
        public static int intSessionTop;
        public static int intSessionWidth;
        public static int intTCPIPPort;
        public static int intARQTimeout;
        public static int intDataPort;
        public static int intH4DriveLevel;
        public static int intTuning;
        public static int intFontSize = 10;
        public static int intBeaconInterval;
        public static int intFrontPorch = 125;
        public static int intBackPorch = 0;

        public static int intCallRepeats;
        // Objects...
        public static Autoupdate objAutoupdate;
        public static WL2KServers objWL2KServers = new WL2KServers();
        //public static INIFile objINIFile;
        public static object objINIFileLock = new object();
        public static object objLogLock = new object();
        public static Main objMain;

        public static Radio objRadio;
        // Queues...

        public static Queue queSessionEvents = Queue.Synchronized(new Queue());

        // Strings...
        public static string strExecutionDirectory;
        public static string strLogsDirectory;
        public static string strDataDirectory;
        public static string strUserFilesDirectory;
        public static string strMyCallsign;
        public static string strMyCallsignSuffix;
        public static string strMyARQID;
        public static string strCaptureDevice;
        public static string strPlaybackDevice;
        public static string strTCPIPAddress = "127.0.0.1";
        public static string strCharacterSet;
        public static string strMode;
        public static string strContact;
        public static string strTargetARQCallsign;
        public static string strLastCallsignCalled;
        public static string strNewAUVersion;
        public static string strFECBeaconText;
        public static string strARQBeaconText;
        public static string strCQText = "";
        // Structures...
        public static List<Channel> lstChannels = new List<Channel>();

        public enum MuxState : int
        {
            // storage for the state of the mux to allow restoring upon exit
            Unknown,
            Aux1,
            Aux2,
            InternalUSB,
            Mic,
            MicPlusAux1,
            MicPlusAux2,
            Other
        }
        // Enum

        public static MuxState enmMuxState;
        public enum DataType
        {
            RcvdFEC,
            RcvdARQ,
            SentFEC,
            SentARQ,
            StrikeThru,
            RcvdFECRbst
        }



        public static DataType enmData;
        public static byte[] StripTypeFromData(byte[] bytData, ref DataType enmType)
        {
            byte[] bytStripped = new byte[bytData.Length - 1];
            Array.Copy(bytData, 1, bytStripped, 0, bytData.Length - 1);
            enmType = (DataType)bytData[0];
            return bytStripped;
        }

        public static void SetMainReference(Main objM)
        {
            objMain = objM;
        }

        public static string DateToRFC822Date(System.DateTime dtUTCDate)
        {
            // This function converts a Date type to a standard RFC 822 date string. The date
            // arguement must be in UTC...

            string sDays = "SunMonTueWedThuFriSat";
            string sMonths = "JanFebMarAprMayJunJulAugSepOctNovDec";
            string sDay = null;
            string sMonth = null;

            sDay = sDays.Substring(3 * (dtUTCDate.DayOfWeek), 3) + ", ";
            sMonth = " " + sMonths.Substring(3 * (dtUTCDate.Month - 1), 3) + " ";
            return sDay + Strings.Format(dtUTCDate, "dd") + sMonth + Strings.Format(dtUTCDate, "yyyy") + " " + Strings.Format(dtUTCDate, "HH:mm:ss") + " -0000";
        }

        public static void EventLog(string strText)
        {
            // Writes the indicated text to the event log...
            lock (objLogLock)
            {
                Microsoft.VisualBasic.FileIO.FileSystem.WriteAllText(strLogsDirectory + "ARDOP Chat " + Strings.Format(DateTime.UtcNow, "yyyyMMdd") + ".log", TimestampEx() + " " + Application.ProductVersion + " " + strText.Trim() + Constants.vbCrLf, true);
            }
        }

        public static void Exceptions(string strText)
        {
            // Writes the indicated text to the exception log...
            lock (objLogLock)
            {
                Debug.WriteLine(strText.Trim());
                Microsoft.VisualBasic.FileIO.FileSystem.WriteAllText(strExecutionDirectory + "Logs\\Chat Exceptions " + Strings.Format(System.DateTime.UtcNow, "yyyyMMdd") + ".log", TimestampEx() + " [" + Application.ProductVersion + "] " + strText.Trim() + Constants.vbCrLf, true);
            }
        }

        public static string FormatDate(System.DateTime dttDate)
        {
            // Returns the dttDate as a string in Winlink format (Example: 2004/08/24 07:23)...

            return Strings.Format(dttDate, "yyyy/MM/dd HH:mm");
        }

        public static string FormatDate(string strDate)
        {
            // Returns the strDate as a string in Winlink format (Example: 2004/08/24 07:23)...

            return Strings.Format(Convert.ToDateTime(strDate), "yyyy/MM/dd HH:mm");
        }

        public static string GetBaseCallsign(string strCallsign)
        {
            if (string.IsNullOrEmpty(strCallsign))
                return "";
            string[] strTokens = strCallsign.Trim().Split('-');
            return strTokens[0].Trim();
        }

        public static byte[] GetBytes(string strText)
        {
            // Converts a text string to a byte array...

            byte[] bytBuffer = new byte[strText.Length];
            for (int intIndex = 0; intIndex <= bytBuffer.Length - 1; intIndex++)
            {
                bytBuffer[intIndex] = Convert.ToByte(Strings.Asc(strText.Substring(intIndex, 1)));
            }
            return bytBuffer;
        }

        public static string GetString(byte[] bytBuffer, int intFirst = 0, int intLast = -1)
        {
            // Converts a byte array to a text string...

            if (intFirst > bytBuffer.GetUpperBound(0))
                return "";
            if (intLast == -1 | intLast > bytBuffer.GetUpperBound(0))
                intLast = bytBuffer.GetUpperBound(0);

            StringBuilder sbdInput = new StringBuilder();
            for (int intIndex = intFirst; intIndex <= intLast; intIndex++)
            {
                byte bytSingle = bytBuffer[intIndex];
                if (bytSingle != 0)
                    sbdInput.Append(Strings.Chr(bytSingle));
            }
            return sbdInput.ToString();
        }

        public static string HzToKHz(int intFrequency)
        {
            try
            {
                string strFrequency = intFrequency.ToString();
                return strFrequency.Insert(strFrequency.Length - 3, ".");
            }
            catch
            {
                return "0.000";
            }
        }

        public static bool IsCMSSiteOnLine(string strURL)
        {
            try
            {
                // Check if the WEB site MySQL server is on line...
                System.Net.Sockets.TcpClient objTCP = new System.Net.Sockets.TcpClient();
                objTCP.Connect(strURL, 3306);
                objTCP.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsInSession()
        {
            //If objTelnet IsNot Nothing Then Return True
            //If objWINMOR IsNot Nothing Then Return True
            return false;
        }

        public static bool IsValidCallsign(string strCallsign)
        {
            if (strCallsign.IndexOf("/") != -1)
                return false;

            if (strCallsign.IndexOf("-") == -1)
            {
                if (strCallsign.Trim().Length > 7)
                    return false;
            }
            else
            {
                if (strCallsign.IndexOf("-") > 7)
                    return false;
            }
            if (IsValidHamCallsign(strCallsign))
                return true;
            if (IsValidMARSCallsign(strCallsign))
                return true;
            return false;
        }

        public static bool IsValidGridSquare(string strGridSquare)
        {
            // Check for valid 4 or 6 character Grid square
            strGridSquare = strGridSquare.ToUpper().Trim();
            if (strGridSquare.Length == 6)
            {
                Regex objRegex = new Regex("^[A-R][A-R][0-9][0-9][A-X][A-X]");
                return objRegex.IsMatch(strGridSquare);
            }
            else if (strGridSquare.Length == 4)
            {
                Regex objRegex = new Regex("^[A-R][A-R][0-9][0-9]");
                return objRegex.IsMatch(strGridSquare);
            }
            else
            {
                return false;
            }

        }

        public static bool IsValidHamCallsign(string strCallsign)
        {
            int intDash = 0;

            strCallsign = strCallsign.ToUpper().Trim();
            intDash = strCallsign.IndexOf("-");
            if (intDash == -1)
            {
                if (strCallsign.Length < 3 | strCallsign.Length > 6)
                    return false;
            }
            else
            {
                if (strCallsign.Length < 3 | strCallsign.Length > 9)
                    return false;
            }

            Regex objRegex = default(Regex);
            objRegex = new Regex("^[A-Z][A-Z][0-9][A-Z]");
            if (objRegex.IsMatch(strCallsign))
                return IsValidSSID(strCallsign);

            objRegex = new Regex("^[A-Z][0-9][A-Z]");
            if (objRegex.IsMatch(strCallsign))
                return IsValidSSID(strCallsign);

            objRegex = new Regex("^[A-Z][0-9][0-9][A-Z]");
            if (objRegex.IsMatch(strCallsign))
                return IsValidSSID(strCallsign);

            objRegex = new Regex("^[0-9][A-Z][0-9][A-Z]");
            if (objRegex.IsMatch(strCallsign))
                return IsValidSSID(strCallsign);

            objRegex = new Regex("^[0-9][A-Z][A-Z][0-9][A-Z]");
            if (objRegex.IsMatch(strCallsign))
                return IsValidSSID(strCallsign);
            //
            objRegex = new Regex("^[A-Z][A-Z][0-9]");
            if (objRegex.IsMatch(strCallsign))
                return IsValidSSID(strCallsign);

            return IsValidUKCadetCallsign(strCallsign);
        }

        public static bool IsValidMARSCallsign(string strCallsign)
        {
            // Tests for legal non-amateur callsigns...
            strCallsign = strCallsign.ToUpper();
            if (strCallsign.StartsWith("M"))
                return false;
            // Special case for UK Cadet callsigns
            Regex objRegex = new Regex("^[A-Z][A-Z][A-Z][0-9]");
            if (objRegex.IsMatch(strCallsign))
                return IsValidSSID(strCallsign);
        }

        public static bool IsValidSSID(string strCallsign)
        {

            if (strCallsign.IndexOf("-") == -1)
                return true;
            if (strCallsign.EndsWith("-0") | strCallsign.EndsWith("-1") | strCallsign.EndsWith("-2") | strCallsign.EndsWith("-3") | strCallsign.EndsWith("-4") | strCallsign.EndsWith("-5") | strCallsign.EndsWith("-6") | strCallsign.EndsWith("-7") | strCallsign.EndsWith("-8") | strCallsign.EndsWith("-9") | strCallsign.EndsWith("-10") | strCallsign.EndsWith("-11") | strCallsign.EndsWith("-12") | strCallsign.EndsWith("-13") | strCallsign.EndsWith("-14") | strCallsign.EndsWith("-15"))
                return true;
            return false;
        }

        public static bool IsValidPortName(string strPortToTest)
        {
            // Function to insure port is valid...

            // This function is used to verify just prior to port opening if a port is still valid
            // Used to prevent trying to access a port that might have been removed...

            string[] strPorts = SerialPort.GetPortNames();
            if (strPorts.Length > 0)
            {
                foreach (string strPort in strPorts)
                {
                    if (strPort == strPortToTest)
                        return true;
                }
            }
            return false;
        }

        public static bool IsValidTacticalAddress(string strCallsign)
        {
            // Checks the format of a tactical address...
            bool blnDigitsOk = false;
            strCallsign = strCallsign.ToUpper().Trim();
            if (strCallsign.Length < 3 | strCallsign.Length > 12)
                return false;
            char[] chrCallsign = strCallsign.ToCharArray();
            foreach (char chr in chrCallsign)
            {
                if (chr.CompareTo('-') == 0)
                    blnDigitsOk = true;
                if (!blnDigitsOk)
                {
                    if (chr.CompareTo('A') < 0 | chr.CompareTo('Z') > 0)
                        return false;
                }
                else
                {
                    if (!((chr.CompareTo('A') >= 0 & chr.CompareTo('Z') <= 0) | (chr.CompareTo('0') >= 0 & chr.CompareTo('9') <= 0) | (chr.CompareTo('-') == 0)))
                        return false;
                }
            }
            return true;
        }

        public static bool IsValidUKCadetCallsign(string strCallsign)
        {
            strCallsign = strCallsign.ToUpper();
            Regex objRegex = new Regex("^[M][A-Z][A-Z][0-9]");
            if (objRegex.IsMatch(strCallsign))
                return IsValidSSID(strCallsign);
        }

        public static bool IsWEBSiteOnLine()
        {
            return objWL2KServers.IsWLWAvailable();
        }

        public static int KHzToHz(string strKHz)
        {
            int functionReturnValue = 0;
            // Convert string (with "." or "," decimal decriptor to integer Hz) 
            // returns 0 if error and logs exception

            string strK = strKHz.Replace(",", ".");
            // replace comma with period for international

            int intPtr = strK.IndexOf("w");
            if (intPtr != -1)
                strK = strK.Substring(0, intPtr);
            intPtr = strK.IndexOf(".");
            if (intPtr == -1)
            {
                strK = strK + ".000";
                intPtr = strK.IndexOf(".");
            }
            else
            {
                strK = (strK + "000").Substring(0, 4 + intPtr);
            }
            try
            {
                functionReturnValue = 1000 * Convert.ToInt32(strK.Substring(0, intPtr)) + Convert.ToInt32(strK.Substring(intPtr + 1, 3));
            }
            catch
            {
                if (!string.IsNullOrEmpty(strKHz))
                    Exceptions("[KHzToHz] Input string: " + strKHz);
                return 0;
            }
            return functionReturnValue;
        }

        public static string ReformatDate(string strSource)
        {
            // Converts a mm/dd/yyyy format to a Winlink format (Example: 2004/08/24 07:23)...

            strSource = Strings.Trim(strSource);
            if (Information.IsDate(strSource))
            {
                string[] sDate = strSource.Split(Convert.ToChar("/"));
                if (Convert.ToInt64(sDate[0]) <= 12)
                {
                    string[] sTime = sDate[2].Split(Strings.Chr(32));
                    if (Information.UBound(sTime) == 0)
                    {
                        Array.Resize(ref sTime, 2);
                    }
                    return Strings.Trim(sTime[0] + "/" + sDate[0] + "/" + sDate[1] + " " + sTime[1]);
                }
                else
                {
                    return strSource;
                }
            }
            else
            {
                return strSource;
            }
        }


        public static System.DateTime RFC822DateToDate(string sDate)
        {
            // This function converts a standard RFC 822 date/time string to a UTC Date type
            // with full correction for UTC to local offset. If the argument does not convert
            // to a proper date then the current UTC date/time is returned...

            if (sDate.IndexOf(",") < 0)
                sDate = "---, " + sDate;
            string sMonths = "   JANFEBMARAPRMAYJUNJULAUGSEPOCTNOVDEC";
            string[] sDateParts = sDate.Split(Strings.Chr(32));
            int nMonth = 0;
            int nOffsetHours = 0;
            int nOffsetMinutes = 0;
            bool bWest = true;
            try
            {
                nMonth = sMonths.IndexOf(sDateParts[2].ToUpper()) / 3;
                if (nMonth < 1)
                    return System.DateTime.UtcNow;
                string sNewDate = sDateParts[3] + "/" + Convert.ToString(nMonth) + "/" + sDateParts[1] + " " + sDateParts[4];
                System.DateTime dtDate = Convert.ToDateTime(sNewDate);
                switch (sDateParts[5])
                {
                    case "UT":
                    case "GMT":
                    case "Z":
                        break;
                    // Do nothing...
                    case "A":
                        nOffsetHours = 1;
                        break;
                    case "M":
                        nOffsetHours = 12;
                        break;
                    case "N":
                        bWest = false;
                        nOffsetHours = 1;
                        break;
                    case "Y":
                        bWest = false;
                        nOffsetHours = 12;
                        break;
                    case "EDT":
                        nOffsetHours = 4;
                        break;
                    case "EST":
                    case "CDT":
                        nOffsetHours = 5;
                        break;
                    case "CST":
                    case "MDT":
                        nOffsetHours = 6;
                        break;
                    case "MST":
                    case "PDT":
                        nOffsetHours = 7;
                        break;
                    case "PST":
                        nOffsetHours = 8;
                        break;
                    default:
                        if (sDateParts[5].Substring(0, 1) != "-")
                            bWest = false;
                        nOffsetHours = Convert.ToInt32(sDateParts[5].Substring(1, 2));
                        nOffsetMinutes = Convert.ToInt32(sDateParts[5].Substring(3, 2));
                        break;
                }
                if (bWest)
                {
                    dtDate = dtDate.AddHours(nOffsetHours);
                    dtDate = dtDate.AddMinutes(nOffsetMinutes);
                }
                else
                {
                    dtDate = dtDate.AddHours(-nOffsetHours);
                    dtDate = dtDate.AddMinutes(-nOffsetMinutes);
                }
                return dtDate;
            }
            catch
            {
                return System.DateTime.UtcNow;
            }
        }

        public static string StringToSQL(string strText)
        {
            // Returns a string formatted to be included in an SQL string...
            StringBuilder sbdText = new StringBuilder();
            foreach (char chrSingle in strText)
            {
                if (chrSingle == '\\')
                {
                    sbdText.Append("\\\\");
                }
                else if (chrSingle == '\'')
                {
                    sbdText.Append("\\'");
                }
                else if (chrSingle >= Strings.Chr(145) & chrSingle <= Strings.Chr(146))
                {
                    sbdText.Append("\\'");
                }
                else
                {
                    sbdText.Append(chrSingle);
                }
            }
            return sbdText.ToString();
        }

        public static string Timestamp()
        {
            // This function returns the current time/date in 
            // 2004/08/24 05:33 format string...

            return Strings.Format(System.DateTime.UtcNow, "yyyy/MM/dd HH:mm");
        }

        public static string TimestampEx()
        {
            // This function returns the current time/date in 
            // 2004/08/24 05:33:12 format string...
            // Dim strMs As String = Now.Millisecond.ToString
            return Strings.Format(DateTime.UtcNow, "yyyy/MM/dd HH:mm:ss");
            //& "." & strMs
        }

        public static void Exception(string strText)
        {
            // Writes the indicated text to the Chat exceptions log...
            lock (objLogLock)
            {
                string strEvent = null;
                strText = strText.Trim();
                if (!string.IsNullOrEmpty(strText))
                {
                    strEvent = TimestampEx() + " " + strText + Constants.vbCrLf;
                    Debug.Write("EXCEPTION: " + strEvent);
                    Microsoft.VisualBasic.FileIO.FileSystem.WriteAllText(strLogsDirectory + Application.ProductName + " Exceptions " + Strings.Format(System.DateTime.UtcNow, "yyyyMMdd") + ".log", strEvent, true);
                }
            }
        }

        public static void Log(string strText)
        {
            // Writes the indicated text to the log...

            lock (objLogLock)
            {
                string strEvent = null;
                strText = strText.Trim();
                if (!string.IsNullOrEmpty(strText))
                {
                    if (strText == "BLANK")
                    {
                        strEvent = Constants.vbCrLf;
                    }
                    else
                    {
                        strEvent = TimestampEx() + " " + strText + Constants.vbCrLf;
                    }
                    Microsoft.VisualBasic.FileIO.FileSystem.WriteAllText(strLogsDirectory + Application.ProductName + " " + Strings.Format(System.DateTime.UtcNow, "yyyyMMdd") + ".log", strEvent, true);
                }
            }
        }

        // Subroutine to compute a 16 bit CRC value and append it to the Data...
        public static void GenCRC16(ref byte[] Data, Int32 intStartIndex, Int32 intStopIndex, Int32 intSeed = 0xffff)
        {
            // For  CRC-16-CCITT =    x^16 + x^12 +x^5 + 1  intPoly = 1021 Init FFFF
            // intSeed is the seed value for the shift register and must be in the range 0-&HFFFF

            int intPoly = 0x8810;
            // This implements the CRC polynomial  x^16 + x^12 +x^5 + 1
            Int32 intRegister = intSeed;

            for (int j = intStartIndex; j <= intStopIndex; j++)
            {
                // for each bit processing MS bit first
                for (int i = 7; i >= 0; i += -1)
                {
                    bool blnBit = (Data[j] & Convert.ToByte(Math.Pow(2, i))) != 0;
                    // the MSB of the register is set
                    if ((intRegister & 0x8000) == 0x8000)
                    {
                        // Shift left, place data bit as LSB, then divide
                        // Register := shiftRegister left shift 1
                        // Register := shiftRegister xor polynomial
                        if (blnBit)
                        {
                            intRegister = 0xffff & (1 + 2 * intRegister);
                        }
                        else
                        {
                            intRegister = 0xffff & (2 * intRegister);
                        }
                        intRegister = intRegister ^ intPoly;
                        // the MSB is not set
                    }
                    else
                    {
                        // Register is not divisible by polynomial yet.
                        // Just shift left and bring current data bit onto LSB of shiftRegister
                        if (blnBit)
                        {
                            intRegister = 0xffff & (1 + 2 * intRegister);
                        }
                        else
                        {
                            intRegister = 0xffff & (2 * intRegister);
                        }
                    }
                }
            }
            // Put the two CRC bytes after the stop index
            Data[intStopIndex + 1] = Convert.ToByte((intRegister & 0xff00) / 256);
            // MS 8 bits of Register
            Data[intStopIndex + 2] = Convert.ToByte(intRegister & 0xff);
            // LS 8 bits of Register
        }
        //GenCRC16

        // Function to compute a 16 bit CRC value and check it against the last 2 bytes of Data (the CRC) ..
        public static bool CheckCRC16(ref byte[] Data, Int32 intSeed = 0xffff)
        {
            // Returns True if CRC matches, else False
            // For  CRC-16-CCITT =    x^16 + x^12 +x^5 + 1  intPoly = 1021 Init FFFF
            // intSeed is the seed value for the shift register and must be in the range 0-&HFFFF

            int intPoly = 0x8810;
            // This implements the CRC polynomial  x^16 + x^12 +x^5 + 1
            Int32 intRegister = intSeed;

            // 2 bytes short of data length
            for (int j = 0; j <= Data.Length - 3; j++)
            {
                // for each bit processing MS bit first
                for (int i = 7; i >= 0; i += -1)
                {
                    bool blnBit = (Data[j] & Convert.ToByte(Math.Pow(2, i))) != 0;
                    // the MSB of the register is set
                    if ((intRegister & 0x8000) == 0x8000)
                    {
                        // Shift left, place data bit as LSB, then divide
                        // Register := shiftRegister left shift 1
                        // Register := shiftRegister xor polynomial
                        if (blnBit)
                        {
                            intRegister = 0xffff & (1 + 2 * intRegister);
                        }
                        else
                        {
                            intRegister = 0xffff & (2 * intRegister);
                        }
                        intRegister = intRegister ^ intPoly;
                        // the MSB is not set
                    }
                    else
                    {
                        // Register is not divisible by polynomial yet.
                        // Just shift left and bring current data bit onto LSB of shiftRegister
                        if (blnBit)
                        {
                            intRegister = 0xffff & (1 + 2 * intRegister);
                        }
                        else
                        {
                            intRegister = 0xffff & (2 * intRegister);
                        }
                    }
                }
            }

            // Compare the register with the last two bytes of Data (the CRC) 
            if (Data[Data.Length - 2] == Convert.ToByte((intRegister & 0xff00) / 256))
            {
                if (Data[Data.Length - 1] == Convert.ToByte(intRegister & 0xff))
                {
                    return true;
                }
            }
            return false;
        }
        //CheckCRC16

    }
    // Globals
}
