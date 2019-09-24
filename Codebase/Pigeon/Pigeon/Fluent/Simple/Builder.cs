using System;

using Pigeon.Addresses;
using Pigeon.Fluent.Transport;
using Pigeon.Monitors;
using Pigeon.Packages;
using Pigeon.Publishers;
using Pigeon.Receivers;
using Pigeon.Requests;
using Pigeon.Routing;
using Pigeon.Senders;
using Pigeon.Serialization;
using Pigeon.Subscribers;
using Pigeon.Topics;
using Pigeon.Transport;

namespace Pigeon.Fluent.Simple
{
    public class Builder : IFluentBuilder<Builder>
    {
        private string name;


        public Builder(string name)
        {
            this.name = name;
            TopicRouter = new TopicRouter();
            MonitorCache = new MonitorCache();
            RequestRouter = new RequestRouter();
            PackageFactory = new PackageFactory();
            TopicDispatcher = new TopicDispatcher();
            RequestDispatcher = new RequestDispatcher();
            SubscriptionsCache = new SubscriptionsCache();
            SerializerCache = new SerializerCache();

            SenderCache = new SenderCache(RequestRouter, MonitorCache);
            ReceiverCache = new ReceiverCache(MonitorCache);
            PublisherCache = new PublisherCache(MonitorCache);
            SubscriberCache = new SubscriberCache(TopicRouter, MonitorCache, SubscriptionsCache);
        }

        public TopicRouter TopicRouter { get; }
        public RequestRouter RequestRouter { get; }
        public PackageFactory PackageFactory { get; }
        public TopicDispatcher TopicDispatcher { get; }
        public RequestDispatcher RequestDispatcher { get; }
        public SubscriptionsCache SubscriptionsCache { get; }

        public SenderCache SenderCache { get; }
        public MonitorCache MonitorCache { get; }
        public ReceiverCache ReceiverCache { get; }
        public PublisherCache PublisherCache { get; }
        public SubscriberCache SubscriberCache { get; }


        public SerializerCache SerializerCache { get; }


        public static Builder WithName(string name)
            => new Builder(name);


        public Builder WithTransport<TTransport>()
            where TTransport : ITransportConfig
        {
            var transport = Activator.CreateInstance<TTransport>();

            if (null != transport.SenderFactory)
                SenderCache.AddFactory(transport.SenderFactory);

            if (null != transport.ReceiverFactory)
                ReceiverCache.AddFactory(transport.ReceiverFactory);

            if (null != transport.PublisherFactory)
                PublisherCache.AddFactory(transport.PublisherFactory);

            if (null != transport.SubscriberFactory)
                SubscriberCache.AddFactory(transport.SubscriberFactory);

            return this;
        }


        public Builder WithTransport<TTransport>(TTransport transport, Action<ITransportSetup> config)
            where TTransport : ITransportConfig
        {
            if (null != transport.SenderFactory)
                SenderCache.AddFactory(transport.SenderFactory);

            if (null != transport.ReceiverFactory)
                ReceiverCache.AddFactory(transport.ReceiverFactory);

            if (null != transport.PublisherFactory)
                PublisherCache.AddFactory(transport.PublisherFactory);

            if (null != transport.SubscriberFactory)
                SubscriberCache.AddFactory(transport.SubscriberFactory);

            config(transport.Configurer);

            return this;
        }


        public Builder WithSerializer<TSerializer>(TSerializer serializer, bool defaultSerializer = false, Action<TSerializer> setup = null)
            where TSerializer : ISerializer
        {
            SerializerCache.AddSerializer(serializer);
            if (defaultSerializer)
                SerializerCache.SetDefaultSerializer(serializer);

            if (!(setup is null))
                setup(serializer);

            return this;
        }


        public Builder WithSenderRouting<TSender, TRequest>(IAddress address)
            where TSender : ISender
            where TRequest : class
        {
            RequestRouter.AddRequestRouting<TRequest, TSender>(address);
            return this;
        }


        public Builder WithReceiver<TReceiver>(IAddress address)
            where TReceiver : IReceiver
        {
            ReceiverCache.AddReceiver<TReceiver>(address);
            return this;
        }


        public Builder WithPublisher<TPublisher>(IAddress address)
            where TPublisher : IPublisher
        {
            PublisherCache.AddPublisher<TPublisher>(address);
            return this;
        }


        public Builder WithSubscriber<TSubscriber, TTopic>(IAddress address)
            where TSubscriber : ISubscriber
        {
            TopicRouter.AddTopicRouting<TTopic, TSubscriber>(address);
            return this;
        }


        public Builder WithRequestHandler<TRequest, TResponse>(IRequestHandler<TRequest, TResponse> handler)
            where TRequest : class
            where TResponse : class
        {
            RequestDispatcher.Register(handler);
            return this;
        }


        public Builder WithRequestHandler<TRequest, TResponse>(RequestHandlerDelegate<TRequest, TResponse> handler)
            where TRequest : class
            where TResponse : class
        {
            RequestDispatcher.Register(handler);
            return this;
        }


        public Builder WithAsyncRequestHandler<TRequest, TResponse>(AsyncRequestHandlerDelegate<TRequest, TResponse> handler)
            where TRequest : class
            where TResponse : class
        {
            RequestDispatcher.RegisterAsync(handler);
            return this;
        }


        public Builder WithTopicHandler<TTopic>(ITopicHandler<TTopic> handler)
        {
            TopicDispatcher.Register(handler);
            return this;
        }


        public Builder WithTopicHandler<TTopic>(TopicHandlerDelegate<TTopic> handler)
        {
            TopicDispatcher.Register(handler);
            return this;
        }


        public Builder WithAsyncTopicHandler<TTopic>(AsyncTopicHandlerDelegate<TTopic> handler)
        {
            TopicDispatcher.Register(handler);
            return this;
        }


        public Router Build()
        {
            var router = new Router(name, SenderCache, MonitorCache, ReceiverCache, PublisherCache, SubscriberCache);
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
