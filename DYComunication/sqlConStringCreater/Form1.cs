using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Security.Cryptography;

namespace sqlConStringCreater
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        //@"Data Source=Sql1001.webweb.com,2433;User ID=DB_98B430_dylc_admin;
        //                        Password=20101937;MultipleActiveResultSets=True;Persist Security Info=True;";
        private void bt_write_Click(object sender, EventArgs e)
        {
            if (tb_sqlserver.Text != string.Empty && tb_userid.Text != string.Empty && tb_pwd.Text != string.Empty)
            {
                richTextBox1.Text = Encrypt(string.Format(@"Data Source={0};User ID={1};Password={2};MultipleActiveResultSets=True;Persist Security Info=True;", 
                    tb_sqlserver.Text, tb_userid.Text, tb_pwd.Text));
            }
            else
            {
                richTextBox1.Text = "数据不齐";
            }
        }

        private void bt_read_Click(object sender, EventArgs e)
        {
            richTextBox2.Text = Decrypt(richTextBox1.Text);
        }

        string Encrypt(string pToEncrypt)
        {
            try
            {
                string sKey = "Ja5Sql@#";
                using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
                {
                    byte[] inputByteArray = Encoding.UTF8.GetBytes(pToEncrypt);
                    des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
                    des.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
                    System.IO.MemoryStream ms = new System.IO.MemoryStream();
                    using (CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(inputByteArray, 0, inputByteArray.Length);
                        cs.FlushFinalBlock();
                        cs.Close();
                    }
                    string str = Convert.ToBase64String(ms.ToArray());
                    ms.Close();
                    return str;
                }
            }
            catch (Exception Ex)
            {
                return Ex.Message;
            }
        }

        string Decrypt(string pToDecrypt)
        {
            try
            {
                string sKey = "Ja5Sql@#";
                byte[] inputByteArray = Convert.FromBase64String(pToDecrypt);
                using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
                {
                    des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
                    des.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
                    System.IO.MemoryStream ms = new System.IO.MemoryStream();
                    using (CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(inputByteArray, 0, inputByteArray.Length);
                        cs.FlushFinalBlock();
                        cs.Close();
                    }
                    string str = Encoding.UTF8.GetString(ms.ToArray());
                    ms.Close();
                    return str;
                }
            }
            catch(Exception ex) { return ex.Message; }
        }

    }
}
