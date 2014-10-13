using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;

namespace DYsmartAspClient
{
    public class DYsmartAsp
    {
        string ServerIP;
        public DYsmartAsp(string smartURL)
        {
            ServerIP = smartURL;
        }

        public byte[] Send(byte[] order)
        {
            WebRequest request = WebRequest.Create(ServerIP);
            request.Method = "POST";
            byte[] dt = Merge(order, Encoding.UTF8.GetBytes(new char[] { '\r', '\n' }));
            dt = Merge(GetDYBytes(dt.Length + 4), dt);
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = dt.Length;
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(dt, 0, dt.Length);
            dataStream.Close();

            byte[] data;
            byte[] result;
            byte[] buffer = new byte[1024];
            using (WebResponse response = request.GetResponse())
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        int count = 0;
                        do
                        {
                            count = responseStream.Read(buffer, 0, buffer.Length);
                            memoryStream.Write(buffer, 0, count);
                        } while (count != 0);
                        data = memoryStream.ToArray();
                        result = new byte[data.Length - 4];
                        Buffer.BlockCopy(data, 4, result, 0, data.Length - 4);
                        data = null;
                        buffer = null;
                    }
                }
            }
            return result;
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

        Byte[] GetDYBytes(Int32 data)
        {
            return BitConverter.GetBytes(data);
        }

    }
}
