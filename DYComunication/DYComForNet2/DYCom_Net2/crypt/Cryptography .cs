using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace DYCom_Net2
{
    /// <summary> 
    /// 加密组件
    /// </summary> 
    public class DYcryption 
    {
        /// <summary>
        /// DES加密消息
        /// </summary>
        /// <param name="input">须加密的数据</param>
        /// <param name="password">加密口令，须为8位字符串，否则返加null值</param>
        /// <returns>加密后byte[]消息内容</returns>
        public byte[] EncryptDES(byte[] input, string password)
        {
            try
            {
                if (password.Length == 8)
                {
                    MemoryStream ms = new MemoryStream();
                    DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                    ICryptoTransform ct = des.CreateEncryptor(getASCIIbytes(password), getASCIIbytes(password));
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

        byte[] getASCIIbytes(string input)
        {
            return ASCIIEncoding.ASCII.GetBytes(input);
        }

        /// <summary>
        /// DES解密消息
        /// </summary>
        /// <param name="Input">须解密的数据</param>
        /// <param name="password">解密口令，与加密口令必须一致,须为8位字符串，否则返加null值</param>
        /// <returns>解密后byte[]消息内容</returns>
        public byte[] DecryptDES(byte[] Input, string password)
        {
            try
            {
                if (password.Length == 8)
                {
                    MemoryStream ms = new MemoryStream(Input);
                    DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                    ICryptoTransform ct = des.CreateDecryptor(getASCIIbytes(password), getASCIIbytes(password));
                    CryptoStream cs = new CryptoStream(ms, ct, CryptoStreamMode.Read);
                    byte[] bb = new byte[Input.Length];
                    cs.Read(bb, 0, Input.Length);
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
