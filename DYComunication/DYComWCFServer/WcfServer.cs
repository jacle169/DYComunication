using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ServiceModel;
using DYCom_Net4;

namespace DYComWCFServer
{
    public class WcfServer
    {
        public WcfServer()
        {
            DYWcfPolicy policy = new DYWcfPolicy("PD key");//产品密钥
            Console.WriteLine(policy.StartPolicy(new Uri("http://localhost:80")));

            NetTcpBinding nt = new NetTcpBinding();
            nt.Security.Mode = SecurityMode.None;
            nt.SendTimeout = TimeSpan.FromSeconds(2);

            DYWcfServer server = new DYWcfServer("PD key");//产品密钥
            server.StartWcfServer(nt, typeof(Service), typeof(ITest), "net.tcp://localhost:4504/Service", "http://localhost:4500/mex");

            Console.WriteLine("程序已启动");
        }
    }

    [ServiceContract(CallbackContract=typeof(ITestCallback))]
    public interface ITest
    {
        [OperationContract(IsOneWay=true)]
        void login(string uid);

        [OperationContract(IsOneWay=true)]
        void Echo(string uid, string text);
    }

    public interface ITestCallback
    {
        [OperationContract(IsOneWay = true)]
        void clientLogin(string uid);
        [OperationContract(IsOneWay = true)]
        void onData(mydata data);
    }

    [ServiceBehavior(InstanceContextMode=InstanceContextMode.Single)]
    public class Service : ITest
    {
        List<ITestCallback> clients = new List<ITestCallback>();

        public void login(string uid)
        {
            var client = OperationContext.Current.GetCallbackChannel<ITestCallback>();
            if (!clients.Contains(client))
            {
                clients.Add(client);
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

    [DataContract]
    public class mydata
    {
        [DataMember(Order = 0)]
        public string userid { get; set; }
        [DataMember(Order = 1)]
        public string message { get; set; }
    }

}
