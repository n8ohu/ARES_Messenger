using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Threading;
using nsoftware.IPWorks;
using Ionic.Zip;
using System.Windows.Forms;

namespace ARES_Messenger
{
    public class Autoupdate
    {
        private bool blnAutoupdateClosing = false;
        private bool blnDownloadComplete = false;
        private bool blnDownloadError = false;
        private bool blnDoAppReset = false;
        private bool blnAutoUpdateSuccessful = false;
        private bool blnAUTestMode = false;
        private bool blnAUInibit = false;
        private bool blnHTTPMode = false;

        private bool blnExceptionOccurred = false;
        // Set the application parameters here.  The values here should work for most of the winlink apps

        private int intHTTPPort = 8776;

        private int intFTPPort = 8777;
        private static string strAuthUser = "Autoupdate";

        private static string strAuthPass = "U53FLW";
        private static string strAppProductName = Application.ProductName;
        private static string strAppProductVersion = Application.ProductVersion;
        private static string strExecutionDirectory = Application.StartupPath + "\\";
        private static string strAutoupdatePath = Path.Combine(strExecutionDirectory, "Autoupdate\\");
        private static string strLogPath = Path.Combine(strExecutionDirectory, "Logs");
        private static string strFtpPath = "/Autoupdate/";
        private static string strHttpPath = "http://*****:" + intHTTPPort.ToString() + strFtpPath;

        private CMSInfo objCmsInfo;
        // Set auto update check frequency to every 24 hours; Perform the initial check after 20 seconds.
        // If an exception occurred trying to access the uptoupdate site, retry after 1 hour

        // 24 hours
        private int intAutoupdateInterval = 1000 * 60 * 60 * 24;
        // 1 hour
        private int intAutoupdateErrInterval = 1000 * 60 * 60;
        // 20 seconds
        private int intInitialAutoupdateCheck = 1000 * 20;

        private Ftp withEventsField_objFTP;
        private Ftp objFTP
        {
            get { return withEventsField_objFTP; }
            set
            {
                if (withEventsField_objFTP != null)
                {
                    withEventsField_objFTP.OnError -= OnFTPException;
                }
                withEventsField_objFTP = value;
                if (withEventsField_objFTP != null)
                {
                    withEventsField_objFTP.OnError += OnFTPException;
                }
            }
        }
        private Http objHTTP;
        private System.Timers.Timer withEventsField_tmrAutoupdate;
        private System.Timers.Timer tmrAutoupdate
        {
            get { return withEventsField_tmrAutoupdate; }
            set
            {
                if (withEventsField_tmrAutoupdate != null)
                {
                    withEventsField_tmrAutoupdate.Elapsed -= AutoupdateTimerEvent;
                }
                withEventsField_tmrAutoupdate = value;
                if (withEventsField_tmrAutoupdate != null)
                {
                    withEventsField_tmrAutoupdate.Elapsed += AutoupdateTimerEvent;
                }
            }

        }
        public Autoupdate(bool blnAutoUpdateTest = false, bool blnInhibitAutoupdate = false)
        {
            //
            // Initialize Autoupdate
            //
            blnAUTestMode = blnAutoUpdateTest;
            blnAUInibit = blnInhibitAutoupdate;

            //
            // Set up the timer.  Set the first update check to start after 2 minutes.
            //
            tmrAutoupdate = new System.Timers.Timer();
            tmrAutoupdate.AutoReset = false;
            tmrAutoupdate.Interval = intInitialAutoupdateCheck;
            tmrAutoupdate.Start();

            //
            // Create the log directory is required
            //
            if (!Directory.Exists(strLogPath))
            {
                Directory.CreateDirectory(strLogPath);
            }
        }

        public void Close()
        {
            tmrAutoupdate.Stop();
            blnAutoupdateClosing = true;

            if (objFTP != null)
            {
                objFTP.Dispose();
                objFTP = null;
            }

            if (objHTTP != null)
            {
                objHTTP.Dispose();
                objHTTP = null;
            }

        }

        public byte[] GetBytes(string strText)
        {
            // Converts a text string to a byte array...

            byte[] bytBuffer = new byte[strText.Length];
            for (int intIndex = 0; intIndex <= bytBuffer.Length - 1; intIndex++)
            {
                bytBuffer[intIndex] = Convert.ToByte(Strings.Asc(strText.Substring(intIndex, 1)));
            }
            return bytBuffer;
        }

