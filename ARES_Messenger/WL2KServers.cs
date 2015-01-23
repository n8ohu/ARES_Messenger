using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Timers;
using System.Threading;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Windows.Forms;

namespace ARES_Messenger
{
    public class CMSInfo
    {
        /*'
         Holds information about the active CMS site
        */
        public string strCMSName;
        public string strCMSIP;
        public string strCMSCity;

        public CMSInfo (string n, string city, string ip)
        {    
            string strCMSCity = city;
            strCMSName = n;
            strCMSIP = ip;
        }
    }

    public class WL2KServers
    {

        public static bool ValidateConn(string strIPAddr, int intPort)
        {
            //
            // Return true if the TCP connection was successful.
            //
            using (TcpClient objConn = new TcpClient())
            {
                //
                // Wrap the socket in a using block to dispose the object and resources as soon as we are done with it.
                //
                try
                {
                    LingerOption objLO = new LingerOption(true, 0);
                    objConn.LingerState = objLO;
                    objConn.Connect(strIPAddr, intPort);
                    objConn.Client.Close();
                    return true;
                }
                catch (Exception ex)
                {
                }
            }
            //
            // Connection failed
            //
            return false;
        }

        private CMSServer[] CMSSites = {
		new CMSServer("Halifax.Winlink.org", "142.177.196.170", "Halifax", CMSServer.ServerType.CMS),
		new CMSServer("Perth.Winlink.org", "203.59.28.61", "Perth", CMSServer.ServerType.CMS),
		new CMSServer("SanDiego.Winlink.org", "204.8.136.30", "San Diego", CMSServer.ServerType.CMS),
		new CMSServer("Wien.Winlink.org", "212.69.162.229", "Wien", CMSServer.ServerType.CMS),
		new CMSServer("Brentwood.Winlink.org", "174.46.122.124", "Brentwood", CMSServer.ServerType.CMS),
		new CMSServer("Server.Winlink.org", "", "Server", CMSServer.ServerType.CMS),
		new CMSServer("www.Winlink.org", "66.11.115.106", "Winlink", CMSServer.ServerType.WLW),
		new CMSServer("Winlink.org", "66.11.115.106", "Winlink", CMSServer.ServerType.WLW),
		new CMSServer("Autoupdate.Winlink.org", "142.177.196.170", "Autoupdate", CMSServer.ServerType.AU)

	};
        private int intCMSSiteCount;

        private int intCurrentSiteIndex = 0;

        private object objSyncLock = new object();
        public WL2KServers()
        {
            //
            // Initialize the list of CMS sites
            //
            intCMSSiteCount = CMSSites.Length;
            //
            // Pick a random CMS to start with based on the second component of the current time
            //
            intCurrentSiteIndex = DateTime.Now.Second % intCMSSiteCount;

        }

        public void Close()
        {
            foreach (CMSServer objCMS in CMSSites)
            {
                objCMS.Close();
            }
        }

        public CMSInfo GetWLWHost(bool blnSkip = false)
        {
            return GetHost(blnSkip, false, CMSServer.ServerType.WLW);
        }

        public CMSInfo GetCMSHost(bool blnSkip = false, bool blnValidate = false)
        {
            return GetHost(blnSkip, blnValidate, CMSServer.ServerType.CMS);
        }

        public CMSInfo GetAUHost()
        {
            return GetHost(false, false, CMSServer.ServerType.AU);
        }

        private CMSInfo GetHost(bool blnSkip, bool blnTestConn, CMSServer.ServerType enmServerType)
        {
            //
            // Returns the name of the current CMS site.  If the current site is not available, we'll
            // attempt to locate another one
            //
            int intOffset = 0;
            if (blnSkip)
            {
                intCurrentSiteIndex = (intCurrentSiteIndex + 1) % (intCMSSiteCount);
            }

            while (intOffset < intCMSSiteCount)
            {
                //
                // Check the CMS info block to see if the site is active
                //
                if (enmServerType == CMSSites[intCurrentSiteIndex].enmServerType)
                {
                    //
                    // Check the status
                    //
                    if (blnTestConn)
                    {
                        CMSSites[intCurrentSiteIndex].TestConn();
                    }

                    if (!string.IsNullOrEmpty(CMSSites[intCurrentSiteIndex].objCMSInfo.strCMSIP))
                    {
                        //
                        // Server is OK, so continue
                        //
                        return CMSSites[intCurrentSiteIndex].objCMSInfo;
                    }

                    if (blnTestConn)
                    {
                        //
                        // CMS Server was tested and found to be offline, so kick off a rescan.
                        //
                        CMSSites[intCurrentSiteIndex].Rescan();
                    }

                }
                intOffset = intOffset + 1;
                intCurrentSiteIndex = (intCurrentSiteIndex + 1) % (intCMSSiteCount);

            }
            //
            // No active site found
            //
            return null;

        }

        public bool IsWLWAvailable(bool blnSkip = false)
        {
            //
            // Check if we are able to perform a connection to the main winlink site on port 8775
            //
            if (GetHost(false, blnSkip, CMSServer.ServerType.WLW) == null)
            {
                //
                // Give it more time
                //
                Thread.Sleep(10000);

                if (GetHost(false, blnSkip, CMSServer.ServerType.WLW) == null)
                {
                    return false;
                }
            }
            return true;

        }

