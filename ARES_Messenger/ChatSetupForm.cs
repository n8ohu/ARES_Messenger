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
using Microsoft.DirectX;
using Microsoft.DirectX.DirectSound;

namespace ARES_Messenger
{
    public partial class ChatSetupForm : Form
    {
        
        private struct DeviceDescription
        {

            public DeviceInformation DeviceInformation;
            public override string ToString()
            {
                return DeviceInformation.Description.ToString().Trim();
            }

            public DeviceDescription(DeviceInformation objDeviceInformation)
            {
                DeviceInformation = objDeviceInformation;
            }
        }


        private void H4Setup_Load(object sender, System.EventArgs e)
	{
		//  CODEC setup...
		CaptureDevicesCollection cllCaptureDevices = new CaptureDevicesCollection();
		DevicesCollection cllPlaybackDevices = new DevicesCollection();
		//DeviceInformation objDeviceInformation = default(DeviceInformation);

		// Capture devices...

		foreach (DeviceInformation objDeviceInformation in cllCaptureDevices) {
			DeviceDescription objDeviceDescription = new DeviceDescription(objDeviceInformation);
			if (objDeviceDescription.ToString().Trim() != "Primary Sound Capture Driver") {
				cmbCapture.Items.Add(objDeviceDescription.ToString().Trim() + "-" + objDeviceInformation.DriverGuid.ToString().Substring(objDeviceInformation.DriverGuid.ToString().Length - 2));
			}
		}

		// Playback devices...
		//cllPlaybackDevices.Reset()
		foreach ( DeviceInformation objDeviceInformation in cllPlaybackDevices) {
			DeviceDescription objDeviceDescription = new DeviceDescription(objDeviceInformation);
			if (objDeviceDescription.ToString().Trim() != "Primary Sound Driver") {
				cmbPlayback.Items.Add(objDeviceDescription.ToString().Trim() + "-" + objDeviceInformation.DriverGuid.ToString().Substring(objDeviceInformation.DriverGuid.ToString().Length - 2));
			}
		}

		chkMorseCodeId.Checked = Globals.blnMorseId;
		cmbCapture.Text = Globals.strCaptureDevice;
		cmbPlayback.Text = Globals.strPlaybackDevice;
		cmbCharacterSet.Text = Globals.strCharacterSet;
		nudTNCTCPIPPort.Value = Globals.intTCPIPPort;
		nudARQTimeout.Value = Globals.intARQTimeout;
		nudDriveLevel.Value = Globals.intH4DriveLevel;
		nudFont.Value = Globals.intFontSize;
		txtTCPIPAddress.Text = Globals.strTCPIPAddress;
		txtCall.Text = Globals.strMyCallsign;
		txtARQID.Text = Globals.strMyARQID;
		rdoCR.Checked = Globals.blnSendCR;
		rdoCtrlCR.Checked = Globals.blnSendCtrlCr;
		rdoSpace.Checked = Globals.blnSendSpace;
		rdoWord.Checked = Globals.blnSendWord;
		nudTuning.Value = Globals.intTuning;
		chkDebugLog.Checked = Globals.blnEnableDebugLogs;
		chkEnbAutoupdate.Checked = Globals.blnEnableAutoupdate;
		nudFrontPorch.Value = Globals.intFrontPorch;
		nudBackPorch.Value = Globals.intBackPorch;
	}


        
        public ChatSetupForm()
        {
            Load += H4Setup_Load;
            // This call is required by the Windows Form Designer.
            InitializeComponent();

            // Add any initialization after the InitializeComponent() call.


        }

        private void btnCancel_Click(System.Object sender, System.EventArgs e)
        {
            if (Globals.intH4DriveLevel != Convert.ToInt32(nudDriveLevel.Value))
            {
                Globals.objMain.SendCommandToTNC("DRIVELEVEL " + Globals.intH4DriveLevel.ToString());
            }
            this.Close();
        }

