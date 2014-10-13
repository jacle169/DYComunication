using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DYComWinClient;
using DYsmartWinClient;

namespace DYsmartPCwinform
{
    public partial class Form1 : Form
    {
        ClientPoxy poxy;

        public Form1()
        {
            InitializeComponent();

            Load += new EventHandler(Form1_Load);
            button1.Click += new EventHandler(button1_Click);
            button2.Click += new EventHandler(button2_Click);
        }

        void button2_Click(object sender, EventArgs e)
        {
            //poxy.Send(smart.StartSerial(19200));
            //var dd = mapper.ToJson(new msg() { nid = 1, hid = 1, op = 0, bk = 1, val = new int[] { 9050, 4450, 600, 550, 550, 550, 600, 550, 550, 1650, 600, 550, 600, 550, 550, 550, 600, 550, 600, 1600, 600, 1700, 550, 1700, 550, 550, 600, 500, 600, 1700, 550, 1650, 600, 1650, 650, 1600, 600, 1650, 600 } });
            //var cc = dd.Length;
            //poxy.Send(smart.SendSerial(dd));
            //poxy.SendToSmart(smart.SendIRRemote("9050,4450,600,550,550,550,600,550,550,1650,600,550,600,550,550,550,600,550,600,1600,600,1700,550,1700,550,550,600,500,600,1700,550,1650,600,1650,650,1600,600,1650,600,550,550,550,600,550,550,550,600,1650,600,550,550,550,600,550,550,1650,600,1700,550,1650,600,1650,650,500,550,1700,600"));
            //poxy.SendToSmart(smart.runIRReader(7));
            //poxy.SendToSmart(smart.StartSerial(19200));
            //var cc = smart.SendIRRemote(1, "9050,4450,600,500,600,1650,600,500,600,500,600,500,600,550,600,500,600,500,600,1650,600,500,600,1650,600,1650,600,1650,600,1650,600,500,600,1700,600,500,600,500,600,1650,600,500,600,1650,600,500,600,500,600,500,600,1650,600,1700,600,500,600,1650,600,500,600,1650,600,1650,600,1650,600");
            //////////var dd = smart.runIRReader(4);
            //////////var cc = smart.SendSerial(dd);
            //////poxy.SendToSmart(smart.SendSerial(smart.GetAllDigitalAndAnalog(1)));
            //List<byte[]> orders = smart.IrSpiler(cc, 100);
            //foreach (var item in orders)
            //{
            //    poxy.SendToSmart(smart.SendSerial(item));
            //    System.Threading.Thread.Sleep(50);
            //}    
            poxy.SendToSmart(smart.SendSerial(smart.GetDigital("client1", 4)));
        }

        DYsmart smart = new DYsmart("123456");
        JsonMapper mapper = new JsonMapper();
        void button1_Click(object sender, EventArgs e)
        {
            poxy.SendToSmart(smart.StartSerial(19200));
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
