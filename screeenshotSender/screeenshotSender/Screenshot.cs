using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace screeenshotSender
{
    public partial class Screenshot : Form
    {
        public Screenshot(Image im, string username)
        {
            InitializeComponent();
            pictureBox.Image = im;
            this.Text = "Скриншот от " + username;
            this.SetDesktopBounds(0, 0, im.Width, im.Height);
            this.Update();
            this.Refresh();
        }
    }
}
