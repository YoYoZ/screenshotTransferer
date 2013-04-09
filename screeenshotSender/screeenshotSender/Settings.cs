using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;


namespace screeenshotSender
{
    public class Settings
    {
        public string path;
        public bool showNotificationImmideatly;
        public int timeoutSeconds;
        public int port;
        public string nick;
        public int saveScreens;

        public Settings(string path, bool showNotificationImmideatly, int timeoutSeconds, int port, string nick, int saveScreens)
        {
            this.path = cleanString(path);
            this.showNotificationImmideatly = showNotificationImmideatly;
            this.timeoutSeconds = timeoutSeconds;
            this.port = port;
            this.nick = cleanString(nick);
            this.saveScreens = saveScreens;
        }

        public static Settings loadSetting(string path)
        {
            try
            {
                FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                StreamReader sr = new StreamReader(fs);
                string tempText = sr.ReadToEnd();
                tempText.Replace("\r", "");

                sr.Close();
                string[] tempLine = tempText.Split('\n');

                string tPath = "";
                bool tShowNotImm = false;
                int tTimeOut = 0;
                int port = 4567;
                string nick = "";
                int saveScreens = 4;

                for (int i = 0; i < tempLine.Length; i++)
                {
                    string[] values = tempLine[i].Split('=');
                    if (values[0].Equals("path"))
                    {
                        tPath = values[1];
                    }
                    if (values[0].Equals("showNotificationImmideatly"))
                    {
                        tShowNotImm = Boolean.Parse(values[1]);
                    }
                    if (values[0].Equals("timeoutSeconds"))
                    {
                        tTimeOut = Int32.Parse(values[1]);
                    }
                    if (values[0].Equals("port"))
                    {
                        port = Int32.Parse(values[1]);
                    }
                    if (values[0].Equals("nick"))
                    {
                        nick = values[1];
                    }
                    if (values[0].Equals("saveScreens"))
                    {
                        saveScreens = Int32.Parse(values[1]);
                    }
                }
                Settings tempSet = new Settings(tPath, tShowNotImm, tTimeOut, port, nick, saveScreens);
                return tempSet;

            }
            catch (Exception)
            {
               
                return null;
            }
        }

        public void saveSetting()
        {
            try
            {
                FileStream fs = new FileStream(Directory.GetCurrentDirectory() + "\\settings.cfg", FileMode.Create, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs);
                sw.WriteLine("path=" + path);
                sw.WriteLine("showNotificationImmideatly=" + showNotificationImmideatly);
                sw.WriteLine("timeoutSeconds=" + timeoutSeconds);
                sw.WriteLine("port=" + port);
                sw.WriteLine("nick=" + nick);
                sw.WriteLine("saveScreens=" + saveScreens);
                sw.Close();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.ToString() + "\n\n" + exc.Message);
            }
        }

        private string cleanString(string par1)
        {
            string result = par1.Replace("\n", "");
            result = result.Replace("\r", "");
            return result;
        }
    }
}
