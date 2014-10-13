using System.Text;
using System.Collections.Generic;
using System;

namespace DYsmartWP7
{
    /// <summary>
    /// DYsmartAPI
    /// </summary>
    public class DYsmart
    {
        /// <summary>
        /// DYsmart安全口令
        /// </summary>
        public string pwd = "";
        /// <summary>
        /// DYsmartAPI
        /// </summary>
        /// <remarks>DYsmart相关操作编程接口</remarks>
        /// <param name="Pwd">硬件安全口令</param>
        public DYsmart(string Pwd)
        {
            pwd = Pwd;
        }
        /// <summary>
        /// 初始化操作
        /// </summary>
        /// <remarks>DYsmart所有基础信息初始化</remarks>
        /// <param name="DYid">硬件设备ID</param>
        public byte[] resetAll(string DYid)
        {
            return DYWriter.Merge(DYWriter.GetDYBytes((int)operations.ResetAll), DYWriter.GetDYBytes(DYid, Encoding.UTF8));
        }
        /// <summary>
        /// 初始化操作(子系统)
        /// </summary>
        /// <remarks>DYsmart所有基础信息初始化(子系统)</remarks>
        /// <param name="DYid">硬件设备ID</param>
        public byte[] resetAllAclient(string DYid) 
        {
            var cc = DYWriter.Merge(DYWriter.GetDYBytes(DYid,Encoding.UTF8), DYWriter.GetDYBytes((int)operations.ResetAll), 
                Encoding.UTF8.GetBytes(new char[] { ':', '|' }));
            return DYWriter.Merge(DYWriter.GetDYBytes(cc.Length + 4), cc);
        }
        /// <summary>
        /// 修改安全口令
        /// </summary>
        /// <remarks>修改安全口令</remarks>
        /// <param name="newPwd">新安全口令</param>
        public byte[] setPwd(string newPwd)
        {
            return DYWriter.Merge(DYWriter.GetDYBytes((int)operations.SetPwd), DYWriter.GetDYBytes(pwd, Encoding.UTF8),
                DYWriter.GetDYBytes(newPwd, Encoding.UTF8));
        }
        /// <summary>
        /// 修改安全口令
        /// </summary>
        /// <remarks>修改安全口令</remarks>
        /// <param name="oldPwd">当前安全口令</param>
        /// <param name="newPwd">新安全口令</param>
        public byte[] setPwd(string oldPwd, string newPwd)
        {
            return DYWriter.Merge(DYWriter.GetDYBytes((int)operations.SetPwd), DYWriter.GetDYBytes(oldPwd, Encoding.UTF8),
         DYWriter.GetDYBytes(newPwd, Encoding.UTF8));
        }
        /// <summary>
        /// 修改安全口令(子系统)
        /// </summary>
        /// <remarks>修改安全口令(子系统)</remarks>
        /// <param name="DYid">硬件设备ID</param>
        /// <param name="oldPwd">当前安全口令</param>
        /// <param name="newPwd">新安全口令</param>
        public byte[] setPwd(string DYid, string oldPwd, string newPwd)
        {
            var cc = DYWriter.Merge(DYWriter.GetDYBytes(DYid,Encoding.UTF8), setPwd(oldPwd, newPwd), 
                Encoding.UTF8.GetBytes(new char[] { ':', '|' }));
            return DYWriter.Merge(DYWriter.GetDYBytes(cc.Length + 4), cc);
        }
        /// <summary>
        /// 启动串口通信
        /// </summary>
        /// <remarks>启动串口通信</remarks>
        /// <param name="baud">比特率(9600,19200等等...)</param>
        public byte[] StartSerial(int baud)
        {
            return DYWriter.Merge(DYWriter.GetDYBytes((int)operations.BeginSerial), DYWriter.GetDYBytes(pwd, Encoding.UTF8),
                DYWriter.GetDYBytes(baud));
        }
        /// <summary>
        /// 发送串口消息
        /// </summary>
        /// <remarks>发送串口消息</remarks>
        /// <param name="data">要发送的消息内容</param>
        public byte[] SendSerial(byte[] data)
        {
            return DYWriter.Merge(DYWriter.GetDYBytes((int)operations.SerialSend), DYWriter.GetDYBytes(pwd, Encoding.UTF8), 
                DYWriter.GetDYBytes(data));
        }
        /// <summary>
        /// 分离过长消息体
        /// </summary>
        /// <remarks>分离过长消息体</remarks>
        /// <param name="data">要发送的消息内容</param>
        /// <param name="spiler">切分因子(按最大此值切分消息)</param>
        public List<byte[]> IrSpiler(byte[] data, int spiler)
        {
            List<byte[]> orders = new List<byte[]>();
            int count = data.Length / spiler;
            int asc = data.Length % spiler;
            for (int i = 0; i < count; i++)
            {
                byte[] bts = new byte[spiler];
                Buffer.BlockCopy(data, i * spiler, bts, 0, spiler);
                orders.Add(bts);
            }
            if (asc != 0)
            {
                byte[] bts = new byte[asc];
                Buffer.BlockCopy(data, count * spiler, bts, 0, asc);
                orders.Add(bts);
            }
            return orders;
        }
        /// <summary>
        /// 设置端口特性
        /// </summary>
        /// <remarks>设置端口特性</remarks>
        /// <param name="pin">端口编号</param>
        /// <param name="mode">端口特性</param>
        public byte[] pinMode(int pin, pMode mode)
        {
            return DYWriter.Merge(DYWriter.GetDYBytes((int)operations.PinMode), DYWriter.GetDYBytes(pwd, Encoding.UTF8),
                DYWriter.GetDYBytes((byte)pin), DYWriter.GetDYBytes((byte)mode));
        }
        /// <summary>
        /// 设置端口特性(子系统)
        /// </summary>
        /// <remarks>设置端口特性(子系统)</remarks>
        /// <param name="DYid">硬件设备ID</param>
        /// <param name="pin">端口编号</param>
        /// <param name="mode">端口特性</param>
        public byte[] pinMode(string DYid, int pin, pMode mode)
        {
            var cc = DYWriter.Merge(DYWriter.GetDYBytes(DYid,Encoding.UTF8), pinMode(pin,mode), 
                Encoding.UTF8.GetBytes(new char[] { ':', '|' }));
            return DYWriter.Merge(DYWriter.GetDYBytes(cc.Length + 4), cc);
        }
        /// <summary>
        /// 设置数字端口
        /// </summary>
        /// <remarks>设置数字端口</remarks>
        /// <param name="pin">端口编号</param>
        /// <param name="mode">端口特性</param>
        public byte[] SetDigital(int pin, dMode mode)
        {
            return DYWriter.Merge(DYWriter.GetDYBytes((int)operations.SetDigital), DYWriter.GetDYBytes(pwd, Encoding.UTF8),
                DYWriter.GetDYBytes((byte)pin), DYWriter.GetDYBytes((byte)mode));
        }
        /// <summary>
        /// 设置数字端口(子系统)
        /// </summary>
        /// <remarks>设置数字端口(子系统)</remarks>
        /// <param name="DYid">硬件设备ID</param>
        /// <param name="pin">端口编号</param>
        /// <param name="mode">端口特性</param>
        public byte[] SetDigital(string DYid, int pin, dMode mode)
        {
            var cc = DYWriter.Merge(DYWriter.GetDYBytes(DYid,Encoding.UTF8), SetDigital(pin, mode), 
                Encoding.UTF8.GetBytes(new char[] { ':', '|' }));
            return DYWriter.Merge(DYWriter.GetDYBytes(cc.Length + 4), cc);
        }
        /// <summary>
        /// 设置脉宽调制
        /// </summary>
        /// <remarks>设置脉宽调制</remarks>
        /// <param name="pin">PWM端口编号</param>
        /// <param name="value">脉宽值(0-255)</param>
        public byte[] SetPWM(int pin, byte value)
        {
            return DYWriter.Merge(DYWriter.GetDYBytes((int)operations.SetPWM), DYWriter.GetDYBytes(pwd, Encoding.UTF8),
                DYWriter.GetDYBytes((byte)pin), DYWriter.GetDYBytes(value));
        }
        /// <summary>
        /// 设置脉宽调制(子系统)
        /// </summary>
        /// <remarks>设置脉宽调制(子系统)</remarks>
        /// <param name="DYid">硬件设备ID</param>
        /// <param name="pin">PWM端口编号</param>
        /// <param name="value">脉宽值(0-255)</param>
        public byte[] SetPWM(string DYid, int pin, byte value)
        {
            var cc = DYWriter.Merge(DYWriter.GetDYBytes(DYid,Encoding.UTF8), SetPWM(pin, value), 
                Encoding.UTF8.GetBytes(new char[] { ':', '|' }));
            return DYWriter.Merge(DYWriter.GetDYBytes(cc.Length + 4), cc);
        }
        /// <summary>
        /// 设置伺服电机
        /// </summary>
        /// <remarks>设置伺服电机</remarks>
        /// <param name="pin">伺服务电机端口编号</param>
        /// <param name="pot">旋转角度(0-180度)</param>
        public byte[] SetServor(int pin, byte pot)
        {
            return DYWriter.Merge(DYWriter.GetDYBytes((int)operations.SetServo), DYWriter.GetDYBytes(pwd, Encoding.UTF8),
                DYWriter.GetDYBytes((byte)pin), DYWriter.GetDYBytes(pot));
        }
        /// <summary>
        /// 设置伺服电机(子系统)
        /// </summary>
        /// <remarks>设置伺服电机(子系统)</remarks>
        /// <param name="DYid">硬件设备ID</param>
        /// <param name="pin">伺服务电机端口编号</param>
        /// <param name="pot">旋转角度(0-180度)</param>
        public byte[] SetServor(string DYid, int pin, byte pot)
        {
            var cc = DYWriter.Merge(DYWriter.GetDYBytes(DYid,Encoding.UTF8), SetServor(pin, pot), 
                Encoding.UTF8.GetBytes(new char[] { ':', '|' }));
            return DYWriter.Merge(DYWriter.GetDYBytes(cc.Length + 4), cc);
        }
        /// <summary>
        /// 设置四线步进电机
        /// </summary>
        /// <remarks>设置四线步进电机</remarks>
        /// <param name="steps">步进基数</param>
        /// <param name="pin1">组1端口1</param>
        /// <param name="pin2">组1端口2</param>
        /// <param name="pin3">组2端口1</param>
        /// <param name="pin4">组2端口2</param>
        /// <param name="speed">转频</param>
        /// <param name="doStep">步进值</param>
        public byte[] SetSteper(int steps, byte pin1, byte pin2, byte pin3, byte pin4, int speed, int doStep)
        {
            return DYWriter.Merge(DYWriter.GetDYBytes((int)operations.SetSteper), DYWriter.GetDYBytes(pwd, Encoding.UTF8),
                DYWriter.GetDYBytes(steps), DYWriter.GetDYBytes(pin1), DYWriter.GetDYBytes(pin2), DYWriter.GetDYBytes(pin3),
                DYWriter.GetDYBytes(pin4), DYWriter.GetDYBytes(speed), DYWriter.GetDYBytes(doStep));
        }
        /// <summary>
        /// 设置四线步进电机
        /// </summary>
        /// <remarks>设置四线步进电机</remarks>
        /// <param name="DYid">硬件设备ID</param>
        /// <param name="steps">步进基数</param>
        /// <param name="pin1">组1端口1</param>
        /// <param name="pin2">组1端口2</param>
        /// <param name="pin3">组2端口1</param>
        /// <param name="pin4">组2端口2</param>
        /// <param name="speed">转频</param>
        /// <param name="doStep">步进值</param>
        public byte[] SetSteper(string DYid, int steps, byte pin1, byte pin2, byte pin3, byte pin4, int speed, int doStep)
        {
            var cc = DYWriter.Merge(DYWriter.GetDYBytes(DYid,Encoding.UTF8), SetSteper(steps, pin1,pin2,pin3,pin4,speed,doStep), 
                Encoding.UTF8.GetBytes(new char[] { ':', '|' }));
            return DYWriter.Merge(DYWriter.GetDYBytes(cc.Length + 4), cc);
        }
        /// <summary>
        /// 取得18B20湿度传感器值
        /// </summary>
        /// <remarks>取得18B20湿度传感器值</remarks>
        /// <param name="pin">18B20湿度传感器端口编号</param>
        public byte[] Get18B20(byte pin)
        {
            return DYWriter.Merge(DYWriter.GetDYBytes((int)operations.Get18B20), DYWriter.GetDYBytes(pwd, Encoding.UTF8),
                DYWriter.GetDYBytes(pin));
        }
        /// <summary>
        /// 取得18B20湿度传感器值(子系统)
        /// </summary>
        /// <remarks>取得18B20湿度传感器值(子系统)</remarks>
        /// <param name="DYid">硬件设备ID</param>
        /// <param name="pin">18B20湿度传感器端口编号</param>
        public byte[] Get18B20(string DYid, byte pin)
        {
            var cc = DYWriter.Merge(DYWriter.GetDYBytes(DYid,Encoding.UTF8), Get18B20(pin), 
                Encoding.UTF8.GetBytes(new char[] { ':', '|' }));
            return DYWriter.Merge(DYWriter.GetDYBytes(cc.Length + 4), cc);
        }
        /// <summary>
        /// 取得数字端口值
        /// </summary>
        /// <remarks>取得数字端口值</remarks>
        /// <param name="pin">数字端口编号</param>
        public byte[] GetDigital(byte pin)
        {
            return DYWriter.Merge(DYWriter.GetDYBytes((int)operations.GetDigital), DYWriter.GetDYBytes(pwd, Encoding.UTF8),
                DYWriter.GetDYBytes(pin));
        }
        /// <summary>
        /// 取得数字端口值(子系统)
        /// </summary>
        /// <remarks>取得数字端口值(子系统)</remarks>
        /// <param name="DYid">硬件设备ID</param>
        /// <param name="pin">数字端口编号</param>
        public byte[] GetDigital(string DYid, byte pin)
        {
            var cc = DYWriter.Merge(DYWriter.GetDYBytes(DYid,Encoding.UTF8), GetDigital(pin), 
                Encoding.UTF8.GetBytes(new char[] { ':', '|' }));
            return DYWriter.Merge(DYWriter.GetDYBytes(cc.Length + 4), cc);
        }
        /// <summary>
        /// 取得模拟端口值
        /// </summary>
        /// <remarks>取得模拟端口值</remarks>
        /// <param name="pin">模拟端口编号</param>
        public byte[] GetAnalog(byte pin)
        {
            return DYWriter.Merge(DYWriter.GetDYBytes((int)operations.GetAnalog), DYWriter.GetDYBytes(pwd, Encoding.UTF8),
                DYWriter.GetDYBytes(pin));
        }
        /// <summary>
        /// 取得模拟端口值(子系统)
        /// </summary>
        /// <remarks>取得模拟端口值(子系统)</remarks>
        /// <param name="DYid">硬件设备ID</param>
        /// <param name="pin">模拟端口编号</param>
        public byte[] GetAnalog(string DYid, byte pin)
        {
            var cc = DYWriter.Merge(DYWriter.GetDYBytes(DYid,Encoding.UTF8), GetAnalog(pin), 
                Encoding.UTF8.GetBytes(new char[] { ':', '|' }));
            return DYWriter.Merge(DYWriter.GetDYBytes(cc.Length + 4), cc);
        }
        /// <summary>
        /// 取得所有数字模拟端口值
        /// </summary>
        /// <remarks>取得所有数字模拟端口值</remarks>
        /// <param name="digitalMax">硬件最大数字接口编号</param>
        /// <param name="analogMax">硬件最大模拟接口编号</param>
        public byte[] GetAllDigitalAndAnalog(int digitalMax,int analogMax)
        {
            return DYWriter.Merge(DYWriter.GetDYBytes((int)operations.GetAllDigitalAndAnalog), 
                DYWriter.GetDYBytes(pwd, Encoding.UTF8),DYWriter.GetDYBytes(digitalMax),DYWriter.GetDYBytes(analogMax));
        }
        /// <summary>
        /// 取得所有数字模拟端口值(子系统)
        /// </summary>
        /// <remarks>取得所有数字模拟端口值(子系统)</remarks>
        /// <param name="DYid">硬件设备ID</param>
        /// <param name="digitalMax">硬件最大数字接口编号</param>
        /// <param name="analogMax">硬件最大模拟接口编号</param>
        public byte[] GetAllDigitalAndAnalog(string DYid, int digitalMax, int analogMax)
        {
            var cc = DYWriter.Merge(DYWriter.GetDYBytes(DYid,Encoding.UTF8), GetAllDigitalAndAnalog(digitalMax,analogMax), 
                Encoding.UTF8.GetBytes(new char[] { ':', '|' }));
            return DYWriter.Merge(DYWriter.GetDYBytes(cc.Length + 4), cc);
        }
        /// <summary>
        /// 启动红外线遥控器学习功能
        /// </summary>
        /// <remarks>启动红外线遥控器学习功能</remarks>
        /// <param name="pin">遥控器红外线接收器所在端口编号</param>
        public byte[] runIRReader(byte pin)
        {
            return DYWriter.Merge(DYWriter.GetDYBytes((int)operations.runIRReader), DYWriter.GetDYBytes(pwd, Encoding.UTF8),
                DYWriter.GetDYBytes(pin));
        }
        /// <summary>
        /// 产生红外线遥控信号指令
        /// </summary>
        /// <remarks>产生红外线遥控信号指令</remarks>
        /// <param name="IRorder">遥控器红外线指令</param>
        public byte[] SendIRRemote(string IRorder)
        {
            string[] orders = IRorder.Split(',');
            byte[] irod = new byte[orders.Length * 4];
            for (int i = 0; i < orders.Length; i++)
            {
                byte[] od = DYWriter.GetDYBytes(int.Parse(orders[i]));
                irod[i * 4] = od[0];
                irod[i * 4 + 1] = od[1];
                irod[i * 4 + 2] = od[2];
                irod[i * 4 + 3] = od[3];
            }
            return DYWriter.Merge(DYWriter.GetDYBytes((int)operations.SendIRRemote), DYWriter.GetDYBytes(pwd, Encoding.UTF8),
                DYWriter.GetDYBytes(orders.Length), irod);
        }
        /// <summary>
        /// 产生红外线遥控信号指令(子系统)
        /// </summary>
        /// <remarks>产生红外线遥控信号指令(子系统)</remarks>
        /// <param name="DYid">硬件设备ID</param>
        /// <param name="IRorder">遥控器红外线指令</param>
        public byte[] SendIRRemote(string DYid, string IRorder)
        {
            var cc = DYWriter.Merge(DYWriter.GetDYBytes(DYid,Encoding.UTF8), SendIRRemote(IRorder), 
                Encoding.UTF8.GetBytes(new char[] { ':', '|' }));
            return DYWriter.Merge(DYWriter.GetDYBytes(cc.Length + 4), cc);
        }
    }
}
