using System;
using System.Text;
using System.Net;
using System.IO;
using System.Management;
using System.Security.Cryptography;
using System.Threading;

namespace DYCom_Net2
{
    internal class LicensingClient
    {
        internal delegate void resultDelegate(bool result, string message);

        string ExprxMessage = string.Empty;
        internal resultDelegate ui { get; set; }
        Random rd = new Random();
        /// <summary>
        /// 启动验证服务
        /// </summary>
        /// <param name="LicensingServerUri">验证服务器地址</param>
        /// <param name="key">产品密钥</param>
        /// <param name="cryKey">第三方加密口令(8位字符串）</param>
        /// <param name="LoopSleepTime">验证时间间隔(验证将以秒为单位，每隔本参数进行验证提交时间比对)</param>
        /// <param name="mixRandomTime">最少验证时间值(以秒为单位)</param>
        /// <param name="maxRandomTime">最大验证时间值(以少为单位)</param>
        /// <param name="canWrong">容错次数(允许验证错误的最大值)</param>
        internal void SendLicensing(string LicensingServerUri, string key, string cryKey, int LoopSleepTime, int mixRandomTime, int maxRandomTime, int canWrong)
        {
            startLicensing(new inputClass()
            {
                serverUrl = LicensingServerUri,
                sekey = key,
                crkey = cryKey,
                loopSleep = LoopSleepTime,
                maxRandomLIcensingTime = maxRandomTime,
                mixRandomLicensingTime = mixRandomTime,
                sureWrong = canWrong
            });
        }

        void startLicensing(inputClass input)
        {
            Thread checkert = new Thread(checker);
            checkert.IsBackground = true;
            checkert.Start(input);
        }

        void checker(object obj)
        {
            inputClass ic = (inputClass)obj;
            DateTime checkTime = DateTime.Now.AddSeconds(rd.Next(ic.mixRandomLicensingTime, ic.maxRandomLIcensingTime));
            int checkCount = 0;
            bool check = true;
            while (check)
            {
                if (DateTime.Now.Subtract(checkTime).TotalSeconds >= 0)
                {
                    check = false;
                    if (authenLicensing(ic.serverUrl, ic.sekey, ic.crkey))
                    {
                        if (ui != null)
                        {
                            checkCount = 0;
                            ui(true, ExprxMessage);
                        }
                        break;
                    }
                    else
                    {
                        check = true;
                        if (checkCount < ic.sureWrong)
                        {
                            checkCount++;
                            checkTime = DateTime.Now.AddSeconds(rd.Next(ic.mixRandomLicensingTime, ic.maxRandomLIcensingTime));
                        }
                        else if (checkCount == ic.sureWrong)
                        {
                            if (ui != null)
                            {
                                ui(false, ExprxMessage);
                            }
                            break;
                        }
                    }
                }
                System.Threading.Thread.Sleep(ic.loopSleep * 1000);
            }
        }

