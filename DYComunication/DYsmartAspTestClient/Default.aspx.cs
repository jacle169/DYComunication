using System;
using DYsmartWinClient;
using DYsmartAspClient;

namespace DYsmartAspTestClient
{
    public partial class Default : System.Web.UI.Page
    {
        DYsmart smart = new DYsmart("123456");
        DYsmartDecoder smartDecoder = new DYsmartDecoder();

        protected void Page_Load(object sender, EventArgs e)
        {
            //var cc = smart.SendIRRemote("9050,4450,600,550,550,550,600,550,550,1650,600,550,600,550,550,550,600,550,600,1600,600,1700,550,1700,550,550,600,500,600,1700,550,1650,600,1650,650,1600,600,1650,600,550,550,550,600,550,550,550,600,1650,600,550,550,550,600,550,550,1650,600,1700,550,1650,600,1650,650,500,550,1700,600");
            //List<byte[]> orders = smart.IrSpiler(cc, 100);
            //foreach (var item in orders)
            //{
            //    smartDecoder.initData(httpClient.Send(smart.SendSerial(item)));
            //    System.Threading.Thread.Sleep(50);
            //}      
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            DYsmartAsp httpClient = new DYsmartAsp("http://192.168.0.254");   
            //初始化Smart回应指令解释器

            smartDecoder.initData(httpClient.Send(smart.pinMode(4, pMode.OUTPUT)));
            operations? opera = smartDecoder.getOperation();
            if (opera.HasValue)
            {
                object dt = smartDecoder.decode(opera.Value);
                if (opera == operations.BeginSerial || opera == operations.ResetAll || opera == operations.SetPwd || opera == operations.BeginSerial || opera == operations.SerialSend
                || opera == operations.PinMode || opera == operations.SetDigital || opera == operations.SetPWM || opera == operations.SetServo
                || opera == operations.SetSteper || opera == operations.runIRReader || opera == operations.SendIRRemote || opera == operations.Get18B20 ||
                    opera == operations.GetDigital || opera == operations.GetAnalog || opera == operations.IRReadOnData)
                {
                    Response.Write(dt.ToString());
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
                    Response.Write(displayString);
                }
            }
        }
    }
}