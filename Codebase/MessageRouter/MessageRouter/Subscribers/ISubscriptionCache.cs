namespace MessageRouter.Subscribers
{
    public interface ISubscriptionCache
    {
        Subscription Add<TTopic>(ISubscriber subscriber);
        void Remove<TTopic>();
    }
}