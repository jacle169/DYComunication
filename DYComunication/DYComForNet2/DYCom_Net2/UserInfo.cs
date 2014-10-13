using System;
using System.Net.Sockets;
using System.Threading;

namespace DYCom_Net2
{
    /// <summary>
    /// 客户端Base实例
    /// </summary>
    public class UserInfo : Queuer 
    {
        /// <summary>
        /// 实例唯一ID
        /// </summary>
        /// <remarks>此值是每个客户端独有的唯一标识，在客户端接入时系统自动标识</remarks>
        public string SessionId { get; set; }
        internal BuffList Bufferlist { get; set; }
        DYSocket sock;
        internal byte[] SendBuffer { get; private set; }
        internal SocketAsyncEventArgs SendEventArgs { get; private set; }
        bool isSending = false;
        internal DateTime LastMessageTime { get; set; }
        /// <summary>
        /// 错误提示代理事件
        /// </summary>
        /// <remarks>此代理所注册的事件会捕捉到客户端通信过程中可能出现的异常</remarks>
        public SendErrorHandler SendHandle { get; set; }

        /// <summary>
        /// 用户静止时间到达通知
        /// </summar>
        /// <remarks>返回Bool值，true为断开此客户端，flash为继续让此客户端连接。</remarks>
        public OnUserStaticTimeOut StaticHandle { get; set; }

        /// <summary>
        /// 接入客户端构造函数
        /// </summary>
        /// <remarks>客户端用户自定义类必须继承于此类,否则将不能进行发送操作</remarks>
        public UserInfo()
        {
            Bufferlist = new BuffList(256);
            SendBuffer = new byte[256];
            SessionId = Guid.NewGuid().ToString("N");
            LastMessageTime = DateTime.Now;

            Thread outThread = new Thread(new ThreadStart(outExcution));
            outThread.IsBackground = true;
            outThread.Start();
        }

        /// <summary>
        /// 设置用户发送缓冲区
        /// </summary>
        /// <remarks>此值必须是你将要发送的单个消息之最大可能值。如果发送消息大小大于此值将会触发SendErrorHandler事件.</remarks>
        /// <param name="Size">缓冲区大小.参数类型:int,表述单位为字节</param>
        public void SetSendBuffer(int Size)
        {
            Bufferlist = new BuffList(Size);
            SendBuffer = new byte[Size];
        }

        /// <summary>
        /// 接入实例Socket实例
        /// </summary>
        /// <remarks>处理消息Socket实例,发送消息通道和接收消息通道均派生于现实例.</remarks>
        public DYSocket Sock
        {
            get { return sock; }
            set { sock = value;
            SendEventArgs = new SocketAsyncEventArgs();
            SendEventArgs.RemoteEndPoint = sock.SocketArgs.AcceptSocket.RemoteEndPoint;
            SendEventArgs.SetBuffer(SendBuffer, 0, SendBuffer.Length);
            }
        }

        int StaticTimeOut = 0;
        Thread stThread;
        /// <summary>
        /// 设置用户静止超时时间
        /// </summary>
        /// <remarks>以用户最后一次到来消息时间开始计算，当用户在指定时间值内没有任何消息到来将会自动关闭此客户端的连接</remarks>
        /// <param name="server">当前DYComer实例，作用是提供断开客户端时操作.</param>
        /// <param name="TimeOutSeconds">设置允许静止最大时间值，超过此值时服务端将主动断开客户端，单位(秒).</param>
        public void SetupTimeoutCheck(DYComer server, int TimeOutSeconds) 
        {
            StaticTimeOut = TimeOutSeconds; 
            if (stThread == null)
            {
                stThread = new Thread(StaticTimeChecker);
                stThread.IsBackground = true;
                stThread.Start(server);
            }
        }

        void StaticTimeChecker(object obj)
        {
            while (true)
            {
                if (DateTime.Now.Subtract(LastMessageTime).TotalSeconds >= StaticTimeOut)
                {
                    try
                    {
                        if (StaticHandle == null)
                        {
                            ((DYComer)obj).DisConnet(sock);
                        }
                        else
                        {
                            if (StaticHandle(SessionId))
                            {
                                ((DYComer)obj).DisConnet(sock);
                            }
                        }
                    }
                    catch { }
                    break;
                }
                Thread.Sleep(1000);
            }
            if (Thread.CurrentThread.IsAlive)
            {
                Thread.CurrentThread.Abort();
            }
        }

        void outExcution()
        {
            while (true)
            {
                if (outQueue.Count > 0 && isSending == false)
                {
                    var m = outQueue.Dequeue();
                    send(m);
                }
                System.Threading.Thread.Sleep(10);
            }
        }

        void send(DYmessage m)
        {
            if (m != null && m.user.sock.SocketArgs.AcceptSocket != null)
            {
                if (m.data.Length <= SendBuffer.Length)
                {
                    isSending = true;
                    Buffer.BlockCopy(m.data, 0, m.user.SendBuffer, 0, m.data.Length);
                    m.user.SendEventArgs.SetBuffer(0, m.data.Length);
                    m.user.SendEventArgs.Completed += SendEventArgs_Completed;
                    if (!m.user.Sock.SocketArgs.AcceptSocket.SendAsync(m.user.SendEventArgs))
                    {
                        Array.Clear(m.user.SendBuffer, 0, m.user.SendBuffer.Length);
                    }
                }
                else
                {
                    if (SendHandle != null)
                    {
                        SendHandle("错误970,发送缓冲区少于消息大小，请在实例化userInfo时创建更大的缓冲区");
                    }
                }
            }
        }

        void SendEventArgs_Completed(object sender, SocketAsyncEventArgs e)
        {
            e.Completed -= SendEventArgs_Completed;
            isSending = false;
        }

        /// <summary>
        /// 关闭本客户端
        /// </summary>
        /// <remarks>切底断开并释放此客户端，此方法客户端将会捕捉到断开状态</remarks>
        public void SocketClose()
        {
            try
            {
                if (sock != null && sock.SocketArgs.AcceptSocket != null)
                {
                    sock.SocketArgs.AcceptSocket.Close();
                }
            }
            catch { }
        }

    }
}
