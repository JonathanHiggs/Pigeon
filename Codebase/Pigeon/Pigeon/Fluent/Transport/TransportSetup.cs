using System;

using Pigeon.Addresses;
using Pigeon.Publishers;
using Pigeon.Receivers;
using Pigeon.Routing;
using Pigeon.Senders;
using Pigeon.Subscribers;

namespace Pigeon.Fluent.Transport
{
    public class TransportSetup<TSender, TReceiver, TPublisher, TSubscriber> : ITransportSetup
        where TSender : ISender
        where TReceiver : IReceiver
        where TPublisher : IPublisher
        where TSubscriber : ISubscriber
    {
        private ITopicRouter topicRouter;
        private IReceiverCache receiverCache;
        private IRequestRouter requestRouter;
        private IPublisherCache publisherCache;


        public TransportSetup(IRequestRouter requestRouter, IReceiverCache receiverCache, ITopicRouter topicRouter, IPublisherCache publisherCache)
        {
            this.topicRouter = topicRouter ?? throw new ArgumentNullException(nameof(topicRouter));
            this.receiverCache = receiverCache ?? throw new ArgumentNullException(nameof(receiverCache));
            this.requestRouter = requestRouter ?? throw new ArgumentNullException(nameof(requestRouter));
            this.publisherCache = publisherCache ?? throw new ArgumentNullException(nameof(publisherCache));
        }

        public void WithPublisher(IAddress address)
        {
            publisherCache.AddPublisher<TPublisher>(address);
        }

        public void WithReceiver(IAddress address)
        {
            receiverCache.AddReceiver<TReceiver>(address);
        }

        public ISenderSetup WithSender(IAddress address)
        {
            return new SenderSetup<TSender>(requestRouter, address);
        }

        public ISubscriberSetup WithSubscriber(IAddress address)
        {
            return new SubscriberSetup<TSubscriber>(topicRouter, address);
        }
    }    
}
