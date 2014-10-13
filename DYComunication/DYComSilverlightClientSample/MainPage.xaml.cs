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
using DYComSLClient;
//******************************************
//http://DY2COM.com
//DYCOM Silverlight3.0 Client Sample
//*******************************************
namespace DYComSilverlightClientSample
{
    public partial class MainPage : UserControl
    {
        //DYCom客户端实例
        ClientPoxy poxy;

        public MainPage()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(MainPage_Loaded);
            button1.Click += new RoutedEventHandler(button1_Click);
        }

        void button1_Click(object sender, RoutedEventArgs e)
        {
            //发送message到服务端
            //这里消息体本身是操作符+message,使用UTF8编码，读取的时候也一样要按这个消息体顺序进行读取消息
            //DYWriter.Merge是合并DYWriter.GetDYBytes消息之用
            poxy.Send(DYWriter.Merge(DYWriter.GetDYBytes((int)op.login),
                 DYWriter.GetDYBytes(textBox1.Text, System.Text.Encoding.UTF8)));
           
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            //实例化DYCom客户端实例,服务端地址，端口号，缓冲区大小
            poxy = new ClientPoxy("127.0.0.1", 4510, 1024);
            //连接事件注册处理函数
            poxy.onConnet = new OnConnet(onConnect);
            //消息到来事件注册处理函数
            poxy.onData = new OnData(onData);
            //断开连接事件注册处理函数
            poxy.onDisconnet = new OnDisConnet(ondisConnect);
        }

        //result：是否成功连接到服务器
        void onConnect(bool result)
        {
            //显示于listbox1
            listBox1.Items.Insert(0, result.ToString());
        }

        //o
        void onData(byte[] data)
        {
            //实例化DYCOM消息解码器
            DYReader read = new DYReader(data);
            //操作符变量
            int type;
            //读取消息中的操作符
            if (!read.ReadInt32(out type))
            {
                return;
            }
            //转换操作符
            op opera = (op)type;
            //判断操作符
            if (opera == op.login)
            {
                //变量
                string message;
                //读出一个字符串值到message变量,使用UTF8解码
                if (read.ReadString(out message, System.Text.Encoding.UTF8))
                {
                    //显示消息到UI
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
        login
    }
}
