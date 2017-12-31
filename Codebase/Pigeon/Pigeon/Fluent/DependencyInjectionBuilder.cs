using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pigeon.Addresses;
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
    public class DependencyInjectionBuilder : IFluentBuilder
    {
        private string name;
        private IContainer container;


        public DependencyInjectionBuilder(string name, IContainer container)
        {
            this.name = name;
            this.container = container ?? throw new ArgumentNullException(nameof(container));

            container.Register<ITopicRouter, TopicRouter>(true);
            container.Register<IRequestRouter, RequestRouter>(true);
            container.Register<IPackageFactory, PackageFactory>(true);
            container.Register<ITopicDispatcher, TopicDispatcher>(true);
            container.Register<IRequestDispatcher, RequestDispatcher>(true);
            container.Register<ISubscriptionsCache, SubscriptionsCache>(true);

            container.Register<ISenderCache, SenderCache>(true);
            container.Register<IMonitorCache, MonitorCache>(true);
            container.Register<IReceiverCache, ReceiverCache>(true);
            container.Register<IPublisherCache, PublisherCache>(true);
            container.Register<ISubscriberCache, SubscriberCache>(true);

            container.Register<Router>(true);
        }


        public Router Build()
        {
            return new Router(
                name,
                container.Resolve<ISenderCache>(),
                container.Resolve<IMonitorCache>(),
                container.Resolve<IReceiverCache>(),
                container.Resolve<IPublisherCache>(),
                container.Resolve<ISubscriberCache>()
            );
        }

        public Router BuildAndStart()
        {
            var router = Build();
            router.Start();
            return router;
        }

        public IFluentBuilder WithPublisher<TPublisher>(IAddress address) where TPublisher : IPublisher
        {
            container.Resolve<IPublisherCache>().AddPublisher<TPublisher>(address);
            return this;
        }

        public IFluentBuilder WithReceiver<TReceiver>(IAddress address) where TReceiver : IReceiver
        {
            container.Resolve<IReceiverCache>().AddReceiver<TReceiver>(address);
            return this;
        }

        public IFluentBuilder WithRequestHandler<TRequest, TResponse>(IRequestHandler<TRequest, TResponse> handler)
            where TRequest : class
            where TResponse : class
        {
            container.Resolve<IRequestDispatcher>().Register(handler);
            return this;
        }

        public IFluentBuilder WithRequestHandler<TRequest, TResponse>(RequestHandlerDelegate<TRequest, TResponse> handler)
            where TRequest : class
            where TResponse : class
        {
            container.Resolve<IRequestDispatcher>().Register(handler);
            return this;
        }

        public IFluentBuilder WithAsyncRequestHandler<TRequest, TResponse>(AsyncRequestHandlerDelegate<TRequest, TResponse> handler)
            where TRequest : class
            where TResponse : class
        {
            container.Resolve<IRequestDispatcher>().RegisterAsync(handler);
            return this;
        }

        public IFluentBuilder WithSenderRouting<TSender, TRequest>(IAddress address)
            where TSender : ISender
            where TRequest : class
        {
            container.Resolve<IRequestRouter>().AddRequestRouting<TRequest, TSender>(address);
            return this;
        }

        public IFluentBuilder WithSubscriber<TSubscriber, TTopic>(IAddress address) where TSubscriber : ISubscriber
        {
            container.Resolve<ITopicRouter>().AddTopicRouting<TTopic, TSubscriber>(address);
            return this;
        }

        public IFluentBuilder WithTopicHandler<TTopic>(ITopicHandler<TTopic> handler)
        {
            container.Resolve<ITopicDispatcher>().Register(handler);
            return this;
        }

        public IFluentBuilder WithTopicHandler<TTopic>(TopicHandlerDelegate<TTopic> handler)
        {
            container.Resolve<ITopicDispatcher>().Register(handler);
            return this;
        }

        public IFluentBuilder WithAsyncTopicHandler<TTopic>(AsyncTopicHandlerDelegate<TTopic> handler)
        {
            container.Resolve<ITopicDispatcher>().RegisterAsync(handler);
            return this;
        }

        public IFluentBuilder WithTransport<TTransport>() where TTransport : ITransportConfig
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

            return this;
        }
    }
}
