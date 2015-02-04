using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.IO.Ports;
using System.Windows.Forms;

namespace ARES_Messenger
{
    public partial class Radio : Form
    {

        // Booleans...

        private bool blnControlDTR;
        private bool blnControlRTS;
        private bool blnFM;
        private bool blnUSB;
        private bool blnUSBDigital;
        private bool blnPTTDTR;
        private bool blnPTTRTS;
        private bool blnTuner;
        private bool blnInitialized;
        private bool blnControlPortOpen;
        private bool blnUseTNCPort;
        private bool blnUSBMod;

        private bool blnUSBModInitialized;
        // Events...
        public event RadioCommandEventHandler RadioCommand;
        public delegate void RadioCommandEventHandler(byte[] bytCommand);
        public event RadioResponseEventHandler RadioResponse;
        public delegate void RadioResponseEventHandler(string strResponse);

        // Integers...

        private int intControlBaud;
        private int intPTTBaud;
        private int intHertz;
        private int intLastSelectedChannel = -1;

        private int intReportedFrequencyHz;
        // Objects...
        private SerialPort objAntennaPort;
        private SerialPort withEventsField_objControlPort;
        private SerialPort objControlPort
        {
            get { return withEventsField_objControlPort; }
            set
            {
                if (withEventsField_objControlPort != null)
                {
                    withEventsField_objControlPort.DataReceived -= OnRadioResponse;
                }
                withEventsField_objControlPort = value;
                if (withEventsField_objControlPort != null)
                {
                    withEventsField_objControlPort.DataReceived += OnRadioResponse;
                }
            }
        }

        private SerialPort objPTTPort;
        // Strings...

        private string strAntennaSelection;
        private string strIcomAddress;
        private string strControlPort;
        private string strModel;
        private string strPTTPort;
        private string strControlResponse = "";

        private string strMuxStateResponse = "";




        public bool USBMod
        {
            // used to switch audio input multiplexer on radios like the ICom 7100, 7200, 7600, Kenwood TS-590
            get { return blnUSBMod; }
            set
            {
                //  Insure initialization but don't duplicate to reduce Keying overhead.
                if ((blnUSBMod != value) | !blnUSBModInitialized)
                {
                    if (objControlPort != null && objControlPort.IsOpen)
                    {
                        switch (strModel)
                        {
                            case "Icom 7100":
                                // not yet verified 9/26/2013
                                // Configure basic command data
                                byte[] bytCommand = new byte[10];
                                bytCommand[0] = 0xfe;
                                bytCommand[1] = 0xfe;
                                bytCommand[2] = Convert.ToByte("&H" + strIcomAddress);
                                bytCommand[3] = 0xf1;
                                bytCommand[4] = 0x1a;
                                // Set Command 1A
                                bytCommand[5] = 0x5;
                                // Sub command 5 Mux control for Data1
                                bytCommand[6] = 0x0;
                                // Set MOD input for USB Digital (Data) mode
                                bytCommand[7] = 0x91;
                                if (value)
                                {
                                    bytCommand[8] = 3;
                                    // select USB (Sound card) modulation
                                }
                                else
                                {
                                    bytCommand[8] = 1;
                                    // Select Aux input modulation
                                }
                                bytCommand[9] = 0xfd;
                                SendBinaryCommand(bytCommand, 0);

                                break;
                            case "Icom 7200":
                                // Configure basic command data
                                byte[] bytCommand2 = new byte[9];
                                bytCommand2[0] = 0xfe;
                                bytCommand2[1] = 0xfe;
                                bytCommand2[2] = Convert.ToByte("&H" + strIcomAddress);
                                bytCommand2[3] = 0xf1;
                                bytCommand2[4] = 0x1a;
                                // Set Command 1A
                                bytCommand2[5] = 0x3;
                                // Sub command 3
                                bytCommand2[6] = 0x24;
                                // Set MOD input for USB Digital (Data) mode
                                if (value)
                                {
                                    bytCommand2[7] = 3;
                                    // select USB (Sound card) modulation
                                }
                                else
                                {
                                    bytCommand2[7] = 1;
                                    // Select Aux input modulation
                                }
                                bytCommand2[8] = 0xfd;
                                SendBinaryCommand(bytCommand2, 0);

                                break;
                            case "Icom 7600":
                                // not verified
                                // Configure basic command data
                                byte[] bytCommand3 = new byte[10];
                                bytCommand3[0] = 0xfe;
                                bytCommand3[1] = 0xfe;
                                bytCommand3[2] = Convert.ToByte("&H" + strIcomAddress);
                                bytCommand3[3] = 0xf1;
                                bytCommand3[4] = 0x1a;
                                // Set Command 1A
                                bytCommand3[5] = 0x5;
                                // Sub command 5 Mux control for Data1
                                bytCommand3[6] = 0x0;
                                // Set MOD input for USB Digital (Data) mode
                                bytCommand3[7] = 0x31;
                                if (value)
                                {
                                    bytCommand3[8] = 3;
                                    // select USB (Sound card) modulation
                                }
                                else
                                {
                                    bytCommand3[8] = 1;
                                    // Select Aux input modulation
                                }
                                bytCommand3[9] = 0xfd;
                                SendBinaryCommand(bytCommand3, 0);

                                break;
                            case "Kenwood TS-590S":
                                if (value)
                                {
                                    SendASCIICommand("EX06300001;");
                                    // select USB for Audio Mux i
                                }
                                else
                                {
                                    SendASCIICommand("EX06300000;");
                                    // select ACC2 for Audio Mux
                                }
                                break;
                        }
                    }
                    blnUSBMod = value;
                    blnUSBModInitialized = true;
                }
            }
        }

