using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DYComWinClient;
using System.Threading;
using DYsmartWinClient;
//******************************************
//http://DY2COM.com
//DYCOM WinForm Client Sample
//*******************************************
namespace DYComWinFormClientSample
{
    public partial class Form1 : Form
    {
        //DYCom客户端实例
        ClientPoxy poxy;

        public Form1()
        {
            InitializeComponent();

            Load += new EventHandler(Form1_Load);
            button1.Click += new EventHandler(button1_Click);
        }

        void button1_Click(object sender, EventArgs e)
        {
            //发送Textbox1的内容到服务端
            //这里消息体本身是操作符+Textbox1的内容,使用UTF8编码，读取的时候也一样要按这个消息体顺序进行读取消息
            //DYWriter.Merge是合并DYWriter.GetDYBytes消息之用
            poxy.Send(DYWriter.Merge(DYWriter.GetDYBytes((int)op.login),
                DYWriter.GetDYBytes(textBox1.Text, System.Text.Encoding.UTF8)));
            Thread t = new Thread(new ParameterizedThreadStart(create));
            t.IsBackground = true;
            t.Start();

            //DateTime t = DateTime.Now;
            for (int i = 0; i < 10000; i++)
            {
                poxy.Send(DYWriter.Merge(DYWriter.GetDYBytes((int)op.login), DYWriter.GetDYBytes(string.Format("{0}sjdfksldkfjlkasdjfklajsdklfjkldjfklasjdfkj时要苦地地 地地 右枯地顶替要夺顶替顶替在需要地厅ksdfkj sdkf ###@#@#@#@#@$$#%#%$#", i), Encoding.UTF8)));
                //System.Threading.Thread.Sleep(5);
            }
            //TimeSpan span = DateTime.Now - t;
            //Console.WriteLine(span.ToString());
        }

        void create(object obj)
        {
            for (int i = 0; i < 50; i++)
            {
                Thread t = new Thread(new ParameterizedThreadStart(createconnet));
                t.IsBackground = true;
                t.Start();
                System.Threading.Thread.Sleep(20);
            }
        }

        void createconnet(object obj)
        {
            Thread t = new Thread(new ParameterizedThreadStart(connettest));
            t.IsBackground = true;
            t.Start();
        }

        void connettest(object obj)
        {
            for (int i = 0; i < 50; i++)
            {
                ClientPoxy xy = new ClientPoxy("192.168.0.2", 4510, 1);
                System.Threading.Thread.Sleep(20);
            }
        }

        void Form1_Load(object sender, EventArgs e)
        {
            //实例化DYCom客户端实例,服务端地址，端口号，缓冲区大小
            poxy = new ClientPoxy("192.168.0.254", 4533, 1024);
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

        //数据消息到来件事
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
    //操作符定义,可以自由添加删除操作符的定义
    public enum op
    {
        login
    }
}
