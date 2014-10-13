using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Linq;

namespace DYComGetKeyService
{
    public partial class _Default : System.Web.UI.Page
    {
        static manager mg = new manager();
        SmtpClient client;
        MailMessage smg;
        string smpt, port, mail, pwd;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (client == null)
            {
                try
                {
                    smpt = ConfigurationManager.AppSettings["smpt"].ToString();
                    port = ConfigurationManager.AppSettings["port"].ToString();
                    mail = ConfigurationManager.AppSettings["mail"].ToString();
                    pwd = ConfigurationManager.AppSettings["pwd"].ToString();
                    client = new SmtpClient(smpt, int.Parse(port));
                    client.Credentials = new NetworkCredential(mail, pwd);
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.EnableSsl = false;
                }
                catch (Exception ex)
                {
                    Label3.Text = ex.Message;
                }

            }
        }

        string sendmail(string from, string to, string sub, string body)
        {
            try
            {
                smg = new MailMessage(from, to, sub, body);
                smg.BodyEncoding = Encoding.UTF8;
                smg.IsBodyHtml = true;
                client.Send(smg);
                smg.Dispose();
                return "ok";
            }
            catch (Exception e)
            {
                return e.Message.ToString();
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            if (ConfigurationManager.AppSettings["power"].ToString() == "on")
            {
                if (isEmail(TextBox2.Text))
                {
                    if (isUserId(TextBox1.Text))
                    {
                        if (Session["vip"].ToString() == "no" && Session["vip"].ToString() != GetCustomerIP())
                        {
                            Session["vip"] = GetCustomerIP();
                            var result = sendmail(mail, TextBox2.Text, "DYCOM验证邮件", getVmailbody(TextBox1.Text, TextBox2.Text));
                            if (result == "ok")
                            {
                                Label3.Text = "操作成功! 您的DYCOM密钥和后续指导将会发送到您的电子邮箱！请注意查收";
                            }
                            else { Label3.Text = result; }
                        }
                        else { Label3.Text = "你已提交过申请了，请登陆您的邮箱进行相关操作."; }
                    }
                    else { Label3.Text ="户名格式错误[只能使用英文字母和数字组成]"; }
                }
                else { Label3.Text ="电子邮件地址错误"; }
            }
            else { Label3.Text ="功能目前临时关闭，更多资讯请联系http://dy2com.com"; }
        }

        public string GetCustomerIP()
        {
            string CustomerIP = "";
            if (HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null)
                CustomerIP = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
            else if (HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"] != null)
                CustomerIP = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString();
            return CustomerIP;
        }

        string geturl()
        {
            string url = ConfigurationManager.AppSettings["vpage"].ToString() + "?v=";
            string dt = TextBox1.Text + "," + TextBox2.Text + "," + DateTime.Now.Ticks.ToString();
            return url + mg.Encrypt(dt).Replace("+", "%2B");
        }

        string getVmailbody(string userName, string userMail)
        {
            return string.Format(@"  <div>
        {0}
&nbsp;先生/小姐<br />
        <br />
        感谢您申请 DY Communication通信套件。&nbsp; 
        <br />
        更多DYCOM资讯请登陆:&nbsp;
        <br />
        http://dy2com.com<br />
        <br />
       请点击以下连接进行申请验证，如果不能点击以下连接请直接复制到浏览器地址栏直接输入进行验证：<br />
        <br />
         {1} <br />
        <br />
        验证通过后，您将会得到DY Communication的产品密钥。<br />
        <br />
        如果您忘记了您的DYCOM产品密钥，请通过以下账号查询您的密钥! 祥情请登陆DYCOM官方网站的授权服务<br />
        <br />
        户名: {0} <br />
        邮箱: {2} <br />
        <br />
        本验证邮件有时效性，有效期自发送之日起三天后将会失效！<br />
        <br />
        本邮件为系统自动发送邮件，不用回复，谢谢合作！<br />
        <br />
         --------------------------------------&nbsp;DYCOM 团队 
        <br />
        <br />
        <img src=""http://dy2com.com/upload/fckeditor/2010-11/30135808.png"" />
        <img src=""http://dy2com.com/images/logo.png"" /></div>", userName, geturl(), userMail);
        }

        string getSmailbody(string userName, string pdkey)
        {
            return string.Format(@"  <div>
        {0}
&nbsp;先生/小姐<br />
        <br />
        感谢您使用 DY Communication通信套件。&nbsp; 
        <br />
        更多DYCOM资讯请登陆:&nbsp;
        <br />
        http://dy2com.com<br />
        <br />
        您的产品密钥是: {1}
        <br />
        本邮件为系统自动发送邮件，不用回复，谢谢合作！<br />
        <br />
         --------------------------------------&nbsp;DYCOM 团队 
        <br />
        <br />
        <img src=""http://dy2com.com/upload/fckeditor/2010-11/30135808.png"" />
        <img src=""http://dy2com.com/images/logo.png"" /></div>", userName, pdkey);
        }

        bool isEmail(string inputEmail)
        {
            string strRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
            Regex re = new Regex(strRegex);
            if (re.IsMatch(inputEmail))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        bool isUserId(string inputEmail)
        {
            string strRegex = @"^([a-zA-Z0-9])+$";
            Regex re = new Regex(strRegex);
            if (re.IsMatch(inputEmail))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            if (isEmail(TextBox2.Text))
            {
                if (isUserId(TextBox1.Text))
                {
                    using (dylcEntities de = new dylcEntities(mg.createConnection()))
                    {
                        if (de.UserKeyTable.Where(k => k.UserMail.Equals(TextBox2.Text)).Count() > 0)
                        {
                            var user = de.UserKeyTable.First(k => k.UserMail.Equals(TextBox2.Text));
                            if (user.UserId.Equals(TextBox1.Text))
                            {
                                //通过邮件发送密钥给用户
                                var result = sendmail(mail, TextBox2.Text, "DYCOM密钥查询邮件",getSmailbody(TextBox1.Text,user.PDKey));
                                if (result == "ok")
                                {
                                    Label3.Text = "操作成功! 您的DYCOM密钥将会发送到您的电子邮箱！请注意查收";
                                }
                                else { Label3.Text = result; }
                            }
                            else { Label3.Text = "你有用户名不正确"; }
                        }
                        else { Label3.Text = "你的邮箱不正确"; }
                    }
                }
                else { Label3.Text = "户名格式错误[只能使用英文字母和数字组成]"; }
            }
            else { Label3.Text = "电子邮件地址错误"; }
        }

    }
}