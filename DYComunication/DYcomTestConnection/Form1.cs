using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DYComWinClient;
using System.Threading.Tasks;

namespace DYcomTestConnection
{
    public partial class Form1 : Form
    {
        List<ClientPoxy> poxys;
        public Form1()
        {
            InitializeComponent();
            poxys = new List<ClientPoxy>();
            Load += new EventHandler(Form1_Load);
        }
        int c = 0;
        void Form1_Load(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                for (int i = 0; i < 10000; i++)
                {
                    ClientPoxy temp = new ClientPoxy("127.0.0.1", 4510, 8);
                    this.poxys.Add(temp);
                    this.BeginInvoke(new EventHandler((a, b) =>
                    {
                        this.label1.Text = this.poxys.Count.ToString();
                    })).AsyncWaitHandle.WaitOne(10);

                    System.Threading.Thread.Sleep(2);
                }
            });
        }

    }
}
