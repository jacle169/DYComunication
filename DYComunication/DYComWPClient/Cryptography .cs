using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace DYComWPClient
{
    /// <summary> 
    /// 加密解密功能
    /// </summary> 
    public class DYcryption 
    {
        /// <summary> 
        /// 加密byte[]消息内容
        /// </summary> 
        /// <param name="input">被加密内容</param> 
        /// <param name="password">加密口令，密须为8位字符串，否则返加null值</param> 
        /// <returns>加密后byte[]消息内容</returns> 
        public byte[] EncryptAES(byte[] input, string password)
        {
            if (password.Length == 8)
            {
                byte[] utfdata = input;
                byte[] saltBytes = UTF8Encoding.UTF8.GetBytes(password);
                AesManaged aes = new AesManaged();
                Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(password, saltBytes);
                aes.BlockSize = aes.LegalBlockSizes[0].MaxSize;
                aes.KeySize = aes.LegalKeySizes[0].MaxSize;
                aes.Key = rfc.GetBytes(aes.KeySize / 8);
                aes.IV = rfc.GetBytes(aes.BlockSize / 8);
                ICryptoTransform encryptTransf = aes.CreateEncryptor();
                MemoryStream encryptStream = new MemoryStream();
                CryptoStream encryptor = new CryptoStream(encryptStream, encryptTransf, CryptoStreamMode.Write);
                encryptor.Write(utfdata, 0, utfdata.Length);
                encryptor.Flush();
                encryptor.Close();
                return encryptStream.ToArray();
            }
            else
            {
                return null;
            }
        }

        /// <summary> 
        /// 解密byte[]消息内容
        /// </summary> 
        /// <param name="Input">被解密内容</param> 
        /// <param name="password">解密口令，与加密口令一致才能正常解密，须为8位字符串，否则返加null值</param> 
        /// <returns>解密后byte[]消息内容</returns> 
        public byte[] DecryptAES(byte[] Input, string password)
        {
            if (password.Length == 8)
            {
                byte[] encryptBytes = Input;
                byte[] saltBytes = Encoding.UTF8.GetBytes(password);
                AesManaged aes = new AesManaged();
                Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(password, saltBytes);
                aes.BlockSize = aes.LegalBlockSizes[0].MaxSize;
                aes.KeySize = aes.LegalKeySizes[0].MaxSize;
                aes.Key = rfc.GetBytes(aes.KeySize / 8);
                aes.IV = rfc.GetBytes(aes.BlockSize / 8);
                ICryptoTransform decryptTrans = aes.CreateDecryptor();
                MemoryStream decryptStream = new MemoryStream();
                CryptoStream decryptor = new CryptoStream(decryptStream, decryptTrans, CryptoStreamMode.Write);
                decryptor.Write(encryptBytes, 0, encryptBytes.Length);
                decryptor.Flush();
                decryptor.Close();
                return decryptStream.ToArray();
            }
            else
            {
                return null;
            }
        }
    }
}
