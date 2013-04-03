using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace screeenshotSender
{
    public partial class ScreenShower : Form
    {
        /// <summary>
        /// Reference to NetManager. Only a single copy per single program.
        /// </summary>
        public static NetManager nm;

        /// <summary>
        /// Reference to Settings.
        /// </summary>
        public static Settings settings;

        /// <summary>
        /// This is used to init Manger only one time.
        /// </summary>
        private static bool isManagedInitializated = false;

        public ScreenShower()
        {         
            InitializeComponent();
            settings = Settings.loadSetting(Directory.GetCurrentDirectory() + "\\settings.cfg");
            if (settings == null)
            {
                settings = new Settings(Directory.GetCurrentDirectory(), true, 2, 4567, "Nickname");
            }
            AppIcon.Visible = true;
        }

        private void Settings_Click(object sender, EventArgs e)
        {
            SettingsScreen Ss = new SettingsScreen();
            Ss.Show();
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            Friend[] friends = nm.getFriends();
            foreach (Friend fr in friends)
            {
                fr.killFriend();
            }
            Environment.Exit(0);
        }

        private void ScreenShower_Load(object sender, EventArgs e)
        {
            if (!isManagedInitializated)
            {
                nm = new NetManager();
                bool result = nm.initServer(settings.port, settings.nick);
                if (!result)
                {
                    MessageBox.Show(this, "Не удалось создать сервер!", "Ошибочка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                    isManagedInitializated = true;
            }
        }

        private void AppIcon_Click(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                DialogResult result = MessageBox.Show(this, "Послать скриншот всем вашим " + nm.getFriends().Length + " друзьям?", "СкриншотоПосылатель 3000", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    ScreenCapture sc = new ScreenCapture();
                    Image screen = sc.CaptureScreen();
                    nm.sendScreen(screen);
                }
            }
        }


    }
}
