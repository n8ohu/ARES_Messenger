using Microsoft.VisualBasic;
using Microsoft.VisualBasic.Devices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace ARES_Messenger
{
    class INIFile
    {
        string strFilePath;

        Dictionary<string, Dictionary<string, string>> dicSections = new Dictionary<string, Dictionary<string, string>>();
        public void DeleteKey(string strSection, string strKey)
        {
            if (dicSections.ContainsKey(strSection) == false)
                return;
            if (dicSections[strSection].ContainsKey(strKey) == false)
                return;
            Dictionary<string, string> dicRecord = dicSections[strSection];
            dicRecord.Remove(strKey);
            Flush();
        }

        public void Load()
        {
            if (Microsoft.VisualBasic.FileIO.FileSystem.FileExists(strFilePath))
            {
                string strContent = Microsoft.VisualBasic.FileIO.FileSystem.ReadAllText(strFilePath);
                string strCurrentSection = "";
                StringReader objStringReader = new StringReader(strContent);

                dicSections.Clear();
                do
                {
                    string strLine = "EOF";
                    try
                    {
                        strLine = objStringReader.ReadLine().Trim();
                    }
                    catch
                    {
                        break; // TODO: might not be correct. Was : Exit Do
                    }
                    if (strLine == null || strLine == "EOF")
                        break; // TODO: might not be correct. Was : Exit Do

                    if (strLine.StartsWith("[") & strLine.EndsWith("]"))
                    {
                        strLine = strLine.Replace("[", "").Replace("]", "");

                        dicSections.Add(strLine, new Dictionary<string, string>());
                        strCurrentSection = strLine;
                    }
                    else
                    {
                        string[] strTokens = strLine.Split('=');
                        if (strTokens.Length == 2)
                        {
                            strTokens[0] = strTokens[0].Trim();
                            strTokens[1] = strTokens[1].Trim();
                            if (!string.IsNullOrEmpty(strTokens[0]))
                            {
                                dicSections[strCurrentSection].Add(strTokens[0].Trim(), strTokens[1].Trim());
                            }
                        }
                    }
                } while (true);
            }
        }

        public void Flush()
        {
            lock (Globals.objINIFileLock)
            {
                StringBuilder sbdContent = new StringBuilder();
                foreach (string strSection in dicSections.Keys)
                {
                    sbdContent.AppendLine(Constants.vbCrLf + "[" + strSection + "]");
                    foreach (string strKey in dicSections[strSection].Keys)
                    {
                        string strValue = dicSections[strSection][strKey];
                        sbdContent.AppendLine(strKey + "=" + strValue);
                    }
                }
                Microsoft.VisualBasic.FileIO.FileSystem.WriteAllText(strFilePath, sbdContent.ToString(), false);
            }
        }

        public bool GetBoolean(string strSection, string strKey, bool blnDefault)
        {
            bool blnResult = false;
            lock (Globals.objINIFileLock)
            {
                try
                {
                    blnResult = Convert.ToBoolean(GetRecord(strSection, strKey, blnDefault.ToString()));
                }
                catch
                {
                    blnResult = blnDefault;
                }
            }
            return blnResult;
        }

        public int GetInteger(string strSection, string strKey, int intDefault)
        {
            int intResult = 0;
            lock (Globals.objINIFileLock)
            {
                try
                {
                    intResult = Convert.ToInt32(GetRecord(strSection, strKey, intDefault.ToString()));
                }
                catch
                {
                    intResult = intDefault;
                }
            }
            return intResult;
        }

        public string GetString(string strSection, string strKey, string strDefault)
        {
            string strResult = null;
            lock (Globals.objINIFileLock)
            {
                strResult = GetRecord(strSection, strKey, strDefault);
            }
            return strResult;
        }

        public INIFile(string strFilePath)
        {
            this.strFilePath = strFilePath;
            Load();
        }

        public void WriteBoolean(string strSection, string strKey, bool blnValue)
        {
            lock (Globals.objINIFileLock)
            {
                WriteRecord(strSection, strKey, blnValue.ToString());
            }
        }

        public void WriteInteger(string strSection, string strKey, int intValue)
        {
            lock (Globals.objINIFileLock)
            {
                WriteRecord(strSection, strKey, intValue.ToString());
            }
        }

        public void WriteString(string strSection, string strKey, string strValue)
        {
            lock (Globals.objINIFileLock)
            {
                WriteRecord(strSection, strKey, strValue);
            }
        }

        private string GetRecord(string strSection, string strKey, string strDefault)
        {
            string strValue = null;
            try
            {
                strValue = dicSections[strSection][strKey];
            }
            catch
            {
                return strDefault;
            }
            if (string.IsNullOrEmpty(strValue))
                return strDefault;
            return strValue;
        }

        private void WriteRecord(string strSection, string strKey, string strValue)
        {
            if (dicSections.ContainsKey(strSection) == false)
            {
                dicSections.Add(strSection, new Dictionary<string, string>());
            }

            if (dicSections[strSection].ContainsKey(strKey) == false)
            {
                dicSections[strSection].Add(strKey, strValue);
            }
            else
            {
                Dictionary<string, string> dicRecord = dicSections[strSection];
                dicRecord.Remove(strKey);
                dicRecord.Add(strKey, strValue);
            }
        }
    }

}
