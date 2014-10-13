using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Text;
using DYComSLClient;

namespace testClient
{
    public partial class MainPage : UserControl
    {
        Storyboard looper;
        ClientPoxy server;
        public MainPage()
        {
            InitializeComponent();
            looper = new Storyboard();
            looper.Duration = new Duration(TimeSpan.FromMilliseconds(1));
            looper.Completed += new EventHandler(looper_Completed);
            Loaded += new RoutedEventHandler(MainPage_Loaded);
            button1.Click += new RoutedEventHandler(button1_Click);
            button2.Click += new RoutedEventHandler(button2_Click);
        }

        void button2_Click(object sender, RoutedEventArgs e)
        {
            looper.Stop();
        }
        List<ClientPoxy> clients = new List<ClientPoxy>();
        void button1_Click(object sender, RoutedEventArgs e)
        {
            //looper.Begin();
            //asyncOper = AsyncOperationManager.CreateOperation(null);
            server.Send(DYWriter.Merge(DYWriter.GetDYBytes((int)op.login),
                DYWriter.GetDYBytes(textBox1.Text, System.Text.Encoding.UTF8)));
            //for (int i = 0; i < 200; i++)
            //{
            //    System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback((obj) =>
            //    {
            //        clients.Add(new ClientPoxy("192.168.0.2", 4510, 1));
            //        //if (i % 100 == 0)
            //        //{
            //        //System.Threading.Thread.Sleep(50);
            //        //}
            //    }), null);
            //}
            //DateTime t = DateTime.Now;
            //for (int i = 0; i < 1000; i++)
            //{
            //    server.Send(DYWriter.Merge(DYWriter.GetDYBytes((int)op.login), DYWriter.GetDYBytes(string.Format("{0}sjdfksldkfjlkasdjfklajsdklfjkldjfklasjdfkj时要苦地地 地地 右枯地顶替要夺顶替顶替在需要地厅ksdfkj sdkf ###@#@#@#@#@$$#%#%$#", i), Encoding.UTF8)));
            //    //System.Threading.Thread.Sleep(20);
            //}
            //TimeSpan span = DateTime.Now - t;
            //Console.WriteLine(span.ToString());
        }

        void looper_Completed(object sender, EventArgs e)
        {
            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback((obj) =>
            {
                server.Send(DYWriter.Merge(DYWriter.GetDYBytes((int)op.login), DYWriter.GetDYBytes(DateTime.Now)));
            }), null);

            looper.Begin();
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            server = new ClientPoxy("127.0.0.1", 4510, 256);
            server.onData = new OnData(ondata);

           // List<string> test = new List<string>();
           // test.Add("kdjfkdjf");
           // test.Add("kdjfdjkf");
           //DYJson.JsonMapper mp = new DYJson.JsonMapper();
           // var dd = mp.ToJson(test);
           // var ss = mp.ToObject<List<string>>(dd);
        }

        void ondata(byte[] data)
        {
            DYReader read = new DYReader(data);
            int type;
            if (!read.ReadInt32(out type))
            {
                return;
            }
            op opera = (op)type;

            if (opera == op.login)
            {
                string date;
                if (read.ReadString(out date, System.Text.Encoding.UTF8))
                {
                    //listBox1.Items.Insert(0, DateTime.Now.Subtract(date).TotalMilliseconds.ToString());
                    listBox1.Items.Insert(0, date);
                }
            }

        }

    }
    public enum op
    {
        login
    }

}
