using System;
using System.Collections.Generic;
using System.Text;
using DYCom_Net2;

namespace DYComSLAndFlash跨域服务
{
    public class SilverlightCrossDomain
    {
        DYSocketCore PolicyServer;
        readonly string policyRequestString = "<policy-file-request/>";
        byte[] policyData;
        string key = "DP key";//产品密钥，请修改为您自己的产品密钥

        public SilverlightCrossDomain()
        {
            policyData = UTF8Encoding.UTF8.GetBytes(@"<?xml version=""1.0"" encoding =""utf-8""?><access-policy><cross-domain-access><policy><allow-from><domain uri=""*"" /></allow-from><grant-to><socket-resource port=""4502-4534"" protocol=""tcp"" /></grant-to></policy></cross-domain-access></access-policy>");
            PolicyServer = new DYSocketCore(943, 1000, 245, key);
            PolicyServer.DataOnCore = new OndataHandler(OnData);
            PolicyServer.ConnectOnCore = new ConnectHandler(OnConnection);
            PolicyServer.ErrorOnCore = new ErrorHandler(OnError);
            PolicyServer.Start();

            Console.WriteLine("silverlight ok");
        }

        void OnData(byte[] data, DYSocket e)
        {
            string txt = Encoding.Default.GetString(data);

            if (txt.Equals(policyRequestString))
            {
                PolicyServer.SendSync(e, policyData);
            }
            PolicyServer.Disconnect(e);
        }

        void OnConnection(DYSocket e, bool IsConneted)
        {
            Console.WriteLine(e.SocketArgs.AcceptSocket.RemoteEndPoint.ToString() + "连接验证端口");
            if (e.SocketArgs.UserToken == null)
            {
                UserInfo info = new UserInfo();
                info.Sock = e;
                e.SocketArgs.UserToken = info;
            }
        }

        void OnError(string message, DYSocket e)
        {
            Console.WriteLine(message + " 验证端口");

            PolicyServer.Disconnect(e);
        }

    }
}
