using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pigeon.Utils
{
    /// <summary>
    /// Manages out-of-process asynchronous tasks to not block threads while waiting for results with automatic timeouts
    /// </summary>
    /// <typeparam name="TResult">Type of the task result</typeparam>
    /// <typeparam name="TId">Type of the task identifier to match tasks to results</typeparam>
    public class RemoteTaskManager<TResult, TId>
    {
        private readonly Dictionary<TId, RemoteTask<TResult>> tasks = new Dictionary<TId, RemoteTask<TResult>>();
        private TId nextId;
        private Func<TId, TId> generateNextId;
        private object requestIdLockObj = new object();


        /// <summary>
        /// Initializes a new instance of <see cref="RemoteTaskManager{TResult, TId}"/>
        /// </summary>
        /// <param name="firstId">First Id to use</param>
        /// <param name="generateNextId">Function to generate the next Id from the previous</param>
        public RemoteTaskManager(TId firstId, Func<TId, TId> generateNextId)
        {
            this.generateNextId = generateNextId ?? throw new ArgumentNullException(nameof(generateNextId));
            nextId = firstId;
        }


        /// <summary>
        /// Wraps the invocation of a (out-of-process asynchronous) task in a native task with an id and timeout
        /// </summary>
        /// <param name="run">Action to perform to initiate the task; passes in a generated identifier for the task if it is
        /// required and which can also be captured </param>
        /// <param name="timeout"><see cref="TimeSpan"/> after which the task will throw a <see cref="TimeoutException"/> in 
        /// the absense of a result being set</param>
        /// <returns>A <see cref="Task{TResult}"/> that respresents the remote task which will either return the result or
        /// throw a <see cref="TimeoutException"/></returns>
        public async Task<TResult> StartRemoteTask(Action<TId> run, TimeSpan timeout)
        {
            var taskCompletionSource = new TaskCompletionSource<TResult>();
            TId id;

            lock (requestIdLockObj)
            {
                id = nextId;
                nextId = generateNextId(nextId);
            }

            var remoteTask = new RemoteTask<TResult>(taskCompletionSource, timeout, () =>
            {
                if (!tasks.ContainsKey(id))
                    return null;

                tasks.Remove(id);
                return new TimeoutException($"RequestId {id} timed out");
            });

            tasks.Add(id, remoteTask);

            run(id);

            return await taskCompletionSource.Task;
        }


        /// <summary>
        /// Transitions the underlying task that matches the identifier to a completed state returning the supplied result
        /// </summary>
        /// <param name="id">Task identifier</param>
        /// <param name="result">Result to set on the underlying task</param>
        public void CompleteTask(TId id, object result)
        {
            if (!tasks.TryGetValue(id, out var asyncTask))
                return;

            try
            { 
                tasks.Remove(id);

                if (result is Exception ex && !typeof(Exception).IsAssignableFrom(typeof(TResult)))
                {
                    asyncTask.CompleteWithException(ex);
                    return;
                }

                if (!(result is TResult tResult))
                    throw new InvalidOperationException($"Unexpected result type received {result.GetType().FullName}, expected {typeof(TResult).FullName}");

                asyncTask.CompleteWithResult(tResult);
            }
            catch (Exception exception)
            {
                asyncTask.CompleteWithException(exception);
            }
        }
    }
}
