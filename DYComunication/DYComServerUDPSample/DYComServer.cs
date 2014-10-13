using System;
using System.Collections.Generic;
using System.Text;
using DYCom_Net4;
//******************************************
//http://DY2COM.com
//DYCOM Sample
//*******************************************
namespace DYComServerUDPSample
{
    public class DYComServer
    {
        //定义一个udp通信器
        DYudpCore com;
        public DYComServer()
        {
            //实例化udp通信器
            com = new DYudpCore(11000, 1024);
            //注册消息到达事件
            com.onData = new OnUdpData(ondata);
            //定义一个udp消息发送器
            //参数1：目标计算机ip
            //参数2：目标计算机端口
            DYudpSender uc = new DYudpSender("127.0.0.1", 11000);
            //发送UDP消息
            //这里消息体本身是操作符+消息，读取的时候也一样要按这个消息体顺序进行读取消息
            //DYWriter.Merge是合并DYWriter.GetDYBytes消息之用
            uc.SendUpd(DYWriter.Merge(DYWriter.GetDYBytes((int)op.login), DYWriter.GetDYBytes("hello", Encoding.UTF8)));
        }

        void ondata(byte[] data, string endpoint)
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
                string msg;
                if (read.ReadString(out msg, Encoding.UTF8))
                {
                    Console.WriteLine(msg);
                    Console.WriteLine(endpoint.ToString());
                }
            }
        }

    }

    //自定义消息操作符
    public enum op
    {
        login
    }
}
