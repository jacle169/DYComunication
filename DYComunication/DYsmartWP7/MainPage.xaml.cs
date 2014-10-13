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
using Microsoft.Phone.Controls;
using DYComWPClient;

namespace DYsmartWP7
{
    public partial class MainPage : PhoneApplicationPage
    {
        ClientPoxy poxy;
        DYsmart smart = new DYsmart("123456");
        JsonMapper mapper = new JsonMapper();

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(MainPage_Loaded);
            button1.Click += new RoutedEventHandler(button1_Click);
            button1_Copy.Click += new RoutedEventHandler(button1_Copy_Click);
        }

        void button1_Copy_Click(object sender, RoutedEventArgs e)
        {
            poxy.SendToSmart(smart.SendSerial(smart.GetAllDigitalAndAnalog("smart0002", 9, 6)));
        }

        void button1_Click(object sender, RoutedEventArgs e)
        {
            poxy.SendToSmart(smart.StartSerial(19200));
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            //实例化DYCom客户端实例,服务端地址，端口号，缓冲区大小
            poxy = new ClientPoxy("dopc.vicp.net", 4533, 1024);
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
            DYsmartDecoder smartDecoder = new DYsmartDecoder();
            smartDecoder.initData(data);
            operations? opera = smartDecoder.getOperation();
            if (opera.HasValue)
            {
                object dt = smartDecoder.decode(opera.Value);
                if (opera == operations.BeginSerial || opera == operations.ResetAll || opera == operations.SetPwd || opera == operations.SerialSend
                || opera == operations.PinMode || opera == operations.SetDigital || opera == operations.SetPWM || opera == operations.SetServo
                || opera == operations.SetSteper || opera == operations.runIRReader || opera == operations.SendIRRemote || opera == operations.Get18B20 ||
                    opera == operations.GetDigital || opera == operations.GetAnalog || opera == operations.IRReadOnData || opera == operations.SerialOnData)
                {
                    listBox1.Items.Insert(0, dt.ToString());
                    //textBox1.Text = dt.ToString();
                }
                else if (opera == operations.GetAllDigitalAndAnalog)
                {
                    IOs io = mapper.ToObject<IOs>(dt.ToString());
                    listBox1.Items.Insert(0, dt.ToString());
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
}