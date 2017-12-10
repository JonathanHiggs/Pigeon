using NetMQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace MessageRouter.NetMQ.Senders
{
    /// <summary>
    /// Adds a timeout to an expected reponse to prevent indefinitly waiting for a response 
    /// </summary>
    public struct NetMQTask
    {
        private readonly TaskCompletionSource<NetMQMessage> taskCompletionSource;
        private readonly Timer timeoutTimer;


        /// <summary>
        /// Initializes a new instance of <see cref="NetMQTask"/>
        /// </summary>
        /// <param name="taskCompletionSource"><see cref="TaskCompletionSource{TResult}"/> that can be completed when a response is received</param>
        /// <param name="timeout"><see cref="TimeSpan"/> after which if a response if not received the timeout handler will be called</param>
        /// <param name="timeoutHandler">Handler that is called when the timeout elapses without a response message having been returned</param>
        public NetMQTask(TaskCompletionSource<NetMQMessage> taskCompletionSource, TimeSpan timeout, ElapsedEventHandler timeoutHandler)
        {
            this.taskCompletionSource = taskCompletionSource ?? throw new ArgumentNullException(nameof(taskCompletionSource));

            timeoutTimer = new Timer
            {
                Interval = Math.Max(timeout.TotalMilliseconds, 5.0), // Min timeout of 5ms
                AutoReset = false,
                Enabled = false
            };

            timeoutTimer.Elapsed += timeoutHandler ?? throw new ArgumentNullException(nameof(timeoutHandler));
            timeoutTimer.Enabled = true;
        }


        /// <summary>
        /// Stops the timeout timer and throws the supplied exception in the task
        /// </summary>
        /// <param name="ex">Timeout exception that is thrown in the wrapped task</param>
        public void ThrowTimeoutException(Exception ex)
        {
            timeoutTimer.Stop();
            taskCompletionSource.TrySetException(ex);
        }


        /// <summary>
        /// Stops the timeout timer and completes the task with the response message
        /// </summary>
        /// <param name="message">Response message that is set to the result of the task</param>
        public void CompleteWithReponse(NetMQMessage message)
        {
            timeoutTimer.Stop();
            taskCompletionSource.SetResult(message);
        }
    }
}
