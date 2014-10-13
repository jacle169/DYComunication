using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text;

namespace DYComWMClient
{
    /// <summary> 
    /// DYComWindowsMobile客户端
    /// </summary> 
    public class ClientPoxy :UserControl, IDisposable
    {
        /// <summary> 
        /// DYComWindowsMobile连接代理属性
        /// </summary> 
        public OnConnet onConnet { get; set; }
        /// <summary> 
        /// DYComWindowsMobile数据代理属性
        /// </summary> 
        public OnData onData { get; set; }
        /// <summary> 
        /// DYComWindowsMobile断开连接代理属性
        /// </summary> 
        public OnDisConnet onDisconnet { get; set; }

        private delegate void InvokeDelegate();
        Socket socket { get; set; }
        BuffList Bufflist;

        /// <summary>
        /// DYcom客户端代理启动
        /// </summary>
        /// <param name="ServerIP">服务器IP地址</param>
        /// <param name="Port">连接端口</param>
        /// <param name="BufferCount">缓冲区大小</param>
        public void Start(string ServerIP, int Port, int BufferCount)
        {
            try
            {
                IPEndPoint ipe;
                if (checkIPAddress(ServerIP))
                {
                    ipe = new IPEndPoint(IPAddress.Parse(ServerIP), Port);
                }
                else
                {
                    IPHostEntry ip = Dns.Resolve(ServerIP);
                    ipe = new IPEndPoint(ip.AddressList[0], Port);
                }

                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(ipe);
            }
            catch
            {
                if (onConnet != null)
                {
                    this.Invoke(new InvokeDelegate(() =>
                    {
                        onConnet(false);
                    }));
                }
            }
            finally
            {
                if (socket.Connected)
                {
                    if (onConnet != null)
                    {
                        this.Invoke(new InvokeDelegate(() =>
                        {
                            onConnet(true);
                        }));
                    }
                    Bufflist = new BuffList(BufferCount);
                    Thread ondataThread = new Thread(new ThreadStart(rece));
                    ondataThread.IsBackground = true;
                    ondataThread.Start();
                }
            }
            Thread outThread = new Thread(new ThreadStart(outExcution));
            outThread.IsBackground = true;
            outThread.Start();
        }

        bool checkIPAddress(string ipAdd)
        {
            try
            {
                var ip = IPAddress.Parse(ipAdd);
                return true;
            }
            catch
            {
                return false;
            }
        }


        byte[] dt;
        void rece()
        {
            byte[] RecvBuffer = new byte[1024];
            int nBytes = 0;

            try
            {
                while (true)
                {
                    nBytes = socket.Receive(RecvBuffer, 0, 1024, SocketFlags.None);
                    if (nBytes > 0)
                    {
                        byte[] rcdata = new byte[nBytes];
                        Buffer.BlockCopy(RecvBuffer, 0, rcdata, 0, rcdata.Length);
                        Bufflist.InsertByteArray(rcdata);

                        byte[] buff = Bufflist.GetData();
                        if (buff != null)
                        {
                            dt = new byte[buff.Length - 4];
                            Buffer.BlockCopy(buff, 4, dt, 0, dt.Length);
                            if (onData != null)
                            {
                                this.Invoke(new InvokeDelegate(() =>
                                {
                                    onData(dt);
                                }));
                            }
                            dt = null;
                            Array.Clear(RecvBuffer, 0, RecvBuffer.Length);
                            nBytes = 0;
                        }
                    }
                    System.Threading.Thread.Sleep(10);
                }
            }
            catch
            {
                Array.Clear(RecvBuffer, 0, RecvBuffer.Length);
                nBytes = 0;
            }
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <remarks>发送一条BYTE[]消息到服务端</remarks>
        /// <param name="buff">消息体</param>
        public void Send(byte[] buff)
        {
            if (socket.Connected)
            {
               ThreadPool.QueueUserWorkItem(addOut, buff);
            }
            else
            {
                handleExpression();
            }
        }

        //分发器
        void outExcution()
        {
            while (true)
            {
                if (outQueue.Count > 0)
                {
                    send(popOut());
                }
                System.Threading.Thread.Sleep(300);
            }
        }

        void send(object data)
        {
            if (data != null)
            {
                var buff = (byte[])data;
                buff = DYWriter.Merge(DYWriter.GetDYBytes(buff.Length + 4), buff);
                try
                {
                    socket.Send(buff);
                }
                catch
                {
                    handleExpression();
                }
            }
        }

        /// <summary>
        /// 发送smart指令
        /// </summary>
        /// <remarks>发送一条消息到DYsmart设备</remarks>
        /// <param name="order">消息体</param>
        public string SendToSmart(byte[] order)
        {
            if (order != null)
            {
                byte[] data = DYWriter.Merge(order, Encoding.UTF8.GetBytes(new char[] { '\r', '\n' }));
                data = DYWriter.Merge(DYWriter.GetDYBytes(data.Length + 4), data);
                try
                {
                    socket.Send(data);
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
            return "ok";
        }

        void handleExpression()
        {
            if (onDisconnet != null)
            {
                this.Invoke(new InvokeDelegate(() =>
                {
                    onDisconnet("连接已断开");
                }));
            }
        }

        #region queue
        /// <summary>
        /// 发送消息队列
        /// </summary>
        public Queue<byte[]> outQueue = new Queue<byte[]>(10000);

        void addOut(object data)
        {
            lock (outQueue)
            {
                outQueue.Enqueue((byte[])data);
            }
        }

        byte[] popOut()
        {
            lock (outQueue)
            {
                return outQueue.Dequeue();
            }
        }
        #endregion

        #region IDisposable Members
        /// <summary>
        /// 释放此客户端代理
        /// </summary>
        /// <remarks>不须刻意调用,系统自动处理</remarks>
        void IDisposable.Dispose()
        {
            if (socket.Connected)
            {
                socket.Shutdown(SocketShutdown.Both);
            }
            socket.Close();
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
