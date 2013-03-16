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
            #warning Удалить следующие две строки, когда закончим.
            Form2 fr = new Form2();
            fr.Show();
        }

        public void generateSettings()
        {
            FileStream fs = new FileStream(Directory.GetCurrentDirectory()+@"/settings.cfg", FileMode.CreateNew, FileAccess.Write);
            StreamWriter sr = new StreamWriter(fs);
            sr.WriteLine("folder=" + Directory.GetCurrentDirectory());
            sr.WriteLine("alwaysDisplay=true");
            sr.WriteLine("timeout=2");
          
        }
        private void connect()
        { 
        }
        

    }
}
