using MessageRouter.Messages;

namespace MessageRouter.Subscribers
{
    /// <summary>
    /// A method that can perform the handling of <see cref="ISubscriber.TopicMessageReceived"/> for incoming messages
    /// </summary>
    /// <param name="subscriber">The source <see cref="ISubscriber"/></param>
    /// <param name="message">Topic message wrapped in a <see cref="Message"/></param>
    public delegate void TopicEventHandler(ISubscriber subscriber, Message message);
}
