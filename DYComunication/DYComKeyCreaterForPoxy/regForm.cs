using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace DYComKeyCreaterForPoxy
{
    public partial class regForm : Form
    {
        public regForm()
        {
            InitializeComponent();
            Load += new EventHandler(regForm_Load);
            button1.Click += new EventHandler(button1_Click);
        }

        void button1_Click(object sender, EventArgs e)
        {
            if (richTextBox1.Text != string.Empty)
            {
                other.getOther().WriteKeyTxt(richTextBox1.Text.Trim());
                MessageBox.Show("注册信息已写入");
                this.Close();
            }
        }

        void regForm_Load(object sender, EventArgs e)
        {
            textBox1.Text = other.getOther().DiskId();
            if (File.Exists("key.txt"))
            {
                richTextBox1.Text = other.getOther().ReadKeyTxt();
            }
            if (other.getOther().checkKey())
            {
                MessageBox.Show("您的软件已经取得正版授权！非常感谢您使用DYCOM代理商授权发行器！");
            }
            else
            {
                MessageBox.Show("您目前使用的是试用版的DYCOM代理商授权发行器！");
            }
        }
    }
}
