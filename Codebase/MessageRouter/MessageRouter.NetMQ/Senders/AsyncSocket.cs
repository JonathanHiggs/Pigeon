using MessageRouter.Addresses;
using NetMQ;
using NetMQ.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace MessageRouter.NetMQ.Senders
{
    /// <summary>
    /// Wraps a NetMQ DealerSocket to provide an asynchronous socket interface 
    /// </summary>
    public class AsyncSocket : IAsyncSocket
    {
        private readonly DealerSocket socket;
        private readonly Dictionary<int, NetMQTask> requests = new Dictionary<int, NetMQTask>();
        private int nextRequestId = 0;
        private object requestIdLockObj = new object();
        

        public NetMQSocket Socket => socket;


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
        /// Dispatches a <see cref="NetMQMessage"/> over the transport to a remote <see cref="IReceiver"/> and
        /// returns a task that will complete when a response is returned from the remote or when the
        /// timeout elapses
        /// </summary>
        /// <param name="message"><see cref="NetMQMessage"/> to send to the remote</param>
        /// <param name="timeout"><see cref="TimeSpan"/> after which the returned <see cref="Task{NetMQMessage}"/> will throw an error if no response has been received</param>
        /// <returns>A task that will complete successfully when a responce is received or that will fail once the timeout elapses</returns>
        public Task<NetMQMessage> SendAndReceive(NetMQMessage message, TimeSpan timeout)
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
        /// Connects the socket to the <see cref="IAddress"/>
        /// </summary>
        /// <param name="address">Address of the remote to connect to</param>
        public void Connect(string address) => socket.Connect(address);


        /// <summary>
        /// Disconects the socket from the <see cref="IAddress"/>
        /// </summary>
        /// <param name="address">Address of the connected remote</param>
        public void Disconnect(string address) => socket.Disconnect(address);


        private ElapsedEventHandler TimeoutHandler(int requestId)
        {
            return (sender, e) =>
            {
                if (requests.TryGetValue(requestId, out var netMQTask))
                {
                    requests.Remove(requestId);
                    netMQTask.ThrowTimeoutException(new TimeoutException($"RequestId {requestId} timed out"));
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
                requests.Remove(requestId);
                netMQTask.CompleteWithReponse(message);
            }
        }
    }
}
