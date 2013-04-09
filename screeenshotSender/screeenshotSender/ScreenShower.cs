﻿using System;
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
        public static Logger log;
        private Icon normalIcon, notWatchedIcon, receivingIcon, sendingIcon;
        public bool isNotWatched;


        private bool isLeftMouseDown;
        private Image selectedImage;
        private int x0, y0, x, y;

        public ScreenShower()
        {  
            Type t = typeof(Icon);
            InitializeComponent();
            settings = Settings.loadSetting(Directory.GetCurrentDirectory() + "\\settings.cfg");
            if (settings == null)
            {
                settings = new Settings(Directory.GetCurrentDirectory(), true, 2, 4567, "Nickname", 4);
            }
            AppIcon.Visible = true;
            log = new Logger(settings.path, true);
            log.writeParamToLog("Program Initialized succesfully");
            
            normalIcon = screeenshotSender.Properties.Resources.normal;
            notWatchedIcon = screeenshotSender.Properties.Resources.notwatched;
            receivingIcon = screeenshotSender.Properties.Resources.receiving;
            sendingIcon = screeenshotSender.Properties.Resources.sending;
        }

        private void Settings_Click(object sender, EventArgs e)
        {
            SettingsScreen Ss = new SettingsScreen(this);
            Ss.Show();
        }

        public enum StateofIcon
        {
            normal, notwatched, receiving, sending
        }

        public void changeIcon(StateofIcon st)
        {
            Action ac = new Action(delegate()
            {
                switch (st)
                {
                    case StateofIcon.normal:
                        if (isNotWatched)
                            AppIcon.Icon = notWatchedIcon;
                        else
                            AppIcon.Icon = normalIcon;
                        break;
                    case StateofIcon.notwatched:
                        AppIcon.Icon = notWatchedIcon;
                        break;
                    case StateofIcon.receiving:
                        AppIcon.Icon = receivingIcon;
                        break;
                    case StateofIcon.sending:
                        AppIcon.Icon = sendingIcon;
                        break;
                }
            });
            this.Invoke(ac);
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            NetManager.isManagerAlive = false;
            Friend[] friends = nm.getFriends();
            foreach (Friend fr in friends)
            {
                fr.killFriend();
            }
            nm.saveFriendList();
            log.writeParamToLog("Program Shutting down");
            log.stopLogger();
            Environment.Exit(0);
           
        }

        private void ScreenShower_Load(object sender, EventArgs e)
        {
                nm = new NetManager(this);
                bool result = nm.initServer(settings.port, settings.nick);
                if (!result)
                {
                    log.writeParamToLog("Error while initializing server; probably busy port?");
                    MessageBox.Show(this, "Не удалось создать сервер!", "Ошибочка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    nm.startUpdater();
                    nm.loadAndConnectToFriends();
                }
        }

        public void displayIconNotification(string name,string message)
        {
            AppIcon.ShowBalloonTip(10, name, message, ToolTipIcon.Info);
        }

        private void AppIcon_Click(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {

                    log.writeParamToLog("screenshot sent " + nm.getFriendsCount() + " friends");
                    ScreenCapture sc = new ScreenCapture();
                    Bitmap temp = new Bitmap(sc.CaptureScreen());
                    ImageLD ld = new ImageLD();
                    pictureBox1.Image = ld.SetBrightness(-100, temp);
                    showScreenCutter();
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Middle)
            {
                isNotWatched = false;
                changeIcon(StateofIcon.normal);
                nm.showLastScreenshot();
            }
        }

        public void updateSettings()
        {
            nm.updateUsername(settings.nick);
        }

        public void showScreenCutter()
        {
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            this.Opacity = 100;
            this.BringToFront();
        }

        private void updateDrawing(object sender, EventArgs e)
        {
            isLeftMouseDown = MouseButtons == System.Windows.Forms.MouseButtons.Left;
            
            pictureBox1.Image = selectedImage;
            Graphics g = Graphics.FromImage(selectedImage);
            if (isLeftMouseDown)
            {
                
            }
        }

        private void mouseCliecked(object sender, EventArgs e)
        {

        }
    }
}
