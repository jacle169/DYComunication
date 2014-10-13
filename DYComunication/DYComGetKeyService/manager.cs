using System;
using System.Collections.Generic;
using System.Web;
using System.Security.Cryptography;
using System.Text;
using System.Data.EntityClient;
using System.Configuration;

namespace DYComGetKeyService
{
    public class manager
    {
        internal EntityConnection createConnection()
        {
            EntityConnectionStringBuilder esb = new EntityConnectionStringBuilder();
            esb.Provider = "System.Data.SqlClient";
            esb.Metadata = @"res://*/Model1.csdl|res://*/Model1.ssdl|res://*/Model1.msl";
            esb.ProviderConnectionString = Decrypt(ConfigurationManager.AppSettings["dycomSqlConString"].ToString());
            return new EntityConnection(esb.ConnectionString);
        }

        internal string Encrypt(string pToEncrypt)
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

        internal string Decrypt(string pToDecrypt)
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
            catch (Exception ex) { return ex.Message; }
        }
    }
}