        private void ReadMuxState()
        {
            strControlResponse = "";
            strMuxStateResponse = "";
            if (strModel == "Kenwood TS-590S")
            {
                SendASCIICommand("EX0630000;");
                // read mux value
            }
            else if (strModel == "Icom 7100")
            {
                byte[] bytCommand = {
				0xfe,
				0xfe,
				0x0,
				0xf1,
				0x1a,
				0x5,
				0x0,
				0x91,
				0xfd
			};
                // Read Mux value command
                bytCommand[2] = Convert.ToByte("&H" + strIcomAddress);
                SendBinaryCommand(bytCommand, 0);
            }
            else if (strModel == "Icom 7200")
            {
                byte[] bytCommand = {
				0xfe,
				0xfe,
				0x0,
				0xf1,
				0x1a,
				0x3,
				0x24,
				0xfd
			};
                // Read Mux value command
                bytCommand[2] = Convert.ToByte("&H" + strIcomAddress);
                SendBinaryCommand(bytCommand, 0);
            }
            else if (strModel == "Icom 7600")
            {
                byte[] bytCommand = {
				0xfe,
				0xfe,
				0x0,
				0xf1,
				0x1a,
				0x5,
				0x0,
				0x31,
				0xfd
			};
                // Read Mux value command
                bytCommand[2] = Convert.ToByte("&H" + strIcomAddress);
                SendBinaryCommand(bytCommand, 0);

            }
            else
            {
                Globals.enmMuxState = Globals.MuxState.Unknown;
                return;
            }
            System.DateTime dttStartWait = DateTime.Now;
            while (DateTime.Now.Subtract(dttStartWait).TotalMilliseconds < 1000 & string.IsNullOrEmpty(strMuxStateResponse))
            {
                Thread.Sleep(20);
            }
            if (string.IsNullOrEmpty(strMuxStateResponse) | strMuxStateResponse == ";")
            {
                Globals.enmMuxState = Globals.MuxState.Unknown;
                return;
            }
            else if (strModel == "Kenwood TS-590S")
            {
                if (strMuxStateResponse == "EX06300000;")
                {
                    Globals.enmMuxState = Globals.MuxState.Aux2;
                    Globals.queSessionEvents.Enqueue("  *** Initial TS-590S audio mux value = ACC2" + Constants.vbCrLf);
                }
                else if (strMuxStateResponse == "EX06300001;")
                {
                    Globals.enmMuxState = Globals.MuxState.InternalUSB;
                    Globals.queSessionEvents.Enqueue("  *** Initial TS-590S audio mux value = USB" + Constants.vbCrLf);
                }
                else
                {
                    Globals.enmMuxState = Globals.MuxState.Unknown;
                }
            }
            else if (strModel == "Icom 7200" | strModel == "Icom 7600")
            {
                string[] strReply = strMuxStateResponse.Split(',');
                if (strReply.Length == 9 && (strReply[2] == "F1" & strReply[4] == "1A"))
                {
                    if (strModel == "Icom 7200")
                    {
                        switch (strReply[7])
                        {
                            case "0":
                                Globals.enmMuxState = Globals.MuxState.Mic;
                                Globals.queSessionEvents.Enqueue("  *** Initial IC-7200 audio mux value = Mic" + Constants.vbCrLf);
                                break;
                            case "1":
                                Globals.enmMuxState = Globals.MuxState.Aux1;
                                Globals.queSessionEvents.Enqueue("  *** Initial IC-7200 audio mux value = Aux" + Constants.vbCrLf);
                                break;
                            case "2":
                                Globals.enmMuxState = Globals.MuxState.MicPlusAux1;
                                Globals.queSessionEvents.Enqueue("  *** Initial IC-7200 audio mux value = Mic+Aux" + Constants.vbCrLf);
                                break;
                            case "3":
                                Globals.enmMuxState = Globals.MuxState.InternalUSB;
                                Globals.queSessionEvents.Enqueue("  *** Initial IC-7200 audio mux value = USB" + Constants.vbCrLf);
                                break;
                            default:
                                Globals.enmMuxState = Globals.MuxState.Unknown;
                                break;
                        }
                    }
                }
                else if (strReply.Length == 10 && (strReply[2] == "F1" & strReply[4] == "1A"))
                {
                    if (strModel == "Icom 7600" | strModel == "Icom 7100")
                    {
                        switch (strReply[8])
                        {
                            case "0":
                                Globals.enmMuxState = Globals.MuxState.Mic;
                                Globals.queSessionEvents.Enqueue("  *** Initial " + strModel + " audio mux value = Mic" + Constants.vbCrLf);
                                break;
                            case "1":
                                Globals.enmMuxState = Globals.MuxState.Aux1;
                                Globals.queSessionEvents.Enqueue("  *** Initial " + strModel + " audio mux value = Aux" + Constants.vbCrLf);
                                break;
                            case "2":
                                Globals.enmMuxState = Globals.MuxState.MicPlusAux1;
                                Globals.queSessionEvents.Enqueue("  *** Initial " + strModel + " audio mux value = Mic+Aux" + Constants.vbCrLf);
                                break;
                            case "3":
                                Globals.enmMuxState = Globals.MuxState.InternalUSB;
                                Globals.queSessionEvents.Enqueue("  *** Initial " + strModel + " audio mux value = USB" + Constants.vbCrLf);
                                break;
                            default:
                                Globals.enmMuxState = Globals.MuxState.Unknown;
                                break;
                        }


                    }
                }
            }
        }

