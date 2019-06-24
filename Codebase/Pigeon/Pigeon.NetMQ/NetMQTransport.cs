using System;

using NetMQ;

using Pigeon.Fluent.Transport;
using Pigeon.NetMQ.Publishers;
using Pigeon.NetMQ.Receivers;
using Pigeon.NetMQ.Senders;
using Pigeon.NetMQ.Subscribers;
using Pigeon.Publishers;
using Pigeon.Receivers;
using Pigeon.Routing;
using Pigeon.Senders;
using Pigeon.Subscribers;
using Pigeon.Transport;

namespace Pigeon.NetMQ
{
    /// <summary>
    /// Internally connects up all dependencies for NetMQ transport and is used to supply <see cref="Router"/> with all
    /// factories and monitors to create and run <see cref="Common.INetMQConnection"/>s
    /// </summary>
    public class NetMQTransport : ITransportConfig
    {
        private readonly INetMQFactory factory;


        /// <summary>
        /// Initializes a new instance of <see cref="NetMQTransport"/>
        /// </summary>
        /// <param name="container"><see cref="IContainer"/> to resolve and wire up dependencies</param>
        public NetMQTransport(IContainer container)
        {
            if (container is null)
                throw new ArgumentNullException(nameof(container));

            container.Register<INetMQMessageFactory, NetMQMessageFactory>(true);
            container.Register<INetMQPoller, NetMQPoller>(true);
            container.Register<INetMQMonitor, NetMQMonitor>(true);
            container.Register<INetMQFactory, NetMQFactory>(true);
            
            factory = container.Resolve<INetMQFactory>();
            Configurer = container.Resolve<TransportSetup<INetMQSender, INetMQReceiver, INetMQPublisher, INetMQSubscriber>>();
        }


        /// <summary>
        /// Initializes a new instance of <see cref="NetMQTransport"/>
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="requestRouter"></param>
        /// <param name="receiverCache"></param>
        /// <param name="topicRouter"></param>
        /// <param name="publisherCache"></param>
        private NetMQTransport(INetMQFactory factory, IRequestRouter requestRouter, IReceiverCache receiverCache, ITopicRouter topicRouter, IPublisherCache publisherCache)
        {
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
            Configurer = new TransportSetup<INetMQSender, INetMQReceiver, INetMQPublisher, INetMQSubscriber>(requestRouter, receiverCache, topicRouter, publisherCache);
        }


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


        public ITransportSetup Configurer { get; private set; }


        public static NetMQTransport Create(INetMQFactory factory, IRequestRouter requestRouter, IReceiverCache receiverCache, ITopicRouter topicRouter, IPublisherCache publisherCache)
        {
            if (factory is null)
                throw new ArgumentNullException(nameof(factory));

            return new NetMQTransport(factory, requestRouter, receiverCache, topicRouter, publisherCache);
        }
    }
}
