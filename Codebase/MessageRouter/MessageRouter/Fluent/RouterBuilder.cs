using System;

using MessageRouter.Addresses;
using MessageRouter.Messages;
using MessageRouter.Monitors;
using MessageRouter.Publishers;
using MessageRouter.Receivers;
using MessageRouter.Requests;
using MessageRouter.Routing;
using MessageRouter.Senders;
using MessageRouter.Subscribers;
using MessageRouter.Topics;
using MessageRouter.Transport;

namespace MessageRouter.Fluent
{
    public class RouterBuilder
    {
        private string name;

        private readonly TopicRouter topicRouter;
        private readonly RequestRouter requestRouter;
        private readonly MessageFactory messageFactory;
        private readonly TopicDispatcher topicDispatcher;
        private readonly RequestDispatcher requestDispatcher;

        private readonly SenderCache senderCache;
        private readonly MonitorCache monitorCache;
        private readonly ReceiverCache receiverCache;
        private readonly PublisherCache publisherCache;
        private readonly SubscriberCache subscriberCache;


        public RouterBuilder(string name)
        {
            this.name = name;
            topicRouter = new TopicRouter();
            monitorCache = new MonitorCache();
            requestRouter = new RequestRouter();
            messageFactory = new MessageFactory();
            topicDispatcher = new TopicDispatcher();
            requestDispatcher = new RequestDispatcher();

            senderCache = new SenderCache(requestRouter, monitorCache, messageFactory);
            receiverCache = new ReceiverCache(monitorCache, messageFactory, requestDispatcher);
            publisherCache = new PublisherCache(monitorCache, messageFactory);
            subscriberCache = new SubscriberCache(topicRouter, monitorCache, messageFactory, topicDispatcher);
        }


        public RouterBuilder WithTransport<TTransport>()
            where TTransport : ITransportConfig
        {
            var transport = Activator.CreateInstance<TTransport>();

            if (null != transport.SenderFactory)
                senderCache.AddFactory(transport.SenderFactory);

            if (null != transport.ReceiverFactory)
                receiverCache.AddFactory(transport.ReceiverFactory);

            if (null != transport.PublisherFactory)
                publisherCache.AddFactory(transport.PublisherFactory);

            if (null != transport.SubscriberFactory)
                subscriberCache.AddFactory(transport.SubscriberFactory);

            return this;
        }


        public RouterBuilder WithSenderRouting<TSender, TRequest>(IAddress address)
            where TSender : ISender
            where TRequest : class
        {
            requestRouter.AddSenderRouting<TRequest, TSender>(address);
            return this;
        }


        public RouterBuilder WithReceiver<TReceiver>(IAddress address)
            where TReceiver : IReceiver
        {
            receiverCache.AddReceiver<TReceiver>(address);
            return this;
        }


        public RouterBuilder WithPublisher<TPublisher>(IAddress address)
            where TPublisher : IPublisher
        {
            publisherCache.AddPublisher<TPublisher>(address);
            return this;
        }


        public RouterBuilder WithSubscriber<TSubscriber, TTopic>(IAddress address)
            where TSubscriber : ISubscriber
        {
            topicRouter.AddSubscriberRouting<TTopic, TSubscriber>(address);
            return this;
        }


        public RouterBuilder WithRequestHandler<TRequest, TResponse>(IRequestHandler<TRequest, TResponse> handler)
            where TRequest : class
            where TResponse : class
        {
            requestDispatcher.Register(handler);
            return this;
        }


        public RouterBuilder WithRequestHandler<TRequest, TResponse>(RequestHandlerDelegate<TRequest, TResponse> handler)
            where TRequest : class
            where TResponse : class
        {
            requestDispatcher.Register(handler);
            return this;
        }


        public RouterBuilder WithTopicHandler<TTopic>(ITopicHandler<TTopic> handler)
        {
            topicDispatcher.Register(handler);
            return this;
        }


        public RouterBuilder WithTopicHandler<TTopic>(TopicHandlerDelegate<TTopic> handler)
        {
            topicDispatcher.Register(handler);
            return this;
        }


        public Router BuildAndStart()
        {
            var router = new Router(name, senderCache, monitorCache, receiverCache, publisherCache, subscriberCache);
            router.Start();
            return router;
        }
    }
}
