 using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using System;
using System.Reflection;

namespace DYCom_Net2
{
    /// <summary>
    /// DYCom服务组件
    /// </summary>
    public class DYComer
    {
        /// <summary>
        /// DYCom组件构造函数
        /// </summary>
        /// <remarks>实例化DYCom服务组件</remarks>
        public DYComer()
        { }

        /// <summary>
        /// DYcom的产品版本号
        /// </summary>
        /// <returns>DYcom的产品版本号</returns>
        public string Version()
        {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        /// <summary>
        /// 取得此DYCOM运行依赖的NETFrameWork版本
        /// </summary>
        /// <returns>DYcom运行要求的.NETFramework版本</returns>
        public string TargetFramework()
        {
            return Assembly.GetExecutingAssembly().ImageRuntimeVersion;
        }

        /// <summary>
        /// DYcom机器码，用于提供给发行商注册授权
        /// </summary>
        public string MachineID()
        {
            return licens.other.getOther().DiskId();
        }

        /// <summary>
        /// 软件授权是否已经验证通过
        /// </summary>
        public bool IsCertificateOK { get; set; }

        /// <summary>
        /// 当客户端连接到服务时的通知事件
        /// </summary>
        /// <value>连接事件实例</value>
        public OnConnet onConnet { get; set; }
        /// <summary>
        /// 当客户端消息到服务端时的通知事件
        /// </summary>
        /// <value>消息通知事件实例</value>
        public OnData onData { get; set; }
        /// <summary>
        /// 在运行过程中出现异常会通知此事件通知
        /// </summary>
        /// <value>异常通知实例</value>
        public OnDisConnet onDisconnet { get; set; }

        DYSocketCore server;
        static byte[] dt;

        string authenServer = "http://dylc.dy2com.com/dylc/Default.aspx";
        //string authenServer = "http://localhost:11711/Default.aspx";
        string crykey = "Jac27$%^";

        /// <summary>
        /// 启动服务
        /// </summary>
        /// <remarks>启动DYCom服务</remarks>
        /// <param name="port">服务端提供服务的端口号</param>
        /// <param name="maxConnets">允许最大接入客户端数量</param>
        /// <param name="BufferSize">每个客户端的接收缓冲区大小</param>
        /// <param name="key">DYCOM产品密钥</param>
        public void Start(int port, int maxConnets, int BufferSize, string key)
        {
            if (licens.other.getOther().checkKey(key))
            {
                IsCertificateOK = true;
                //lc = new LicensingClient();
                //lc.ui = new LicensingClient.resultDelegate(onLicensing);
                //lc.SendLicensing(authenServer, key, crykey, 36000, 36000, 72000, 5);
                //lc.SendLicensing(authenServer, key, crykey, 2, 5, 10, 0);
                server = new DYSocketCore(port, maxConnets, BufferSize, key);
                server.DataOnCore = new OndataHandler(DataIn);
                server.ConnectOnCore = new ConnectHandler(Connection);
                server.ErrorOnCore = new ErrorHandler(ErrorIn);
                server.Start();
            }
            else
            {
                IsCertificateOK = false;
                Console.WriteLine("DYCOM提示：不是合法的授权码！请与DYCOM官方或第三方开发组织联系！");
            }
        }

        //void onLicensing(bool result, string message)
        //{
        //    if (!result)
        //    {
        //        Stop();
        //        System.Diagnostics.Process.GetCurrentProcess().Kill();
        //    }
        //    //Console.WriteLine(message);
        //}

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
        /// 断开连接事件通信属性
        /// </summary>
        /// <remarks>断开并释放指定的客户端</remarks>
        /// <param name="e">要断开的客户端DYSocket</param>
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
        LicensingClient lc;
        byte[] addLeng(byte[] buff)
        {
            return DYWriter.Merge(DYWriter.GetDYBytes(buff.Length + 4), buff);
        }
        private string typeName = "DYCom_Net2.UserInfo";
        /// <summary>
        /// 发送消息给指定一个客户端
        /// </summary>
        /// <remarks>发送消息到参数中的客户端对象,客户端必须继承于UserInfo</remarks>
        /// <param name="info">客户端实例</param>
        /// <param name="data">消息体</param>
        /// <returns>返回True即发送成功，返回Flase即发送失败。</returns>
        public bool SendToSingle<T>(T info, byte[] data) 
        {
            if (typeof(T).BaseType.FullName.Equals(typeName) || typeof(T).FullName.Equals(typeName))
            {
                object obj = info;
                Send(data, ((UserInfo)obj).Sock.SocketArgs);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 发送消息到r指定的客户端集合
        /// </summary>
        /// <remarks>把消息发送到参数客户端集合里的所有客户端元素.客户端对象必须继承于UserInfo</remarks>
        /// <param name="Clients">客户端集合</param>
        /// <param name="Data">消息体</param>
        /// <returns>返回True即发送成功，返回Flase即发送失败。</returns>
        public bool SendToAll<T>(List<T> Clients, byte[] Data)
        {
            if (typeof(T).BaseType.FullName.Equals(typeName) || typeof(T).FullName.Equals(typeName))
            {
                for (int i = 0; i < Clients.Count; i++)
                {
                    object obj = Clients[i];
                    Send(Data, ((UserInfo)obj).Sock.SocketArgs);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// 从集合中发送消息，不发送到指定一个客户端
        /// </summary>
        /// <remarks>消息发送到指定客户端的其他对象，客户端必须继承于UserInfo</remarks>
        /// <param name="Clients">指定要发送的客户端集合</param>
        /// <param name="info">指定要发送例外的客户端实例</param>
        /// <param name="Data">消息体</param>
        /// <returns>返回True即发送成功，返回Flase即发送失败。</returns>
        public bool SendWithOut<T>(List<T> Clients, UserInfo info, byte[] Data)
        {
            if (typeof(T).BaseType.FullName.Equals(typeName) || typeof(T).FullName.Equals(typeName))
            {
                for (int i = 0; i < Clients.Count; i++)
                {
                    object obj = Clients[i];
                    if (((UserInfo)obj).Sock.SocketArgs.AcceptSocket.RemoteEndPoint 
                        != info.Sock.SocketArgs.AcceptSocket.RemoteEndPoint)
                    {
                        Send(Data, ((UserInfo)obj).Sock.SocketArgs);
                    }
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
