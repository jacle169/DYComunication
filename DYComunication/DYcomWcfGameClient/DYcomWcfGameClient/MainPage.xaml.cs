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

namespace DYcomWcfGameClient
{
    public partial class MainPage : UserControl,poxy.IdyGameServerCallback 
    {
        poxy.IdyGameServerClient server;
        public MainPage()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(MainPage_Loaded);
            button1.Click += new RoutedEventHandler(button1_Click);
        }

        void button1_Click(object sender, RoutedEventArgs e)
        {
            if (textBox1.Text != string.Empty)
            {
                listBox1.Items.Insert(0, DateTime.Now.ToString());
                for (int i = 0; i < 10000; i++)
                {
                    server.SendAsync(DYWriter.Merge(DYWriter.GetDYBytes((int)op.toAll), DYWriter.GetDYBytes(textBox1.Text, System.Text.Encoding.UTF8)));
                }
                listBox1.Items.Insert(0, DateTime.Now.ToString());
            }
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            server = new poxy.IdyGameServerClient(new System.ServiceModel.InstanceContext(this));
            server.SendCompleted += new EventHandler<System.ComponentModel.AsyncCompletedEventArgs>(server_SendCompleted);
            server.SendAsync(DYWriter.Merge(DYWriter.GetDYBytes((int)op.login), DYWriter.GetDYBytes("jacob", System.Text.Encoding.UTF8)));
        }

        void server_SendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            button1.IsEnabled = true;
        }

        public void onData(byte[] data)
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
           if (opera == op.toAll)
           {
               string msg;
               if (read.ReadString(out msg, System.Text.Encoding.UTF8))
               {
                   listBox1.Items.Insert(0, msg);
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
