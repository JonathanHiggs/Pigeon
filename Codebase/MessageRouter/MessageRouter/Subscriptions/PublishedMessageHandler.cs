using MessageRouter.Subscribers;

namespace MessageRouter.Subscriptions
{
    /// <summary>
    /// Represents the method to handle a <see cref="ISubscriber"/>s PublishedMessage event
    /// </summary>
    /// <param name="subscriber">The source <see cref="ISubscriber"/> that received the published message</param>
    /// <param name="publishedMessage">The message object that was published</param>
    public delegate void PublishedMessageHandler(ISubscriber subscriber, object publishedMessage);
}
