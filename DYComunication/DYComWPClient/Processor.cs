
namespace DYComWPClient
{
    /// <summary>
    /// 连接通知代理
    /// </summary>
    public delegate void OnConnet(bool message);
    /// <summary>
    /// 消息通知代理
    /// </summary>
    public delegate void OnData(byte[] data);
    /// <summary>
    /// 错误通知代理
    /// </summary>
    public delegate void OnDisConnet(string message);
}
