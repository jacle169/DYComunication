using System.Net.Sockets;
using System.Net;

namespace DYCom_Net4
{
    #region 参数
    /// <summary>
    /// DYSocketCore的连接代理
    /// </summary>
    /// <param name="socketAsync">dysocket实例</param>
    /// <param name="IsConneted">连接结果</param>
    public delegate void ConnectHandler(DYSocket socketAsync, bool IsConneted);

    /// <summary>
    /// DYSocketCore的数据包输入代理
    /// </summary>
    /// <param name="data">输入包</param>
    /// <param name="socketAsync">dysocket实例</param>
    public delegate void OndataHandler(byte[] data, DYSocket socketAsync);

    /// <summary>
    /// DYSocketCore的异常错误,通常是用户断开的代理
    /// </summary>
    /// <param name="message">消息</param>
    /// <param name="socketAsync">dysocket实例</param>
    public delegate void ErrorHandler(string message, DYSocket socketAsync);

    /// <summary>
    /// UserInfo中的异常错误,通常是客户端缓冲区小于消息大小
    /// </summary>
    /// <param name="message">消息</param>
    public delegate void SendErrorHandler(string message);

    /// <summary>
    /// DYComer的连接通知
    /// </summary>
    public delegate void OnConnet(DYSocket e, bool IsConneted);
    /// <summary>
    /// DYComer的消息通知
    /// </summary>
    public delegate void OnData(byte[] data, UserInfo user);
    /// <summary>
    /// DYComer的错误通知
    /// </summary>
    public delegate void OnDisConnet(DYSocket e);
    #endregion

    /// <summary>
    /// DYComSocket实例
    /// </summary>
    public class DYSocket
    {
        /// <summary> 
        /// socket实例
        /// </summary> 
        public SocketAsyncEventArgs SocketArgs { get; set; }
    }

    /// <summary>
    /// 用户静止时间到达通知
    /// </summary>
    /// <param name="SessionId">用户唯一ID标识</param>
    /// <returns>返回Bool值，true为断开此客户端，flash为继续让此客户端连接。</returns>
    public delegate bool OnUserStaticTimeOut(string SessionId);

    /// <summary>
    /// udp消息通知
    /// </summary>
    public delegate void OnUdpData(byte[] data, string IPEndPoint);
}
