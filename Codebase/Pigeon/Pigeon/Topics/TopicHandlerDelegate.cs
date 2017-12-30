using Pigeon.Subscribers;

namespace Pigeon.Topics
{
    /// <summary>
    /// Represents the method to handle <see cref="ISubscriber.Handler"/> events
    /// </summary>
    /// <typeparam name="TTopic">Type of the topic message that is handled</typeparam>
    /// <param name="message">The topic message object that was published</param>
    public delegate void TopicHandlerDelegate<TTopic>(TTopic message);
}
