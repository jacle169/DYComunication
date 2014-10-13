using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;

namespace DYLicensingServer
{
    public class DYLicensing
    {
        public delegate void validationDelegate(licensingClass validata);
        public validationDelegate valiHandle { get; set; }
        DYcryption cry = new DYcryption();

        public void startLicensing(Page page, string cryKey)
        {
            readOrder(page, cryKey);
        }

        void readOrder(Page page, string cryKey)
        {
            var data = page.Request.BinaryRead(page.Request.ContentLength);
            if (data.Length > 0)
            {
                byte[] iv= new byte[8];
                Buffer.BlockCopy(data, 0, iv, 0, iv.Length);
                byte[] redt = new byte[data.Length - 8];
                Buffer.BlockCopy(data, 8, redt, 0, redt.Length);
                var pMsg = cry.desDecode(redt, Encoding.UTF8.GetBytes(cryKey), iv);
                if (pMsg != null)
                {
                    var licensing = readLiceningClass(page.Request.ContentEncoding.GetString(pMsg));
                    if (licensing != null)
                    {
                        if (valiHandle != null)
                        {
                            valiHandle(licensing);
                        }
                    }
                }
            }
        }

        public void SendBack(Page page, string crykey, DateTime time, string key, resultSkin result)
        {
            if (result == resultSkin.Pass)
            {
                send(page, crykey, time, key, "pass by dyLicensing");
            }
            else if (result == resultSkin.NotUsing)
            {
                send(page, crykey, time, key, "key not using");
            }
            else if (result == resultSkin.KeyError)
            {
                send(page, crykey, time, key, "key error");
            }
            else if (result == resultSkin.EnableTimeOut)
            {
                send(page, crykey, time, key, "key time out");
            }
        }

        licensingClass readLiceningClass(string input)
        {
            string[] redata = input.Split(',');
            if (redata.Length != 3)
            {
                return null;
            }
            if (!checkLong(redata[2]))
            {
                return null;
            }
            string cpu = redata[0];
            string rekey = redata[1];
            DateTime retime = new DateTime(long.Parse(redata[2]));
            return new licensingClass() { key = rekey, tm = retime,  cpu=cpu };
        }

        bool checkLong(string input)
        {
            try
            {
                var dd = long.Parse(input);
                return true;
            }
            catch
            {
                return false;
            }
        }

        void send(System.Web.UI.Page page, string crykey, DateTime time, string key, string data)
        {
            string iv = Guid.NewGuid().ToString("N").Substring(0, 8);
            var msg = cry.desEncode(getLicensingBytes(time, key, data),
                Encoding.UTF8.GetBytes(crykey), Encoding.UTF8.GetBytes(iv));
            page.Response.BinaryWrite(Merge(Encoding.UTF8.GetBytes(iv), msg));
        }

        byte[] getLicensingBytes(DateTime time,string key,string msg)
        {
            string stime = time.Ticks.ToString();
            string licensing = key + "," + time.Ticks.ToString() + "," + msg;
            return Encoding.UTF8.GetBytes(licensing);
        }

        Byte[] Merge(params Byte[][] args)
        {
            Int32 length = 0;
            foreach (byte[] tempbyte in args)
            {
                length += tempbyte.Length;
            }
            Byte[] bytes = new Byte[length];
            Int32 tempLength = 0;
            foreach (byte[] tempByte in args)
            {
                tempByte.CopyTo(bytes, tempLength);
                tempLength += tempByte.Length;
            }
            return bytes;
        }

    }

    public class licensingClass
    {
        public string key { get; set; }
        public DateTime tm { get; set; }
        public string cpu { get; set; }
    }

    public enum resultSkin
    {
        Pass,
        NotUsing,
        KeyError,
        EnableTimeOut
    }

}
