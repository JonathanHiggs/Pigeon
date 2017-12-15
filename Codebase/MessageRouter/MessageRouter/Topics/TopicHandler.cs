using MessageRouter.Subscribers;

namespace MessageRouter.Topics
{
    /// <summary>
    /// Represents the method to handle <see cref="ISubscriber.TopicMessageReceived"/> events
    /// </summary>
    /// <typeparam name="TTopic">Type of the topic message that is handled</typeparam>
    /// <param name="message">The topic message object that was published</param>
    public delegate void TopicHandler<TTopic>(TTopic message);
}
