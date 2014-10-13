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
//引入代理
using DYComWCFTextClient.poxy;
//************************************************************************
// http://dy2com.com 
//基于dycom beta1.0.0.2 的wcf服务器端示例
//本示例演示如何快速建立一个可以让silverlight4调用的wcf.net.tcp服务器
//客户端演示
//************************************************************************
namespace DYComWCFTextClient
{
    //继承wcf回调接口ITestCallback
    public partial class MainPage : UserControl,ITestCallback 
    {
        //创建客户端代理
        TestClient tc;
        //用户名变量
        string uid;

        public MainPage()
        {
            InitializeComponent();
            button1.Click += new RoutedEventHandler(button1_Click);
            button2.Click += new RoutedEventHandler(button2_Click);
        }

        void button2_Click(object sender, RoutedEventArgs e)
        {
            if (textBox1.Text != string.Empty && tc != null)
            {
                //调用服务端的广播方法
                tc.EchoAsync(uid, textBox1.Text);
            }
        }

        void button1_Click(object sender, RoutedEventArgs e)
        {
            if (tc == null && textBox1.Text != string.Empty)
            {
                //实例化客户端代理,由于客户端接口继承于本类，所以InstanceContext(this)
                tc = new TestClient(new System.ServiceModel.InstanceContext(this));
                //注册调用服务器端的login方法的完成通知事件
                tc.loginCompleted += new EventHandler<System.ComponentModel.AsyncCompletedEventArgs>(tc_loginCompleted);
                //调用服务器的login方法
                tc.loginAsync(textBox1.Text);
                //记录用户名
                uid = textBox1.Text;
            }
        }

        void tc_loginCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            //调用login方法完成后让button1不可用
            button1.IsEnabled = false;
        }

        public void clientLogin(string uid)
        {
            //服务器回调客户端的clientLogin方法，把内容显示到UI
            listBox1.Items.Insert(0, uid);
        }

        public void onData(SilverlightApplication1.poxy.mydata data)
        {
            //服务器回调客户端的onData方法，把内容显示到UI
            listBox1.Items.Insert(0, data.userid + ": " + data.message);
        }
    }
}
