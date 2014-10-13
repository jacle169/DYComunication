using System;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace DYCom_Net4
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
        /// DYSocket对象,只允许提取
        /// </summary>
        public Socket DYSock { get { return dySock; } }


        /// <summary>
        /// 连接完成处理
        /// </summary>
        public ConnectHandler ConnectOnCore { get; set; }

        /// <summary>
        /// 数据输入处理
        /// </summary>
        public OndataHandler DataOnCore { get; set; }

        /// <summary>
        /// 异常或用户断开处理
        /// </summary>
        public ErrorHandler ErrorOnCore { get; set; }

        /// <summary>
        /// 是否开启SOCKET Delay算法
        /// </summary>
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
        /// SOCKET 的  接收消息超时属性
        /// </summary>
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
        /// SOCKET 的 发送超时属性
        /// </summary>
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
        /// 取得缓冲区大小
        /// </summary>
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
        /// 最大用户连接数
        /// </summary>
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
        public void Start()
        {
            reset[0].Set();
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        public void Stop()
        {
            reset[0].Reset();
        }

        /// <summary>
        /// DYcom核心服务
        /// </summary>
        /// <param name="port">端口</param>
        /// <param name="maxconnectcout">最大接入客户端数</param>
        /// <param name="maxbuffersize">每个客户端接收缓冲区大小</param>
        /// <param name="PDkey">DYCOM产品密钥</param> 
        public DYSocketCore(int port, int maxconnectcout, int maxbuffersize)
        {
            Port = port;
            MaxBufferSize = maxbuffersize;
            MaxConnectCout = maxconnectcout;
            this.reset = new System.Threading.AutoResetEvent[1];
            reset[0] = new System.Threading.AutoResetEvent(false);
            Run();
        }

        /// <summary>
        /// 启动
        /// </summary>
        private void Run()
        {
            dySock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            dySock.Bind(new IPEndPoint(IPAddress.Any, Port));
            dySock.Listen(-1);
            SendTimeout = 2000;
            ReceiveTimeout = 2000;
            ThreadPool.SetMaxThreads(MaxConnectCout, MaxConnectCout);
            BuffManagers = new BufferManager(MaxConnectCout * MaxBufferSize, MaxBufferSize);
            BuffManagers.Inint();
            SocketAsynPool = new SocketAsyncEventArgsPool();
            for (int i = 0; i < MaxConnectCout; i++)
            {
                SocketAsyncEventArgs socketasyn = new SocketAsyncEventArgs();
                socketasyn.Completed += Asyn_Completed;
                SocketAsynPool.Push(socketasyn);
            }
            Accept();
        }

        void Accept()
        {
            SocketAsyncEventArgs sockasyn;
            if (SocketAsynPool.pool.TryPop(out sockasyn))
            {
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
                if (SocketAsynPool.pool.Count == 1)
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
        /// <param name="sk">DYSocket对象</param>
        /// <param name="Data">数据</param>
        public void SendSync(DYSocket sk, byte[] Data)
        {
            ThreadPool.QueueUserWorkItem((o) =>
            {
                try
                {
                    sk.SocketArgs.AcceptSocket.Send(Data);
                }
                catch { }
            });
        }

        /// <summary>
        /// 断开此SOCKET,但并未释放
        /// </summary>
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