        private void AutoupdateTimerEvent(object s, System.Timers.ElapsedEventArgs e)
        {
            string strRemotePath = strFtpPath;
            string strLocalPath = strAutoupdatePath;
            bool blnDownloadFlag = false;
            blnExceptionOccurred = false;
            //
            // This timer fires when it is time to check for program updates on the Winlink FTP server
            // The entire update process is driven off this timer event
            //
            tmrAutoupdate.Stop();
            string strUpdateFile = "";

            //
            // Set this flag to Ture to use download using HTTP.  Otherwise, downloads will use FTP
            //
            blnHTTPMode = true;

            if (blnHTTPMode)
            {
                strRemotePath = strHttpPath;
            }
#if DEBUG
            WriteAutoupdateLog("[Autoupdate] Start Check");
#endif
            if (blnAUTestMode)
            {
                //
                // Append '_Test' to the download path if we are in test mode.
                //
                strRemotePath = strRemotePath.Insert(strRemotePath.Length - 1, "_Test");
                strLocalPath = strLocalPath.Insert(strLocalPath.Length - 1, "_Test");

                WriteAutoupdateLog("Remote path:" + strRemotePath + " - Local Path:" + strLocalPath);
            }

            if ((blnAutoupdateClosing))
            {
                //
                // We've been asked to shut down, so leave the timer off and exit
                //
                return;
            }

            //
            // Clean any stale content from the Autoupdate subdirectory
            //
            CleanupAutoupdate(strLocalPath);

            //
            // Should we do an autoupdate check?
            //
            if (((!blnAUInibit) | blnAUTestMode) & (!blnAutoUpdateSuccessful))
            {
                //
                // Only perform autoupdate if this is production code or we are in autoupdate test mode 
                // and we have not already done a successfulauto-update
                //
                try
                {
                    //
                    // Get the most appropriate autoupdate file.  Call returns null string if no update available
                    //
                    strUpdateFile = CheckForUpdateFile(strRemotePath, strAppProductName, strAppProductVersion, blnHTTPMode);
                    if (strUpdateFile.Length > 0)
                    {
                        //
                        // We have an update, so create the Autoupdate directory to deposit the file in
                        //
                        if (!Directory.Exists(strLocalPath))
                        {
                            Directory.CreateDirectory(strLocalPath);
                        }

                        //
                        // Download the file from the Winlink server
                        //

                        if (blnHTTPMode)
                        {
                            int intAltURL = strUpdateFile.IndexOf("|");
                            if (intAltURL > 0)
                            {
                                //
                                // Alternate URL specified 
                                //
                                strRemotePath = strUpdateFile.Substring(intAltURL + 1).Trim();
                                if (!strRemotePath.EndsWith("/"))
                                    strRemotePath += "/";
                                if (blnAUTestMode)
                                {
                                    //
                                    // Append '_Test' to the download path if we are in test mode.
                                    //
                                    strRemotePath = strRemotePath.Insert(strRemotePath.Length - 1, "_Test");
                                }
                                strUpdateFile = strUpdateFile.Substring(0, intAltURL).Trim();
                            }
                            blnDownloadFlag = DownloadFileHTTP(strRemotePath, strUpdateFile, strLocalPath);
                        }
                        else
                        {
                            blnDownloadFlag = DownloadFileFTP(strRemotePath, strUpdateFile, strLocalPath);
                        }
                        if (blnDownloadFlag)
                        {
                            //
                            // File was downloaded, so let's process.
                            // Unzip the archive to the Autoupdate directory
                            //
                            if (!blnAutoupdateClosing)
                            {
                                UnzipFile(Path.Combine(strLocalPath, strUpdateFile));
                            }

                            //
                            // Update the target files on the machine
                            //
                            if (!blnAutoupdateClosing)
                            {
                                blnAutoUpdateSuccessful = UpdateTargetFiles(strExecutionDirectory, strLocalPath);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    WriteAutoupdateLog("[Autoupdate] EXCEPTION: " + ex.Message);
                }
            }

            if (blnAutoUpdateSuccessful & blnAUTestMode)
            {
                //
                // Update worked and we are in test mode, so clear the test flag from the ini file
                //
                ClearTestAutoupdateINI(Path.Combine(strExecutionDirectory, strAppProductName) + ".ini");
                blnAUTestMode = false;
            }

            //
            // Remove all files from the Autoupdate subdirectory and delete it.
            //
            CleanupAutoupdate(strLocalPath);

            if (blnDoAppReset)
            {
                //
                // The update was successful and the update specified an app reset.  Add whatever code is
                // required to ensure the application is idle before performing the reset
                //
                Globals.blnAutoupdateRestart = true;
                //Application.Restart()
            }

            if ((!blnAutoUpdateSuccessful) & !(blnAutoupdateClosing))
            {
                //
                // Restart the update timer if there was no update or the update failed.
                //
                if (blnExceptionOccurred)
                {
                    //
                    // Error accessing autoupdate site, try again in 1 hour.
                    //
                    tmrAutoupdate.Interval = intAutoupdateErrInterval;
                }
                else
                {
                    //
                    // Normal rescan
                    //
                    tmrAutoupdate.Interval = intAutoupdateInterval;
                }
                tmrAutoupdate.Start();
            }
        }

        private string CheckForUpdateFile(string strRemotePath, string strAppName, string strAppVersion, bool blnUseHTTP = false)
        {
            //
            // Check to see if an applicable update file is available.
            // Fetch the names of the files in the FTP directory and look for the most suitable update (if any)
            //
            string[] strVersions = null;
            string strReturnFilename = "";
            string strPatchVersion = "0.0.0.0";
            string strMinVersion = "0.0.0.0";
            List<string> lstFileNames = null;

            if (blnAUTestMode)
            {
                WriteAutoupdateLog("Fetch information for:" + strAppName);
            }

            if (blnUseHTTP)
            {
                lstFileNames = FetchUpdateFilesHTTP(strRemotePath);
            }
            else
            {
                lstFileNames = FetchUpdateFilesFTP(strRemotePath);
            }

            //
            // Now we proces the files we found
            //
#if DEBUG
            WriteAutoupdateLog("[Autoupdate] Scan count: " + lstFileNames.Count.ToString());
#endif

            foreach (string sEnt in lstFileNames)
            {
                if ((sEnt.ToLower().StartsWith(strAppName.ToLower())) & (sEnt.ToLower().Contains(".zip")))
                {
                    //
                    // Loop through all the possible update files and find the highest version for which the application meets
                    // the minimum version bar
                    //
                    strVersions = GetAutoupdateVersionInfo(sEnt);
                    if ((CompareVersions(strAppVersion, strVersions[0]) >= 0) & (CompareVersions(strVersions[1], strAppVersion) > 0) & (CompareVersions(strVersions[1], strPatchVersion) >= 0) & (CompareVersions(strVersions[0], strMinVersion) >= 0))
                    {
                        //
                        // Select this patch file if all three of these conditions are met:
                        //     1) The current version of this application is greater than or equal to the min version required for this patch file
                        //     2) The patch file version is greater than the current version of this application
                        //     3) The patch file version is greater than or equal to the one we already found.  (We start at version 0.0.0.0)
                        //     4) The patch file min version is greater than or equal to the one we already found. (We start at version 0.0.0.0)
                        //
                        // When we are all done, we'll return the most up-to-date patch file(if one exists) whose min version is 
                        // closest to the app version.
                        //
                        strReturnFilename = sEnt;
                        strMinVersion = strVersions[0];
                        strPatchVersion = strVersions[1];
                    }
                }
            }

            if (strReturnFilename.Length > 0)
            {
                //
                // A new version of the app is available, so do a modal pop-up asking the user if they wish to update. 
                // The goal is to 'nag' the user into updating.
                //
                Globals.blnAbortAU = true;
                strNewAUVersion = strPatchVersion;

                Globals.objMain.Enabled = false;
                DialogAutoupdate objAU = new DialogAutoupdate();
                objAU.ShowDialog();
                Globals.objMain.Enabled = true;
                if (Globals.blnAbortAU)
                {
                    //
                    // User aborted the autoupdate, to dont continue
                    //
                    WriteAutoupdateLog("[Autoupdate] Cancelled by User");
                    return "";
                }

                //
                // Always reset the app if the user gives confirmation of the update
                //
                WriteAutoupdateLog("[Autoupdate] Updating to version: " + strPatchVersion);
                blnDoAppReset = true;
#if DEBUG
            }
            else
            {
                WriteAutoupdateLog("[Autoupdate] No updates");
#endif
            }

            //
            // Return the best match, or a null string if no match
            //
            return strReturnFilename;
        }
        // End CheckForUpdateFile

        private List<string> FetchUpdateFilesFTP(string strRemotePath)
        {
            //
            // Fetch the list of possible updates.
            //
            DirEntryList dList = default(DirEntryList);
            List<string> lstFileNames = new List<string>();

            if (objFTP != null)
                objFTP.Dispose();

            objFTP = new Ftp();
            objFTP.Passive = true;
            objFTP.TransferMode = FtpTransferModes.tmBinary;
            objFTP.User = strAuthUser;
            objFTP.Password = strAuthPass;
            objFTP.RemotePort = 8777;
            objFTP.Timeout = 600;
            objFTP.RemotePath = strRemotePath;

            objCmsInfo = Globals.objWL2KServers.GetAUHost();
            if (objCmsInfo != null)
            {
                try
                {
                    if (blnAUTestMode)
                    {
                        WriteAutoupdateLog("Checking ftp site:" + objCmsInfo.strCMSIP + strRemotePath);
                    }

                    objFTP.RemoteHost = objCmsInfo.strCMSIP;
                    objFTP.Logon();

                    //
                    // Kick off a directory listing
                    //
                    objFTP.ListDirectory();

                    //
                    // Get the directory listing
                    //
                    dList = objFTP.DirList;

                    //
                    // Save the file names to a list for procesing, then close down the FTP connection
                    //
                    foreach (DirEntry dEnt in dList)
                    {
                        lstFileNames.Add(dEnt.FileName.Trim());
                    }

                    objFTP.Logoff();
                    objFTP.Dispose();
                    objFTP = null;
                    return lstFileNames;

                }
                catch (Exception ex)
                {
                    WriteAutoupdateLog("[Autoupdate.CheckForUpdateFile.FTP] EXCEPTION: " + ex.Message);
                    blnExceptionOccurred = true;
                }
            }
            else
            {
                WriteAutoupdateLog("[Autoupdate.CheckForUpdateFile.FTP] Server not available");
                blnExceptionOccurred = true;
            }

            if (objFTP != null)
            {
                //
                // Cleanup
                //
                if (objFTP.Connected)
                {
                    objFTP.Logoff();
                }
                objFTP.Dispose();
                objFTP = null;
            }

            return lstFileNames;

        }
        // End of FetchUpdateFilesFTP

        private List<string> FetchUpdateFilesHTTP(string strURL)
        {
            //
            // Fetch the list of possible updates.
            //
            string strLine = null;
            string strReturnFilename = "";
            List<string> lstFileNames = new List<string>();

            if (objHTTP != null)
                objHTTP.Dispose();

            objHTTP = new Http();
            objHTTP.AuthScheme = HttpAuthSchemes.authBasic;
            objHTTP.User = strAuthUser;
            objHTTP.Password = strAuthPass;

            objCmsInfo = Globals.objWL2KServers.GetAUHost();
            if (objCmsInfo != null)
            {
                try
                {
                    if (blnAUTestMode)
                    {
                        WriteAutoupdateLog("Checking location:" + strURL.Replace("*****", objCmsInfo.strCMSIP));
                    }

                    objHTTP.Get((strURL.Replace("*****", objCmsInfo.strCMSIP) + "Files.txt").Replace(" ", "%20"));

                    string FileInfo = objHTTP.TransferredData;

                    //
                    // Build the list of return files
                    //
                    string[] strFiles = FileInfo.Split(Convert.ToChar(Constants.vbCr));

                    foreach (string strFile in strFiles)
                    {
                        //
                        // Loop through all the lines in the file
                        //
                        strLine = StripComment(strFile);
                        if (strLine.Length > 0)
                        {
                            lstFileNames.Add(strLine);
                        }
                    }

                    objHTTP.Dispose();
                    objHTTP = null;
                    return lstFileNames;
                }
                catch (Exception ex)
                {
                    WriteAutoupdateLog("[Autoupdate.CheckForUpdateFile.HTTP] Exception fetching listing: " + ex.Message);
                    blnExceptionOccurred = true;
                }
            }
            else
            {
                WriteAutoupdateLog("[Autoupdate.CheckForUpdateFile.HTTP] Server not available");
                blnExceptionOccurred = true;
            }

            if (objHTTP != null)
            {
                objHTTP.Dispose();
                objHTTP = null;
            }

            return lstFileNames;
        }
        // End of FetchUpdateFilesHTTP

        private string Parse(string strIn, string strAppName)
        {

            //
            // Strip off comments
            //
            strIn = StripComment(strIn);

            if (strIn.StartsWith(strAppName) & strIn.ToLower().Contains(".zip"))
            {
                //
                // Found an autoupdate file for this app, so ad it to the list
                //
                return strIn;
            }
            //
            // Invalid line, so return empty string
            //
            return "";
        }
        // End Parse

        private string StripComment(string strIn)
        {
            int intComment = 0;

            intComment = strIn.IndexOf(";");
            if (intComment >= 0)
            {
                //
                // Strip off comments
                //
                strIn = strIn.Substring(0, intComment);
            }
            return strIn.Trim();
        }
        //End StripComment

        private bool DownloadFileFTP(string strRemotePath, string strRemoteFile, string strLocalPath)
        {
            // 
            // Initiate a download of the patch file
            //
            if (objFTP != null)
                objFTP.Dispose();
            if (blnAUTestMode)
            {
                WriteAutoupdateLog("Download Remote path:" + strRemotePath + " - File:" + strRemoteFile + " - Local Path:" + strLocalPath);
            }

            objFTP = new Ftp();
            objFTP.Passive = true;
            objFTP.TransferMode = FtpTransferModes.tmBinary;
            objFTP.User = strAuthUser;
            objFTP.Password = strAuthPass;
            objFTP.RemotePort = intFTPPort;
            objFTP.Timeout = 600;
            objFTP.LocalFile = Path.Combine(strLocalPath, strRemoteFile);
            objFTP.RemoteFile = strRemoteFile;
            objFTP.RemotePath = strRemotePath;

            objCmsInfo = Globals.objWL2KServers.GetAUHost();
            if (objCmsInfo != null)
            {
                try
                {
                    objFTP.RemoteHost = objCmsInfo.strCMSIP;
                    objFTP.Logon();
                    if (objFTP.FileExists == false)
                    {
                        objFTP.Logoff();
                        objFTP.Dispose();
                        objFTP = null;
                        return false;
                    }

                    objFTP.Passive = true;
                    objFTP.TransferMode = FtpTransferModes.tmBinary;
                    objFTP.User = strAuthUser;
                    objFTP.Password = strAuthPass;

                    objFTP.RemotePort = intFTPPort;
                    objFTP.Timeout = 600;
                    objFTP.LocalFile = Path.Combine(strLocalPath, strRemoteFile);
                    objFTP.RemoteFile = strRemoteFile;
                    objFTP.RemotePath = strRemotePath;

                    blnDownloadError = false;

                    objFTP.Download();

                    if (blnAutoupdateClosing | blnDownloadError)
                    {
                        //
                        // We've been told to shut the auto update down
                        // or we received an error.  Either way we exit
                        //
                        if (objFTP != null)
                        {
                            objFTP.Dispose();
                            objFTP = null;
                        }
                        if (blnAUTestMode)
                        {
                            WriteAutoupdateLog("Aborting download");
                        }
                        return false;
                    }

                    objFTP.Logoff();
                    objFTP.Dispose();
                    objFTP = null;
                    if (blnAUTestMode)
                    {
                        WriteAutoupdateLog("Download Complete");
                    }

                    //
                    // Indicate sucessful download
                    //
                    return true;
                }
                catch (Exception ex)
                {
                    WriteAutoupdateLog("[Autoupdate.DownloadFile.FTP] EXCEPTION: " + ex.Message);
                    blnExceptionOccurred = true;
                }
            }
            else
            {
                WriteAutoupdateLog("[AutoupdateDownloadFile.FTP] Server not available");
                blnExceptionOccurred = true;
            }

            objFTP.Dispose();
            objFTP = null;
            return false;

        }
        // End DownloadFileFTP

        private bool DownloadFileHTTP(string strRemotePath, string strRemoteFile, string strLocalPath)
        {
            // 
            // Initiate a download of the patch file
            //
            if (objHTTP != null)
                objHTTP.Dispose();

            objHTTP = new Http();

            objHTTP.LocalFile = Path.Combine(strLocalPath, strRemoteFile);
            objHTTP.AuthScheme = HttpAuthSchemes.authBasic;
            objHTTP.User = strAuthUser;
            objHTTP.Password = strAuthPass;

            objCmsInfo = Globals.objWL2KServers.GetAUHost();
            if (objCmsInfo != null)
            {
                try
                {
                    if (blnAUTestMode)
                    {
                        WriteAutoupdateLog("Download Remote path:" + strRemotePath.Replace("*****", objCmsInfo.strCMSIP) + " - File:" + strRemoteFile + " - Local Path:" + strLocalPath);
                    }

                    objHTTP.Get((strRemotePath.Replace("*****", objCmsInfo.strCMSIP) + strRemoteFile).Replace(" ", "%20"));

                    objHTTP.Dispose();
                    objHTTP = null;

                    if (blnAutoupdateClosing)
                    {
                        WriteAutoupdateLog("Download aborted");
                        return false;
                    }

                    WriteAutoupdateLog("Download Complete");
                    return true;

                }
                catch (Exception ex)
                {
                    WriteAutoupdateLog("[Autoupdate.DownloadFile.HTTP] EXCEPTION: " + ex.Message);
                    blnExceptionOccurred = true;
                }
            }
            else
            {
                WriteAutoupdateLog("[AutoupdateDownloadFile.HTTP] Server not available");
                blnExceptionOccurred = true;
            }

            objHTTP.Dispose();
            objHTTP = null;
            return false;
        }
        // End DownloadFileHTTP

        // FTP object event handlers

        private void OnFTPException(object sender, nsoftware.IPWorks.FtpErrorEventArgs e)
        {
            //
            // Notify the user if an error occurs during an FTP operation
            //
            WriteAutoupdateLog("FTP EXCEPTION: " + e.Description);
            blnExceptionOccurred = true;
            blnDownloadError = true;

        }

        private bool UpdateTargetFiles(string strAppPath, string strSourcePath)
        {
            //
            // Deploy the files in the update to the specified target location.  Information on file deploymentis stored
            // in a file called Target.dat, which should be included in the archive.
            //
            List<FileTarget> lstFileList = new List<FileTarget>();

            string[] strFiles = null;
            string strFile = null;
            string strSourceFile = null;
            bool blnInRecovery = false;
            bool blnFoundResetTag = false;
            int intPos = 0;

            try
            {
                //
                // Read the update instructions
                //
                strFiles = File.ReadAllLines(Path.Combine(strSourcePath, "Target.dat"));

            }
            catch (Exception ex)
            {
                if (blnAUTestMode)
                {
                    WriteAutoupdateLog("[Autoupdate.UpdateTargetFile] EXCEPTION: " + ex.Message);
                }
                return false;
            }

            foreach (string strLine in strFiles)
            {
                strFile = StripComment(strLine);
                if (strFile.Length == 0)
                {
                    //
                    // Ignore blank lines
                    //
                    continue;
                }

                if (strFile.ToLower().Contains("<restart>"))
                {
                    //
                    // Set the app reset flag
                    //
                    blnFoundResetTag = true;
                    if (blnAUTestMode)
                    {
                        WriteAutoupdateLog("[Autoupdate.UpdateTargetFile] Restart flag found " + strFile);
                    }
                    continue;
                }

                //
                // Split the string across the '?'
                //
                string[] strEntry = strFile.Split('?');

                if (strEntry.Length != 2)
                {
                    if (blnAUTestMode)
                    {
                        WriteAutoupdateLog("[Autoupdate.UpdateTargetFile] EXCEPTION: Bad file line: " + strFile);
                    }
                    return false;
                }
                strSourceFile = Path.Combine(strSourcePath, strEntry[0]);

                if (!File.Exists(strSourceFile))
                {
                    if (blnAUTestMode)
                    {
                        WriteAutoupdateLog("[Autoupdate.UpdateTargetFile] EXCEPTION: Source file not in archive: " + strSourceFile);
                    }
                    return false;
                }

                //
                // Add the file to process to the list
                //
                File.SetAttributes(strSourceFile, FileAttributes.Normal);
                lstFileList.Add(new FileTarget(strSourceFile, Path.Combine(strAppPath, strEntry[1]), Path.Combine(Path.Combine(strAppPath, strEntry[1]), strEntry[0])));
                if (blnAUTestMode)
                {
                    WriteAutoupdateLog("[Autoupdate.UpdateTargetFile] Queueing update for: " + Path.Combine(Path.Combine(strAppPath, strEntry[1]), strEntry[0]));
                }

            }

            //
            // The source list was successfully built, so process it
            //
            foreach (FileTarget objFileTarget in lstFileList)
            {
                //
                // Delete .old files
                //
                try
                {
                    if (File.Exists(objFileTarget.strDestFile + ".old"))
                    {
                        File.SetAttributes(objFileTarget.strDestFile + ".old", FileAttributes.Normal);
                        File.Delete(objFileTarget.strDestFile + ".old");
                    }
                }
                catch (Exception ex)
                {
                    if (blnAUTestMode)
                    {
                        WriteAutoupdateLog("[Autoupdate.UpdateTargetFile] EXCEPTION: " + ex.Message);
                    }
                    return false;
                }
            }

            foreach (FileTarget objFileTarget in lstFileList)
            {
                //
                // Move current files to .old
                //
                try
                {
                    if (!Directory.Exists(objFileTarget.strDestPath))
                    {
                        Directory.CreateDirectory(objFileTarget.strDestPath);
                    }

                    if (File.Exists(objFileTarget.strDestFile))
                    {
                        File.Move(objFileTarget.strDestFile, objFileTarget.strDestFile + ".old");
                        objFileTarget.blnOK = true;
                    }

                }
                catch (Exception ex)
                {
                    if (blnAUTestMode)
                    {
                        WriteAutoupdateLog("[Autoupdate.UpdateTargetFile] EXCEPTION: " + ex.Message);
                    }
                    blnInRecovery = true;
                    break; // TODO: might not be correct. Was : Exit For
                }
            }

            if (!blnInRecovery)
            {
                //
                // So far so good, so continue with update
                //
                foreach (FileTarget objFileTarget in lstFileList)
                {
                    //
                    // Plop down the new file
                    //
                    try
                    {
                        File.SetAttributes(objFileTarget.strSourceFile, FileAttributes.Normal);
                        File.Copy(objFileTarget.strSourceFile, objFileTarget.strDestFile);
                        objFileTarget.blnOK = true;

                        if (blnAUTestMode)
                        {
                            WriteAutoupdateLog("[Autoupdate.UpdateTargetFile] File successfully deployed:  " + objFileTarget.strDestFile);
                        }
                    }
                    catch (Exception ex)
                    {
                        if (blnAUTestMode)
                        {
                            WriteAutoupdateLog("[Autoupdate.UpdateTargetFile] EXCEPTION: " + ex.Message);
                        }
                        blnInRecovery = true;
                        break; // TODO: might not be correct. Was : Exit For
                    }

                }
            }

            if (blnInRecovery)
            {
                //
                // Uh oh!  We hit a snag with the update, unwind the changes
                //
                if (blnAUTestMode)
                {
                    WriteAutoupdateLog("[Autoupdate.UpdateTargetFile] Deployment failed.  Attempting to restore original files");
                }
                foreach (FileTarget objFileTarget in lstFileList)
                {
                    //
                    // Restore original file
                    //
                    if (objFileTarget.blnOK)
                    {
                        //
                        // This file was updated, so put it back
                        //
                        try
                        {
                            if (File.Exists(objFileTarget.strDestFile))
                            {
                                File.SetAttributes(objFileTarget.strDestFile, FileAttributes.Normal);
                                File.Delete(objFileTarget.strDestFile);
                            }
                            if (File.Exists(objFileTarget.strDestFile + ".old"))
                            {
                                File.Move(objFileTarget.strDestFile + ".old", objFileTarget.strDestFile);
                                if (blnAUTestMode)
                                {
                                    WriteAutoupdateLog("[Autoupdate.UpdateTargetFile] File restored: " + objFileTarget.strDestFile);
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            if (blnAUTestMode)
                            {
                                WriteAutoupdateLog("[Autoupdate.UpdateTargetFile] Recovery attempt error - EXCEPTION: " + ex.Message);
                            }
                        }
                    }
                }
            }

            //
            // Set the force app reset global if the update was successful and we found a reset tag in the target.dat file
            //
            blnDoAppReset = blnFoundResetTag & (!blnInRecovery);
            return !blnInRecovery;

        }
        // End UpdateTargetFile

        private int CompareVersions(string strVersionA, string strVersionB)
        {
            // 
            // Compare version number strings 
            //  Returns -2 if A or B is invalid
            //  Returns -1 if A < B
            //  Returns  0 if A = B
            //  Returns  1 if A > B
            //
            string[] strVA = strVersionA.Split('.');
            string[] strVB = strVersionB.Split('.');
            int intIndex = 0;

            if (strVA.Length != 4 | strVB.Length != 4)
            {
                return 2;
            }

            try
            {
                for (intIndex = 0; intIndex <= strVA.Length - 1; intIndex++)
                {
                    if (Convert.ToInt32(strVA[intIndex]) < Convert.ToInt32(strVB[intIndex]))
                    {
                        return -1;
                    }
                    if (Convert.ToInt32(strVA[intIndex]) > Convert.ToInt32(strVB[intIndex]))
                    {
                        return 1;
                    }
                }
            }
            catch
            {
                return 2;
            }
            //
            // Versions are equal
            //
            return 0;
        }
        // End CompareVersions

        private string TimestampEx()
        {
            //
            // Returns the current time/date formatted
            //
            return Strings.Format(DateTime.UtcNow, "yyyy/MM/dd HH:mm:ss");
        }

        private string[] GetAutoupdateVersionInfo(string strFile)
        {
            //
            // Extract the version number from the Autoupdate zip filename
            // format of filename is Product_w-x-y-z.zip
            // return value would be w.x.y.z
            //
            int intAltURL = strFile.IndexOf("|");
            if (intAltURL > 0)
            {
                strFile = strFile.Substring(0, intAltURL);
            }

            string strVersion = Path.GetFileNameWithoutExtension(strFile);
            string[] strRetVersion = new string[2] {
			"",
			"0.0.0.0"
		};

            string[] strTmp = strVersion.Split('_');
            if (strTmp.Length == 3)
            {
                strRetVersion[0] = strTmp[1].Trim().Replace("-", ".");
                strRetVersion[1] = strTmp[2].Trim().Replace("-", ".");
            }

            return strRetVersion;
        }
        // End GetAutoupdateVersionInfo

        private void UnzipFile(string strFile)
        {
            //
            // Open the zip archive and extract all files to the target directory
            //
            Ionic.Zip.ZipFile zFile = new ZipFile(strFile);
            zFile.FlattenFoldersOnExtract = true;
            zFile.ExtractAll(Path.GetDirectoryName(strFile), ExtractExistingFileAction.OverwriteSilently);
            zFile.Dispose();
        }
        // End UnzipAutoupdateFile

        private void WriteAutoupdateLog(string strData)
        {
            //
            // Writes an entry to the autoupdate log file
            //
            try
            {
                string strLogPath = strExecutionDirectory + "\\Logs";
                strLogPath += "\\" + strAppProductName + " Autoupdate";
                if (blnAUTestMode)
                {
                    strLogPath += "_Test";
                }
                strLogPath += " " + Strings.Format(System.DateTime.UtcNow, "yyyyMMdd") + ".log";

                File.AppendAllText(strLogPath, TimestampEx() + "  " + strData + Constants.vbCrLf);
            }
            catch
            {
            }
        }
        // End WriteAutoupdateLog

        private void ClearTestAutoupdateINI(string strIniFilePath)
        {
            //
            // Remove the 'Test Autoupdate' key from the ini file
            //
            objINIFile.DeleteKey("Main", "Test Autoupdate");

        }
        // End ClearTestAutoupdateINI

        public void CleanupAutoupdate(string strPath)
        {
            //
            // Clean up the autoupdate directory
            //
            string[] fso = null;
            if ((Directory.Exists(strPath)))
            {
                try
                {
                    //
                    // Make sure no files are set to read-only.
                    //
                    fso = Directory.GetFiles(strPath, "*.*", SearchOption.AllDirectories);
                    foreach (string fi in fso)
                    {
                        File.SetAttributes(fi, FileAttributes.Normal);
                        File.Delete(fi);
                    }

                    //
                    // Delete it all
                    //
                    Microsoft.VisualBasic.FileIO.FileSystem.DeleteDirectory(strPath, FileIO.DeleteDirectoryOption.DeleteAllContents);

                }
                catch (Exception ex)
                {
                    WriteAutoupdateLog("[Autoupdate] Directory cleanup EXCEPTION: " + ex.Message);
                    // Error cleaning up the Autoupdate directory
                }
            }

        }
        //End CleanupAutoupdate

        // Class to hold the update files we are processing

        public class FileTarget
        {
            public string strSourceFile;
            public string strDestPath;
            public string strDestFile;

            public bool blnOK = false;
            public FileTarget(string strSrcFile, string strDstPth, string strDstFile)
            {
                strSourceFile = strSrcFile;
                strDestPath = strDstPth;
                strDestFile = strDstFile;
            }
        }

    }
}
