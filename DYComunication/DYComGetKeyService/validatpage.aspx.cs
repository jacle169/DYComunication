using System;
using System.Collections.Generic;
using System.Linq;

using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.EntityClient;
using System.Configuration;

namespace DYComGetKeyService
{
    public partial class validatpage : System.Web.UI.Page
    {
        static manager mg = new manager();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var v = Request.QueryString["v"];
                if (v != null)
                {
                    var d = mg.Decrypt(v.Replace("%2B", "+"));
                    if (d != null)
                    {
                        string[] dt = d.Split(',');
                        if (dt.Length == 3)
                        {
                            DateTime postTime = new DateTime(long.Parse(dt[2]));
                            if (DateTime.Now.Subtract(postTime).TotalDays <= 3)
                            {
                                var key = Guid.NewGuid().ToString();
                                using (dylcEntities de = new dylcEntities(mg.createConnection()))
                                {
                                    var m = dt[1];
                                    if (de.UserKeyTable.Where(k => k.UserMail.Equals(m)).Count() == 0)
                                    {
                                        UserKeyTable ut = new UserKeyTable();
                                        ut.EnableTime = DateTime.Now.AddDays(1000);
                                        ut.IsForever = false;
                                        ut.IsUsing = true;
                                        ut.PDKey = key;
                                        ut.RegTime = DateTime.Now;
                                        ut.UserId = dt[0];
                                        ut.UserMail = dt[1];
                                        de.AddToUserKeyTable(ut);
                                        de.SaveChanges();
                                        tb_msg.Text = "您的密钥是: " + key;
                                    }
                                    else
                                    {
                                        tb_msg.Text = "此邮箱已经申请过密钥";
                                    }
                                }
                            }
                            else
                            {
                                tb_msg.Text = "邮件已经过期";
                            }
                        }
                    }
                }
            }
        }



    }
}