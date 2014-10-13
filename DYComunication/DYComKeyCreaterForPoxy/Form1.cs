using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Security.Cryptography;

namespace DYComKeyCreaterForPoxy
{
    public partial class Form1 : Form
    {
        string prikey = "<RSAKeyValue><Modulus>l+v5oQSuvF1AqJrzhNcax8DK2wFK92ed0WTqAumoyb1/nIFBqP0iFBTbbIO3SoPgc2N03nCUEv1ujBWvskhhQW+xM4L6IaumlsFnolx8CYl9QkTF9pjJBzj9isPLQ3/BgnQSQagc0PbV2dOGQ7KsVgJ2MGty265TRYxMZ18oYK0=</Modulus><Exponent>AQAB</Exponent><P>x0XWDF/XGJAedNgyBiQMpVc40wi0aR1kiyi4y/2wKBqy1z6evfBBsdjZBlF2SI1CkDGtAbyvr83CcM4RQNVsnQ==</P><Q>wytlTTEBftvI5JW9TNAwOn5JSoKUNUrC/BLIZ+tDYTdeuv3e24ZUmlB3Xy9F/JLmU48c+lA/Zse6YFMnH0IfUQ==</Q><DP>H/gSOPX/Og+U80Xj8JxD8xqlISYaW2q9wJ2N2Bwg8K4n1uRS70HmKDQTzTGwej8/WIa/rLGqtdeaxCIrHm2e+Q==</DP><DQ>M/XRuoNZipSpH3JeO50RugD1MkkhfC6zSrkVcdVI0xESv2XndzqEO7FAlq7XSy8w8v4fEOVce9ig3hRFTiUkgQ==</DQ><InverseQ>toK6e27h7FKwGy9Gr90q5FM/OXi24CLdluolcbTNgoKhMJ/EcLq0BZlxifQlJP2q2a3BZTkoaUV/pGruweUZnw==</InverseQ><D>gEGX3Xg2jI+tpqoqLFvtH/aDS6EryPKRKdYoOY1KEgcJVPdKl4Ac5Rc2p8YLV70+ICw23hPs7ptGWL12Nu0kLO09tCRB3pBVewAsrhm9q+sO1ApKX9ahMji/t5LhXOCP9Jyz+3DipWSxhNR9lnfFf9INEw243qS8dFqPen2bmAE=</D></RSAKeyValue>";

        public Form1()
        {
            InitializeComponent();
            button1.Click += new EventHandler(button1_Click);
            button4.Click += new EventHandler(button4_Click);
            button2.Click += new EventHandler(button2_Click);
        }

        void button2_Click(object sender, EventArgs e)
        {
            regForm rf = new regForm();
            rf.Show();
        }

        void button4_Click(object sender, EventArgs e)
        {
            if (richTextBox1.Text != string.Empty)
            {
                Clipboard.SetDataObject(richTextBox1.Text.Trim(), true);
                MessageBox.Show("已复制到粘贴板");
            }
        }

        void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != string.Empty)
            {
                richTextBox1.Text = createKey(textBox1.Text.Trim());
            }
        }

        string createKey(string machincId)
        {
            if (other.getOther().checkKey())
            {
                using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
                {
                    rsa.FromXmlString(prikey);
                    // 加密对象 
                    RSAPKCS1SignatureFormatter f = new RSAPKCS1SignatureFormatter(rsa);
                    f.SetHashAlgorithm("SHA1");
                    byte[] source = System.Text.ASCIIEncoding.ASCII.GetBytes(machincId);
                    SHA1Managed sha = new SHA1Managed();
                    byte[] result = sha.ComputeHash(source);

                    byte[] b = f.CreateSignature(result);

                    return Convert.ToBase64String(b);
                }
            }
            else
            {
                MessageBox.Show("你的程序未注册！");
                return null;
            }
        }



    }
}
