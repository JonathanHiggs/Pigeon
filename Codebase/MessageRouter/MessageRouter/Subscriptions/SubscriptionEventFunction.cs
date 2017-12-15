namespace MessageRouter.Subscriptions
{
    /// <summary>
    /// Delegate for handling subscription events
    /// </summary>
    /// <param name="eventMessage">Subscription event message</param>
    internal delegate void SubscriptionEventFunction(object eventMessage);
}