        public void TestServers()
        {
            //
            // This causes the code perform a test for available servers right away
            //
            foreach (CMSServer cm in CMSSites)
            {
                cm.Rescan();
            }
            Thread.Sleep(2000);
        }


        private class CMSServer
        {
            //
            // CMS Instance class
            //
            public string strName;
            public string strCity;
            public ServerType enmServerType;
            public List<string> lstIP;
            public string strDefaultIP;
            public object objLock = new object();

            public CMSInfo objCMSInfo;
            public enum ServerType
            {
                WLW = 0,
                CMS = 1,
                AU = 2
            }

            private List<string> lstTempIP;
            private WL2KTestConn objWL;

            private DnsLookup objDnsLookup;
            public CMSServer(string name, string defaultIP, string city, ServerType srvType)
            {
                strName = name;
                strDefaultIP = defaultIP;
                strCity = city;
                enmServerType = srvType;
                objCMSInfo = new CMSInfo(strName, strCity, "");
                lstIP = new List<string>();
                if (!string.IsNullOrEmpty(strDefaultIP))
                {
                    //
                    // Add the default IP to the list
                    //
                    lstIP.Add(strDefaultIP);
                }
                //
                // Start up the DNS resolver and connection test objects
                //
                objDnsLookup = new DnsLookup(this);
                objWL = new WL2KTestConn(this);
            }

            public void Close()
            {
                //
                // Time to go
                //
                objDnsLookup.Close();
                objWL.Close();
            }

            public void Rescan()
            {
                //
                // Rescan the connections on demand
                //
                objWL.Rescan();
            }

            public void TestConn()
            {
                //
                // Perform a quick connection test to the CMS
                //
                if (!string.IsNullOrEmpty(objCMSInfo.strCMSIP))
                {
                    if (!objWL.DoConn(objCMSInfo.strCMSIP, 8775, false))
                    {
                        objCMSInfo.strCMSIP = "";
                    }
                }
            }
            //
            // Helper class to perform a DNS loopkup of the server name for this CMS site.
            //
            private class DnsLookup
            {
                //
                // Perform a fail-safe lookup of the Winlink domains.  If the lookup fails, then we'll return the
                // hard-coded IP address.
                //
                private CMSServer objCmsSite;
                private int intResponse;
                private int intError;
                private System.Timers.Timer tmrDns;
                private bool blnRunOK = true;

                private List<string> lstTmpIPAddr;
                public DnsLookup(CMSServer cmsSite)
                {
                    //
                    // Load up the pointer to the CMS server
                    //
                    objCmsSite = cmsSite;
                    tmrDns = new System.Timers.Timer(100);
                    tmrDns.Elapsed += ScanDNS;
                    tmrDns.AutoReset = false;
                    tmrDns.Start();
                }

                public void Close()
                {
                    //
                    // Time to go
                    //
                    blnRunOK = false;
                    tmrDns.Stop();
                }