        private void RestoreMuxState()
        {
            if (strModel == "Kenwood TS-590S")
            {
                if (Globals.enmMuxState == Globals.MuxState.Aux2)
                {
                    SendASCIICommand("EX06300000;");
                    Globals.queSessionEvents.Enqueue("  *** Restore TS-590S audio mux to ACC2" + Constants.vbCrLf);
                    Thread.Sleep(1000);
                }
                else if (Globals.enmMuxState == Globals.MuxState.InternalUSB)
                {
                    SendASCIICommand("EX06300001;");
                    Globals.queSessionEvents.Enqueue("  *** Restore TS-590S audio mux to USB" + Constants.vbCrLf);
                    Thread.Sleep(1000);
                }

            }
            else if (strModel == "Icom 7200")
            {
                byte[] bytCommand = {
				0xfe,
				0xfe,
				0x0,
				0xf1,
				0x1a,
				0x3,
				0x24,
				0x0,
				0xfd
			};
                // Write Mux value command
                bytCommand[2] = Convert.ToByte("&H" + strIcomAddress);
                switch (Globals.enmMuxState)
                {
                    case Globals.MuxState.Mic:
                        Globals.queSessionEvents.Enqueue("  *** Restore Icom 7200 audio mux to Mic" + Constants.vbCrLf);
                        bytCommand[7] = 0;
                        break;
                    case Globals.MuxState.Aux1:
                        Globals.queSessionEvents.Enqueue("  *** Restore Icom 7200 audio mux to Aux" + Constants.vbCrLf);
                        bytCommand[7] = 1;
                        break;
                    case Globals.MuxState.MicPlusAux1:
                        Globals.queSessionEvents.Enqueue("  *** Restore Icom 7200 audio mux to Mic+Aux" + Constants.vbCrLf);
                        bytCommand[7] = 2;
                        break;
                    case Globals.MuxState.InternalUSB:
                        Globals.queSessionEvents.Enqueue("  *** Restore Icom 7200 audio mux to USB" + Constants.vbCrLf);
                        bytCommand[7] = 3;
                        break;
                    default:
                        return;

                        break;
                }
                SendBinaryCommand(bytCommand, 0);
                Thread.Sleep(1000);
            }
            else if (strModel == "Icom 7600")
            {
                byte[] bytCommand = {
				0xfe,
				0xfe,
				0x0,
				0xf1,
				0x1a,
				0x5,
				0x0,
				0x31,
				0x0,
				0xfd
			};
                // Write Mux value command
                bytCommand[2] = Convert.ToByte("&H" + strIcomAddress);
                switch (Globals.enmMuxState)
                {
                    case Globals.MuxState.Mic:
                        Globals.queSessionEvents.Enqueue("  *** Restore Icom 7600 audio mux to Mic" + Constants.vbCrLf);
                        bytCommand[8] = 0;
                        break;
                    case Globals.MuxState.Aux1:
                        Globals.queSessionEvents.Enqueue("  *** Restore Icom 7600 audio mux to Aux" + Constants.vbCrLf);
                        bytCommand[8] = 1;
                        break;
                    case Globals.MuxState.MicPlusAux1:
                        Globals.queSessionEvents.Enqueue("  *** Restore Icom 7600 audio mux to Mic+Aux" + Constants.vbCrLf);
                        bytCommand[8] = 2;
                        break;
                    case Globals.MuxState.InternalUSB:
                        Globals.queSessionEvents.Enqueue("  *** Restore Icom 7600 audio mux to USB" + Constants.vbCrLf);
                        bytCommand[8] = 3;
                        break;
                    default:
                        return;

                        break;
                }
                SendBinaryCommand(bytCommand, 0);
                Thread.Sleep(1000);
            }
        }

        public void CloseRadio()
        {
            if (strModel == "Icom HF Marine Radios")
            {
                SendNMEACommand(ComputeNMEACommand("REMOTE,OFF"));
                Thread.Sleep(1000);
            }

            RestoreMuxState();
            // restore the internal audio mux state for TS-590S, IC-7200 or IC-7600 ' no effect with other radios

            if (objControlPort != null)
            {
                objControlPort.Close();
                objControlPort.Dispose();
                objControlPort = null;
            }

            if (objAntennaPort != null)
            {
                objAntennaPort.Close();
                objAntennaPort.Dispose();
                objAntennaPort = null;
            }

            if (objPTTPort != null)
            {
                objPTTPort.Close();
                objPTTPort.Dispose();
                objPTTPort = null;
            }

            this.Close();
        }
        // Public Method