        bool authenLicensing(string LicensingServerUri, string key, string cryKey)
        {
            WebRequest request = WebRequest.Create(LicensingServerUri);
            request.Method = "POST";

            string iv = Guid.NewGuid().ToString("N").Substring(0, 8);
            var msg = Merge(Encoding.UTF8.GetBytes(iv), desEncode(firstPostData(key, DateTime.Now),
                Encoding.UTF8.GetBytes(cryKey), Encoding.UTF8.GetBytes(iv)));

            if (msg == null)
            {
                ExprxMessage = "错误0001(加密出错)";
                return false;
            }

            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = msg.Length;

            Stream dataStream;
            if (tryResponse(request, out dataStream))
            {
                dataStream.Write(msg, 0, msg.Length);
                dataStream.Close();
            }
            else
            {
                ExprxMessage = "错误0002(连接验证服务器出错)";
                return false;
            }


            WebResponse response;
            if (tryRequest(request, out response))
            {
                dataStream = response.GetResponseStream();
            }
            else
            {
                //连接验证服务器失败
                ExprxMessage = "错误0003(验证服务器返回过程出错)";
                return false;
            }

            byte[] LicensingBytes;
            if (readLiceningBytes(dataStream, out LicensingBytes))
            {
                byte[] biv = new byte[8];
                Buffer.BlockCopy(LicensingBytes, 0, biv, 0, biv.Length);
                byte[] redtt = new byte[LicensingBytes.Length - 8];
                Buffer.BlockCopy(LicensingBytes, 8, redtt, 0, redtt.Length);

                dataStream.Close();
                response.Close();

                var redt = desDecode(redtt, Encoding.UTF8.GetBytes(cryKey), biv);
                if (redt != null)
                {
                    var reData = readLiceningClass(redt);
                    if (reData != null)
                    {
                        TimeSpan ts = DateTime.Now.Subtract(reData.tm);
                        if (ts.TotalMinutes < 1 && ts.TotalSeconds > 0)
                        {
                            if (key.Equals(reData.key))
                            {
                                if (reData.result == "pass by dyLicensing")
                                {
                                    //验证通过
                                    ExprxMessage = "验证通过";
                                    return true;
                                }
                                else if (reData.result == "key not using")
                                {
                                    //服务端key被禁用
                                    ExprxMessage = "错误0004(密钥已过期)";
                                    return false;
                                }
                                else if (reData.result == "key error")
                                {
                                    //服务端key不存在
                                    ExprxMessage = "错误0005(密钥不存在)";
                                    return false;
                                }
                                else if (reData.result == "key time out")
                                {
                                    //密钥过期
                                    ExprxMessage = "错误0012(密钥过期)";
                                    return false;
                                }
                                else
                                {
                                    //未知错误
                                    ExprxMessage = "错误0006(返回值中未知错误)";
                                    return false;
                                }
                            }
                            else
                            {
                                //本地key错误
                                ExprxMessage = "错误0007(密钥比对时未知错误)";
                                return false;
                            }
                        }
                        else
                        {
                            //验证超时
                            ExprxMessage = "错误0008(验证过程操作超时)";
                            return false;
                        }
                    }
                    else
                    {
                        //空返回值
                        ExprxMessage = "错误0009(验证返回数据结构错误)";
                        return false;
                    }

                }
                else
                {
                    //读取返回数据错误，黑客
                    ExprxMessage = "错误0010(验证返回数据时解密错误)";
                    return false;
                }
            }
            else
            {
                //读取返回数据空，黑客
                ExprxMessage = "错误0011(返回数据不合法)";
                return false;
            }
        }


        licensingClass readLiceningClass(byte[] input)
        {
            string[] redata = Encoding.UTF8.GetString(input).Split(',');
            if (redata.Length != 3)
            {
                return null;
            }
            if (!checkLong(redata[1]))
            {
                return null;
            }
            string rekey = redata[0];
            DateTime retime = new DateTime(long.Parse(redata[1]));
            string rest = redata[2];
            return new licensingClass() { key = rekey, tm = retime, result = rest.TrimEnd('\0') };
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

        bool readLiceningBytes(Stream dataStream, out byte[] Licensing)
        {
            using (MemoryStream stmMemory = new MemoryStream())
            {
                byte[] buffer = new byte[1024];
                int i;
                try
                {
                    while ((i = dataStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        stmMemory.Write(buffer, 0, i);
                    }

                    if (stmMemory.Length > 30)
                    {
                        Licensing = stmMemory.ToArray();
                        return true;
                    }
                    else
                    {
                        Licensing = null;
                        return false;
                    }
                }
                catch
                {
                    Licensing = null;
                    return false;
                }
            }
        }

        bool tryResponse(WebRequest request, out Stream sm)
        {
            try
            {
                Stream dataStream = request.GetRequestStream();
                sm = dataStream;
                return true;
            }
            catch
            {
                sm = null;
                return false;
            }
        }

        bool tryRequest(WebRequest request, out WebResponse response)
        {
            try
            {
                WebResponse respon = request.GetResponse();
                response = respon;
                return true;
            }
            catch
            {
                response = null;
                return false;
            }
        }

        byte[] firstPostData(string key, DateTime dt)
        {
            string licensing = getCPU() + "," + key + "," + dt.Ticks.ToString();
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

        string getCPU()
        {
            string cpuInfo = String.Empty;
            string temp = String.Empty;
            using (ManagementClass mc = new ManagementClass("Win32_Processor"))
            {
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    if (cpuInfo == String.Empty)
                    {
                        cpuInfo = mo.Properties["ProcessorId"].Value.ToString();
                    }
                }
            }
            return cpuInfo;
        }

        byte[] desEncode(byte[] input, byte[] key, byte[] iv)
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

        byte[] desDecode(byte[] input, byte[] key, byte[] iv)
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
    internal class inputClass
    {
        internal string serverUrl { get; set; }
        internal string sekey { get; set; }
        internal string crkey { get; set; }
        internal int loopSleep { get; set; }
        internal int mixRandomLicensingTime { get; set; }
        internal int maxRandomLIcensingTime { get; set; }
        internal int sureWrong { get; set; }
    }
    internal class licensingClass
    {
        internal string key { get; set; }
        internal DateTime tm { get; set; }
        internal string result { get; set; }
    }
}
