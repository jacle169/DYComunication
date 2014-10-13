using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DYsmartWinClient;
using DYsmartAspClient;
//********************************
//DYsmart asp.net sample
//http://smart.dy2com.com
//********************************
namespace DYsmartAspSample
{
    public partial class _default : System.Web.UI.Page
    {
        //实例化DYsmart asp.net 通信器
        DYsmart smart = new DYsmart("123456");
        //实例化DYsmart指令解释器
        DYsmartDecoder smartDecoder = new DYsmartDecoder();

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            //实例化DYsmart通信器,参数是DYsmart硬件的公网或内网IP地址
            DYsmartAsp httpClient = new DYsmartAsp("http://192.168.0.254");
            
            //发送设置端口特性指令,设置第四个数字端口为
            var result = httpClient.Send(smart.pinMode(4, pMode.OUTPUT));

            //初始化Smart操作结果指令解释器
            smartDecoder.initData(result);

            //取得操作结果操作符
            operations? opera = smartDecoder.getOperation();

            //跟据不同操作符解释结果并显示于页面
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