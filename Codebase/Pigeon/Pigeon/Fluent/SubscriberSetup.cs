using System;
using Pigeon.Addresses;
using Pigeon.Fluent;
using Pigeon.Routing;
using Pigeon.Subscribers;

namespace Pigeon.NetMQ.Fluent
{
    public class SubscriberSetup<TSubscriber> : ISubscriberSetup
        where TSubscriber : ISubscriber
    {
        private IAddress address;
        private ITopicRouter router;

        public SubscriberSetup(ITopicRouter router, IAddress address)
        {
            this.router = router ?? throw new ArgumentNullException(nameof(router));
            this.address = address ?? throw new ArgumentNullException(nameof(address));
        }

        public ISubscriberSetup Handles<TTopic>()
        {
            router.AddTopicRouting<TTopic, TSubscriber>(address);
            return this;
        }
    }
    
}
