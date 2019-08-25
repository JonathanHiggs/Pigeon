using Pigeon.Addresses;
using Pigeon.Common;
using Pigeon.Publishers;

namespace Pigeon.Subscribers
{
    /// <summary>
    /// Interface encapsulates a connection that is able to connect to <see cref="IAddress"/> to receive messages
    /// from <see cref="IPublisher"/>s
    /// </summary>
    public interface ISubscriber : IConnection
    {
        /// <summary>
        /// Initializes a subscription to the topic message stream from a remote <see cref="IPublisher"/>
        /// </summary>
        /// <typeparam name="TTopic">The type of the published topic message</typeparam>
        void Subscribe<TTopic>();


        /// <summary>
        /// Terminates a subscription to the topic message stream
        /// </summary>
        void Unsubscribe<TTopic>();
    }
}
