using MessageRouter.Receivers;
using MessageRouter.Serialization;
using NetMQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.NetMQ
{
    public class NetMQAsyncReceiverManager : IAsyncReceiverManager
    {
        private readonly NetMQAsyncReceiver receiver;
        private readonly NetMQPoller poller;
        private Task pollerTask;


        public event RequestTaskDelegate RequestReceived;


        public NetMQAsyncReceiverManager(NetMQAsyncReceiver receiver)
        {
            this.receiver = receiver ?? throw new ArgumentNullException(nameof(receiver));
            this.poller = new NetMQPoller { receiver.PollableSocket };

            receiver.RequestReceived += RequestReceived;

            poller.RunAsync();
        }
    }
}
