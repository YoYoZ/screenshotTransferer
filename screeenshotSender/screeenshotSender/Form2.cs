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
    public partial class Form2 : Form
    {
        private List<string[]> friends;
        public Form2()
        {
            InitializeComponent();
        }

        private void selectFolder(object sender, EventArgs e)
        {
           FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.ShowDialog();
            textBox1.Text = fbd.SelectedPath;
        }

        private void addFriends()
        {
            friends.Add(string[] = {"name", "ip"});
        }
        private void saveSettings()
        {

        }
    }
}
