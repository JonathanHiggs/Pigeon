namespace MessageRouter.Subscribers
{
    public interface ISubscriptionsCache
    {
        Subscription Add<TTopic>(ISubscriber subscriber);
        void Remove<TTopic>();
    }
}