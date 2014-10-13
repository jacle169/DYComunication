using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DYComServerInWinform
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(ondycom));
            t.IsBackground = true;
            t.Start();
        }
        void ondycom(object obj)
        {
            DYComServer server = new DYComServer();
        }
    }
}
