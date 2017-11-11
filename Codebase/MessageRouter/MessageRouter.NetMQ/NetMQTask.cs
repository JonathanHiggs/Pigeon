using NetMQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace MessageRouter.NetMQ
{
    /// <summary>
    /// Wraps together a messaging task with a timeout for async messaging
    /// </summary>
    public struct NetMQTask
    {
        public readonly TaskCompletionSource<NetMQMessage> TaskCompletionSource;
        public readonly Timer TimeoutTimer;


        /// <summary>
        /// Initializes a new instance of NetMQTask
        /// </summary>
        /// <param name="taskCompletionSource"></param>
        /// <param name="timeout"></param>
        /// <param name="timeoutHandler"></param>
        public NetMQTask(TaskCompletionSource<NetMQMessage> taskCompletionSource, double timeout, ElapsedEventHandler timeoutHandler)
        {
            TaskCompletionSource = taskCompletionSource ?? throw new ArgumentNullException(nameof(taskCompletionSource));

            TimeoutTimer = new Timer
            {
                Interval = timeout,
                AutoReset = false,
                Enabled = true,
            };

            TimeoutTimer.Elapsed += timeoutHandler ?? throw new ArgumentNullException(nameof(timeoutHandler));
        }
    }
}
