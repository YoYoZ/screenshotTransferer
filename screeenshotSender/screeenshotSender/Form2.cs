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

    
    public partial class Form2 : Form
    {
        public Settings settings;

        public Form2()
        {
            InitializeComponent();
            settings = Settings.loadSetting(Directory.GetCurrentDirectory()+ "\\settings.cfg");
            if (settings == null)
            {
                settings = new Settings(Directory.GetCurrentDirectory() , true, 2);
            }
            textBox1.Text = settings.path;
            checkBox1.Checked = settings.showNotificationImmideatly;
            comboBox1.SelectedIndex = settings.timeoutSeconds;
        }

        private void selectFolder(object sender, EventArgs e)
        {
           FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.ShowDialog();
            textBox1.Text = fbd.SelectedPath;
        }

        private void addFriends(object sender, EventArgs e)
        {
        }

        private void saveSettings(object sender, EventArgs e)
        {
            settings = new Settings(textBox1.Text, checkBox1.Checked, comboBox1.SelectedIndex);
            settings.saveSetting();
        }

    }

    public class Me
    {
    }

    public class Friend
    {
        string ip;
        string name;
        public Friend(string ip)
        {
            this.ip = ip;
            this.name = getFriendName();
        }

        private bool handShake(int version)
        {
            return true;
        }

        private string getFriendName()
        {
        #warning Не забыть код соединения.
            return "";
        }
        
    }
    
    public class Settings
    {
        public string path;
        public bool showNotificationImmideatly;
        public int timeoutSeconds;

        public Settings(string path, bool showNotificationImmideatly, int timeoutSeconds)
        {
            this.path = path;
            this.showNotificationImmideatly = showNotificationImmideatly;
            this.timeoutSeconds = timeoutSeconds;
        }

        public static Settings loadSetting(string path)
        {
            try
            {
                FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                StreamReader sr = new StreamReader(fs);
                string tempText = sr.ReadToEnd();
                tempText.Replace("\r","");

                sr.Close();
                string[] tempLine = tempText.Split('\n');

                string tPath = "";
                bool tShowNotImm = false;
                int tTimeOut = 0;

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
                }
                Settings tempSet = new Settings(tPath, tShowNotImm, tTimeOut);
                return tempSet;

            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.ToString() + "\n\n" + exc.Message);
                return null;
            }
        }

        public void saveSetting()
        {
            try
            {
                FileStream fs = new FileStream(Directory.GetCurrentDirectory()  + "\\settings.cfg", FileMode.Create, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs);
                sw.WriteLine("path=" + path);
                sw.WriteLine("showNotificationImmideatly=" + showNotificationImmideatly);
                sw.WriteLine("timeoutSeconds=" + timeoutSeconds);
                sw.Close();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.ToString() + "\n\n" + exc.Message);
            }
        }
    }
}
