namespace Pigeon.Subscribers
{
    /// <summary>
    /// A method that can perform the handling of <see cref="ISubscriber.Handler"/> for incoming messages
    /// </summary>
    /// <param name="subscriber">The source <see cref="ISubscriber"/></param>
    /// <param name="topicEvent">Topic event received by the <see cref="ISubscriber"/></param>
    public delegate void TopicEventHandler(ISubscriber subscriber, object topicEvent);
}
