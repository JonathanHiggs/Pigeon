using System;
using System.Threading.Tasks;
using System.Timers;

namespace MessageRouter.Utils
{
    public struct AsyncTask
    {
        private TaskCompletionSource<object> taskCompletionSource;
        private Timer timeoutTimer;

        public AsyncTask(TaskCompletionSource<object> taskCompletionSource, TimeSpan timeout, ElapsedEventHandler timeoutHandler)
        {
            if (timeout.TotalMilliseconds < 0.0)
                throw new ArgumentNullException("Timeout requires a positive TimeSpan");

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

        public void ThrowTimeoutException(Exception ex)
        {
            timeoutTimer.Stop();
            taskCompletionSource.TrySetException(ex);
            taskCompletionSource = null;
            timeoutTimer.Dispose();
            timeoutTimer = null;
        }

        public void CompleteWithResult(object result)
        {
            timeoutTimer.Stop();
            taskCompletionSource.TrySetResult(result);
            taskCompletionSource = null;
            timeoutTimer.Dispose();
            timeoutTimer = null;
        }
    }
}
