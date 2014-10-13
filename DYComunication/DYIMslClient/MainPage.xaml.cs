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
using DYComWPClient;
using System.Text.RegularExpressions;

namespace DYIMslClient
{
    public partial class MainPage : UserControl
    {
        //DYCom客户端实例
        ClientPoxy poxy;
        JsonMapper mapper = new JsonMapper();
        string ServerAddress;
        string pwd;

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


            login("jacle@127.0.0.1", "pwd");
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {

        }


        public string login(string uid, string pwd)
        {
            if (!isEmail(uid))
            {
                return "uid not sure";
            }
            ServerAddress = uid;
            this.pwd = pwd;

            //实例化DYCom客户端实例,服务端地址，端口号，缓冲区大小
            poxy = new ClientPoxy(ServerAddress.Split('@')[1], 4510, 1024);
            //连接事件注册处理函数
            poxy.onConnet = new OnConnet(onConnect);
            //消息到来事件注册处理函数
            poxy.onData = new OnData(onData);
            //断开连接事件注册处理函数
            poxy.onDisconnet = new OnDisConnet(ondisConnect);

            return "ok";
        }

        //result：是否成功连接到服务器
        void onConnect(bool result)
        {
            if (result)
            {
                listBox1.Items.Insert(0, "ok");
                dyMessage msg = new dyMessage();
                msg.uid = ServerAddress.Split('@')[0];
                msg.to = ServerAddress.Split('@')[1];
                msg.pwd = this.pwd;
                msg.state = "online";
                msg.from = "127.0.0.1";
                msg.operation = "get";
                msg.message = "login";
                poxy.Send(DYWriter.GetDYBytes(mapper.ToJson(msg), System.Text.Encoding.UTF8));
            }
        }

        //o
        void onData(byte[] data)
        {
            //实例化DYCOM消息解码器
            DYReader read = new DYReader(data);
            ////操作符变量
            //int type;
            ////读取消息中的操作符
            //if (!read.ReadInt32(out type))
            //{
            //    return;
            //}
            ////转换操作符
            //op opera = (op)type;
            ////判断操作符
            //if (opera == op.login)
            //{
                //变量
                string message;
                //读出一个字符串值到message变量,使用UTF8解码
                if (read.ReadString(out message, System.Text.Encoding.UTF8))
                {
                    var msg = mapper.ToObject<dyMessage>(message);
                    //显示消息到UI
                    listBox1.Items.Insert(0, msg.message);
                }
            //}
        }

        //用户断开事件
        void ondisConnect(string message)
        {
            //显示断开消息到UI
            listBox1.Items.Insert(0, message);
        }


        bool isEmail(string inputEmail)
        {
            string strRegex = @"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*";
            Regex re = new Regex(strRegex);
            if (re.IsMatch(inputEmail))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }

    public class dyMessage
    {
        public string from { get; set; }
        public string to { get; set; }
        public string uid { get; set; }
        public string pwd { get; set; }
        public string state { get; set; }
        public string operation { get; set; }
        public string message { get; set; }
    }

}
