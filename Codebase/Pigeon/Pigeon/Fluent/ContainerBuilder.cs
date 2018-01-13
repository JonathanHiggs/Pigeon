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
using Pigeon.Subscribers;
using Pigeon.Topics;
using Pigeon.Transport;

namespace Pigeon.Fluent
{
    public class ContainerBuilder : ITransportBuilder, IHandlerBuilder, IRouterBuilder
    {
        private string name;
        private IContainer container;


        public ContainerBuilder(string name, IContainer container)
        {
            this.name = name;
            this.container = container ?? throw new ArgumentNullException(nameof(container));

            container.Register<ITopicRouter, TopicRouter>(true);
            container.Register<IRequestRouter, RequestRouter>(true);
            container.Register<IPackageFactory, PackageFactory>(true);

            var topicDispatcher = container.Resolve<DITopicDispatcher>();
            container.Register<ITopicDispatcher>(topicDispatcher);
            container.Register<IDITopicDispatcher>(topicDispatcher);

            var requestDispatcher = container.Resolve<DIRequestDispatcher>();
            container.Register<IRequestDispatcher>(requestDispatcher);
            container.Register<IDIRequestDispatcher>(requestDispatcher);

            container.Register<ISubscriptionsCache, SubscriptionsCache>(true);

            container.Register<ISenderCache, SenderCache>(true);
            container.Register<IMonitorCache, MonitorCache>(true);
            container.Register<IReceiverCache, ReceiverCache>(true);
            container.Register<IPublisherCache, PublisherCache>(true);
            container.Register<ISubscriberCache, SubscriberCache>(true);
            
            container.Register(new Router(
                name,
                container.Resolve<ISenderCache>(),
                container.Resolve<IMonitorCache>(),
                container.Resolve<IReceiverCache>(),
                container.Resolve<IPublisherCache>(),
                container.Resolve<ISubscriberCache>()
            ));
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