        public void Edit()
        {
            this.Text = "   H4 Chat1 Settings";
            this.ShowDialog();
        }
        // Public Method

        public Radio()
        {
            Resize += Radio_Resize;
            RadioResponse += ProcessRadioResponse;
            RadioCommand += Radio_RadioCommand;
            Load += Radio_Load;
            FormClosing += Radio_FormClosing;
            FormClosed += Radio_FormClosed;
            InitializeComponent();

            // Initializes the serial port drop down...
            cmbControlPort.Items.Add("None");


            //cmbAntennaPort.Items.Add("None")
            string[] strPorts = SerialPort.GetPortNames();
            if (strPorts.Length > 0)
            {
                foreach (string strPort in strPorts)
                {
                    cmbControlPort.Items.Add(strPort);
                    cmbPTTPort.Items.Add(strPort);
                }

                if (string.IsNullOrEmpty(strControlPort))
                {
                    cmbControlPort.Text = cmbControlPort.Items[0].ToString();
                }
                else
                {
                    cmbControlPort.Text = strControlPort;
                }

                if (string.IsNullOrEmpty(strPTTPort))
                {
                    cmbPTTPort.Text = cmbPTTPort.Items[0].ToString();
                }
                else
                {
                    cmbPTTPort.Text = strPTTPort;
                }

                //If strAntennaPort = "" Then
                //   cmbAntennaPort.Text = cmbAntennaPort.Items(0).ToString
                //Else
                //   cmbAntennaPort.Text = strAntennaPort
                //End If
            }

            // Initialized the radio model drop down...
            cmbModel.Items.Add("Manual");
            cmbModel.Items.Add("Elecraft Radios");
            cmbModel.Items.Add("Flex radios");
            cmbModel.Items.Add("Icom Amateur Radios");
            cmbModel.Items.Add("Icom Amateur Radios (Early)");
            cmbModel.Items.Add("Icom HF Marine Radios");
            cmbModel.Items.Add("Icom 7100");
            cmbModel.Items.Add("Icom 7200");
            cmbModel.Items.Add("Icom 7600");
            cmbModel.Items.Add("Kenwood Radios");
            cmbModel.Items.Add("Kenwood TS-590S");
            cmbModel.Items.Add("Yaesu FT-100");
            cmbModel.Items.Add("Yaesu FT-450");
            cmbModel.Items.Add("Yaesu FT-600");
            cmbModel.Items.Add("Yaesu FT-817");
            cmbModel.Items.Add("Yaesu FT-840");
            cmbModel.Items.Add("Yaesu FT-847");
            cmbModel.Items.Add("Yaesu FT-857");
            cmbModel.Items.Add("Yaesu FT-897");
            cmbModel.Items.Add("Yaesu FT-920");
            cmbModel.Items.Add("Yaesu FT-950");
            cmbModel.Items.Add("Yaesu FT-1000");
            cmbModel.Items.Add("Yaesu FT-2000");

            cmbModel.Text = "";

            GetRadioSettings();
            ShowPropertyValues();
            RefreshPropertyValues();

        }

