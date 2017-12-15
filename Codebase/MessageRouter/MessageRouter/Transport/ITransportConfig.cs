using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageRouter.Publishers;
using MessageRouter.Receivers;
using MessageRouter.Senders;
using MessageRouter.Subscribers;

namespace MessageRouter.Transport
{
    /// <summary>
    /// Used to configure a <see cref="Router"/> with the necessary factories to resolve transport end points when
    /// required. If an endpoint is not supported by the transport the factory getter will return null
    /// </summary>
    public interface ITransportConfig
    {
        /// <summary>
        /// Gets a factory for creating <see cref="ISender"/>s if available, otherwise null
        /// </summary>
        ISenderFactory SenderFactory { get; }


        /// <summary>
        /// Gets a factory for creating <see cref="IReceiver"/>s if available, otherwise null
        /// </summary>
        IReceiverFactory ReceiverFactory { get; }


        /// <summary>
        /// Gets a factory for creating <see cref="IPublisher"/>s if available, otherwise null
        /// </summary>
        IPublisherFactory PublisherFactory { get; }


        /// <summary>
        /// Gets a factory for creating <see cref="ISubscriber"/>s if available, otherwise null
        /// </summary>
        ISubscriberFactory SubscriberFactory { get; }
    }
}
