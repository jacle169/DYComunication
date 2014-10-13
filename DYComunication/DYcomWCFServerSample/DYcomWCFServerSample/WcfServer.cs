using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ServiceModel;
using DYCom_Net4;
//************************************************************************
// http://dy2com.com 
//基于dycom beta1.0.0.2 的wcf服务器端示例
//本示例演示如何快速建立一个可以让silverlight4调用的wcf.net.tcp服务器
//************************************************************************
namespace DYcomWCFServerSample
{
    public class WcfServer
    {
        public WcfServer()
        {
            //启动DYCOM的wcf.net.tcp跨域服务，针对silverlight 4
            DYWcfPolicy policy = new DYWcfPolicy("PD key");//产品密钥
            //以下URI是服务端机器的公开IP地址和80端口，否则会造成slilverlgiht无法访问服务
            Console.WriteLine(policy.StartPolicy(new Uri("http://localhost:80")));

            //协议绑定，这里可按用户喜好选择各自要使用的各种wcf绑定
            NetTcpBinding nt = new NetTcpBinding();
            nt.Security.Mode = SecurityMode.None;
            nt.SendTimeout = TimeSpan.FromSeconds(2);

            //DYCOM的wcf服务器组件
            DYWcfServer server = new DYWcfServer("PD key");//产品密钥
            //启动wcf服务, * 绑定，服务逻辑类type，服务接口type, 服务通信地址，MEX地址
            //mex地址用于提供给客户端引用服务时使用, 在你的应用已经开发完成后请使用另一个不用mex的构造函数,这样可以僻免非法用户引用你的服务
            server.StartWcfServer(nt, typeof(Service), typeof(ITest), "net.tcp://localhost:4504/Service", "http://localhost:4500/mex");

            Console.WriteLine("程序已启动");
        }
    }

    //wcf服务端接口
    [ServiceContract(CallbackContract=typeof(ITestCallback))]
    public interface ITest
    {
        [OperationContract(IsOneWay=true)]
        void login(string uid);

        [OperationContract(IsOneWay=true)]
        void Echo(string uid, string text);
    }

    //wcf客户端回调接口
    public interface ITestCallback
    {
        [OperationContract(IsOneWay = true)]
        void clientLogin(string uid);
        [OperationContract(IsOneWay = true)]
        void onData(mydata data);
    }

    //wcf服务逻辑实例
    [ServiceBehavior(InstanceContextMode=InstanceContextMode.Single)]
    public class Service : ITest
    {
        List<ITestCallback> clients = new List<ITestCallback>();

        public void login(string uid)
        {
            //取得当前用户
            var client = OperationContext.Current.GetCallbackChannel<ITestCallback>();
            if (!clients.Contains(client))
            {
                //添加用户到用户集合
                clients.Add(client);
                //调用集合中的所有用户的客户端clientLogin方法
                for (int i = 0; i < clients.Count; i++)
                {
                    try
                    {
                        clients[i].clientLogin(uid);
                    }
                    catch
                    {
                        clients.Remove(clients[i]);
                        i--;
                    }
                }
            }
        }

        //广播消息到所有客户端
       public void Echo(string uid, string text)
        {
            for (int i = 0; i < clients.Count; i++)
            {
                try
                {
                    clients[i].onData(new mydata() { userid = uid, message = text });
                }
                catch
                {
                    clients.Remove(clients[i]);
                    i--;
                }
            }
        }

    }

    //wcf数据契约
    [DataContract]
    public class mydata
    {
        [DataMember(Order = 0)]
        public string userid { get; set; }
        [DataMember(Order = 1)]
        public string message { get; set; }
    }

}
