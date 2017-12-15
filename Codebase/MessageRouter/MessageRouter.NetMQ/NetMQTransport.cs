using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageRouter.NetMQ.Receivers;
using MessageRouter.NetMQ.Senders;
using MessageRouter.Receivers;
using MessageRouter.Senders;
using MessageRouter.Serialization;
using MessageRouter.Transport;
using NetMQ;

namespace MessageRouter.NetMQ
{
    public class NetMQTransport : ITransport<INetMQSender, INetMQReceiver>
    {
        private readonly NetMQFactory factory;


        public ISenderFactory<INetMQSender> SenderFactory => factory;


        public IReceiverFactory<INetMQReceiver> ReceiverFactory => factory;


        public NetMQTransport()
        {
            var poller = new NetMQPoller();
            var monitor = new NetMQMonitor(poller);

            factory = new NetMQFactory(monitor, monitor, new BinarySerializer());
        }
    }
}
