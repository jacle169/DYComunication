using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.IO;

namespace DYComKeyCreaterForPoxy
{
    class other
    {
        static other otherManager;

        internal static other getOther()
        {
            if (otherManager == null) { return otherManager = new other(); } else { return otherManager; }
        }
        string pubkey = "<RSAKeyValue><Modulus>kaqsIGygRuV04vm6jXR0fQLhnX8Klmwl5YaGRTHudj12AfJ0GpZFalE6LEu91j5UD2ex/c1NV/sGI5xo2BI1aZDIHeFQeFGY29U+u2YnKRiSBnuxLcEWabMMB/o+jgAX+/smf6hRYxJoygKLy+WXcgTJtagzxXbNbz8ma8q4GSk=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

        internal string ReadKeyTxt()
        {
            FileStream fs = new FileStream("key.txt", FileMode.Open, FileAccess.Read);
            using (StreamReader m_streamReader = new StreamReader(fs))
            {
                m_streamReader.BaseStream.Seek(0, SeekOrigin.Begin);
                return m_streamReader.ReadToEnd();
            }
        }

        internal void WriteKeyTxt(string data)
        {
            FileStream fs = new FileStream("key.txt", FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter m_streamWriter = new StreamWriter(fs);
            m_streamWriter.Flush();
            m_streamWriter.BaseStream.Seek(0, SeekOrigin.Begin);
            m_streamWriter.Write(data);
            m_streamWriter.Flush();
            m_streamWriter.Close();
        }

        internal bool checkKey()
        {
            if (!File.Exists(@"\key.txt"))
            {
                using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
                {
                    rsa.FromXmlString(pubkey);
                    RSAPKCS1SignatureDeformatter f = new RSAPKCS1SignatureDeformatter(rsa);
                    f.SetHashAlgorithm("SHA1");
                    try
                    {
                        byte[] key = Convert.FromBase64String(ReadKeyTxt().Trim());
                        SHA1Managed sha = new SHA1Managed();
                        byte[] name = sha.ComputeHash(ASCIIEncoding.ASCII.GetBytes(DiskId()));
                        if (f.VerifySignature(name, key))
                        {
                            return true;
                        }
                    }
                    catch { }
                }
            }
            return false;
        }

        internal string DiskId()
        {
            const int MAX_FILENAME_LEN = 256;
            int retVal = 0;
            int a = 0;
            int b = 0;
            string str1 = null;
            string str2 = null;
            GetVolumeInformation(
                   @"C:\",
                   str1,
                   MAX_FILENAME_LEN,
                   ref retVal,
                   a,
                   b,
                   str2,
                   MAX_FILENAME_LEN);
            return Math.Abs(retVal).ToString();
        }

        [DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
        public static extern bool GetVolumeInformation(
        string lpRootPathName,
        string lpVolumeNameBuffer,
        int nVolumeNameSize,
        ref int lpVolumeSerialNumber,
        int lpMaximumComponentLength,
        int lpFileSystemFlags,
        string lpFileSystemNameBuffer,
        int nFileSystemNameSize); 
    }
}
