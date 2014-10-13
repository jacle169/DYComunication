using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ServiceModel;
using DYCom_Net4;

namespace DYComWcfGameServer
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
            server.StartWcfServer(nt, typeof(Service), typeof(IdyGameServer), "net.tcp://localhost:4504/Service", "http://localhost:4500/mex");

            Console.WriteLine("程序已启动");
        }
    }

    [ServiceContract(CallbackContract=typeof(IdyGameCallback))]
    public interface IdyGameServer
    {
        [OperationContract(IsOneWay=true)]
        void Send(byte[] data);
    }

    public interface IdyGameCallback
    {
        [OperationContract(IsOneWay = true)]
        void onData(byte[] data);
    }

    [ServiceBehavior(InstanceContextMode=InstanceContextMode.Single, ConcurrencyMode=ConcurrencyMode.Multiple)]
    public class Service : IdyGameServer
    {
        static List<IdyGameCallback> clients = new List<IdyGameCallback>();

       public void Send(byte[] data)
       {
           //实例化DYCOM消息解码器
           DYReader read = new DYReader(data);
           //操作符变量
           int type;
           //读取消息中的操作符
           if (!read.ReadInt32(out type))
           {
               //跳出本函数
               return;
           }
           //转换操作符
           op opera = (op)type;
           //判断操作符
           if (opera == op.login)
           {
               //变量
               string userId;
               //读出一个字符串值到message变量,使用UTF8解码
               if (read.ReadString(out userId, Encoding.UTF8))
               {
                   //发送message到所有指定客户端售
                   //这里消息体本身是操作符+message，读取的时候也一样要按这个消息体顺序进行读取消息
                   //DYWriter.Merge是合并DYWriter.GetDYBytes消息之用
                   var client = OperationContext.Current.GetCallbackChannel<IdyGameCallback>();
                   if (!clients.Contains(client))
                   {
                       clients.Add(client);
                       SendToAll(DYWriter.Merge(DYWriter.GetDYBytes((int)op.login),
                       DYWriter.GetDYBytes(userId, Encoding.UTF8)));
                   }
               }
           }
           else if (opera == op.toAll)
           {
               //广播到所有客户端
               SendToAll(data);
           }

       }

       void SendToAll(byte[] data)
       {
           for (int i = 0; i < clients.Count; i++)
           {
               try
               {
                   clients[i].onData(data);
               }
               catch
               {
                   if (clients.Count > 0)
                   {
                       clients.Remove(clients[i]);
                       i--;
                   }
               }
           }
       }

    }

    //自定义消息操作符
    public enum op
    {
        login,
        toAll
    }

}
