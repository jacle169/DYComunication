using System.Collections.Concurrent;
using System.Net.Sockets;

namespace DYCom_Net4
{
    internal sealed class SocketAsyncEventArgsPool
    {
        /// <summary>
        /// SocketAsyncEventArgs栈
        /// </summary>
        internal ConcurrentStack<SocketAsyncEventArgs> pool;

        /// <summary>
        /// 初始化SocketAsyncEventArgs池
        /// </summary>
        internal SocketAsyncEventArgsPool()
        {
            pool = new ConcurrentStack<SocketAsyncEventArgs>();
        }

        /// <summary>
        /// 添加一个 SocketAsyncEventArgs
        /// </summary>
        /// <param name="item">SocketAsyncEventArgs instance to add to the pool.</param>
        internal void Push(SocketAsyncEventArgs item)
        {
            if (item != null)
            {
                pool.Push(item);
            }
        }
    }
}
