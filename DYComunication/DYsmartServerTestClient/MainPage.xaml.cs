using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Text;
using DYComSLClient;

namespace DYsmartServerTestClient
{
    public partial class MainPage : UserControl
    {
        ClientPoxy poxy;

        public MainPage()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(MainPage_Loaded);
            button1.Click += new RoutedEventHandler(button1_Click);
        }

        void button1_Click(object sender, RoutedEventArgs e)
        {
            poxy.Send(DYComSLClient.DYWriter.Merge(DYComSLClient.DYWriter.GetDYBytes((int)op.resetDYsmart),
                DYComSLClient.DYWriter.GetDYBytes("183.35.172.90", Encoding.UTF8)));
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            //实例化DYCom客户端实例,服务端地址，端口号，缓冲区大小
            poxy = new ClientPoxy("192.168.0.2", 4510, 1024);
            //连接事件注册处理函数
            poxy.onConnet = new OnConnet(onConnect);
            //消息到来事件注册处理函数
            poxy.onData = new OnData(onData);
            //断开连接事件注册处理函数
            poxy.onDisconnet = new OnDisConnet(ondisConnect);
        }

        void onConnect(bool result)
        {
            //显示于listbox1
            listBox1.Items.Insert(0, result.ToString());
        }

        //数据消息到来件事
        void onData(byte[] data)
        {
            //实例化DYCOM消息解码器
            DYComSLClient.DYReader read = new DYComSLClient.DYReader(data);
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
            if (opera == op.resetDYsmart)
            {
                string message;
                if (read.ReadString(out message, Encoding.UTF8))
                {
                    listBox1.Items.Insert(0, message);
                }
            }
        }

        //用户断开事件
        void ondisConnect(string message)
        {
            //显示断开消息到UI
            listBox1.Items.Insert(0, message);
        }

    }

    public enum op
    {
        resetDYsmart
    }
}
