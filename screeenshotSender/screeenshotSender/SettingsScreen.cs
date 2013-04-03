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

    
    public partial class SettingsScreen : Form
    {
        public SettingsScreen()
        {
            InitializeComponent();
            textBox1.Text = ScreenShower.settings.path;
            checkBox1.Checked = ScreenShower.settings.showNotificationImmideatly;
            comboBox1.SelectedIndex = ScreenShower.settings.timeoutSeconds;
            textBox3.Text = ScreenShower.settings.port + "";
            textBox4.Text = ScreenShower.settings.nick + "";

        }

        private void selectFolder(object sender, EventArgs e)
        {
           FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.ShowDialog();
            textBox1.Text = fbd.SelectedPath;
        }

        private void addFriends(object sender, EventArgs e)
        {
            Friend fr = new Friend(textBox2.Text, ScreenShower.settings.port, ScreenShower.nm);
            if (!fr.connectToFriend())
                MessageBox.Show("Ахтунг!!!! Мы не подключились к другу!");
            else
            {
                ScreenShower.nm.addFriend(fr);
            }
        }

        private void saveSettings(object sender, EventArgs e)
        {
            ScreenShower.settings = new Settings(textBox1.Text, checkBox1.Checked, comboBox1.SelectedIndex, Int32.Parse(textBox3.Text), textBox4.Text);
            ScreenShower.settings.saveSetting();
        }

    }
    
   
}
