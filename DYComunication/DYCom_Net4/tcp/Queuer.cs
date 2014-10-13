using System.Collections.Generic;

namespace DYCom_Net4
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
        /// <remarks>消息队列正在排除中的消息完部清空</remarks>
        public void ClearQueues()
        {
            outQueue.Clear();
        }

        internal Queue<DYmessage> outQueue = new Queue<DYmessage>(10000);
        object obj = new object();
        internal void addOut(object data)
        {
            lock (obj)
            {
                outQueue.Enqueue((DYmessage)data);
            }
        }
        #endregion
    }
}
