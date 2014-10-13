using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Threading;
using System.Net.Sockets;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;

namespace DYComWPClient
{
    /// <summary> 
    /// DYComSilverlight客户端
    /// </summary> 
    public class ClientPoxy : IDisposable
    {
        /// <summary> 
        /// DYComSilverlight连接代理属性
        /// </summary> 
        public OnConnet onConnet { get; set; }
        /// <summary> 
        /// DYComSilverlight数据代理属性
        /// </summary> 
        public OnData onData { get; set; }
        /// <summary> 
        /// DYComSilverlight断开连接代理属性
        /// </summary> 
        public OnDisConnet onDisconnet { get; set; }

        private AsyncOperation asyncOp;

        byte[] ReceiveBuffer { get; set; }
        Socket socket { get; set; }
        SocketAsyncEventArgs ReceiveEventArgs { get; set; }
        byte[] SendBuffer { get; set; }
        SocketAsyncEventArgs SendEventArgs { get; set; }
        BuffList Bufflist;
        static byte[] dt;

        /// <summary>
        /// DYcom客户端代理
        /// </summary>
        /// <param name="ServerIP">服务器IP地址</param>
        /// <param name="Port">连接端口</param>
        /// <param name="BufferCount">缓冲区大小</param>
        public ClientPoxy(string ServerIP, int Port, int BufferCount)
        {
            asyncOp = AsyncOperationManager.CreateOperation(null);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Bufflist = new BuffList(BufferCount);
            ReceiveEventArgs = new SocketAsyncEventArgs();
            ReceiveBuffer = new byte[BufferCount];
            ReceiveEventArgs.Completed += Connect_Completed;
            ReceiveEventArgs.RemoteEndPoint = new DnsEndPoint(ServerIP, Port);
            socket.ConnectAsync(ReceiveEventArgs);
            Thread outThread = new Thread(new ThreadStart(outExcution));
            outThread.IsBackground = true;
            outThread.Start();
        }

        void Connect_Completed(object send, SocketAsyncEventArgs e)
        {
            ReceiveEventArgs.Completed -= Connect_Completed;
            if (e.SocketError == SocketError.Success)
            {
                ReceiveEventArgs.SetBuffer(ReceiveBuffer, 0, ReceiveBuffer.Length);

                SendEventArgs = new SocketAsyncEventArgs();
                SendBuffer = new byte[ReceiveBuffer.Length];
                SendEventArgs.SetBuffer(SendBuffer, 0, SendBuffer.Length);
                SendEventArgs.RemoteEndPoint = e.RemoteEndPoint;
                SendEventArgs.Completed += SendEventArgs_Completed;

                ReceiveEventArgs.Completed += Receive_Completed;
                socket.ReceiveAsync(ReceiveEventArgs);
                if (onConnet != null)
                {
                    asyncOp.Post(result =>
                    {
                        onConnet(true);
                    }, null);
                }
            }
            else
            {
                if (onConnet != null)
                {
                    asyncOp.Post(result =>
                    {
                        onConnet(false);
                    }, null);
                }
                handleExpression();
            }
        }

        bool isSending = false;
        void SendEventArgs_Completed(object sender, SocketAsyncEventArgs e)
        {
            isSending = false;
        }

        void Receive_Completed(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success && e.BytesTransferred > 0)
            {
                byte[] data = new byte[e.BytesTransferred];
                Buffer.BlockCopy(e.Buffer, 0, data, 0, data.Length);
                Array.Clear(e.Buffer, e.Offset, e.BytesTransferred);
                Bufflist.InsertByteArray(data);
                do
                {
                    byte[] buff = Bufflist.GetData();
                    if (buff != null)
                    {
                        dt = new byte[buff.Length - 4];
                        Buffer.BlockCopy(buff, 4, dt, 0, dt.Length);
                        ThreadPool.QueueUserWorkItem(ondata, dt);
                    }
                    else
                        break;
                } while (true);
                socket.ReceiveAsync(ReceiveEventArgs);
            }
        }

        void ondata(object data)
        {
            if (onData != null)
            {
                asyncOp.Post(result =>
                {
                    onData((byte[])data);
                }, null);
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
                if (outQueue.Count > 0 && isSending == false)
                {
                    send(popOut());
                }
                System.Threading.Thread.Sleep(20);
            }
        }

        void send(object data)
        {
            if (data != null)
            {
                isSending = true;
                var buff = (byte[])data;
                buff = DYWriter.Merge(DYWriter.GetDYBytes(buff.Length + 4), buff);
                Buffer.BlockCopy(buff, 0, SendBuffer, 0, buff.Length);
                SendEventArgs.SetBuffer(0, buff.Length);
                if (!socket.SendAsync(SendEventArgs))
                {
                    Array.Clear(SendBuffer, 0, SendBuffer.Length);
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
                Buffer.BlockCopy(data, 0, SendBuffer, 0, data.Length);
                SendEventArgs.SetBuffer(0, data.Length);
                try
                {
                    if (!socket.SendAsync(SendEventArgs))
                    {
                        Array.Clear(SendBuffer, 0, SendBuffer.Length);
                    }
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
                asyncOp.Post(result =>
                {
                    onDisconnet("连接已断开");
                    Dispose();
                }, null);
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

        /// <summary>
        /// 释放此客户端代理
        /// </summary>
        /// <remarks>不须刻意调用,系统自动处理</remarks>
        public void Dispose()
        {
            if (socket.Connected)
            {
                socket.Shutdown(SocketShutdown.Both);
            }
            socket.Close(); socket.Dispose();
            ReceiveEventArgs.Dispose();
            try
            {
                SendEventArgs.Dispose();
            }
            catch { }
            GC.SuppressFinalize(this);
        }
    }
}
