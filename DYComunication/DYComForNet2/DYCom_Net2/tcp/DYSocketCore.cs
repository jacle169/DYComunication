using System;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace DYCom_Net2
{
    /// <summary>
    /// DYcom核心组件
    /// </summary>
    public class DYSocketCore  
    {
        #region 参数
        /// <summary>
        /// 数据包管理
        /// </summary>
        private BufferManager BuffManagers;

        /// <summary>
        /// Socket异步对象池
        /// </summary>
        private SocketAsyncEventArgsPool SocketAsynPool;

        /// <summary>
        /// SOCK对象
        /// </summary>
        private Socket dySock;

        /// <summary>
        /// DYSocket对象
        /// </summary>
        /// <remarks>DYSocket对象,只允许提取</remarks>
        public Socket DYSock { get { return dySock; } }


        /// <summary>
        /// 核心服务连接事件处理
        /// </summary>
        /// <remarks>当客户端连接成功后马上触发本事件.</remarks>
        /// <value>已注册事件实例</value>
        public ConnectHandler ConnectOnCore { get; set; }

        /// <summary>
        /// 核心服务消息到达事件处理
        /// </summary>
        /// <remarks>当客户端消息到达后马上触发本事件.</remarks>
        /// <value>消息事件实例</value>
        public OndataHandler DataOnCore { get; set; }

        /// <summary>
        /// 核心服务异常或用户断开事件处理
        /// </summary>
        /// <remarks>当客户端异常或用户断开后马上触发本事件.</remarks>
        /// <value>异常事件实例</value>
        public ErrorHandler ErrorOnCore { get; set; }

        /// <summary>
        /// Delay算法
        /// </summary>
        /// <remarks>取得或设置Delay状态</remarks>
        /// <value>True为开启,False为禁用</value>
        public bool NoDelay
        {
            get
            {
                return dySock.NoDelay;
            }

            set
            {
                dySock.NoDelay = value;
            }

        }

        /// <summary>
        /// 接收消息超时属性
        /// </summary>
        /// <remarks>当接收某客户端连接传来数据时出现IO读取异常超过此属性值时,将触发异常通过到DYCom异常事件.</remarks>
        /// <value>超时时间[毫秒]</value>
        public int ReceiveTimeout
        {
            get
            {
                return dySock.ReceiveTimeout;
            }

            set
            {
                dySock.ReceiveTimeout = value;

            }


        }

        /// <summary>
        /// 发送消息超时属性
        /// </summary>
        /// <remarks>当向某客户端发送数据时出现IO读取异常超过此属性值时,将触发异常通过到DYCom异常事件.</remarks>
        /// <value>超时时间[毫秒]</value>
        public int SendTimeout
        {
            get
            {
                return dySock.SendTimeout;
            }

            set
            {
                dySock.SendTimeout = value;
            }

        }


        /// <summary>
        /// 接收包大小
        /// </summary>
        private int MaxBufferSize;

        /// <summary>
        /// 取得核心服务接收缓冲区大小
        /// </summary>
        /// <remarks>取得核心服务接收缓冲区大小</remarks>
        /// <value>缓冲区大小[字节]</value>
        public int GetMaxBufferSize
        {
            get
            {
                return MaxBufferSize;
            }
        }

        /// <summary>
        /// 最大用户连接
        /// </summary>
        private int MaxConnectCout;

        /// <summary>
        /// 取得最大用户连接数
        /// </summary>
        /// <remarks>取得最大用户连接数</remarks>
        /// <value>最大用户连接数[Int]</value>
        public int GetMaxUserConnect
        {
            get
            {
                return MaxConnectCout;
            }
            set { MaxConnectCout = value; }
        }


        /// <summary>
        /// 服务使用的端口
        /// </summary>
        private int Port;       

#endregion

        private AutoResetEvent[] reset;

        /// <summary>
        /// 启动服务
        /// </summary>
        /// <remarks>启动核心服务</remarks>
        public void Start()
        {
            reset[0].Set();
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        /// <remarks>停止核心服务</remarks>
        public void Stop()
        {
            reset[0].Reset();
        }

        LicensingClient lc;

        /// <summary>
        /// DYcom核心服务
        /// </summary>
        /// <remarks>他提供了最基础的通信方式,而且支持不用你自己协议进行同步通信,如果异步通信请使用DYComer里的通信接口,它同时支持异步通信.</remarks>
        /// <param name="port">端口</param>
        /// <param name="maxconnectcout">最大接入客户端数</param>
        /// <param name="maxbuffersize">每个客户端接收缓冲区大小</param>
        /// <param name="PDkey">DYCOM产品密钥</param>
        public DYSocketCore(int port, int maxconnectcout, int maxbuffersize, string PDkey)
        {
            key = PDkey;
            Port = port;
            MaxBufferSize = maxbuffersize;
            MaxConnectCout = maxconnectcout;
            this.reset = new System.Threading.AutoResetEvent[1];
            reset[0] = new System.Threading.AutoResetEvent(false);
            Run();
        }
        string authenServer = "http://dylc.dy2com.com/dylc/Default.aspx";
        //string authenServer = "http://localhost:11711/Default.aspx";
        string key = "4dae70be";
        string crykey = "Jac27$%^";
        /// <summary>
        /// 启动
        /// </summary>
        private void Run()
        {
            if (licens.other.getOther().checkKey(key))
            {
                //lc = new LicensingClient();
                //lc.ui = new LicensingClient.resultDelegate(onLicensing);
                //lc.SendLicensing(authenServer, key, crykey, 36000, 36000, 72000, 5);
                //lc.SendLicensing(authenServer, key, crykey, 2, 5, 10, 0);
                dySock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                dySock.Bind(new IPEndPoint(IPAddress.Any, Port));
                dySock.Listen(-1);
                SendTimeout = 2000;
                ReceiveTimeout = 2000;
                ThreadPool.SetMaxThreads(MaxConnectCout, MaxConnectCout);
                BuffManagers = new BufferManager(MaxConnectCout * MaxBufferSize, MaxBufferSize);
                BuffManagers.Inint();
                SocketAsynPool = new SocketAsyncEventArgsPool(MaxConnectCout);
                for (int i = 0; i < MaxConnectCout; i++)
                {
                    SocketAsyncEventArgs socketasyn = new SocketAsyncEventArgs();
                    socketasyn.Completed += Asyn_Completed;
                    SocketAsynPool.Push(socketasyn);
                }
                Accept();
            }
            else
            {
                Console.WriteLine("DYCOM提示：不是合法的授权码！请与DYCOM官方或第三方开发组织联系！");
            }
        }

        //void onLicensing(bool result, string message)
        //{
        //    if (!result)
        //    {
        //        Stop();
        //        dySock.Close();
        //        System.Diagnostics.Process.GetCurrentProcess().Kill();
        //    }
        //    //Console.WriteLine(message);
        //}

        void Accept()
        {
            if (SocketAsynPool.Count > 0)
            {
                SocketAsyncEventArgs sockasyn = SocketAsynPool.Pop();
                if (!DYSock.AcceptAsync(sockasyn))
                {
                    BeginAccep(sockasyn);
                }
            }
        }

        void BeginAccep(SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                WaitHandle.WaitAll(reset);
                reset[0].Set();
                if (ConnectOnCore != null) 
                { 
                    ConnectOnCore(new DYSocket() { SocketArgs = e }, true); 
                }
                if (BuffManagers.SetBuffer(e))
                {
                    if (!e.AcceptSocket.ReceiveAsync(e))
                    {
                        BeginReceive(e);
                    }
                }
            }
            else
            {
                e.AcceptSocket = null;
                SocketAsynPool.Push(e);
                if (ConnectOnCore != null)
                {
                    ConnectOnCore(new DYSocket() { SocketArgs = e }, false);
                }
            }
            Accept();
        }

        void BeginReceive(SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success && e.BytesTransferred > 0)
            {
                byte[] data = new byte[e.BytesTransferred];
                Buffer.BlockCopy(e.Buffer, e.Offset, data, 0, data.Length);
                Array.Clear(e.Buffer, e.Offset, e.BytesTransferred);
                if (DataOnCore != null)
                {
                    DataOnCore.BeginInvoke(data, new DYSocket() { SocketArgs = e }, RecevieCallBack, DataOnCore);
                }
                if (!e.AcceptSocket.ReceiveAsync(e))
                {
                    BeginReceive(e);
                }
            }
            else
            {
                if (ErrorOnCore != null)
                {
                    ErrorOnCore("User Disconnect", new DYSocket() { SocketArgs = e });
                }
                e.AcceptSocket = null;
                BuffManagers.FreeBuffer(e);
                SocketAsynPool.Push(e);
                if (SocketAsynPool.Count == 1)
                {
                    Accept();
                }
            }
        }

        void RecevieCallBack(IAsyncResult result)
        {
            DataOnCore.EndInvoke(result);
        }

        void Asyn_Completed(object sender, SocketAsyncEventArgs e)
        {
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Accept:
                    BeginAccep(e);
                    break;
                case SocketAsyncOperation.Receive:
                    BeginReceive(e);
                    break;
            }
        }

        /// <summary>
        /// 发送异步数据
        /// </summary>
        /// <param name="sk">socket对象</param>
        /// <param name="Data">数据</param>
        internal void SendData(SocketAsyncEventArgs sk, byte[] Data)
        {
            DYmessage m = new DYmessage();
            m.data = Data;
            m.user = sk.UserToken as UserInfo;
            ThreadPool.QueueUserWorkItem(m.user.addOut, m);
        }

        /// <summary>
        /// 以同步方式发送数据
        /// </summary>
        /// <remarks>此方此为同步发送数据使用</remarks>
        /// <param name="sk">DYSocket对象</param>
        /// <param name="Data">要发送的数据</param>
        public void SendSync(DYSocket sk, byte[] Data)
        {
            sk.SocketArgs.AcceptSocket.Send(Data);
        }

        /// <summary>
        /// 关闭此客户端
        /// </summary>
        /// <remarks>关闭此客户端,但并未释放,可以复用.</remarks>
        /// <param name="socks">socket对象</param>
        public void Disconnect(DYSocket socks)
        {
            try
            {
                if (socks.SocketArgs.AcceptSocket != null && socks.SocketArgs.AcceptSocket.Connected)
                {
                    socks.SocketArgs.AcceptSocket.BeginDisconnect(false, AsynCallBackDisconnect, socks.SocketArgs.AcceptSocket);
                }
            }
            catch
            {
                socks.SocketArgs.AcceptSocket.Shutdown(SocketShutdown.Both);
            }
        }

        void AsynCallBackDisconnect(IAsyncResult result)
        {
            Socket resock = result.AsyncState as Socket;
            if (resock != null)
            {
                try{resock.EndDisconnect(result);}catch { }
            }
        }

    }
}
