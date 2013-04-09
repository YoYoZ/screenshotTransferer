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
using System.Threading;

namespace screeenshotSender
{

    
    public partial class SettingsScreen : Form
    {
        private ScreenShower sShower;

        public SettingsScreen(ScreenShower par1)
        {
            InitializeComponent();
            textBox1.Text = ScreenShower.settings.path;
            checkBox1.Checked = ScreenShower.settings.showNotificationImmideatly;
            comboBox1.SelectedIndex = ScreenShower.settings.timeoutSeconds;
            textBox3.Text = ScreenShower.settings.port + "";
            textBox4.Text = ScreenShower.settings.nick + "";
            comboBox2.SelectedIndex = ScreenShower.settings.saveScreens;
            sShower = par1;
            progressBar.Visible = false;
            timer1_Tick(null, null);
        }

        private void selectFolder(object sender, EventArgs e)
        {
           FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.ShowDialog();
            textBox1.Text = fbd.SelectedPath;
        }

        public void startConnection(object sender, EventArgs e)
        {
            Friend fr = new Friend(textBox2.Text, ScreenShower.settings.port, ScreenShower.nm);
#warning Раскоментить
            //if (ScreenShower.nm.isLocalIP(textBox2.Text))
            //{
            //    MessageBox.Show(null, "Нельзя подключаться к самому себе.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return;
            //}
            progressBar.Visible = true;
            progressBar.MarqueeAnimationSpeed = 100;
            button2.Enabled = false;
            connStatus.Text = "Подключение к " + fr.getLocalAddress();
            Thread connecter = new Thread(delegate()
                {
                    bool result = fr.connectToFriend();
                    object[] paramS = new object[] {result, fr};
                    Action ac = new Action(() => 
                    {
                        this.endConnection(result , fr);
                    });
                    this.Invoke(ac);
                });
            connecter.Start();
            
        }

        public void endConnection(bool isSuccess, Friend friend)
        {
            progressBar.Visible = false;
            progressBar.MarqueeAnimationSpeed = 0;
            button2.Enabled = true;
            connStatus.Text = "";
            if (!isSuccess)
                MessageBox.Show(this, "Не удалось подключиться к " + friend.getLocalAddress(), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                
                ScreenShower.nm.addFriend(friend);
            }
        }

        private void saveSettings(object sender, EventArgs e)
        {
            ScreenShower.settings = new Settings(textBox1.Text, checkBox1.Checked, comboBox1.SelectedIndex, Int32.Parse(textBox3.Text), textBox4.Text, comboBox2.SelectedIndex);
            ScreenShower.settings.saveSetting();
            sShower.updateSettings();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            Friend[] friends = ScreenShower.nm.getFriends();
            foreach (Friend fr in friends)
            {
                listBox1.Items.Add(fr.getStatus());
            }

        }

    }
    
   
}
