using MessageRouter.Subscribers;

namespace MessageRouter.Subscriptions
{
    /// <summary>
    /// Represents the method to handle a <see cref="ISubscriber"/>s PublishedMessage event
    /// </summary>
    /// <typeparam name="TEvent">Type of the subscription event that is handled</typeparam>
    /// <param name="publishedMessage">The message object that was published</param>
    public delegate void SubscriptionEventHandler<TEvent>(TEvent publishedMessage);
}
