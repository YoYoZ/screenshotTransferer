using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace screeenshotSender
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            notifyIcon1.ShowBalloonTip(50000, "TEST", "tipText", ToolTipIcon.Error);
            ContextMenu cm = new ContextMenu();
            MenuItem[] mn = new MenuItem[3];
            notifyIcon1.ContextMenu = cm;
            this.WindowState = FormWindowState.Minimized;
            this.Visible = false;
            this.Hide();
        }
        private void connect()
        { 
        }
    }
}
