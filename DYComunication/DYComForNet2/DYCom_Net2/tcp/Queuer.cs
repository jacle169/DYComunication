using System.Collections.Generic;

namespace DYCom_Net2
{
    /// <summary>
    /// 消息队列
    /// </summary>
    public class Queuer
    {
        #region queue
        /// <summary>
        /// 清空消息队列
        /// </summary>
        /// <remarks>清空消息队列正在排队中的所有消息,调用本方法将会导致消息不发送的可能.</remarks>
        public void ClearQueues()
        {
            outQueue.Clear();
        }

        internal Queue<DYmessage> outQueue = new Queue<DYmessage>(10000);

        internal void addOut(object data)
        {
            outQueue.Enqueue((DYmessage)data);
        }
        #endregion
    }
}