        private void btnUpdate_Click(System.Object sender, System.EventArgs e)
        {
            if (txtCall.Text.Trim().Length > 10 | !Globals.IsValidCallsign(txtCall.Text.Trim().ToUpper()))
            {
                Interaction.MsgBox("Call sign must be legitimate Ham or Mars call sign with optional -ssid (e.g. '-8') " + "with a total length of 7 characters + optional 1 character SSID or less.", MsgBoxStyle.Information, "Invalid Call Sign!");
                return;
            }
            if (!string.IsNullOrEmpty(txtARQID.Text.Trim()))
            {
                if (txtARQID.Text.Trim().Length > 18)
                {
                    Interaction.MsgBox("Max total length of ARQ ID is 18 characters!", MsgBoxStyle.Exclamation, "ARQ ID too large");
                    return;
                }
                if (txtARQID.Text.Trim().IndexOf("<") != -1)
                {
                    Interaction.MsgBox("ARQ ID may not contain the character '<' !", MsgBoxStyle.Exclamation, "ARQ ID Syntax error");
                    return;
                }
            }
            if (!(rdoCR.Checked | rdoCtrlCR.Checked | rdoSpace.Checked | rdoWord.Checked))
            {
                Interaction.MsgBox("One of the Send to OB Queue options must be checked.  Understanding how to queue out bound data" + Constants.vbCr + "is essential for proper Chat operation. Click the Setup Help for complete details.", MsgBoxStyle.Exclamation, "No OB Data Queue Selected!");
                return;
            }
            Globals.blnMorseId = this.chkMorseCodeId.Checked;
            Globals.objINIFile.WriteString("ARDOP Chat", "MorseID", Globals.blnMorseId.ToString());
            Globals.strCaptureDevice = cmbCapture.Text;
            Globals.objINIFile.WriteString("ARDOP Chat", "CaptureDevice", Globals.strCaptureDevice);
            Globals.strPlaybackDevice = cmbPlayback.Text;
            Globals.objINIFile.WriteString("ARDOP Chat", "PlaybackDevice", Globals.strPlaybackDevice);
            if (rdoTCPIP.Checked)
            {
                Globals.intTCPIPPort = Convert.ToInt32(nudTNCTCPIPPort.Value);
                Globals.objINIFile.WriteInteger("ARDOP Chat", "TCPIPPort", Globals.intTCPIPPort);
                Globals.strTCPIPAddress = txtTCPIPAddress.Text.Trim();
                Globals.objINIFile.WriteString("ARDOP Chat", "TCPIPAddress", Globals.strTCPIPAddress);

            }
            else
            {
            }

            Globals.intARQTimeout = Convert.ToInt32(nudARQTimeout.Value);
            Globals.objINIFile.WriteInteger("ARDOP Chat", "ARQTimeout", Globals.intARQTimeout);
            Globals.intH4DriveLevel = Convert.ToInt32(nudDriveLevel.Value);
            Globals.objINIFile.WriteInteger("ARDOP Chat", "DriveLevel", Globals.intH4DriveLevel);

            Globals.strCharacterSet = cmbCharacterSet.Text;
            Globals.objINIFile.WriteString("ARDOP Chat", "CharacterSet", Globals.strCharacterSet);
            Globals.strMyCallsign = this.txtCall.Text.ToUpper().Trim();
            Globals.objINIFile.WriteString("ARDOP Chat", "MyCallSign", Globals.strMyCallsign);
            Globals.strMyARQID = this.txtARQID.Text.Trim();
            Globals.objINIFile.WriteString("ARDOP Chat", "MyARQID", Globals.strMyARQID);
            Globals.intTuning = Convert.ToInt32(nudTuning.Value);
            Globals.objINIFile.WriteInteger("ARDOP Chat", "Tuning", Globals.intTuning);
            Globals.blnEnableDebugLogs = chkDebugLog.Checked;
            Globals. objINIFile.WriteString("ARDOP Chat", "EnableDebugLogs", Globals.blnEnableDebugLogs.ToString());
            Globals.blnEnableAutoupdate = chkEnbAutoupdate.Checked;
            Globals.objINIFile.WriteString("ARDOP Chat", "EnableAutoUpdate", Globals.blnEnableAutoupdate.ToString());
            Globals.blnSendCR = rdoCR.Checked;
            Globals.objINIFile.WriteString("ARDOP Chat", "SendCR", Globals.blnSendCR.ToString());
            Globals.blnSendCtrlCr = rdoCtrlCR.Checked;
            Globals.objINIFile.WriteString("ARDOP Chat", "SendCtrlCR", Globals.blnSendCtrlCr.ToString());
            Globals.blnSendSpace = rdoSpace.Checked;
            Globals.objINIFile.WriteString("ARDOP Chat", "SendSpace", Globals.blnSendSpace.ToString());
            Globals.blnSendWord = rdoWord.Checked;
            Globals.objINIFile.WriteString("ARDOP Chat", "SendWord", Globals.blnSendWord.ToString());
            Globals.intFontSize = Convert.ToInt32(nudFont.Value);
            Globals.objINIFile.WriteInteger("ARDOP Chat", "TextBoxFontSize", Globals.intFontSize);
            Globals.intFrontPorch = Convert.ToInt32(nudFrontPorch.Value);
            Globals.objINIFile.WriteInteger("ARDOP Chat", "FrontPorch", Globals.intFrontPorch);
            Globals.intBackPorch = Convert.ToInt32(nudBackPorch.Value);
            Globals.objINIFile.WriteInteger("ARDOP Chat", "BackPorch", Globals.intBackPorch);
            Globals.objINIFile.Flush();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }


