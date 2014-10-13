using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace DYCom_Net2
{
    public class DYudpCore
    {
        UdpClient udpClient;
        IPEndPoint remoteEND;
        public Dictionary<string, udpObj> udps;
        int bufSize = 512;
        public OnUdpData onData { get; set; }

        public DYudpCore(int port, int bufferSize)
        {
            udpClient = new UdpClient(port);
            remoteEND = new IPEndPoint(IPAddress.Any, 0);
            udps = new Dictionary<string, udpObj>();
            bufSize = bufferSize;
            Thread td = new Thread(new System.Threading.ThreadStart(doRead));
            td.Start();
        }

        static byte[] dt;
        void doRead()
        {
            while (true)
            {
                Byte[] receiveBytes = udpClient.Receive(ref remoteEND);
                if (!udps.ContainsKey(remoteEND.ToString()))
                {
                    udps.Add(remoteEND.ToString(), new udpObj() { buf = new BuffList(bufSize), lastPostTime = DateTime.Now });
                }
                udps[remoteEND.ToString()].buf.InsertByteArray(receiveBytes);
                do
                {
                    byte[] buff = udps[remoteEND.ToString()].buf.GetData();
                    if (buff != null)
                    {
                        dt = new byte[buff.Length - 4];
                        Buffer.BlockCopy(buff, 4, dt, 0, dt.Length);
                        ThreadPool.QueueUserWorkItem(ondata, new DYudpMessage() { RemoteHost = remoteEND, data = dt });
                    }
                    else
                        break;
                } while (true);
            }
        }
        void ondata(object data)
        {
            var m = (DYudpMessage)data;
            onData(m.data, m.RemoteHost.ToString());
        }

    }

    internal class DYudpMessage
    {
        /// <summary>
        /// 接入端信息
        /// </summary>
        /// <remarks>保存了一个相应接入端信息</remarks>
        public IPEndPoint RemoteHost { get; set; }
        /// <summary>
        /// 消息体
        /// </summary>
        /// <remarks>包含了一条完整的消息内容</remarks>
        public byte[] data { get; set; }
    }

    public class udpObj
    {
        internal BuffList buf { get; set; }
        public DateTime lastPostTime { get; set; }
    }
}
