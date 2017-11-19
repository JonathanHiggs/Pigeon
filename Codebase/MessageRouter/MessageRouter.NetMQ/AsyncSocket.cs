using MessageRouter.Addresses;
using NetMQ;
using NetMQ.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace MessageRouter.NetMQ
{
    /// <summary>
    /// Wraps a NetMQ DealerSocket to provide an asynchronous socket interface 
    /// </summary>
    public class AsyncSocket
    {
        private readonly DealerSocket socket;
        private readonly Dictionary<int, NetMQTask> requests = new Dictionary<int, NetMQTask>();
        private int nextRequestId = 0;
        private object requestIdLockObj = new object();


        public ISocketPollable PollableSocket => socket;


        /// <summary>
        /// Initializes a new instance of an AsyncSocket
        /// </summary>
        /// <param name="socket">NetMQ inner DealerSocket</param>
        public AsyncSocket(DealerSocket socket)
        {
            this.socket = socket ?? throw new ArgumentNullException(nameof(socket));
            socket.ReceiveReady += PendingMessage;
        }


        /// <summary>
        /// Sends a message and returns a task that completes when a response is returned
        /// </summary>
        /// <param name="message">Outgoing message</param>
        /// <param name="timeout">Response timeout</param>
        /// <returns>Task for receiving a response message</returns>
        public Task<NetMQMessage> SendAndReceive(NetMQMessage message, double timeout = 0.0)
        {
            var requestMessage = new NetMQMessage(message);

            var task = new Task<Task<NetMQMessage>>(() =>
            {
                var taskCompletionSource = new TaskCompletionSource<NetMQMessage>();
                int requestId;

                lock (requestIdLockObj)
                {
                    requestId = nextRequestId++;
                }

                requestMessage.Push(requestId);
                requestMessage.PushEmptyFrame();

                var netTask = new NetMQTask(taskCompletionSource, timeout, TimeoutHandler(requestId));

                requests.Add(requestId, netTask);
                socket.SendMultipartMessage(requestMessage);

                return taskCompletionSource.Task;
            });

            task.Start();

            return task.Result;
        }


        /// <summary>
        /// Connects the socket to a remote at the <see cref="IAddress"/> endpoint
        /// </summary>
        /// <param name="address">Address of the remote</param>
        public void Connect(IAddress address) => socket.Connect(address.ToString());


        /// <summary>
        /// Disconects the socket from the remote at the <see cref="IAddress"/> endpoint
        /// </summary>
        /// <param name="address">Address of the connected remote</param>
        public void Disconnect(IAddress address) => socket.Disconnect(address.ToString());


        private ElapsedEventHandler TimeoutHandler(int requestId)
        {
            return (sender, e) =>
            {
                if (requests.TryGetValue(requestId, out var netMQTask))
                {
                    netMQTask.TimeoutTimer.Stop();
                    requests.Remove(requestId);
                    netMQTask.TaskCompletionSource.TrySetException(new TimeoutException($"RequestId {requestId} timedout"));
                }
            };
        }


        private void PendingMessage(object sender, NetMQSocketEventArgs e)
        {
            var message = socket.ReceiveMultipartMessage();

            message.Pop();
            var requestId = message.Pop().ConvertToInt32();

            if (requests.TryGetValue(requestId, out var netMQTask))
            {
                netMQTask.TimeoutTimer.Stop();
                requests.Remove(requestId);
                netMQTask.TaskCompletionSource.SetResult(message);
            }
        }
    }
}
