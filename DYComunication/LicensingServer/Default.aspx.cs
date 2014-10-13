using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DYLicensingServer;
using System.Linq;
using System.Data.SqlClient;
using System.Data.EntityClient;
using System.Configuration;
using System.Xml.Linq;

namespace LicensingServer
{
    public partial class _Default : System.Web.UI.Page
    {
        DYLicensing ls = new DYLicensing();
        manager mg;
        string logpath;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (ls.valiHandle == null)
            {
                logpath = ConfigurationManager.AppSettings["logPath"].ToString();
                mg = new manager();
                ls.valiHandle = new DYLicensing.validationDelegate(validateFunction);
                ls.startLicensing(Page, mg.cryKey);
            }
        }

        EntityConnection createConnection()
        {
            EntityConnectionStringBuilder esb = new EntityConnectionStringBuilder();
            esb.Provider = "System.Data.SqlClient";
            esb.Metadata = @"res://*/Model1.csdl|res://*/Model1.ssdl|res://*/Model1.msl";
            esb.ProviderConnectionString =mg.Decrypt(ConfigurationManager.AppSettings["dycomSqlConString"].ToString());
           return new EntityConnection(esb.ConnectionString);
        }

        void validateFunction(licensingClass data)
        {
            using (dylcEntities de = new dylcEntities(createConnection()))
            {
                if (de.UserKeyTable.Where(k => k.PDKey.Equals(data.key)).Count() > 0)
                {
                    UserKeyTable dt = de.UserKeyTable.First(k => k.PDKey.Equals(data.key));
                    if (dt.IsUsing)
                    {
                        if (dt.IsForever)
                        {
                            ls.SendBack(Page, mg.cryKey, data.tm, data.key, resultSkin.Pass);
                            log(data, "Pass");
                        }
                        else
                        {
                            //有可能因为服务器上的时间不准确而出错
                            if (dt.EnableTime.Subtract(DateTime.Now).TotalSeconds >= 0)
                            {
                                ls.SendBack(Page, mg.cryKey, data.tm, data.key, resultSkin.Pass);
                                log(data, "Pass");
                            }
                            else
                            {
                                ls.SendBack(Page, mg.cryKey, data.tm, data.key, resultSkin.EnableTimeOut);
                                log(data, "EnableTimeOut");
                            }
                        }
                    }
                    else
                    {
                        ls.SendBack(Page, mg.cryKey, data.tm, data.key, resultSkin.NotUsing);
                        log(data, "keyNotUsing");
                    }
                }
                else
                {
                    ls.SendBack(Page, mg.cryKey, data.tm, data.key, resultSkin.KeyError);
                    log(data, "KeyError");
                }
            }
        }

        void log(licensingClass data, string result)
        {
            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(addLog),
        new logData() { licenData = data, result = result });
        }

        void addLog(object obj)
        {
            logData ld = (logData)obj;
            addFileLog(ld.licenData, ld.result);
        }

        void addFileLog(licensingClass data, string result)
        {
            if (!System.IO.File.Exists(Server.MapPath(logpath)))
            {
                createLogFile();
            }
            XDocument xml = XDocument.Load(Server.MapPath(logpath));
            XElement logBit = new XElement("log",
                     new XElement("cpu", data.cpu),
                     new XElement("key", mg.Encrypt(data.key)),
                     new XElement("time", data.tm),
                     new XElement("result", result)
                     );

            xml.Root.Add(logBit);

            xml.Save(Server.MapPath(logpath));

            xml = null;
        }

        void createLogFile()
        {
            XDocument doc = new XDocument(new XElement("root"));
            doc.Save(Server.MapPath(logpath));
        }

    }

    public class logData
    {
        public licensingClass licenData { get; set; }
        public string result { get; set; }
    }
}