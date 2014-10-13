using System;
using System.Collections.Generic;
using System.Text;
using DYCom_Net4;

namespace DYComSLAndFlash跨域服务
{
    public class FlashCrossDomain
    {
        DYSocketCore PolicyServer;
        readonly string policyRequestString = "<policy-file-request/>\0";
        byte[] policyData;

        public FlashCrossDomain()
        {
            policyData = UnicodeEncoding.ASCII.GetBytes("<cross-domain-policy><allow-access-from domain=\"*\" to-ports=\"*\"/></cross-domain-policy>\0");
            PolicyServer = new DYSocketCore(843, 1000, 88);
            PolicyServer.DataOnCore = new OndataHandler(OnData);
            PolicyServer.ConnectOnCore = new ConnectHandler(OnConnection);
            PolicyServer.ErrorOnCore = new ErrorHandler(OnError);
            PolicyServer.Start();

            Console.WriteLine("flash ok");
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
