using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KeyCreater
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            bt_load.Click += new RoutedEventHandler(bt_load_Click);
            dg_userTable.SelectionChanged += new SelectionChangedEventHandler(dg_userTable_SelectionChanged);
        }

        void dg_userTable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dg_userTable.SelectedItems.Count == 1)
            {

            }
        }
        dylcEntities de;
        DYCom_Net4.JsonMapper mapper = new DYCom_Net4.JsonMapper();
        void bt_load_Click(object sender, RoutedEventArgs e)
        {
            //de = new dylcEntities();
            //dg_userTable.ItemsSource = de.UserKeyTable.Select(u => u);

            //UserKeyTable ut = new UserKeyTable();
            //ut.EnableTime = DateTime.Now.AddSeconds(10);
            //ut.IsForever = false;
            //ut.IsUsing = true;
            //ut.PDKey = "jacobkey";
            ////ut.RegTime = DateTime.Now;
            //ut.UserId = "jac";
            //ut.UserMail = "jacob@dd.com";
            //ut.UserMobile = "19245698857";
            //ut.UserQQ = "343443434";
            mydata ut = new mydata();
            ut.userid = "jacob";
            ut.regtime = null;
            string json = mapper.ToJson(ut);
            var dt = mapper.ToObject<mydata>(json);

            //de.AddToUserKeyTable(ut);
            //de.SaveChanges();
            //MessageBox.Show("ok");
        }
    }

    public class mydata
    {
        public string userid { get; set; }
        public DateTime? regtime { get; set; }
    }
}
