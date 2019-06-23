using System;

using Pigeon.Fluent.Handlers;
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

namespace Pigeon.Fluent
{
    public class ContainerBuilder : ITransportBuilder, IHandlerBuilder, IRouterBuilder, ISerializerBuilder
    {
        private string name;
        private IContainer container;


        public ContainerBuilder(string name, IContainer container)
        {
            this.name = name;
            this.container = container ?? throw new ArgumentNullException(nameof(container));

            container
                .Register<ITopicRouter, TopicRouter>(true)
                .Register<IRequestRouter, RequestRouter>(true)
                .Register<IPackageFactory, PackageFactory>(true);

            var topicDispatcher = container.Resolve<DITopicDispatcher>();
            container
                .Register<ITopicDispatcher>(topicDispatcher)
                .Register<IDITopicDispatcher>(topicDispatcher);

            var requestDispatcher = container.Resolve<DIRequestDispatcher>();
            container
                .Register<IRequestDispatcher>(requestDispatcher)
                .Register<IDIRequestDispatcher>(requestDispatcher);

            container
                .Register<ISenderCache, SenderCache>(true)
                .Register<IMonitorCache, MonitorCache>(true)
                .Register<IReceiverCache, ReceiverCache>(true)
                .Register<IPublisherCache, PublisherCache>(true)
                .Register<ISubscriberCache, SubscriberCache>(true)
                .Register<ISerializerCache, SerializerCache>(true)
                .Register<ISubscriptionsCache, SubscriptionsCache>(true);
            
            var router = new Router(
                name,
                container.Resolve<ISenderCache>(),
                container.Resolve<IMonitorCache>(),
                container.Resolve<IReceiverCache>(),
                container.Resolve<IPublisherCache>(),
                container.Resolve<ISubscriberCache>()
            );

            container
                .Register<IRouter<IRouterInfo>>(router)
                .Register(router);
        }


        public Router Build()
        {
            return container.Resolve<Router>();
        }


        public Router BuildAndStart()
        {
            var router = Build();
            router.Start();
            return router;
        }


        public IHandlerBuilder WithHandlers(Action<IHandlerSetup> config)
        {
            config(container.Resolve<HandlerSetup>());
            return this;
        }


        public ContainerBuilder WithSerializer<TSerializer>(bool defaultSerializer = false, Action<TSerializer> setup = null)
            where TSerializer : ISerializer
        {
            container.Register<TSerializer>(true);

            var cache = container.Resolve<ISerializerCache>();
            var serializer = container.Resolve<TSerializer>();

            cache.AddSerializer(serializer);
            if (defaultSerializer)
                cache.SetDefaultSerializer(serializer);

            if (!(setup is null))
                setup(serializer);

            return this;
        }


        public ITransportBuilder WithTransport<TTransport>(Action<ITransportSetup> config) where TTransport : ITransportConfig
        {
            container.Register<TTransport>(true);
            var transport = container.Resolve<TTransport>();

            if (null != transport.SenderFactory)
                container.Resolve<ISenderCache>().AddFactory(transport.SenderFactory);

            if (null != transport.ReceiverFactory)
                container.Resolve<IReceiverCache>().AddFactory(transport.ReceiverFactory);

            if (null != transport.PublisherFactory)
                container.Resolve<IPublisherCache>().AddFactory(transport.PublisherFactory);

            if (null != transport.SubscriberFactory)
                container.Resolve<ISubscriberCache>().AddFactory(transport.SubscriberFactory);

            config(transport.Configurer);

            return this;
        }
    }
}
