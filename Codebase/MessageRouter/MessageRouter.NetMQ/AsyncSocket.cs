using MessageRouter.Addresses;
using NetMQ;
using NetMQ.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.NetMQ
{
    public class AsyncSocket
    {
        private readonly DealerSocket socket;
        private readonly Dictionary<int, NetMQTask> requests = new Dictionary<int, NetMQTask>();
        private int nextRequestId = 0;
        private object requestIdLockObj = new object();


        public AsyncSocket(DealerSocket socket)
        {
            this.socket = socket ?? throw new ArgumentNullException(nameof(socket));
            socket.ReceiveReady += PendingMessage;
        }


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


        private System.Timers.ElapsedEventHandler TimeoutHandler(int requestId)
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


        public void Connect(IAddress address) => socket.Connect(address.ToString());
        public void Disconnect(IAddress address) => socket.Disconnect(address.ToString());
    }
}
