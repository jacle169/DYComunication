
namespace DYsmartWinClient
{
    /// <summary>
    /// 端口集数据定义
    /// </summary>
    /// <remarks>端口集数据定义</remarks>
    public class IOs
    {
        public int[] analogs { get; set; }
        public byte[] digitals { get; set; }
    }

    public enum operations
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <remarks>初始化</remarks>
        ResetAll,
        /// <summary>
        /// 修改安全口令
        /// </summary>
        /// <remarks>修改安全口令</remarks>
        SetPwd,
        /// <summary>
        /// 启动串口通信
        /// </summary>
        /// <remarks>启动串口通信</remarks>
        BeginSerial,
        /// <summary>
        /// 发送串口消息
        /// </summary>
        /// <remarks>发送串口消息</remarks>
        SerialSend,
        /// <summary>
        /// 设置端口特性
        /// </summary>
        /// <remarks>设置端口特性</remarks>
        PinMode,
        /// <summary>
        /// 设置数字端口操作
        /// </summary>
        /// <remarks>设置数字端口操作</remarks>
        SetDigital,
        /// <summary>
        /// 设置脉宽调频操作
        /// </summary>
        /// <remarks>设置脉宽调频操作</remarks>
        SetPWM,
        /// <summary>
        /// 伺服电机操作
        /// </summary>
        /// <remarks>伺服电机操作</remarks>
        SetServo,
        /// <summary>
        /// 步进电机操作
        /// </summary>
        /// <remarks>步进电机操作</remarks>
        SetSteper,
        /// <summary>
        /// 18B20温度传感器操作
        /// </summary>
        /// <remarks>18B20温度传感器操作</remarks>
        Get18B20,
        /// <summary>
        /// 取得数字端口值操作
        /// </summary>
        /// <remarks>取得数字端口值操作</remarks>
        GetDigital,
        /// <summary>
        /// 取得模拟端口值操作
        /// </summary>
        /// <remarks>取得模拟端口值操作</remarks>
        GetAnalog,
        /// <summary>
        /// 取得所有数字模拟端口值操作
        /// </summary>
        /// <remarks>取得所有数字模拟端口值操作</remarks>
        GetAllDigitalAndAnalog,
        /// <summary>
        /// 启动遥控器红外线学习功能
        /// </summary>
        /// <remarks>启动遥控器红外线学习功能</remarks>
        runIRReader,
        /// <summary>
        /// 红外红学习数据通知
        /// </summary>
        /// <remarks>红外红学习数据通知</remarks>
        IRReadOnData,
        /// <summary>
        /// 发送遥控红外线信号指令
        /// </summary>
        /// <remarks>发送遥控红外线信号指令</remarks>
        SendIRRemote,
        /// <summary>
        /// 串口消息到达通知
        /// </summary>
        /// <remarks>串口消息到达通知</remarks>
        SerialOnData
    }

    /// <summary>
    /// 端口数字操作类型
    /// </summary>
    /// <remarks>端口数字操作类型</remarks>
    public enum dMode
    {
        /// <summary>
        /// 端口数字操作
        /// </summary>
        /// <remarks>将数字端口设置为高电位</remarks>
        LOW,
        /// <summary>
        /// 端口数字操作
        /// </summary>
        /// <remarks>将数字端口设置为低电位</remarks>
        HIGH
    }
    /// <summary>
    /// 端口特性类型
    /// </summary>
    /// <remarks>端口特性类型</remarks>
    public enum pMode
    {
        /// <summary>
        /// 端口特性
        /// </summary>
        /// <remarks>将端口设置为输入装置模式</remarks>
        INPUT,
        /// <summary>
        /// 端口特性
        /// </summary>
        /// <remarks>将端口设置为输出装置模式</remarks>
        OUTPUT
    }
}
