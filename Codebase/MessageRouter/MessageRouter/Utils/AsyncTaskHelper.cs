using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace MessageRouter.Utils
{
    public class AsyncTaskHelper
    {
        private readonly Dictionary<int, AsyncTask> tasks = new Dictionary<int, AsyncTask>();
        private int nextRequestId = 0;
        private object requestIdLockObj = new object();


        public Task<object> StartAsyncTask(Action<int> run, TimeSpan timeout)
        {
            var task = new Task<Task<object>>(() =>
            {
                var taskCompletionSource = new TaskCompletionSource<object>();
                int requestId;

                lock (requestIdLockObj)
                {
                    requestId = nextRequestId++;

                }

                var asyncTask = new AsyncTask(taskCompletionSource, timeout, TimeoutHandler(requestId));
                tasks.Add(requestId, asyncTask);

                run(requestId);

                return taskCompletionSource.Task;
            });

            task.Start();

            return task.Result;
        }


        public void CompleteTask(int requestId, object result)
        {
            if (!tasks.TryGetValue(requestId, out var asyncTask))
                return;

            tasks.Remove(requestId);
            asyncTask.CompleteWithResult(result);
        }


        private ElapsedEventHandler TimeoutHandler(int requestId)
        {
            return (sender, args) =>
            {
                if (!tasks.TryGetValue(requestId, out var asyncTask))
                    return;

                tasks.Remove(requestId);
                asyncTask.ThrowTimeoutException(new TimeoutException($"RequestId {requestId} timed out"));
            };
        }
    }
}
