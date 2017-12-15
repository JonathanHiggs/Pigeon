using System;

using MessageRouter.Publishers;
using MessageRouter.Receivers;
using MessageRouter.Senders;
using MessageRouter.Serialization;
using MessageRouter.Subscribers;
using MessageRouter.Transport;

using NetMQ;

namespace MessageRouter.NetMQ
{
    public class NetMQTransport : ITransportConfig
    {
        private readonly NetMQFactory factory;
        


        public NetMQTransport()
        {
            var poller = new NetMQPoller();
            var monitor = new NetMQMonitor(poller);

            factory = new NetMQFactory(monitor, monitor, new BinarySerializer());
        }
        

        public ISenderFactory SenderFactory => throw new NotImplementedException();

        public IReceiverFactory ReceiverFactory => throw new NotImplementedException();

        public IPublisherFactory PublisherFactory => throw new NotImplementedException();

        public ISubscriberFactory SubscriberFactory => throw new NotImplementedException();
    }
}
