using System.Collections.Generic;
using System.Net.Sockets;
using System;

namespace DYCom_Net2
{
    internal sealed class SocketAsyncEventArgsPool
    {
        /// <summary>
        /// SocketAsyncEventArgs栈
        /// </summary>
        Stack<SocketAsyncEventArgs> pool;

        /// <summary>
        /// 初始化SocketAsyncEventArgs池
        /// </summary>
        /// <param name="capacity">最大可能使用的SocketAsyncEventArgs对象.</param>
        internal SocketAsyncEventArgsPool(int capacity)
        {
            this.pool = new Stack<SocketAsyncEventArgs>(capacity);
        }

        /// <summary>
        /// 返回SocketAsyncEventArgs池中的 数量
        /// </summary>
        internal int Count
        {
            get { return this.pool.Count; }
        }

        /// <summary>
        /// 弹出一个SocketAsyncEventArgs
        /// </summary>
        /// <returns>SocketAsyncEventArgs removed from the pool.</returns>
        internal SocketAsyncEventArgs Pop()
        {
            lock (this.pool)
            {
                return this.pool.Pop();
            }
        }

        /// <summary>
        /// 添加一个 SocketAsyncEventArgs
        /// </summary>
        /// <param name="item">SocketAsyncEventArgs instance to add to the pool.</param>
        internal void Push(SocketAsyncEventArgs item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("Items added to a SocketAsyncEventArgsPool cannot be null");
            }
            lock (this.pool)
            {
                this.pool.Push(item);
            }
        }
    }
}
