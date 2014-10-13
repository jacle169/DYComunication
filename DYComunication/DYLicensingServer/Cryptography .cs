using System.Security.Cryptography;
using System.IO;

namespace DYLicensingServer
{
    internal class DYcryption
    {
        internal byte[] desEncode(byte[] input, byte[] key, byte[] iv)
        {
            try
            {
                if (key.Length == 8 && iv.Length == 8)
                {
                    MemoryStream ms = new MemoryStream();
                    DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                    ICryptoTransform ct = des.CreateEncryptor(key, iv);
                    CryptoStream cs = new CryptoStream(ms, ct, CryptoStreamMode.Write);
                    cs.Write(input, 0, input.Length);
                    cs.Close();
                    ms.Close();
                    return ms.ToArray();
                }
                else { return null; }
            }
            catch
            {
                return null;
            }
        }

        internal byte[] desDecode(byte[] input, byte[] key, byte[] iv)
        {
            try
            {
                if (key.Length == 8 && iv.Length == 8)
                {
                    MemoryStream ms = new MemoryStream(input);
                    DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                    ICryptoTransform ct = des.CreateDecryptor(key, iv);
                    CryptoStream cs = new CryptoStream(ms, ct, CryptoStreamMode.Read);
                    byte[] bb = new byte[input.Length];
                    cs.Read(bb, 0, input.Length);
                    cs.Close();
                    ms.Close();
                    return bb;
                }
                else { return null; }
            }
            catch
            {
                return null;
            }
        }
    }
}
