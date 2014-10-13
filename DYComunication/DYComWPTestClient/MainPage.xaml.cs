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

namespace DYComWPTestClient
{
    public partial class MainPage : PhoneApplicationPage
    {
        ClientPoxy server;
        
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(MainPage_Loaded);
            bt_send.Click += new RoutedEventHandler(bt_send_Click);
        }

        void bt_send_Click(object sender, RoutedEventArgs e)
        {
            if (tb_msg.Text != string.Empty)
            {
                server.Send(DYWriter.Merge(DYWriter.GetDYBytes((int)op.login),
                  DYWriter.GetDYBytes(tb_msg.Text, System.Text.Encoding.UTF8)));
            }
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            server = new ClientPoxy("192.168.0.2", 4510, 256);
            server.onConnet = new OnConnet(onconect);
            server.onData = new OnData(ondata);
        }

        void onconect(bool result)
        {
            lb_msgShow.Items.Insert(0, result.ToString());
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
                    lb_msgShow.Items.Insert(0, date);
                }
            }

        }

    }
    public enum op
    {
        login
    }
}