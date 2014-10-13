using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DYComWinClient;
using DYsmartWinClient;

namespace DYsmartWINSample
{
    public partial class Form1 : Form
    {
        //定义一个连接通信器
        ClientPoxy poxy;

        public Form1()
        {
            InitializeComponent();
            button1.Click += new EventHandler(button1_Click);
            Load += new EventHandler(Form1_Load);
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

        //实例一个DYsmart指令处理器
        DYsmart smart = new DYsmart("123456");

        void button1_Click(object sender, EventArgs e)
        {
            //通过通信器发送一个DYsmart指令（启动DYsmart的串口，并设置串口比特率为19200）
            poxy.SendToSmart(smart.StartSerial(19200));
        }

        void onConnect(bool result)
        {
            //显示于listbox1
            listBox1.Items.Insert(0, result.ToString());
        }

        //数据消息到来件事
        void onData(byte[] data)
        {
            //实例化DYsmart指令解释器
            DYsmartDecoder smartDecoder = new DYsmartDecoder();
            //初始化Smart操作结果指令解释器
            smartDecoder.initData(data);
            //取得操作结果操作符
            operations? opera = smartDecoder.getOperation();
            //跟据不同操作符解释结果并显示于页面
            if (opera.HasValue)
            {
                object dt = smartDecoder.decode(opera.Value);
                if (opera == operations.BeginSerial || opera == operations.ResetAll || opera == operations.SetPwd || opera == operations.BeginSerial || opera == operations.SerialSend
                || opera == operations.PinMode || opera == operations.SetDigital || opera == operations.SetPWM || opera == operations.SetServo
                || opera == operations.SetSteper || opera == operations.runIRReader || opera == operations.SendIRRemote || opera == operations.Get18B20 ||
                    opera == operations.GetDigital || opera == operations.GetAnalog || opera == operations.IRReadOnData || opera == operations.SerialOnData)
                {
                    listBox1.Items.Insert(0, dt.ToString());
                    textBox1.Text = dt.ToString();
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
                    listBox1.Items.Insert(0, displayString);
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
