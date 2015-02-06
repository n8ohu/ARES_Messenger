using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Timers;

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
		private SerialPort objControlPort {
			get { return withEventsField_objControlPort; }
			set {
				if (withEventsField_objControlPort != null) {
					withEventsField_objControlPort.DataReceived -= OnRadioResponse;
					
				}
				
                withEventsField_objControlPort = value;
                if (withEventsField_objControlPort != null) {
                
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
        // Public Method

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

        public void SetFrequency(int intHertz)
        {
            this.intHertz = intHertz;
            SetRadio();
        }
        // Public Method

        public int GetFrequency()
        {
            int functionReturnValue = 0;
            if ((strControlPort == "None"))
                return (0);
            if (intReportedFrequencyHz > 0)
            {
                functionReturnValue = intReportedFrequencyHz;
                // This is to handle a late reply to a prior frequency request
                intReportedFrequencyHz = 0;
                return functionReturnValue;
            }
            //  Send a frequency request command
            strControlResponse = "";
            intReportedFrequencyHz = 0;
            if (strModel.StartsWith("Icom"))
            {
                byte[] bytCommand = {
				0xfe,
				0xfe,
				0x0,
				0xf1,
				0x3,
				0xfd
			};
                // Read frequency command
                bytCommand[2] = Convert.ToByte("&H" + strIcomAddress);
                SendBinaryCommand(bytCommand, 0);
            }
            else if ((strModel.StartsWith("Kenwood") | strModel.StartsWith("Flex")))
            {
                if (strModel == "Kenwood TS-590S")
                {
                    SendASCIICommand("IF;");
                    // Read Radio display
                }
                else
                {
                    SendASCIICommand("FA;");
                    // Read VFO A
                }


                //cmbModel.Items.Add("Yaesu FT-100")
                //cmbModel.Items.Add("Yaesu FT-450") '
                //cmbModel.Items.Add("Yaesu FT-600")
                //cmbModel.Items.Add("Yaesu FT-817") '
                //cmbModel.Items.Add("Yaesu FT-840")
                //cmbModel.Items.Add("Yaesu FT-847") '
                //cmbModel.Items.Add("Yaesu FT-857") '
                //cmbModel.Items.Add("Yaesu FT-897") '
                //cmbModel.Items.Add("Yaesu FT-920") ?
                //cmbModel.Items.Add("Yaesu FT-950") '   
                //cmbModel.Items.Add("Yaesu FT-1000") ' Command for this unclear from Yaesu Docs...
                //cmbModel.Items.Add("Yaesu FT-2000") '
            }
            else if (strModel == "Yaesu FT-920")
            {
                byte[] bytCommand = {
				0,
				0,
				0,
				0,
				5
			};
                // should be read back for VFO A ....not sure of this
                SendBinaryCommand(bytCommand, 0);
            }
            else if (strModel.StartsWith("Elecraft") | strModel == "Yaesu FT-450" | strModel == "Yaesu FT-2000" | strModel == "Yaesu FT-950")
            {
                SendASCIICommand("FA;");
                // Read VFO A
            }
            else if (strModel == "Yaesu FT-897" | strModel == "Yaesu FT-817" | strModel == "Yaesu FT-847" | strModel == "Yaesu FT-857")
            {
                byte[] bytCommand = {
				0,
				0,
				0,
				0,
				3
			};
                SendBinaryCommand(bytCommand, 0);
            }
            System.DateTime dttStartWait = DateTime.Now;
            while (DateTime.Now.Subtract(dttStartWait).TotalMilliseconds < 500 & intReportedFrequencyHz == 0)
            {
                Thread.Sleep(20);
            }
            //If intReportedFrequencyHz <> 0 Then
            //    Log("Icom Radio GetFrequency, intReportedFrequencyHz = " & intReportedFrequencyHz.ToString) ' TEST:  Remove after verification
            //End If
            functionReturnValue = intReportedFrequencyHz;
            intReportedFrequencyHz = 0;
            return functionReturnValue;
        }
        // Public Method

        public void SetAntennaSwitch(bool blnAntennaRTS, bool blnAntennaDTR)
        {
            if (objAntennaPort != null && objAntennaPort.IsOpen)
            {
                objAntennaPort.RtsEnable = blnAntennaRTS;
                objAntennaPort.DtrEnable = blnAntennaDTR;
            }
        }

        public bool UseTNCPort
        {
            get { return strPTTPort == "Via TNC"; }
        }
        // Public Method

        // Private Methods...
        private void btnClose_Click(System.Object sender, System.EventArgs e)
        {
            PTT(false);
            this.Hide();
        }

        private void btnMakeDefault_Click(System.Object sender, System.EventArgs e)
        {
            RefreshPropertyValues();
            WriteRadioSettings();
        }

        private void btnUpdate_Click(System.Object sender, System.EventArgs e)
        {
            if (cmbControlPort.Text == cmbPTTPort.Text & cmbControlPort.Text.StartsWith("COM"))
            {
                if (Interaction.MsgBox("The same serial port is selected for both control and PTT. This may not work with all radios!   Continue?", MsgBoxStyle.YesNo) == MsgBoxResult.No)
                    return;
            }
            //If cmbControlPort.Text = cmbAntennaPort.Text And cmbControlPort.Text.StartsWith("COM") Then
            //   MsgBox("The same COM port cannot be used for Control and Antenna.!", MsgBoxStyle.Critical)
            //   Exit Sub
            //End If
            //If cmbPTTPort.Text = cmbAntennaPort.Text And cmbPTTPort.Text.StartsWith("COM") Then
            //   MsgBox("The same COM port cannot be used for PTT and Antenna.!", MsgBoxStyle.Critical)
            //   Exit Sub
            //End If
            if (!(cmbPTTPort.Text == "External" | cmbPTTPort.Text.StartsWith("COM")))
            {
                if (Information.IsNumeric(cmbControlBaud.Text) && Convert.ToInt32(cmbControlBaud.Text) < 4800)
                {
                    Interaction.MsgBox("Radio control port baud must be 4800 or higher for command PTT", MsgBoxStyle.Information);
                    return;
                }
            }

            PTT(false);
            RefreshPropertyValues();
            WriteRadioSettings();
            this.DialogResult = DialogResult.OK;

        }

        private void btnUseDefault_Click(System.Object sender, System.EventArgs e)
        {
            PTT(false);
            ShowPropertyValues();
        }

        private void cmbControlPort_TextChanged(object sender, System.EventArgs e)
        {
            string[] strPorts = SerialPort.GetPortNames();

            if (cmbControlPort.Text == "None" | string.IsNullOrEmpty(cmbControlPort.Text))
            {
                cmbPTTPort.Items.Clear();
                cmbPTTPort.Items.Add("External");
                // use VOX control
                if (strPorts.Length > 0)
                {
                    cmbPTTBaud.Enabled = true;
                    chkPTTDTR.Enabled = true;
                    chkPTTRTS.Enabled = true;
                    foreach (string strPort in strPorts)
                    {
                        cmbPTTPort.Items.Add(strPort);
                    }
                }
                else
                {
                    cmbPTTBaud.Enabled = false;
                    chkPTTDTR.Enabled = false;
                    chkPTTRTS.Enabled = false;
                }
                cmbPTTPort.Text = "External";
                cmbControlBaud.Enabled = false;
                chkControlDTR.Enabled = false;
                chkControlRTS.Enabled = false;
            }
            else if (cmbControlPort.Text == "Via TNC")
            {
                cmbPTTPort.Items.Clear();
                cmbPTTPort.Enabled = false;
                cmbControlBaud.Enabled = true;
                chkControlDTR.Enabled = false;
                chkControlRTS.Enabled = false;
            }
            else
            {
                cmbControlBaud.Enabled = true;
                chkControlDTR.Enabled = true;
                chkControlRTS.Enabled = true;

                cmbPTTPort.Items.Clear();
                cmbPTTPort.Items.Add("External");
                // use VOX control
                cmbPTTPort.Items.Add("CI-V");
                // Icom
                cmbPTTPort.Items.Add("Icom 7100");
                cmbPTTPort.Items.Add("Icom 7200");
                cmbPTTPort.Items.Add("Icom 7600");
                cmbPTTPort.Items.Add("K450");
                cmbPTTPort.Items.Add("K480-Aux");
                // allows two keying options on 480 to select AUX or Mike input
                cmbPTTPort.Items.Add("K480-Mic");
                cmbPTTPort.Items.Add("K590S");
                // Changed in 0.6.3.1 to only select Data mode for 590 (Sound card is built in)
                cmbPTTPort.Items.Add("K2000");
                // may not work
                cmbPTTPort.Items.Add("Y450");
                cmbPTTPort.Items.Add("Y8x7");
                cmbPTTPort.Items.Add("Y950");
                cmbPTTPort.Items.Add("Y1000");
                cmbPTTPort.Items.Add("Y2000");
                cmbPTTPort.Items.Add("Flex");
                cmbPTTPort.Items.Add("K3");
                // verified
                if (strPorts.Length > 0)
                {
                    cmbPTTBaud.Enabled = true;
                    chkPTTDTR.Enabled = true;
                    chkPTTRTS.Enabled = true;
                    foreach (string strPort in strPorts)
                    {
                        cmbPTTPort.Items.Add(strPort);
                    }
                }
                else
                {
                    cmbPTTBaud.Enabled = false;
                    chkPTTDTR.Enabled = false;
                    chkPTTRTS.Enabled = false;
                }
                cmbPTTPort.Text = "External";
            }
        }

        private void cmbModel_TextChanged(System.Object sender, System.EventArgs e)
        {
            cmbAntenna.Enabled = false;
            chkInternalTuner.Enabled = false;
            lblIcomAddress.Enabled = false;
            txtIcomAddress.Enabled = false;
            switch (cmbModel.Text)
            {
                case "Manual":
                    break;

                case "Icom Amateur Radios":
                    // Icom Amateur
                    cmbAntenna.Enabled = true;
                    lblIcomAddress.Enabled = true;
                    txtIcomAddress.Enabled = true;
                    break;
                case "Icom 7100":
                case "Icom 7200":
                    // Icom 7100 and 7200
                    lblIcomAddress.Enabled = true;
                    txtIcomAddress.Enabled = true;
                    break;
                case "Icom 7600":
                    // Icom 7600
                    cmbAntenna.Enabled = true;
                    lblIcomAddress.Enabled = true;
                    txtIcomAddress.Enabled = true;
                    break;
                case "Icom Amateur Radios (Early)":
                    // Icom Amateur (early)
                    cmbAntenna.Enabled = true;
                    lblIcomAddress.Enabled = true;
                    txtIcomAddress.Enabled = true;
                    break;
                case "Icom HF Marine Radios":
                    // Icom Marine
                    cmbAntenna.Enabled = true;
                    lblIcomAddress.Enabled = true;
                    txtIcomAddress.Enabled = true;
                    break;
                case "Kenwood Radios":
                case "Kenwood TS-590S":
                case "Flex Radios":
                    // Kenwood & Flex
                    cmbAntenna.Enabled = true;
                    chkInternalTuner.Enabled = true;
                    break;
                case "Yaesu FT-857":
                    // Yaesu FT-857
                    break;

                case "Yaesu FT-897":
                    // Yaesu FT-897
                    break;

                case "Yaesu FT-1000MP":
                    // Yaesu FT-1000MP
                    break;

                case "Yaesu Other":
                    // Yaesu Other
                    break;

            }
        }

        private void cmbPTTPort_TextChanged(System.Object sender, System.EventArgs e)
        {
            if (cmbPTTPort.Text.StartsWith("COM") == false)
            {
                cmbPTTBaud.Enabled = false;
                chkPTTDTR.Enabled = false;
                chkPTTRTS.Enabled = false;
            }
            else if ((cmbPTTPort.Text == cmbControlPort.Text) & cmbPTTPort.Text.StartsWith("COM"))
            {
                cmbPTTBaud.Enabled = false;
                chkPTTDTR.Enabled = true;
                chkPTTRTS.Enabled = true;
            }
            else
            {
                cmbPTTBaud.Enabled = true;
                chkPTTDTR.Enabled = true;
                chkPTTRTS.Enabled = true;
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



        private bool OpenControlPort()
        {
            if (strControlPort == "Via TNC")
            {
                return true;
            }
            else if (strControlPort == "None")
            {
                return true;
            }
            else
            {
                if (objControlPort != null)
                {
                    if (objControlPort.IsOpen)
                    {
                        objControlPort.Close();
                        Thread.Sleep(200);
                    }
                    objControlPort.Dispose();
                    objControlPort = null;
                }

                blnControlPortOpen = false;
                objControlPort = new SerialPort();
                objControlPort.WriteTimeout = 1000;
                objControlPort.ReceivedBytesThreshold = 1;
                objControlPort.BaudRate = intControlBaud;
                objControlPort.DataBits = 8;
                objControlPort.Parity = Parity.None;
                objControlPort.StopBits = StopBits.Two;
                objControlPort.PortName = strControlPort;
                objControlPort.Handshake = Handshake.None;
                objControlPort.RtsEnable = true;
                objControlPort.DtrEnable = true;
                if ((strModel == "Kenwood Radios" | strModel == "Flex Radios"))
                    objControlPort.NewLine = ";";
                try
                {
                    objControlPort.Open();
                    objControlPort.DiscardInBuffer();
                    objControlPort.DiscardOutBuffer();
                }
                catch
                {
                    Interaction.MsgBox("Unable to open serial port " + strControlPort + ". May be in use by another application...", MsgBoxStyle.Information);
                    return false;
                }
                blnControlPortOpen = objControlPort.IsOpen;
                return objControlPort.IsOpen;
            }
        }

        private bool OpenPTTPort()
        {
            if (strPTTPort.StartsWith("COM") == false)
            {
                objPTTPort = null;
                return true;
            }
            else if (strPTTPort == strControlPort & strPTTPort.StartsWith("COM"))
            {
                objPTTPort = null;
                // use the control ports DTR/RTS for keying
                return true;
                // set up a normal Serial port for PTT.
            }
            else
            {
                try
                {
                    if (objPTTPort != null)
                    {
                        if (objPTTPort.IsOpen)
                        {
                            objPTTPort.Close();
                            Thread.Sleep(200);
                        }
                        objPTTPort.Dispose();
                        objPTTPort = null;
                    }

                    objPTTPort = new SerialPort();
                    objPTTPort.WriteTimeout = 1000;
                    objPTTPort.ReceivedBytesThreshold = 1;
                    objPTTPort.BaudRate = intPTTBaud;
                    objPTTPort.DataBits = 8;
                    objPTTPort.Parity = Parity.None;
                    objPTTPort.StopBits = StopBits.Two;
                    objPTTPort.PortName = strPTTPort;
                    objPTTPort.Handshake = Handshake.None;
                    objPTTPort.RtsEnable = false;
                    objPTTPort.DtrEnable = false;
                    try
                    {
                        objPTTPort.Open();
                    }
                    catch
                    {
                        Interaction.MsgBox("Unable to open serial port " + strPTTPort + ". May be in use by another application...", MsgBoxStyle.Information);
                        return false;
                    }
                    objPTTPort.DiscardInBuffer();
                    objPTTPort.DiscardOutBuffer();
                    return objPTTPort.IsOpen;
                }
                catch
                {
                    return false;
                }
            }
        }

        private bool ProcessChannelSelection(int intHertz = 0)
        {
            return false;
        }

        private void ProcessIcom(int intHertz)
        {
            byte bytCIVAddress = Convert.ToByte("&H" + strIcomAddress);
            byte[] bytCommand = new byte[11];
            string strHertz = null;
            int intPa1 = 0;
            int intPa2 = 0;
            int intPa3 = 0;
            int intPa4 = 0;
            int intPa5 = 0;

            if (intHertz < 1800000)
                return;

            if (strControlPort == "Via TNC")
            {
                if (strModel.StartsWith("Icom HF Marine"))
                {
                    if (RadioCommand != null)
                    {
                        RadioCommand(GetBytes("#TRX TY K " + intControlBaud.ToString() + " - V"));
                    }
                }
                else
                {
                    if (RadioCommand != null)
                    {
                        RadioCommand(GetBytes("#TRX TY I " + intControlBaud.ToString() + " $" + strIcomAddress));
                    }
                }
            }

            switch (strModel)
            {
                case "Icom Amateur Radios":
                case "Icom 7200":
                case "Icom 7100":
                    // Encode HF/VHF frequency...
                    int intDial = 0;
                    if (blnFM == false)
                    {
                        intDial = intHertz;
                    }
                    else
                    {
                        intDial = intHertz;
                    }
                    strHertz = Strings.Format(intDial, "000000000");
                    Globals.EventLog("[ProcessIcom] Set Freq:" + strHertz);
                    intPa1 = 16 * Convert.ToInt32(strHertz.Substring(7, 1)) + Convert.ToInt32(strHertz.Substring(8, 1));
                    intPa2 = 16 * Convert.ToInt32(strHertz.Substring(5, 1)) + Convert.ToInt32(strHertz.Substring(6, 1));
                    intPa3 = 16 * Convert.ToInt32(strHertz.Substring(3, 1)) + Convert.ToInt32(strHertz.Substring(4, 1));
                    intPa4 = 16 * Convert.ToInt32(strHertz.Substring(1, 1)) + Convert.ToInt32(strHertz.Substring(2, 1));
                    if (intDial > 0)
                    {
                        intPa5 = Convert.ToInt32(strHertz.Substring(0, 1));
                    }
                    else
                    {
                        intPa5 = 0;
                    }

                    // Select HF/VHF frequency...
                    bytCommand[0] = 0xfe;
                    bytCommand[1] = 0xfe;
                    bytCommand[2] = bytCIVAddress;
                    bytCommand[3] = 0xf1;
                    bytCommand[4] = 0x5;
                    // Set frequency command
                    bytCommand[5] = Convert.ToByte(intPa1);
                    bytCommand[6] = Convert.ToByte(intPa2);
                    bytCommand[7] = Convert.ToByte(intPa3);
                    bytCommand[8] = Convert.ToByte(intPa4);
                    bytCommand[9] = Convert.ToByte(intPa5);
                    bytCommand[10] = 0xfd;
                    SendBinaryCommand(bytCommand);
                    if (blnFM == false)
                    {
                        // Select USB mode
                        bytCommand = new byte[7];
                        bytCommand[0] = 0xfe;
                        bytCommand[1] = 0xfe;
                        bytCommand[2] = bytCIVAddress;
                        bytCommand[3] = 0xf1;
                        bytCommand[4] = 0x6;
                        bytCommand[5] = 0x1;
                        // command to set USB mode
                        bytCommand[6] = 0xfd;
                        SendBinaryCommand(bytCommand, 100);
                        // set USB mode

                        if (strModel == "Icom 7200")
                        {
                            Array.Resize(ref bytCommand, 9);
                            bytCommand[4] = 0x1a;
                            bytCommand[5] = 0x4;
                            bytCommand[8] = 0xfd;
                            if (blnUSB)
                            {
                                bytCommand[6] = 0x0;
                                // command to exit data mode
                                bytCommand[7] = 0x0;
                                // command to exit data mode
                            }
                            else if (blnUSBDigital)
                            {
                                bytCommand[6] = 0x1;
                                // command to enter data mode
                                bytCommand[7] = 0x1;
                                // command to enter data mode
                            }
                        }
                        else
                        {
                            Array.Resize(ref bytCommand, 8);
                            bytCommand[4] = 0x1a;
                            bytCommand[5] = 0x6;
                            bytCommand[7] = 0xfd;
                            if (blnUSB)
                            {
                                bytCommand[6] = 0x0;
                                // command to exit data mode
                            }
                            else if (blnUSBDigital)
                            {
                                bytCommand[6] = 0x1;
                                // command to enter data mode
                            }
                        }
                        SendBinaryCommand(bytCommand, 100);
                        // set or clear digital mode
                    }
                    else
                    {
                        // Select VHF FM mode (ICOM-7000 and similar)...
                        bytCommand = new byte[7];
                        bytCommand[0] = 0xfe;
                        bytCommand[1] = 0xfe;
                        bytCommand[2] = bytCIVAddress;
                        bytCommand[3] = 0xf1;
                        bytCommand[4] = 6;
                        // Set mode
                        bytCommand[5] = 5;
                        // Set FM
                        bytCommand[6] = 0xfd;
                        SendBinaryCommand(bytCommand, 100);
                    }

                    break;
                case "Icom Amateur Radios (Early)":
                    // Encode HF frequency...
                    strHertz = Strings.Format(intHertz, "00000000");
                    intPa1 = 16 * Convert.ToInt32(strHertz.Substring(6, 1)) + Convert.ToInt32(strHertz.Substring(7, 1));
                    intPa2 = 16 * Convert.ToInt32(strHertz.Substring(4, 1)) + Convert.ToInt32(strHertz.Substring(5, 1));
                    intPa3 = 16 * Convert.ToInt32(strHertz.Substring(2, 1)) + Convert.ToInt32(strHertz.Substring(3, 1));
                    intPa4 = 16 * Convert.ToInt32(strHertz.Substring(0, 1)) + Convert.ToInt32(strHertz.Substring(1, 1));

                    // Select HF frequency...
                    bytCommand = new byte[10];
                    bytCommand[0] = 0xfe;
                    bytCommand[1] = 0xfe;
                    bytCommand[2] = bytCIVAddress;
                    bytCommand[3] = 0xf1;
                    bytCommand[4] = 0x5;
                    // Set frequency command
                    bytCommand[5] = Convert.ToByte(intPa1);
                    bytCommand[6] = Convert.ToByte(intPa2);
                    bytCommand[7] = Convert.ToByte(intPa3);
                    bytCommand[8] = Convert.ToByte(intPa4);
                    bytCommand[9] = 0xfd;
                    SendBinaryCommand(bytCommand, 100);

                    break;
                // Select USB mode...  ' Mode command commented out in 1.0.0.2
                //ReDim bytCommand(6)
                //bytCommand(0) = &HFE
                //bytCommand(1) = &HFE
                //bytCommand(2) = bytCIVAddress
                //bytCommand(3) = &HF1
                //bytCommand(4) = &H6  ' Mode command
                //bytCommand(5) = &H1  ' USB (was missing in rev 0.5.7.0 which set to LSB)
                //bytCommand(6) = &HFD
                //SendBinaryCommand(bytCommand)

                case "Icom HF Marine Radios":
                    // Send radio commands in NMEA format...
                    SendNMEACommand(ComputeNMEACommand("REMOTE,ON"));
                    SendNMEACommand(ComputeNMEACommand("MODE,USB"));
                    SendNMEACommand(ComputeNMEACommand("RXF," + Strings.Format((intHertz) / 1000000, "#0.000000")));
                    SendNMEACommand(ComputeNMEACommand("TXF," + Strings.Format((intHertz) / 1000000, "#0.000000")));
                    break;
            }
        }

        private void ProcessKenwood(int intHertz)
        {
            string strHertz = null;

            if (intHertz < 1800000)
                return;

            if (strControlPort == "Via TNC")
            {
                if (RadioCommand != null)
                {
                    RadioCommand(GetBytes("#TRX TY K " + intControlBaud.ToString() + " A V24"));
                }
            }

            if (blnFM == false)
            {
                strHertz = Strings.Format(intHertz, "00000000000");
                //SendASCIICommand("BC0;")                  ' Beat canceller off
                //SendASCIICommand("NB0;")                  ' Noise blanker off
                //SendASCIICommand("NR0;")                  ' Noise reducer off
                SendASCIICommand("FR0;");
                // Select VFO A receive, non split
                //SendASCIICommand("FT0;")                  ' Select VFO A transmit  Removed Chat 0.6.4.0
                SendASCIICommand("RC;");
                SendASCIICommand("FA" + strHertz + ";");
                // Select frequency
                if (blnUSB)
                {
                    SendASCIICommand("MD2;");
                    // Select USB
                    if (strModel == "Kenwood TS-590S")
                        SendASCIICommand("DA0;");
                    // Select Data off
                }
                else if (blnUSBDigital)
                {
                    if (strModel == "Kenwood TS-590S")
                    {
                        SendASCIICommand("MD2;");
                        // Select USB
                        SendASCIICommand("DA1;");
                        // Select DataSB
                    }
                    else
                    {
                        SendASCIICommand("MD9;");
                        // (switches the Flex radios to Digtial mode)
                    }

                }

                if (strAntennaSelection != "Default")
                {
                    if (strAntennaSelection == "Internal 1")
                    {
                        if (strModel == "Kenwood TS-590S")
                        {
                            SendASCIICommand("AN199;");
                        }
                        else
                        {
                            SendASCIICommand("AN1;");
                        }

                    }
                    else if (strAntennaSelection == "Internal 2")
                    {
                        if (strModel == "Kenwood TS-590S")
                        {
                            SendASCIICommand("AN299;");
                        }
                        else
                        {
                            SendASCIICommand("AN2;");
                        }
                    }
                }
                if (blnTuner)
                {
                    SendASCIICommand("AC111;");
                    //Else
                    //    SendASCIICommand("AC000;") ' Commented out 0.1.6.1
                }
            }
            else
            {
                strHertz = Strings.Format(intHertz, "00000000000");
                SendASCIICommand("MD4;");
                // Select FM
                SendASCIICommand("FR0;");
                // Select VFO A receive, non split
                //SendASCIICommand("FT0;")                  ' Select VFO A transmit Disable Chat 0.6.4.0
                SendASCIICommand("FA" + strHertz + ";");
                // Select frequency
            }
        }

        private void ProcessYaesu(int intHertz)
        {
            if (intHertz < 1800000)
                return;
            string strHertz = null;
            byte[] bytCommand = {
			0,
			0,
			0,
			0,
			0
		};

            if (blnFM)
                return;
            // FM not supported
            if (strControlPort == "Via TNC")
            {
                if (RadioCommand != null)
                {
                    RadioCommand(GetBytes("#TRX TY Y " + intControlBaud.ToString() + " A V24"));
                }
            }
            SendBinaryCommand(bytCommand);
            // CAT ON  (needed for FT-847 and others?) 
            strHertz = Strings.Format(intHertz, "000000000");
            switch (strModel)
            {
                case "Yaesu FT-450":
                    if (intHertz < 1800000)
                        return;
                    strHertz = Strings.Format(intHertz, "00000000");
                    if (blnUSBDigital)
                        SendASCIICommand("MD0C;");
                    // Select User U ' This confirmed to be correct for FT-450
                    if (blnUSB)
                        SendASCIICommand("MD02;");
                    // Select USB
                    SendASCIICommand("FA" + strHertz + ";");
                    // Select frequency
                    break;
                case "Yaesu FT-2000":
                case "Yaesu FT-950":
                    if (intHertz < 1800000)
                        return;
                    strHertz = Strings.Format(intHertz, "00000000");
                    if (blnUSB)
                        SendASCIICommand("MD02;");
                    // Select USB
                    if (blnUSBDigital)
                        SendASCIICommand("MD0C;");
                    // Select USB/Data
                    SendASCIICommand("FA" + strHertz + ";");
                    // Select frequency
                    break;
                case "Yaesu FT-857":
                    if (blnUSB)
                        bytCommand[0] = 1;
                    // Set USB...
                    if (blnUSBDigital)
                        bytCommand[0] = 0xa;
                    // Set USB Digital...
                    bytCommand[4] = 7;
                    SendBinaryCommand(bytCommand);

                    // Turn off split for 857...
                    bytCommand[0] = 0;
                    bytCommand[4] = 0x82;
                    SendBinaryCommand(bytCommand);

                    // Set frequency...
                    bytCommand[0] = Convert.ToByte(Convert.ToInt32(strHertz.Substring(0, 1)) * 16 + Convert.ToInt32(strHertz.Substring(1, 1)));
                    bytCommand[1] = Convert.ToByte(Convert.ToInt32(strHertz.Substring(2, 1)) * 16 + Convert.ToInt32(strHertz.Substring(3, 1)));
                    bytCommand[2] = Convert.ToByte(Convert.ToInt32(strHertz.Substring(4, 1)) * 16 + Convert.ToInt32(strHertz.Substring(5, 1)));
                    bytCommand[3] = Convert.ToByte(Convert.ToInt32(strHertz.Substring(6, 1)) * 16 + Convert.ToInt32(strHertz.Substring(7, 1)));
                    bytCommand[4] = 1;
                    SendBinaryCommand(bytCommand);
                    // Set the frequency 
                    break;

                case "Yaesu FT-897":
                case "Yaesu FT-817":
                case "Yaesu FT-847":
                    // Set USB...
                    if (blnUSB)
                        bytCommand[0] = 1;
                    if (blnUSBDigital)
                        bytCommand[0] = 0xa;
                    bytCommand[4] = 7;
                    SendBinaryCommand(bytCommand);

                    // Set frequency...
                    bytCommand[0] = Convert.ToByte(Convert.ToInt32(strHertz.Substring(0, 1)) * 16 + Convert.ToInt32(strHertz.Substring(1, 1)));
                    bytCommand[1] = Convert.ToByte(Convert.ToInt32(strHertz.Substring(2, 1)) * 16 + Convert.ToInt32(strHertz.Substring(3, 1)));
                    bytCommand[2] = Convert.ToByte(Convert.ToInt32(strHertz.Substring(4, 1)) * 16 + Convert.ToInt32(strHertz.Substring(5, 1)));
                    bytCommand[3] = Convert.ToByte(Convert.ToInt32(strHertz.Substring(6, 1)) * 16 + Convert.ToInt32(strHertz.Substring(7, 1)));
                    bytCommand[4] = 1;
                    SendBinaryCommand(bytCommand);
                    // Set the frequency 
                    break;

                case "Yaesu FT-600":
                case "Yaesu FT-840":
                case "Yaesu FT-100":
                    bytCommand[0] = 0;
                    bytCommand[1] = 0;
                    bytCommand[2] = 0;
                    if (blnUSB)
                        bytCommand[3] = 1;
                    // USB 
                    if (blnUSBDigital)
                        bytCommand[3] = 0xa;
                    // Data USB
                    bytCommand[4] = 0xc;
                    SendBinaryCommand(bytCommand);

                    bytCommand[0] = Convert.ToByte(Convert.ToInt32(strHertz.Substring(6, 1)) * 16 + Convert.ToInt32(strHertz.Substring(7, 1)));
                    bytCommand[1] = Convert.ToByte(Convert.ToInt32(strHertz.Substring(4, 1)) * 16 + Convert.ToInt32(strHertz.Substring(5, 1)));
                    bytCommand[2] = Convert.ToByte(Convert.ToInt32(strHertz.Substring(2, 1)) * 16 + Convert.ToInt32(strHertz.Substring(3, 1)));
                    bytCommand[3] = Convert.ToByte(Convert.ToInt32(strHertz.Substring(0, 1)) * 16 + Convert.ToInt32(strHertz.Substring(1, 1)));
                    bytCommand[4] = 10;
                    SendBinaryCommand(bytCommand);

                    // Set frequency...
                    bytCommand[0] = Convert.ToByte(Convert.ToInt32(strHertz.Substring(6, 1)) * 16 + Convert.ToInt32(strHertz.Substring(7, 1)));
                    bytCommand[1] = Convert.ToByte(Convert.ToInt32(strHertz.Substring(4, 1)) * 16 + Convert.ToInt32(strHertz.Substring(5, 1)));
                    bytCommand[2] = Convert.ToByte(Convert.ToInt32(strHertz.Substring(2, 1)) * 16 + Convert.ToInt32(strHertz.Substring(3, 1)));
                    bytCommand[3] = Convert.ToByte(Convert.ToInt32(strHertz.Substring(0, 1)) * 16 + Convert.ToInt32(strHertz.Substring(1, 1)));
                    bytCommand[4] = 0x8a;
                    SendBinaryCommand(bytCommand);

                    break;
                default:
                    // Set USB...
                    bytCommand[0] = 0;
                    bytCommand[1] = 0;
                    bytCommand[2] = 0;
                    if (blnUSB)
                        bytCommand[3] = 1;
                    // USB 
                    if (blnUSBDigital)
                        bytCommand[3] = 0xa;
                    // Data USB
                    bytCommand[4] = 0xc;
                    SendBinaryCommand(bytCommand);

                    bytCommand[0] = Convert.ToByte(Convert.ToInt32(strHertz.Substring(6, 1)) * 16 + Convert.ToInt32(strHertz.Substring(7, 1)));
                    bytCommand[1] = Convert.ToByte(Convert.ToInt32(strHertz.Substring(4, 1)) * 16 + Convert.ToInt32(strHertz.Substring(5, 1)));
                    bytCommand[2] = Convert.ToByte(Convert.ToInt32(strHertz.Substring(2, 1)) * 16 + Convert.ToInt32(strHertz.Substring(3, 1)));
                    bytCommand[3] = Convert.ToByte(Convert.ToInt32(strHertz.Substring(0, 1)) * 16 + Convert.ToInt32(strHertz.Substring(1, 1)));
                    bytCommand[4] = 0xa;
                    SendBinaryCommand(bytCommand);
                    break;
            }
        }

        private void ProcessMicom(int intHertz = 0)
        {
        }

        private void ProcessElecraft(int intHertz)
        {
            string strHertz = null;

            if (intHertz < 1800000)
                return;

            if (strControlPort == "Via TNC")
            {
                if (RadioCommand != null)
                {
                    RadioCommand(GetBytes("#TRX TY K " + intControlBaud.ToString() + " A V24"));
                }
            }

            if (blnFM == false)
            {
                strHertz = Strings.Format(intHertz, "00000000000");
                SendASCIICommand("FA" + strHertz + ";", 100);
                // Select frequency
                SendASCIICommand("FR0;", 100);
                // Select VFO A (split off)
                SendASCIICommand("RT0;", 100);
                // RIT off
                SendASCIICommand("XT0;", 100);
                // XIT off

                if (blnUSB)
                    SendASCIICommand("MD2;", 0);
                // Select USB
                if (blnUSBDigital)
                {
                    SendASCIICommand("MD6;", 100);
                    // Select USB Digital
                    SendASCIICommand("DT0;", 0);
                }
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
                            RadioCommand(GetBytes(ComputeNMEACommand("REMOTE,OFF")));
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

        private void SetRadio()
        {
            if (strControlPort != "None")
            {
                if (objControlPort != null)
                {
                    if (objControlPort.IsOpen & objControlPort.PortName == strControlPort & objControlPort.BaudRate == intControlBaud)
                    {
                        // Do nothing - the port is OK as is...
                    }
                    else
                    {
                        if (OpenControlPort() == false)
                            return;
                        ReadMuxState();
                    }
                }
                else
                {
                    if (OpenControlPort() == false)
                        return;
                    ReadMuxState();
                }

                if (objControlPort != null)
                {
                    objControlPort.DtrEnable = blnControlDTR;
                    objControlPort.RtsEnable = blnControlRTS;
                }
            }


            if (objPTTPort != null)
            {
                if (objPTTPort.IsOpen & objPTTPort.PortName == strPTTPort & objPTTPort.BaudRate == intPTTBaud)
                {
                    // Do nothing - the port is OK as is...
                }
                else
                {
                    OpenPTTPort();
                }
            }
            else
            {
                OpenPTTPort();
            }
            PTT(false);
            if ((strControlPort == "None" | intHertz == 0))
                return;
            if (strModel.StartsWith("Icom"))
            {
                ProcessIcom(intHertz);
            }
            else if ((strModel.StartsWith("Kenwood") | strModel.StartsWith("Flex")))
            {
                ProcessKenwood(intHertz);
            }
            else if (strModel.StartsWith("Yaesu"))
            {
                ProcessYaesu(intHertz);
            }
            else if (strModel.StartsWith("Elecraft"))
            {
                ProcessElecraft(intHertz);
            }
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
                        RadioCommand(GetBytes("#TRX T " + strCommand));
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
                        RadioCommand(ConcatByteArrays(GetBytes("#TRX T "), bytCommand));
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
                        RadioCommand(GetBytes(strCommand));
                    }
                }
                else
                {
                    if (objControlPort != null)
                        objControlPort.Write(GetBytes(strCommand), 0, strCommand.Length);
                }
                Thread.Sleep(100);
                return true;
            }
            catch
            {
                return false;
            }
        }


        private void ShowPropertyValues()
        {
            chkControlDTR.Checked = blnControlDTR;
            chkControlRTS.Checked = blnControlRTS;
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
                chkControlRTS.Enabled = false;
            }
            if (cmbPTTPort.Text.StartsWith("COM") == false)
            {
                cmbPTTBaud.Enabled = false;
                chkPTTDTR.Enabled = false;
                chkPTTRTS.Enabled = false;
            }
        }


        private void WriteRadioSettings()
        {
            string strSection = "Radio";


            Globals.objINIFile.WriteInteger(strSection, "PTT Baud", Convert.ToInt32(intPTTBaud.ToString()));
            Globals.objINIFile.WriteInteger(strSection, "Control Baud", Convert.ToInt32(intControlBaud.ToString()));
            Globals.objINIFile.WriteString(strSection, "Antenna Selection", strAntennaSelection);
            Globals.objINIFile.WriteString(strSection, "Control DTR", blnControlDTR.ToString());
            Globals.objINIFile.WriteString(strSection, "Control RTS", blnControlRTS.ToString());
            Globals.objINIFile.WriteString(strSection, "Control Port", strControlPort);
            Globals.objINIFile.WriteString(strSection, "FM", blnFM.ToString());
            Globals.objINIFile.WriteString(strSection, "USB", blnUSB.ToString());
            Globals.objINIFile.WriteString(strSection, "USBDigital", blnUSBDigital.ToString());
            Globals.objINIFile.WriteString(strSection, "Icom Address", strIcomAddress);
            Globals.objINIFile.WriteString(strSection, "Model", strModel);
            Globals.objINIFile.WriteString(strSection, "PTT Port", strPTTPort);
            Globals.objINIFile.WriteString(strSection, "PTT DTR", blnPTTDTR.ToString());
            Globals.objINIFile.WriteString(strSection, "PTT RTS", blnPTTRTS.ToString());
            Globals.objINIFile.WriteString(strSection, "Tuner", blnTuner.ToString());
            Globals.objINIFile.Flush();

        }


        private void Radio_RadioCommand(byte[] bytCommand)
        {
        }

        private void ProcessRadioResponse(string strResponse)
        {
            string strHz = "";
            Debug.WriteLine("RadioResponse:" + strResponse);
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
                        Debug.WriteLine("response: " + strResponse);
                        strHz = strResponse.Substring(2 + strResponse.IndexOf("IF"));
                        strHz = strHz.Substring(0, strHz.IndexOf(" ")).Trim();
                        Debug.WriteLine("strHz: " + strHz);
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


        private void Radio_Resize(object sender, System.EventArgs e)
        {
        }


        private void btnUpdate_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
        {
        }


        private void cmbAntenna_SelectedIndexChanged(System.Object sender, System.EventArgs e)
        {
        }

        private void btnHelpRadio_Click(System.Object sender, System.EventArgs e)
        {
            // Get the Chat Radio setup topic (index of 50)
            try
            {
                Help.ShowHelp(this, Globals.strExecutionDirectory + "Chat Help.chm", HelpNavigator.Topic, "html\\hs50.htm");
            }
            catch
            {
                //Exception("[Chat Radio Setup Help] " + Err.Description);
            }
        }


        private void cmbModel_SelectedIndexChanged(System.Object sender, System.EventArgs e)
        {
        }


        private void cmbPTTPort_SelectedIndexChanged(System.Object sender, System.EventArgs e)
        {
        }
    }
}
