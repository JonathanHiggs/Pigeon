using System;

using MessageRouter.Publishers;
using MessageRouter.Receivers;
using MessageRouter.Senders;
using MessageRouter.Serialization;
using MessageRouter.Subscribers;
using MessageRouter.Transport;
using MessageRouter.NetMQ.Senders;
using MessageRouter.NetMQ.Publishers;
using MessageRouter.NetMQ.Receivers;
using MessageRouter.NetMQ.Subscribers;

using NetMQ;

namespace MessageRouter.NetMQ
{
    /// <summary>
    /// Internally connects up all dependencies for NetMQ transport and is used to supply <see cref="Router"/> with all
    /// factories and monitors to create and run NetMQ endpoints 
    /// </summary>
    public class NetMQConfig : ITransportConfig
    {
        private readonly NetMQFactory factory = new NetMQFactory(new NetMQMonitor(new NetMQPoller()), new BinarySerializer());
        

        /// <summary>
        /// Gets a factory for creating <see cref="INetMQSender"/>s if available, otherwise null
        /// </summary>
        public ISenderFactory SenderFactory => factory;


        /// <summary>
        /// Gets a factory for creating <see cref="INetMQReceiver"/>s if available, otherwise null
        /// </summary>
        public IReceiverFactory ReceiverFactory => factory;


        /// <summary>
        /// Gets a factory for creating <see cref="INetMQPublisher"/>s if available, otherwise null
        /// </summary>
        public IPublisherFactory PublisherFactory => factory;


        /// <summary>
        /// Gets a factory for creating <see cref="INetMQSubscriber"/>s if available, otherwise null
        /// </summary>
        public ISubscriberFactory SubscriberFactory => factory;
    }
}
