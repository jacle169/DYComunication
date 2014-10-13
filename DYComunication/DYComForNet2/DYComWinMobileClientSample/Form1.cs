using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DYComWMClient;
using DYsmartWMClient;
//******************************************
//http://DY2COM.com
//DYCOM WindowsMobile Client Sample
//*******************************************
namespace DYComWinMobileClientSample
{
    public partial class Form1 : Form
    {
        //DYCom客户端实例
        ClientPoxy poxy;

        public Form1()
        {
            InitializeComponent();
            Load += new EventHandler(Form1_Load);
        }

        void Form1_Load(object sender, EventArgs e)
        {
            //实例化DYCom客户端实例
            poxy = new ClientPoxy();
            //连接事件注册处理函数
            poxy.onConnet = new OnConnet(onConnect);
            //消息到来事件注册处理函数
            poxy.onData = new OnData(onData);
            //断开连接事件注册处理函数
            poxy.onDisconnet = new OnDisConnet(ondisConnect);
        }
        DYsmart smart = new DYsmart("123456");
        JsonMapper mapper = new JsonMapper();
        private void menuItem1_Click(object sender, EventArgs e)
        {
            //连接到服务器(服务端地址，端口号，缓冲区大小)
            poxy.Start("192.168.0.2", 4510, 1024);
        }

        //result：是否成功连接到服务器
        void onConnect(bool result)
        {
            //显示于listbox1
            listBox1.Items.Insert(0, result.ToString());
            //if (result)
            //{
            //    for (int i = 0; i < 10; i++)
            //    {
            //        poxy.Send(DYWriter.Merge(DYWriter.GetDYBytes((int)op.login), DYWriter.GetDYBytes(string.Format("{0}sjdfksldkfjlkasdjfklajsdklfjkldjfklasjdfkj时要苦地地 地地 右枯地顶替要夺顶替顶替在需要地厅ksdfkj sdkf ###@#@#@#@#@$$#%#%$#", i), Encoding.UTF8)));
            //        //System.Threading.Thread.Sleep(300);
            //    }
            //}
        }

        //数据消息到来件事
        void onData(byte[] data)
        {
            //DYsmartDecoder smartDecoder = new DYsmartDecoder();
            //smartDecoder.initData(data);
            //operations? opera = smartDecoder.getOperation();
            //if (opera.HasValue)
            //{
            //    object dt = smartDecoder.decode(opera.Value);
            //    if (opera == operations.BeginSerial || opera == operations.ResetAll || opera == operations.SetPwd || opera == operations.BeginSerial || opera == operations.SerialSend
            //    || opera == operations.PinMode || opera == operations.SetDigital || opera == operations.SetPWM || opera == operations.SetServo
            //    || opera == operations.SetSteper || opera == operations.runIRReader || opera == operations.SendIRRemote || opera == operations.Get18B20 ||
            //        opera == operations.GetDigital || opera == operations.GetAnalog || opera == operations.IRReadOnData || opera == operations.SerialOnData)
            //    {
            //        listBox1.Items.Insert(0, dt.ToString());
            //        textBox1.Text = dt.ToString();
            //    }
            //    else if (opera == operations.GetAllDigitalAndAnalog)
            //    {
            //        IOs io = (IOs)dt;
            //        string displayString = "Analogs:";
            //        for (int i = 0; i < io.analogs.Length; i++)
            //        {
            //            displayString += io.analogs[i].ToString();
            //            if (i < io.analogs.Length - 1)
            //            {
            //                displayString += ",";
            //            }
            //            else
            //            {
            //                displayString += " Digitals:";
            //            }
            //        }
            //        for (int i = 0; i < io.digitals.Length; i++)
            //        {
            //            displayString += io.digitals[i].ToString();
            //            if (i < io.digitals.Length - 1)
            //            {
            //                displayString += ",";
            //            }
            //        }
            //        listBox1.Items.Insert(0, displayString);
            //    }
            //}
            ////实例化DYCOM消息解码器
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

        private void button1_Click_1(object sender, EventArgs e)
        {
            //poxy.Send(smart.StartSerial(19200));
            //var dd = mapper.ToJson(new msg() { nid = 1, hid = 1, op = 0, bk = 1, val = new int[] { 9050, 4450, 600, 550, 550, 550, 600, 550, 550, 1650, 600, 550, 600, 550, 550, 550, 600, 550, 600, 1600, 600, 1700, 550, 1700, 550, 550, 600, 500, 600, 1700, 550, 1650, 600, 1650, 650, 1600, 600, 1650, 600 } });
            //var cc = dd.Length;
            //poxy.Send(smart.SendSerial(dd));
            //poxy.SendToSmart(smart.SendIRRemote("9050,4450,600,550,550,550,600,550,550,1650,600,550,600,550,550,550,600,550,600,1600,600,1700,550,1700,550,550,600,500,600,1700,550,1650,600,1650,650,1600,600,1650,600,550,550,550,600,550,550,550,600,1650,600,550,550,550,600,550,550,1650,600,1700,550,1650,600,1650,650,500,550,1700,600"));
            //poxy.SendToSmart(smart.runIRReader(7));
            //poxy.SendToSmart(smart.StartSerial(19200));
            //var cc = smart.SendIRRemote("9050,4450,600,500,600,1650,600,500,600,500,600,500,600,550,600,500,600,500,600,1650,600,500,600,1650,600,1650,600,1650,600,1650,600,500,600,1700,600,500,600,500,600,1650,600,500,600,1650,600,500,600,500,600,500,600,1650,600,1700,600,500,600,1650,600,500,600,1650,600,1650,600,1650,600");
            ////////var dd = smart.runIRReader(4);
            ////////var cc = smart.SendSerial(dd);
            ////poxy.SendToSmart(smart.SendSerial(smart.GetAllDigitalAndAnalog(1)));
            //List<byte[]> orders = smart.IrSpiler(cc, 100);
            //foreach (var item in orders)
            //{
            //    poxy.SendToSmart(smart.SendSerial(item));
            //    System.Threading.Thread.Sleep(50);
            //}    
            //发送Textbox1的内容到服务端
            //这里消息体本身是操作符+Textbox1的内容,使用UTF8编码，读取的时候也一样要按这个消息体顺序进行读取消息
            //DYWriter.Merge是合并DYWriter.GetDYBytes消息之用
            poxy.Send(DYWriter.Merge(DYWriter.GetDYBytes((int)op.login), 
                DYWriter.GetDYBytes(textBox1.Text, System.Text.Encoding.UTF8)));
        }

    }
    //操作符定义,可以自由添加删除操作符的定义
    public enum op
    {
        login
    }
}