        public void PTT(bool blnPTT)
        {
            try
            {
                if ((strControlPort == "None" | strControlPort == "Via TNC") & (strPTTPort == "External" | string.IsNullOrEmpty(strPTTPort)))
                {
                    // Do nothing...
                    // Note commands are not delyed for PTT for timing reasons should be OK.

                }
                else if (strPTTPort == "CI-V")
                {
                    if (objControlPort != null && objControlPort.IsOpen)
                    {
                        byte[] bytCommand = new byte[8];
                        bytCommand[0] = 0xfe;
                        bytCommand[1] = 0xfe;
                        bytCommand[2] = Convert.ToByte("&H" + strIcomAddress);
                        bytCommand[3] = 0xf1;
                        bytCommand[4] = 0x1c;
                        // Set Transceiver PTT
                        bytCommand[5] = 0;
                        if (blnPTT)
                        {
                            bytCommand[6] = 1;
                            // Set to Transmit PTT on
                        }
                        else
                        {
                            bytCommand[6] = 0;
                            // Set to transmit PTT Off
                        }
                        bytCommand[7] = 0xfd;
                        SendBinaryCommand(bytCommand, 0);
                    }

                }
                else if (strPTTPort == "Icom 7200")
                {
                    if (objControlPort != null && objControlPort.IsOpen)
                    {
                        USBMod = true;
                        // switch the modulation input if not already switched 
                        Thread.Sleep(20);
                        // Configure basic command data
                        byte[] bytCommand = new byte[8];
                        bytCommand[0] = 0xfe;
                        bytCommand[1] = 0xfe;
                        bytCommand[2] = Convert.ToByte("&H" + strIcomAddress);
                        bytCommand[3] = 0xf1;
                        bytCommand[7] = 0xfd;
                        //This performs the actual PTT keying for the 7200
                        bytCommand[4] = 0x1c;
                        // Set Transceiver PTT cpmmand
                        bytCommand[5] = 0;
                        if (blnPTT)
                        {
                            bytCommand[6] = 1;
                            // Set to Transmit PTT on
                        }
                        else
                        {
                            bytCommand[6] = 0;
                            // Set to transmit PTT Off
                        }
                        SendBinaryCommand(bytCommand, 0);
                    }

                    // not yet verified
                }
                else if (strPTTPort == "Icom 7600" | strPTTPort == "Icom 7100")
                {
                    if (objControlPort != null && objControlPort.IsOpen)
                    {
                        USBMod = true;
                        // switch the modulation input if not already switched 
                        Thread.Sleep(20);
                        // Configure basic command data
                        byte[] bytCommand = new byte[8];
                        bytCommand[0] = 0xfe;
                        bytCommand[1] = 0xfe;
                        bytCommand[2] = Convert.ToByte("&H" + strIcomAddress);
                        bytCommand[3] = 0xf1;
                        bytCommand[7] = 0xfd;
                        //This performs the actual PTT keying for the 7200
                        bytCommand[4] = 0x1c;
                        // Set Transceiver PTT cpmmand
                        bytCommand[5] = 0;
                        if (blnPTT)
                        {
                            bytCommand[6] = 1;
                            // Set to Transmit PTT on
                        }
                        else
                        {
                            bytCommand[6] = 0;
                            // Set to transmit PTT Off
                        }
                        SendBinaryCommand(bytCommand, 0);
                    }

                }
                else if (strPTTPort == "K450")
                {
                    if (blnPTT)
                    {
                        SendASCIICommand("TX;", 0);
                    }
                    else
                    {
                        SendASCIICommand("RX;", 0);
                    }

                }
                else if (strPTTPort.IndexOf("K480") != -1)
                {
                    if (blnPTT)
                    {
                        if (strPTTPort.EndsWith("Mic"))
                        {
                            SendASCIICommand("TX0;", 0);
                        }
                        else
                        {
                            SendASCIICommand("TX1;", 0);
                        }
                    }
                    else
                    {
                        SendASCIICommand("RX;", 0);
                    }
                }
                else if (strPTTPort.IndexOf("K590S") != -1)
                {
                    if (objControlPort != null && objControlPort.IsOpen)
                    {
                        USBMod = true;
                        // switch the modulation input if not already switched 
                        Thread.Sleep(20);
                        // Configure basic command data
                        if (blnPTT)
                        {
                            SendASCIICommand("TX1;", 0);
                            // Data send using USB or ACC2 input (depending on MUX value)

                        }
                        else
                        {
                            SendASCIICommand("RX;", 0);
                            // Set to Receive
                        }
                    }


                    // not sure if this works documentation not clear
                }
                else if (strPTTPort == "K2000")
                {
                    if (blnPTT)
                    {
                        SendASCIICommand("TX0;", 0);
                    }
                    else
                    {
                        SendASCIICommand("RX0;", 0);
                    }
                }
                else if (strPTTPort == "Y450" | strPTTPort == "Y2000" | strPTTPort == "Y950")
                {
                    if (blnPTT)
                    {
                        SendASCIICommand("TX1;", 0);
                    }
                    else
                    {
                        SendASCIICommand("TX0;", 0);
                    }

                }
                else if (strPTTPort == "Y8x7")
                {
                    byte[] bytCommand = {
					0,
					0,
					0,
					0,
					0
				};
                    bytCommand[0] = 0;
                    bytCommand[1] = 0;
                    bytCommand[2] = 0;
                    bytCommand[3] = 0;
                    if (blnPTT)
                    {
                        bytCommand[4] = 0x8;
                    }
                    else
                    {
                        bytCommand[4] = 0x88;
                    }
                    SendBinaryCommand(bytCommand, 0);

                }
                else if (strPTTPort == "Y1000")
                {
                    byte[] bytCommand = {
					0,
					0,
					0,
					0,
					0
				};
                    bytCommand[0] = 0;
                    bytCommand[1] = 0;
                    bytCommand[2] = 0;
                    if (blnPTT == true)
                        bytCommand[3] = 1;
                    else
                        bytCommand[3] = 0;
                    bytCommand[4] = 0xf;
                    SendBinaryCommand(bytCommand, 0);
                    // reported working
                }
                else if ((strPTTPort == "Flex" | strPTTPort == "K3"))
                {
                    if (blnPTT)
                    {
                        SendASCIICommand("TX;", 0);
                    }
                    else
                    {
                        SendASCIICommand("RX;", 0);
                    }

                    // normal serial port PTT control
                }
                else
                {
                    if ((strPTTPort == strControlPort) & strPTTPort.StartsWith("COM"))
                    {
                        // this tries to use the Control port RTS/DTR to toggle the PTT (may work on some radios)
                        if (objControlPort != null && objControlPort.IsOpen)
                        {
                            if (blnPTTDTR)
                            {
                                objControlPort.DtrEnable = blnPTT;
                            }
                            if (blnPTTRTS)
                            {
                                objControlPort.RtsEnable = blnPTT;
                            }
                        }
                        // This uses the dedicated PTT COM port to key 
                    }
                    else
                    {
                        if (objPTTPort != null && objPTTPort.IsOpen)
                        {
                            if (blnPTTDTR)
                            {
                                objPTTPort.DtrEnable = blnPTT;
                            }
                            if (blnPTTRTS)
                            {
                                objPTTPort.RtsEnable = blnPTT;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Globals.Exception("[Radio.PTT] Err: " + ex.ToString());
            }
        }
        // Public Method

        private void Radio_RadioCommand(byte[] bytCommand)
        {
        }

        private void ProcessRadioResponse(string strResponse)
        {
            string strHz = "";
            //Debug.WriteLine("RadioResponse:" + strResponse);
            try
            {
                if (strModel.StartsWith("Icom"))
                {
                    //Log("Icom Radio Response: " & strResponse) ' TEST:  Remove after verification
                    string[] strReply = strResponse.Split(',');
                    if (strReply[2] == "F1" & strReply[4] == "3")
                    {
                        // reply to frequency request
                        if (strReply.Length == 11 & (strModel == "Icom Amateur Radios" | strModel == "Icom 7200" | strModel == "Icom 7100" | strModel == "Icom 7600"))
                        {
                            for (int i = 9; i >= 5; i += -1)
                            {
                                strHz += ("0" + strReply[i]).Substring(strReply[i].Length - 1);
                            }
                        }
                        else if (strReply.Length == 10 & strModel == "Icom Amateur Radios (Early)")
                        {
                            for (int i = 8; i >= 5; i += -1)
                            {
                                strHz += ("0" + strReply[i]).Substring(strReply[i].Length - 1);
                            }
                        }
                    }
                }
                else if (strModel.StartsWith("Kenwood") | strModel.StartsWith("Flex") | strModel.StartsWith("Elecraft") | strModel == "Yaesu FT-450" | strModel == "Yaesu FT-2000" | strModel == "Yaesu FT-950")
                {
                    if (strResponse.IndexOf("FA") != -1)
                    {
                        strHz = strResponse.Substring(2 + strResponse.IndexOf("FA"));
                        if (strHz.IndexOf(";") != -1)
                        {
                            strHz = strHz.Substring(0, strHz.IndexOf(";"));
                        }
                    }
                    else if (strResponse.IndexOf("IF") != -1)
                    {
                        //Debug.WriteLine("response: " + strResponse);
                        strHz = strResponse.Substring(2 + strResponse.IndexOf("IF"));
                        strHz = strHz.Substring(0, strHz.IndexOf(" ")).Trim();
                        //Debug.WriteLine("strHz: " + strHz);
                    }
                }
                else if (strModel == "Yaesu FT-897" | strModel == "Yaesu FT-817" | strModel == "Yaesu FT-847" | strModel == "Yaesu FT-857")
                {
                    Globals.Log("Yaesu 8x7 Radio Response: " + strResponse);
                    // TEST:  Remove after verification
                    if (strResponse.Length == 10)
                    {
                        strHz = strResponse.Substring(0, 8) + "0";
                        // add the 0 for the units Hz
                    }
                }
                if (Information.IsNumeric(strHz))
                {
                    intReportedFrequencyHz = Convert.ToInt32(strHz);
                }
            }
            catch (Exception ex)
            {
                Globals.Exceptions("[RadioResponse] " + strResponse + Constants.vbCr + " Err:" + ex.ToString());
            }
        }

        private void Radio_FormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            if (strModel == "Icom HF Marine Radios")
            {
                if (strControlPort == "Via TNC")
                {
                    if (RadioCommand != null)
                    {
                        RadioCommand(GetBytes(ComputeNMEACommand("REMOTE,OFF")));
                    }
                }
                else
                {
                    if (objControlPort != null && objControlPort.IsOpen)
                    {
                        if (RadioCommand != null)
                        {
                            RadioCommand(Globals.GetBytes(ComputeNMEACommand("REMOTE,OFF")));
                        }
                    }
                    Thread.Sleep(2000);
                }
            }
            CloseRadio();
        }


        private void Radio_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
        }

        private void Radio_Load(System.Object sender, System.EventArgs e)
        {
        }

        private void Radio_Resize(object sender, System.EventArgs e)
        {
        }

        private bool SendASCIICommand(string strCommand, int intDelayMs = 50)
        {
            // Sends a command to the radio...
            // The Delay is needed for most radios to allow time for processing the next command
            try
            {
                if (strControlPort == "Via TNC")
                {
                    if (RadioCommand != null)
                    {
                        RadioCommand(Globals.GetBytes("#TRX T " + strCommand));
                    }
                }
                else if (objControlPort.IsOpen)
                {
                    objControlPort.Write(strCommand);
                }
                if (intDelayMs > 0)
                    Thread.Sleep(intDelayMs);
                return true;
            }
            catch
            {
                return false;
            }
        }

        
        private bool SendBinaryCommand(byte[] bytCommand, int intDelayMs = 50)
        {
            try
            {
                if (strControlPort == "Via TNC")
                {
                    if (RadioCommand != null)
                    {
                        RadioCommand(ConcatByteArrays(Globals.GetBytes("#TRX T "), bytCommand));
                    }
                }
                else
                {
                    objControlPort.Write(bytCommand, 0, bytCommand.Length);
                }
                if (intDelayMs > 0)
                    Thread.Sleep(intDelayMs);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool SendNMEACommand(string strCommand)
        {
            // Sends an NMEA command to the radio...
            try
            {
                if (strControlPort == "Via TNC")
                {
                    if (RadioCommand != null)
                    {
                        RadioCommand(Globals.GetBytes(strCommand));
                    }
                }
                else
                {
                    if (objControlPort != null)
                        objControlPort.Write(Globals.GetBytes(strCommand), 0, strCommand.Length);
                }
                Thread.Sleep(100);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void OnRadioResponse(object s, SerialDataReceivedEventArgs e)
        {
            // This captures the reply in strResponse until an end of command is encountered and then Raises RadioResponse event.
            // The static variable strResponse will hold the partial response if necessary

            int intBytesToRead = 0;
            string strRead = null;
            if (strModel == "Icom Amateur Radios" | strModel == "Icom 7200" | strModel == "Icom 7600" | strModel == "Icom 7100" | strModel == "Icom Amateur Radios (Early)")
            {
                if (objControlPort != null & objControlPort.IsOpen)
                {
                    intBytesToRead = objControlPort.BytesToRead;
                    for (int i = 1; i <= intBytesToRead; i++)
                    {
                        strRead = Conversion.Hex(objControlPort.ReadByte()).ToUpper();
                        if (strRead == "FD")
                        {
                            if (RadioResponse != null)
                            {
                                RadioResponse("[" + strControlResponse + strRead + "]");
                            }
                            strMuxStateResponse = strControlResponse + strRead;
                            strControlResponse = "";
                        }
                        else
                        {
                            strControlResponse += strRead + ",";
                        }
                    }
                }
            }
            else if (strModel.StartsWith("Kenwood") | strModel.StartsWith("Flex") | strModel.StartsWith("Elecraft") | strModel == "Yaesu FT-450" | strModel == "Yaesu FT-2000" | strModel == "Yaesu FT-950")
            {
                if (objControlPort != null & objControlPort.IsOpen)
                {
                    intBytesToRead = objControlPort.BytesToRead;
                    for (int i = 1; i <= intBytesToRead; i++)
                    {
                        strRead = Strings.Chr(objControlPort.ReadByte()).ToString();
                        if (strRead == ";")
                        {
                            if (RadioResponse != null)
                            {
                                RadioResponse(strControlResponse + strRead);
                            }
                            strMuxStateResponse = strControlResponse + strRead;
                            strControlResponse = "";
                        }
                        else
                        {
                            strControlResponse += strRead;
                        }
                    }
                }
            }
            else if (strModel == "Yaesu FT-897" | strModel == "Yaesu FT-817" | strModel == "Yaesu FT-847" | strModel == "Yaesu FT-857")
            {
                intBytesToRead = objControlPort.BytesToRead;
                for (int i = 1; i <= intBytesToRead; i++)
                {
                    strRead = Conversion.Hex(objControlPort.ReadByte()).ToUpper();
                    if (strRead.Length == 1)
                        strRead = "0" + strRead;
                    // insure single digits are reported as "0x" and not just "x"
                    if (strControlResponse.Length == 8)
                    {
                        if (RadioResponse != null)
                        {
                            RadioResponse(strControlResponse + strRead);
                        }
                        strControlResponse = "";
                        break; // TODO: might not be correct. Was : Exit For
                    }
                    else
                    {
                        strControlResponse += strRead;
                    }
                }
                if (!string.IsNullOrEmpty(strControlResponse))
                    Globals.Log("[objControlPort.DataReceived, Yaesu 8x7] BTR: " + intBytesToRead.ToString() + " strControlResponse:" + strControlResponse);
            }
        }

        private string ComputeNMEACommand(string strCommand)
        {
            string strBuffer = null;
            string strCheckSum = null;
            int intCheckSum = 0;
            int intIndex = 0;

            strBuffer = "$PICOA,90," + strIcomAddress + "," + strCommand;
            for (intIndex = 1; intIndex <= strBuffer.Length - 1; intIndex++)
            {
                intCheckSum = intCheckSum ^ Strings.Asc(strBuffer.Substring(intIndex, 1));
            }
            strCheckSum = "*" + Strings.Right("0" + Conversion.Hex(intCheckSum), 2);
            return strBuffer + strCheckSum + Constants.vbCrLf;
        }

        private byte[] ConcatByteArrays(byte[] bytFirst, byte[] bytSecond)
        {
            ArrayList aryResult = new ArrayList();
            byte bytSingle = 0;
            var _with1 = aryResult;
            foreach (byte bytSingle_loopVariable in bytFirst)
            {
                bytSingle = bytSingle_loopVariable;
                _with1.Add(bytSingle);
            }
            foreach (byte bytSingle_loopVariable in bytSecond)
            {
                bytSingle = bytSingle_loopVariable;
                _with1.Add(bytSingle);
            }
            byte[] bytResult = new byte[bytFirst.Length + bytSecond.Length + 2];
            aryResult.CopyTo(bytResult);
            return bytResult;
        }

        private byte[] GetBytes(string strText)
        {
            // Converts a text string to a byte array...
            byte[] bytBuffer = new byte[strText.Length];
            for (int intIndex = 0; intIndex <= bytBuffer.Length - 1; intIndex++)
            {
                bytBuffer[intIndex] = Convert.ToByte(Strings.Asc(strText.Substring(intIndex, 1)));
            }
            return bytBuffer;
        }

        private void GetRadioSettings()
        {
            //

            string strSection = "Radio";



            blnControlDTR = Convert.ToBoolean(Globals.objINIFile.GetString(strSection, "Control DTR", "True"));
            blnControlRTS = Convert.ToBoolean(Globals.objINIFile.GetString(strSection, "Control RTS", "True"));
            blnFM = Convert.ToBoolean(Globals.objINIFile.GetString(strSection, "FM", "False"));
            blnUSB = Convert.ToBoolean(Globals.objINIFile.GetString(strSection, "USB", "True"));
            blnUSBDigital = Convert.ToBoolean(Globals.objINIFile.GetString(strSection, "USBDigital", "False"));
            blnTuner = Convert.ToBoolean(Globals.objINIFile.GetString(strSection, "Tuner", "False"));
            strModel = Globals.objINIFile.GetString(strSection, "Model", "Manual");
            intControlBaud = Globals.objINIFile.GetInteger(strSection, "Control Baud", 9600);
            intPTTBaud = Globals.objINIFile.GetInteger(strSection, "PTT Baud", 9600);
            strAntennaSelection = Globals.objINIFile.GetString(strSection, "Antenna Selection", "Default");
            strIcomAddress = Globals.objINIFile.GetString(strSection, "Icom Address", "00");
            strControlPort = Globals.objINIFile.GetString(strSection, "Control Port", "None");
            strPTTPort = Globals.objINIFile.GetString(strSection, "PTT Port", "External");
            if (strPTTPort == "None")
                strPTTPort = "External";
            blnPTTDTR = Convert.ToBoolean(Globals.objINIFile.GetString(strSection, "PTT DTR", "True"));
            blnPTTRTS = Convert.ToBoolean(Globals.objINIFile.GetString(strSection, "PTT RTS", "True"));
        }

        private void ShowPropertyValues()
        {
            chkControlDTR.Checked = blnControlDTR;
            chkControlDTR.Checked = blnControlRTS;
            chkInternalTuner.Checked = blnTuner;
            rdoFM.Checked = blnFM;
            rdoUSB.Checked = blnUSB;
            rdoUSBDigital.Checked = blnUSBDigital;
            chkPTTDTR.Checked = blnPTTDTR;
            chkPTTRTS.Checked = blnPTTRTS;
            cmbAntenna.Text = strAntennaSelection;
            cmbControlBaud.Text = intControlBaud.ToString();
            cmbControlPort.Text = strControlPort;
            cmbModel.Text = strModel;
            cmbPTTBaud.Text = intPTTBaud.ToString();
            cmbPTTPort.Text = strPTTPort;
            txtIcomAddress.Text = strIcomAddress;
            if (cmbControlPort.Text == "Via TNC")
            {
                cmbControlBaud.Enabled = false;
                chkControlDTR.Enabled = false;
                chkControlDTR.Enabled = false;
            }
            if (cmbPTTPort.Text.StartsWith("COM") == false)
            {
                cmbPTTBaud.Enabled = false;
                chkPTTDTR.Enabled = false;
                chkPTTRTS.Enabled = false;
            }
        }

        private void RefreshPropertyValues()
        {
            blnControlDTR = chkControlDTR.Checked;
            blnControlRTS = chkControlRTS.Checked;
            blnFM = rdoFM.Checked;
            blnUSB = rdoUSB.Checked;
            blnUSBDigital = rdoUSBDigital.Checked;
            blnPTTDTR = chkPTTDTR.Checked;
            blnPTTRTS = chkPTTRTS.Checked;
            blnTuner = chkInternalTuner.Checked;
            strModel = cmbModel.Text;
            if (string.IsNullOrEmpty(cmbControlBaud.Text))
            {
                intControlBaud = 9600;
            }
            else
            {
                intControlBaud = Convert.ToInt32(cmbControlBaud.Text);
            }
            if (string.IsNullOrEmpty(cmbPTTBaud.Text))
            {
                intPTTBaud = 9600;
            }
            else
            {
                intPTTBaud = Convert.ToInt32(cmbPTTBaud.Text);
            }
            strAntennaSelection = cmbAntenna.Text;
            strIcomAddress = txtIcomAddress.Text;
            strControlPort = cmbControlPort.Text;
            strPTTPort = cmbPTTPort.Text;
        }


    }
}
