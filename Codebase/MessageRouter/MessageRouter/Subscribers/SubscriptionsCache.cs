using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Subscribers
{
    public class SubscriptionsCache : ISubscriptionsCache
    {
        private readonly Dictionary<Type, Subscription> subscriptions = new Dictionary<Type, Subscription>();


        public Subscription Add<TTopic>(ISubscriber subscriber)
        {
            var subscription = new Subscription(subscriber, typeof(TTopic), () =>
            {
                subscriber.Unsubscribe<TTopic>();
                subscriptions.Remove(typeof(TTopic));
            });

            subscriptions.Add(typeof(TTopic), subscription);

            return subscription;
        }


        public void Remove<TTopic>()
        {
            if (!subscriptions.TryGetValue(typeof(TTopic), out var subscription))
                return;

            subscription.Dispose();
        }
    }
}
