using System;
using System.Collections.Generic;
using System.Text;
using DYCom_Net4;
using DYComWinClient;
using DYsmartWinClient;
//******************************************
//http://DY2COM.com
//DYCOM Sample
//*******************************************
namespace DYsmartServer
{
    public class DYComServer
    {
        //客户端集合
        List<myClient> clients = new List<myClient>();
        //DYCOM实例
        DYComer server;
        ClientPoxy poxy;
        DYsmartWinClient.DYsmart smart = new DYsmartWinClient.DYsmart("123456");

        string key = "PD Key";//产品密钥，请修改为您自己的产品密钥

        public DYComServer()
        {
            //实例化DYCOM
            server = new DYComer();
            //注册用户连接事件
            server.onConnet = new DYCom_Net4.OnConnet(server_OnConnetEvent);
            //注册用户通信消息到来事件
            server.onData = new DYCom_Net4.OnData(server_OnDataEvent);
            //注册用户异常和断开事件
            server.onDisconnet = new DYCom_Net4.OnDisConnet(server_OnDisConnetEvent);
            //启动服务
            server.Start(4510, 1000, 1024, key);//端口，最大允许客户端连接数，每个客户端核心缓冲区大小，产品密钥

            //显示DYCOM组件的.net fw版本
            Console.WriteLine("当前DYCOM运行的.NET版本是：" + server.TargetFramework());
            //显示DYCOM的版本号
            Console.WriteLine("当前DYCOM的产品版本是：" + server.Version());
        }

        //用户连接事件
        void server_OnConnetEvent(DYSocket e, bool IsConneted)
        {
            //客户端连接提示
            Console.WriteLine(e.SocketArgs.AcceptSocket.RemoteEndPoint.ToString() + " 已接入");
            //判断连接客户端是否新接入（新接入用户UserToken是等于null的）
            if (e.SocketArgs.UserToken == null)
            {
                //实例化
                var mc = new myClient();
                //把连接客户端设置到客户端实例
                mc.Sock = e;
                //希望用户在一定时间内没有任何通信将其断开(秒)
                //mc.SetupTimeoutCheck(server, 5);
                //操作完成，保存客户实例
                e.SocketArgs.UserToken = mc;
                //添加新客户端到客户端集合
                clients.Add(e.SocketArgs.UserToken as myClient);
            }
        }

        //用户消息收到事件
        void server_OnDataEvent(byte[] data, UserInfo user)
        {
            //实例化DYCOM消息解码器
            DYCom_Net4.DYReader read = new DYCom_Net4.DYReader(data);
            //操作符变量
            int type;
            //读取消息中的操作符
            if (!read.ReadInt32(out type))
            {
                //操作符不存即断开当前客户端连接
                server.DisConnet(user.Sock);
                //跳出本函数
                return;
            }
            //转换操作符
            op opera = (op)type;
            //判断操作符
            if (opera == op.resetDYsmart)
            {
                string dySmartIP;
                if (read.ReadString(out dySmartIP, Encoding.UTF8))
                {
                    poxy = new ClientPoxy(dySmartIP, 4533, 1024);
                    poxy.onConnet = new DYComWinClient.OnConnet(poxyOnConnect);
                    poxy.onData = new DYComWinClient.OnData(poxyOnData);
                    poxy.onDisconnet = new DYComWinClient.OnDisConnet(poxyOndisConnect);
                }
            }
        }

        void poxyOnConnect(bool result)
        {
            if (result)
            {
                poxy.Send(smart.resetAll());
            }
            else { Console.WriteLine("no connect to dysmart"); }
        }

        //数据消息到来件事
        void poxyOnData(byte[] data)
        {
            DYsmartDecoder smartDecoder = new DYsmartDecoder();
            smartDecoder.initData(data);
            operations? opera = smartDecoder.getOperation();
            if (opera.HasValue)
            {
                object dt = smartDecoder.decode(opera.Value);
                if (opera == operations.BeginSerial || opera == operations.ResetAll || opera == operations.SetPwd || opera == operations.BeginSerial || opera == operations.SerialSend
                || opera == operations.PinMode || opera == operations.SetDigital || opera == operations.SetPWM || opera == operations.SetServo
                || opera == operations.SetSteper || opera == operations.runIRReader || opera == operations.SendIRRemote || opera == operations.Get18B20 ||
                    opera == operations.GetDigital || opera == operations.GetAnalog || opera == operations.IRReadOnData)
                {
                    Console.WriteLine(dt.ToString());
                    server.SendToAll<myClient>(clients, DYCom_Net4.DYWriter.Merge(DYCom_Net4.DYWriter.GetDYBytes((int)operations.ResetAll),
                        DYCom_Net4.DYWriter.GetDYBytes(dt.ToString(), Encoding.UTF8)));
                }
                else if (opera == operations.GetAllDigitalAndAnalog)
                {
                    IOs io = (IOs)dt;
                    string displayString = "Analogs:";
                    for (int i = 0; i < io.analogs.Length; i++)
                    {
                        displayString += io.analogs[i].ToString();
                        if (i < io.analogs.Length - 1)
                        {
                            displayString += ",";
                        }
                        else
                        {
                            displayString += " Digitals:";
                        }
                    }
                    for (int i = 0; i < io.digitals.Length; i++)
                    {
                        displayString += io.digitals[i].ToString();
                        if (i < io.digitals.Length - 1)
                        {
                            displayString += ",";
                        }
                    }
                    Console.WriteLine(displayString);
                }
            }
        }

        //用户断开事件
        void poxyOndisConnect(string message)
        {
            //显示断开消息到UI
            Console.WriteLine(message);
        }

        //用户异常或断开连接事件
        void server_OnDisConnetEvent(DYSocket e)
        {
            //判断触发是否真实实例
            if (e.SocketArgs.UserToken != null)
            {
                //从客户端集合中查询实例
                myClient info = e.SocketArgs.UserToken as myClient;
                //断开客户端
                info.SocketClose();
                //从客户端集合中删除实例
                clients.Remove(info);
                //清空DYCOM内部关于本实例的相关内容
                e.SocketArgs.UserToken = null;
            }
        }

    }

    //自定义客户端
    public class myClient : UserInfo
    {
        public myClient()
        {
            //设置每个客户端缓冲区
            SetSendBuffer(1024);
            //捕捉每个客户端内部错误
            SendHandle = new SendErrorHandler(sendError);
        }
        public string userName { get; set; }

        //内部错误捕捉事件
        void sendError(string message)
        {
            Console.WriteLine(message);
        }
    }

    //自定义消息操作符
    public enum op
    {
        resetDYsmart
    }
}
