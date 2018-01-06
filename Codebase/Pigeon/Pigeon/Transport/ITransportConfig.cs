using Pigeon.Fluent.Transport;
using Pigeon.Publishers;
using Pigeon.Receivers;
using Pigeon.Senders;
using Pigeon.Subscribers;

namespace Pigeon.Transport
{
    /// <summary>
    /// Used to configure a <see cref="Router"/> with the necessary factories to resolve transport end points when
    /// required. If an <see cref="IConnection"/> is not supported by the transport the factory getter will return null
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


        /// <summary>
        /// Makes a configuration unit for fluent construction
        /// </summary>
        /// <returns></returns>
        ITransportSetup Configurer { get; }
    }
}