                private void ScanDNS(object s, System.Timers.ElapsedEventArgs e)
                {
                    //
                    // Perform a DNS resolution for the associated CMS site when the timer fires
                    //
                    tmrDns.Stop();

                    if (!blnRunOK)
                    {
                        return;
                    }

                    lstTmpIPAddr = new List<string>();

                    try
                    {
                        IPHostEntry objHostEntry = Dns.GetHostEntry(objCmsSite.strName);

                        foreach (IPAddress objIP in objHostEntry.AddressList)
                        {
                            string strIP = objIP.ToString();
                            if (!lstTmpIPAddr.Contains(strIP))
                            {
                                //
                                // Load the resolved IP if it is not already in the list
                                //
                                lstTmpIPAddr.Add(strIP);
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        //Exception("[DNSLookup] " & objCmsSite.strName & ": " & ex.Message)
                    }

                    if (!string.IsNullOrEmpty(objCmsSite.strDefaultIP))
                    {
                        if (!lstTmpIPAddr.Contains(objCmsSite.strDefaultIP))
                        {
                            //
                            // The default IP is not on the list, so we'll add it
                            //
                            lstTmpIPAddr.Add(objCmsSite.strDefaultIP);
                        }
                    }

                    lock (objCmsSite.objLock)
                    {
                        objCmsSite.lstIP = lstTmpIPAddr;
                    }

                    //
                    // Set DNS rescan interval to 30 minutes
                    //
                    tmrDns.Interval = 30 * 60 * 1000;
                    if (blnRunOK)
                    {
                        tmrDns.Start();
                    }
                }
            }

            //
            // Helper class to perform connection tests on the IP list returned by the DNS Lookup class
            //
            private class WL2KTestConn
            {
                //
                // Performs test TCP connections to a target IP to check for availability
                //
                bool blnRunOK = true;
                System.Timers.Timer tmrScan;
                CMSServer objCmsSite;
                List<string> lstIPList;
                byte[] buf = {
				34,
				13

			};
                public WL2KTestConn(CMSServer cmsServ)
                {
                    //
                    // Set up the TCP Client
                    //
                    objCmsSite = cmsServ;
                    //
                    // Begin first scan right away
                    //
                    tmrScan = new System.Timers.Timer(100);
                    tmrScan.Elapsed += CheckConn;
                    tmrScan.AutoReset = false;
                    tmrScan.Start();
                }

                public void Rescan()
                {
                    //
                    // Change the timer interval to initiate an on-demand rescan
                    //
                    tmrScan.Interval = 100;
                }

                public void Close()
                {
                    //
                    // Time to go
                    //
                    blnRunOK = false;
                    tmrScan.Stop();
                }

                public void CheckConn(object s, System.Timers.ElapsedEventArgs e)
                {
                    //
                    // Test the connection 
                    //
                    tmrScan.Stop();

                    if (!blnRunOK)
                    {
                        //
                        // Exit if we are shutting down
                        //
                        return;
                    }

                    bool blnResult = false;

                    //
                    // Set the scan intervals.  15 min if success, 2 min if failure
                    //
                    double dblSuccessInterval = 15 * 60 * 1000;
                    double dblFailInterval = 2 * 60 * 1000;

                    if (objCmsSite.enmServerType == ServerType.AU)
                    {
                        //
                        // Reset the default rescan interval for Autoupdate site.  6 hours if success, 1 hour if failure
                        //
                        dblSuccessInterval = 6 * 60 * 60 * 1000;
                        dblFailInterval = 1 * 60 * 60 * 1000;
                    }

                    lock (objCmsSite.objLock)
                    {
                        //
                        // Fetch the latest IP list
                        //
                        lstIPList = objCmsSite.lstIP;
                    }


                    foreach (string strIPAddr in lstIPList)
                    {
                        //
                        // Check for CMS Type
                        //
                        if (objCmsSite.enmServerType == ServerType.AU)
                        {
                            //
                            // We're checking the AU, so validate the HTTP AU port (8776)
                            //
                            blnResult = DoConn(strIPAddr, 8776, false);
                        }

                        if (objCmsSite.enmServerType == ServerType.WLW | objCmsSite.enmServerType == ServerType.CMS)
                        {
                            //
                            // We're checking the WLW or CMS, so validate the reporting port (8775)
                            //
                            blnResult = DoConn(strIPAddr, 8775, true);

                            if (objCmsSite.enmServerType == ServerType.CMS)
                            {
                                //
                                // CMS, so check 8772 also
                                //
                                if (blnResult)
                                {
                                    //
                                    // Reporting port was OK, so check the telnet port
                                    //
                                    blnResult = DoConn(strIPAddr, 8772, true);
                                }
                            }
                        }

                        if (blnResult)
                        {
                            //
                            // Successfully connected.  Record the IP
                            //
                            objCmsSite.objCMSInfo.strCMSIP = strIPAddr;
                            break; // TODO: might not be correct. Was : Exit For
                        }
                    }

                    if (blnResult)
                    {
                        //
                        // Connection OK, set recheck frequency
                        //
                        tmrScan.Interval = dblSuccessInterval;
                    }
                    else
                    {
                        //
                        // Connection failed, clear the IP and set recheck frequency
                        //
                        objCmsSite.objCMSInfo.strCMSIP = "";
                        tmrScan.Interval = dblFailInterval;
                    }

                    if (blnRunOK)
                    {
                        //
                        // Restart the timer
                        //
                        tmrScan.Start();
                    }

                }

                public bool DoConn(string strIPAddr, int intPort, bool blnValidateResp)
                {
                    //
                    // Return true if the TCP connection was successful.
                    //
                    using (TcpClient objConn = new TcpClient())
                    {
                        //
                        // Wrap the socket in a using block to dispose the object and resources as soon as we are done with it.
                        //
                        try
                        {
                            objConn.NoDelay = true;
                            objConn.ReceiveTimeout = 10000;
                            objConn.SendTimeout = 10000;
                            objConn.Connect(strIPAddr, intPort);
                            if (blnValidateResp)
                            {
                                //
                                // Do a packet handshake with the remote server
                                //
                                using (NetworkStream st = objConn.GetStream())
                                {
                                    int ret = 0;
                                    byte[] bufIn = new byte[1025];
                                    try
                                    {
                                        st.Write(buf, 0, buf.Length);
                                        ret = st.Read(bufIn, 0, bufIn.Length);
                                    }
                                    catch
                                    {
                                    }
                                    string q = Encoding.ASCII.GetString(bufIn, 0, ret).ToLower();
                                    objConn.Client.Close();
                                    if (q.Contains("callsign") | q.Contains("#") | Information.IsNumeric(q))
                                    {
                                        return true;
                                    }
                                }
                            }
                            else
                            {
                                objConn.Client.Close();
                                return true;
                            }
                        }
                        catch (Exception ex)
                        {
                        }

                    }
                    //
                    // Connection failed
                    //
                    return false;

                }

                private bool IsNumeric(string strVal)
                {
                    byte[] buf = Encoding.ASCII.GetBytes(strVal);
                    foreach (byte b in buf)
                    {
                        if (b < 48 | b > 59)
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }

        }
    }

}
