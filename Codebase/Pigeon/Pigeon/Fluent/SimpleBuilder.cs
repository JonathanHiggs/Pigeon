using System;

using Pigeon.Addresses;
using Pigeon.Packages;
using Pigeon.Monitors;
using Pigeon.Publishers;
using Pigeon.Receivers;
using Pigeon.Requests;
using Pigeon.Routing;
using Pigeon.Senders;
using Pigeon.Subscribers;
using Pigeon.Topics;
using Pigeon.Transport;

namespace Pigeon.Fluent
{
    public class SimpleBuilder : IFluentBuilder
    {
        private string name;

        private readonly TopicRouter topicRouter;
        private readonly RequestRouter requestRouter;
        private readonly PackageFactory packageFactory;
        private readonly TopicDispatcher topicDispatcher;
        private readonly RequestDispatcher requestDispatcher;
        private readonly SubscriptionsCache subscriptionsCache;

        private readonly SenderCache senderCache;
        private readonly MonitorCache monitorCache;
        private readonly ReceiverCache receiverCache;
        private readonly PublisherCache publisherCache;
        private readonly SubscriberCache subscriberCache;


        public SimpleBuilder(string name)
        {
            this.name = name;
            topicRouter = new TopicRouter();
            monitorCache = new MonitorCache();
            requestRouter = new RequestRouter();
            packageFactory = new PackageFactory();
            topicDispatcher = new TopicDispatcher();
            requestDispatcher = new RequestDispatcher();
            subscriptionsCache = new SubscriptionsCache();

            senderCache = new SenderCache(requestRouter, monitorCache);
            receiverCache = new ReceiverCache(monitorCache);
            publisherCache = new PublisherCache(monitorCache);
            subscriberCache = new SubscriberCache(topicRouter, monitorCache, subscriptionsCache);
        }


        public IFluentBuilder WithTransport<TTransport>()
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


        public IFluentBuilder WithSenderRouting<TSender, TRequest>(IAddress address)
            where TSender : ISender
            where TRequest : class
        {
            requestRouter.AddRequestRouting<TRequest, TSender>(address);
            return this;
        }


        public IFluentBuilder WithReceiver<TReceiver>(IAddress address)
            where TReceiver : IReceiver
        {
            receiverCache.AddReceiver<TReceiver>(address);
            return this;
        }


        public IFluentBuilder WithPublisher<TPublisher>(IAddress address)
            where TPublisher : IPublisher
        {
            publisherCache.AddPublisher<TPublisher>(address);
            return this;
        }


        public IFluentBuilder WithSubscriber<TSubscriber, TTopic>(IAddress address)
            where TSubscriber : ISubscriber
        {
            topicRouter.AddTopicRouting<TTopic, TSubscriber>(address);
            return this;
        }


        public IFluentBuilder WithRequestHandler<TRequest, TResponse>(IRequestHandler<TRequest, TResponse> handler)
            where TRequest : class
            where TResponse : class
        {
            requestDispatcher.Register(handler);
            return this;
        }


        public IFluentBuilder WithRequestHandler<TRequest, TResponse>(RequestHandlerDelegate<TRequest, TResponse> handler)
            where TRequest : class
            where TResponse : class
        {
            requestDispatcher.Register(handler);
            return this;
        }


        public IFluentBuilder WithAsyncRequestHandler<TRequest, TResponse>(AsyncRequestHandlerDelegate<TRequest, TResponse> handler)
            where TRequest : class
            where TResponse : class
        {
            requestDispatcher.RegisterAsync(handler);
            return this;
        }


        public IFluentBuilder WithTopicHandler<TTopic>(ITopicHandler<TTopic> handler)
        {
            topicDispatcher.Register(handler);
            return this;
        }


        public IFluentBuilder WithTopicHandler<TTopic>(TopicHandlerDelegate<TTopic> handler)
        {
            topicDispatcher.Register(handler);
            return this;
        }


        public IFluentBuilder WithAsyncTopicHandler<TTopic>(AsyncTopicHandlerDelegate<TTopic> handler)
        {
            topicDispatcher.RegisterAsync(handler);
            return this;
        }


        public Router Build()
        {
            var router = new Router(name, senderCache, monitorCache, receiverCache, publisherCache, subscriberCache);
            return router;
        }


        public Router BuildAndStart()
        {
            var router = Build();
            router.Start();
            return router;
        }
    }
}
