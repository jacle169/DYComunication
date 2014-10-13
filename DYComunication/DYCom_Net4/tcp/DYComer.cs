 using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Collections;

namespace DYCom_Net4
{
    /// <summary>
    /// DYcom服务组件
    /// </summary>
    public class DYComer
    {
        /// <summary>
        /// DYcom的产品版本号
        /// </summary>
        public string Version()
        {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        /// <summary>
        /// DYcom运行要求的.NETFramework版本
        /// </summary>
        public string TargetFramework()
        {
            return Assembly.GetExecutingAssembly().ImageRuntimeVersion;
        }

        /// <summary>
        /// 连接通知属性
        /// </summary>
        public OnConnet onConnet { get; set; }
        /// <summary>
        /// 消息通知属性
        /// </summary>
        public OnData onData { get; set; }
        /// <summary>
        /// 错误通知属性
        /// </summary>
        public OnDisConnet onDisconnet { get; set; }

        DYSocketCore server;
        static byte[] dt;
       
        /// <summary>
        /// 启动服务
        /// </summary>
        /// <remarks>启动DYCom服务</remarks>
        /// <param name="port">服务端口</param>
        /// <param name="maxConnets">允许最大接入客户端数</param>
        /// <param name="BufferSize">每个客户端接收缓冲区大小</param>
        /// <param name="key">DYCOM产品密钥</param>
        public void Start(int port, int maxConnets, int BufferSize)
        {
            server = new DYSocketCore(port, maxConnets, BufferSize);
            server.DataOnCore = new OndataHandler(DataIn);
            server.ConnectOnCore = new ConnectHandler(Connection);
            server.ErrorOnCore = new ErrorHandler(ErrorIn);
            server.Start();
        }

        /// <summary>
        ///启动服务
        /// </summary>
        /// <remarks>在服务运行过程中执行了Stop操作时可使用本方法重新恢复服务进入运行状态</remarks>
        public void Start()
        {
            server.Start();
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        public void Stop()
        {
            server.Stop();
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        /// <remarks>断开并释放一个接入实列</remarks>
        public void DisConnet(DYSocket e)
        {
            if (onDisconnet != null)
            {
                onDisconnet(e);
            }
        }

        void Connection(DYSocket e, bool IsConneted)
        {
            if (onConnet != null)
            {
                onConnet(e,IsConneted);
            }
        }

        void DataIn(byte[] data, DYSocket e)
        {
            if (e.SocketArgs.UserToken != null)
            {
                UserInfo info = e.SocketArgs.UserToken as UserInfo;
                if (info != null)
                {
                    info.Bufferlist.InsertByteArray(data);
                    do
                    {
                        byte[] buff = info.Bufferlist.GetData();
                        if (buff != null)
                        {
                            dt = new byte[buff.Length - 4];
                            Buffer.BlockCopy(buff, 4, dt, 0, dt.Length);
                            info.LastMessageTime = DateTime.Now;
                            ThreadPool.QueueUserWorkItem(ondata, new DYmessage() { user = info, data = dt });
                        }
                        else
                            break;
                    } while (true);
                }
            }
        }

        void ondata(object data)
        {
            var m = (DYmessage)data;
            onData(m.data, m.user);
        }

        void ErrorIn(string message, DYSocket e)
        {
            if (onDisconnet != null)
            {
                onDisconnet(e);
            }
        }

        byte[] addLeng(byte[] buff)
        {
            return DYWriter.Merge(DYWriter.GetDYBytes(buff.Length + 4), buff);
        }
        private string typeName = MethodBase.GetCurrentMethod().DeclaringType.Namespace + ".UserInfo";
        /// <summary>
        /// 发送消息给指定客户端
        /// </summary>
        /// <remarks>客户端必须继承于UserInfo</remarks>
        /// <param name="info">客户端实例</param>
        /// <param name="data">消息体</param>
        public bool SendToSingle<T>(T info, byte[] data) 
        {
            if (typeof(T).BaseType.FullName.Equals(typeName) || typeof(T).FullName.Equals(typeName)) 
            {
                ThreadPool.QueueUserWorkItem((o) =>
                    {
                        try
                        {
                            object obj = info;
                            Send(data, ((UserInfo)obj).Sock.SocketArgs);
                        }
                        catch { }
                    });
                return true;
            }
            return false;
        }

        /// <summary>
        /// 发送消息到客户端集合
        /// </summary>
        /// <remarks>客户端必须继承于UserInfo</remarks>
        /// <param name="Clients">客户端集合</param>
        /// <param name="Data">消息体</param>
        public bool SendToAll<T>(List<T> Clients, byte[] Data)
        {
            if (typeof(T).BaseType.FullName.Equals(typeName) || typeof(T).FullName.Equals(typeName)) 
            {
                List<T> sendList = null;
                lock (((ICollection)Clients).SyncRoot)
                {
                    sendList = new List<T>(Clients);
                }
                if (sendList != null && sendList.Count > 0)
                {
                    sendList.ForEach(i => ThreadPool.QueueUserWorkItem((o) =>
                    {
                        try {
                            Send(Data, ((UserInfo)o).Sock.SocketArgs);
                        }
                        catch { }
                    }, i));
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// 从集合中发送消息，不发送到指定一个客户端
        /// </summary>
        /// <remarks>客户端必须继承于UserInfo</remarks>
        /// <param name="Clients">准备发送客户端集合</param>
        /// <param name="info">被排除客户端实例</param>
        /// <param name="Data">消息体</param>
        public bool SendWithOut<T>(List<T> Clients, UserInfo info, byte[] Data)
        {
            if (typeof(T).BaseType.FullName.Equals(typeName) || typeof(T).FullName.Equals(typeName)) 
            {
                List<T> sendList = null;
                lock (((ICollection)Clients).SyncRoot)
                {
                    sendList = new List<T>(Clients);
                }
                if (sendList != null && sendList.Count > 0)
                {
                    sendList.ForEach(i => ThreadPool.QueueUserWorkItem((o) =>
                    {
                        try
                        {
                            if (((UserInfo)o).Sock.SocketArgs.AcceptSocket.RemoteEndPoint != info.Sock.SocketArgs.AcceptSocket.RemoteEndPoint)
                            {
                                Send(Data, ((UserInfo)o).Sock.SocketArgs);
                            }
                        }
                        catch { }
                    }, i));
                }
                return true;
            }
            return false;
        }

        void Send(byte[] buff, SocketAsyncEventArgs e)
        {
            server.SendData(e, addLeng(buff));
        }
    }

    /// <summary>
    /// dycom内部消息结构
    /// </summary>
    internal class DYmessage
    {
        /// <summary>
        /// 接入端实例
        /// </summary>
        /// <remarks>保存了一个相应接入端实例</remarks>
        public UserInfo user { get; set; }
        /// <summary>
        /// 消息体
        /// </summary>
        /// <remarks>包含了一条完整的消息内容</remarks>
        public byte[] data { get; set; }
    }
}
