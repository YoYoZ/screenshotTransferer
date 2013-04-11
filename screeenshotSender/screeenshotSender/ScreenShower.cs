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
        public static Logger log;
        private Icon normalIcon, notWatchedIcon, receivingIcon, sendingIcon;
        public bool isNotWatched;


        private Image selectedImage, imageClone;
        private Graphics g;
        private int x0, y0, x, y;
        private Rectangle r;

        public ScreenShower()
        {
            InitializeComponent();
            settings = Settings.loadSetting(Directory.GetCurrentDirectory() + "\\settings.cfg");
            if (settings == null)
            {
                settings = new Settings(Directory.GetCurrentDirectory(), true, 2, 4567, "Nickname", 4);
            }
            log = new Logger(settings.path, true);
            log.writeParamToLog("Program is loading...");
            AppIcon.Visible = true;
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
                log.writeParamToLog("Program initializated!");
        }

        public void displayIconNotification(string name,string message)
        {
            AppIcon.ShowBalloonTip(10, name, message, ToolTipIcon.Info);
        }

        private void AppIcon_Click(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {

                    log.writeParamToLog("Screenshot sent " + nm.getFriendsCount() + " friends");
                    ScreenCapture sc = new ScreenCapture();
                    selectedImage = sc.CaptureScreen();
                    showEditor();
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

        private void showEditor()
        {
            this.Opacity = 100;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            this.BringToFront();
            timer1.Start();
        }

        private void closeEditor()
        {
            timer1.Stop();
            this.Opacity = 0;
            this.AppIcon.Visible = true;
            g = null;
            selectedImage = null;
            imageClone = null;
            x0 = 0;
            y0 = 0;
            x = 0;
            y = 0;
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                this.x0 = e.X;
                this.y0 = e.Y;
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                bool var1 = r.Contains(new Point(e.X, e.Y));
                if (var1)
                {
                   Bitmap original = new Bitmap(selectedImage);
                   Bitmap areaofOriginal = original.Clone(r, original.PixelFormat);
                   Image im = Image.FromHbitmap(areaofOriginal.GetHbitmap());
                   DialogResult dr = MessageBox.Show(this,  "Послать скриншот " + nm.getFriendsCount() + " друзьям?" , "СкриншотоПосылатель 3000", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dr == System.Windows.Forms.DialogResult.Yes)
                    {
                        nm.sendScreen(im);
                        log.writeParamToLog("INFO " + "Weight= " + r.Width + " Height= " + r.Height);
                    }

                }
                closeEditor();
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            this.x = e.X;
            this.y = e.Y;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (MouseButtons == System.Windows.Forms.MouseButtons.Left)
            {
                imageClone = (Image)selectedImage.Clone();
                g = Graphics.FromImage(imageClone);
                this.pictureBox1.Image = imageClone;

                if (x0 != 0 && y0 != 0)
                {
                    r = new Rectangle(x0, y0, x - x0, y - y0);
                    g.DrawRectangle(Pens.Blue, r);
                    this.Refresh();
                }
            }
        }
    }
}
