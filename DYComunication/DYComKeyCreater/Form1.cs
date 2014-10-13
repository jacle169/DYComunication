using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.IO;

namespace DYComKeyCreater
{
    public partial class Form1 : Form
    {
        string prikey = "<RSAKeyValue><Modulus>kaqsIGygRuV04vm6jXR0fQLhnX8Klmwl5YaGRTHudj12AfJ0GpZFalE6LEu91j5UD2ex/c1NV/sGI5xo2BI1aZDIHeFQeFGY29U+u2YnKRiSBnuxLcEWabMMB/o+jgAX+/smf6hRYxJoygKLy+WXcgTJtagzxXbNbz8ma8q4GSk=</Modulus><Exponent>AQAB</Exponent><P>yCEg4Yfr0+NgVQo2QG39nuzGtFLOLXgUIqOv9C1f4FckirePuN5X/4w1icTKyH5EZFRAUb2tkWHJHeOUzXl5vw==</P><Q>ulUyMNTLlg97C4KR2GoUnjYRsVcKCrBCsaJNUVGbRLi+bYfIZdKVjKeLknzdJAGZv0gF/t0vYUnb6nufoKIXFw==</Q><DP>aKAZJo7+lTmr5Ql/r1NRYkJ6507bBx5duHZGyKroEsq8CeFJO+bRroHIg4vkT8jjTGhXb+Rv1y9+CygtZPZ61Q==</DP><DQ>oj+RJqgEZjQwpkMZj+I+9cyK92qc2dXFHTwAKzDuDJb5ahJz0wXdJs61X+bOAI5MPB0Q623Z1dMkZTckNhEuHQ==</DQ><InverseQ>DA5zxGdh4Qu3HRYmbBWWeeFVe4PBBUdBEOPH0vc/QDeBvFHwo8ois9ieHBx69O52oDg+9Uc4t0+kueOWZzv0sg==</InverseQ><D>Tw0eaGJLAO6ZZmw8T3P5m7YqlnxRVJzQXCWLTJXyYXytRBU96QfQGAiI120rUs90cv/FWoVGCg8Sn9TMGSaYkSXkQXnaUr/kGK87u1wjJbZkOdB+nm63NMptE3ZP2PTBCkHHDG/mAqGYuoGvjqIpp3k2XQxe0SCT7deYeXTrBhk=</D></RSAKeyValue>";

        public Form1()
        {
            InitializeComponent();
            button1.Click += new EventHandler(button1_Click);
            button4.Click += new EventHandler(button4_Click);
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
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                ////    // 公钥 
                //var pbk = rsa.ToXmlString(false);

                ////    // 私钥 
                //var pk = rsa.ToXmlString(true);

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

    }
}
