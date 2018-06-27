using System;
using System.Collections.Generic;

namespace SDBees.Core.Connectivity.SDBeesLink
{
    public class SDBeesCallbackCommandQueue
    {
        private readonly object m_locker;

        private Queue<SDBeesCallbackCommand> m_commandQueue;

        /// <summary>
        /// full constructor
        /// </summary>
        public SDBeesCallbackCommandQueue()
        {
            m_commandQueue = new Queue<SDBeesCallbackCommand>();
            m_locker = new object();
        }

        /// <summary>
        /// push a command on the stack thread safe 
        /// </summary>
        /// <param name="callbackCommand">the command to push</param>
        public void push (SDBeesCallbackCommand callbackCommand)
        {
            lock (m_locker)
            {
                m_commandQueue.Enqueue(callbackCommand);
            }
        }

        public SDBeesCallbackCommand popFirst ()
        {
            SDBeesCallbackCommand retval = null;
            lock (m_locker)
            {
                try
                {
                    if (0 < m_commandQueue.Count)
                    {
                        retval = m_commandQueue.Dequeue();
                    }
                }
                catch (InvalidOperationException)
                {
                    retval = null;
                }
            }
            return retval;
        }
    }
}
