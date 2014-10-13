using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;

namespace DYsmartHttpIPrequest
{
    public partial class _default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                logData ld = new logData();
                ld.smartId = Request.QueryString["id"];
                ld.ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                if (ld.smartId != null)
                {
                    addFileLog(ld);
                }
            }
        }

        void addFileLog(logData data)
        {
            if (!System.IO.File.Exists(Server.MapPath("log.xml")))
            {
                createLogFile();
            }
            XDocument xml = XDocument.Load(Server.MapPath("log.xml"));
            XElement logBit = new XElement("log",
                     new XElement("smartId", data.smartId),
                     new XElement("ip", data.ip));

            xml.Root.Add(logBit);

            xml.Save(Server.MapPath("log.xml"));

            xml = null;
        }

        void createLogFile()
        {
            XDocument doc = new XDocument(new XElement("root"));
            doc.Save(Server.MapPath("log.xml"));
        }

    }

    public class logData
    {
        public string smartId { get; set; }
        public string ip { get; set; }
    }
}