        private void btnRadio_Click(System.Object sender, System.EventArgs e)
        {
            if ((Globals.objRadio == null))
            {
                Globals.objRadio = new Radio();
            }
            if (Globals.objRadio.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Globals.objRadio.Close();
                Globals.objRadio = null;
                Globals.objRadio = new Radio();
                Globals.objRadio.SetFrequency(0);
                // illegal frequency to set up ports.
            }
        }


        private void btnTwoToneTest_Click(System.Object sender, System.EventArgs e)
        {
        }

        private void btnTwoTone_Click(System.Object sender, System.EventArgs e)
        {
            Globals.objMain.SendCommandToTNC("DRIVELEVEL " + nudDriveLevel.Value.ToString());
            Globals.objMain.SendCommandToTNC("TWOTONETEST");
        }


        private void btnHelp_Click(System.Object sender, System.EventArgs e)
        {
            // Get the Chat setup topic (index of 40)
            try
            {
                Help.ShowHelp(this, Globals.strExecutionDirectory + "Chat Help.chm", HelpNavigator.Topic, "html\\hs40.htm");
            }
            catch
            {
                //Globals.Exception("[Chat Setup Help] " + Err.Description);
            }
        }


        private void chkFullDup_CheckedChanged(System.Object sender, System.EventArgs e)
        {
        }

        private void rdoTCPIP_CheckedChanged(System.Object sender, System.EventArgs e)
        {
            if (rdoTCPIP.Checked)
            {
                lblPairing.Enabled = false;
                txtPairing.Enabled = false;
                lblTNCTCPIPPort.Enabled = true;
                lblHostAddress.Enabled = true;
                txtTCPIPAddress.Enabled = true;
                nudTNCTCPIPPort.Enabled = true;
            }
            else
            {
                lblPairing.Enabled = true;
                txtPairing.Enabled = true;
                lblTNCTCPIPPort.Enabled = false;
                lblHostAddress.Enabled = false;
                txtTCPIPAddress.Enabled = false;
                nudTNCTCPIPPort.Enabled = false;
            }
        }

    }
}
