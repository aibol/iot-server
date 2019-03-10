using System;
using System.Collections.Generic;

namespace Socket.Server
{
    public class SocketAsyncEventArgsPool
    {
        private readonly Stack<DualModeSocketAsyncEventArgs> _pool;

        public SocketAsyncEventArgsPool(int capacity)
        {
            _pool = new Stack<DualModeSocketAsyncEventArgs>(capacity);
        }

        /// <summary>
        /// Add a DualModeSocketAsyncEventArg instance to the pool 
        /// </summary>
        public void Push(DualModeSocketAsyncEventArgs dmsaea)
        {
            if (dmsaea == null)
                throw new ArgumentNullException(nameof(dmsaea));

            lock (_pool)
            {
                _pool.Push(dmsaea);
            }
        }

        /// <summary>
        /// Removes a DualModeSocketAsyncEventArgs instance from the pool
        /// </summary>
        /// <returns>the object removed from the pool </returns>
        public DualModeSocketAsyncEventArgs Pop()
        {
            lock (_pool)
            {
                return _pool.Pop();
            }
        }
    }
}