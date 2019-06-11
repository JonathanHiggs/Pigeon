using Pigeon.Publishers;

namespace Pigeon.Subscribers
{
    /// <summary>
    /// Maintains a set of added subscriptions
    /// </summary>
    public interface ISubscriptionsCache
    {
        /// <summary>
        /// Initializes and stores a subscription to the topic message stream from a remote <see cref="IPublisher"/>
        /// </summary>
        /// <typeparam name="TTopic">The type of the published topic message</typeparam>
        /// <returns>A representation of the subscription, the dispose method can be used to terminate the subscription</returns>
        Subscription Add<TTopic>(ISubscriber subscriber);


        /// <summary>
        /// Terminates a stored subscription to the topic message stream
        /// </summary>
        /// <typeparam name="TTopic">The type of the published topic message</typeparam>
        /// <param name="subscriber"><see cref="ISubscriber"/> for which to terminate the topic subscription</param>
        void Remove<TTopic>(ISubscriber subscriber);
    }
}