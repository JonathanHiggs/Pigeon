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
    /// factories and monitors to create and run <see cref="Common.INetMQConnection"/>s
    /// </summary>
    public class NetMQConfig : ITransportConfig
    {
        private readonly INetMQFactory factory;
        

        /// <summary>
        /// Initializes a new instance of <see cref="NetMQConfig"/>
        /// </summary>
        /// <param name="container">DI Container to wire up dependencies</param>
        public NetMQConfig(IContainer container)
        {
            if (null == container)
                throw new ArgumentNullException(nameof(container));

            container.Register<ISerializer, DotNetSerializer>(true);
            container.Register<INetMQMessageFactory, NetMQMessageFactory>(true);
            container.Register<INetMQPoller, NetMQPoller>(true);
            container.Register<INetMQMonitor, NetMQMonitor>(true);
            container.Register<INetMQFactory, NetMQFactory>(true);
            
            factory = container.Resolve<INetMQFactory>();
        }


        private NetMQConfig(INetMQFactory factory)
        {
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
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


        /// <summary>
        /// Initializes a new instance of <see cref="NetMQConfig"/>
        /// </summary>
        /// <param name="factory"></param>
        /// <returns></returns>
        public static NetMQConfig Create(INetMQFactory factory)
        {
            if (null == factory)
                throw new ArgumentNullException(nameof(factory));

            return new NetMQConfig(factory);
        }
    }
}
