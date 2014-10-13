using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace DYCom_Net2.licens
{
    internal class other
    {
        static other otherManager;

        internal static other getOther()
        {
            if (otherManager == null) { return otherManager = new other(); } else { return otherManager; }
        }
        // 公钥 
        string pubkey = "<RSAKeyValue><Modulus>l+v5oQSuvF1AqJrzhNcax8DK2wFK92ed0WTqAumoyb1/nIFBqP0iFBTbbIO3SoPgc2N03nCUEv1ujBWvskhhQW+xM4L6IaumlsFnolx8CYl9QkTF9pjJBzj9isPLQ3/BgnQSQagc0PbV2dOGQ7KsVgJ2MGty265TRYxMZ18oYK0=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

        internal bool checkKey(string keyValue)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(pubkey);
                RSAPKCS1SignatureDeformatter f = new RSAPKCS1SignatureDeformatter(rsa);
                f.SetHashAlgorithm("SHA1");
                try
                {
                    byte[] key = Convert.FromBase64String(keyValue);
                    SHA1Managed sha = new SHA1Managed();
                    byte[] name = sha.ComputeHash(ASCIIEncoding.ASCII.GetBytes(DiskId()));
                    if (f.VerifySignature(name, key))
                    {
                        return true;
                    }
                }
                catch { }
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
