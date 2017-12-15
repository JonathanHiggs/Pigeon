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
            monitorCache = new MonitorCache();
            requestRouter = new RequestRouter();
            messageFactory = new MessageFactory();
            topicDispatcher = new TopicDispatcher();
            requestDispatcher = new RequestDispatcher();

            senderCache = new SenderCache(requestRouter, monitorCache, messageFactory);
            receiverCache = new ReceiverCache(monitorCache, messageFactory, requestDispatcher);
            publisherCache = new PublisherCache(monitorCache, messageFactory);
            subscriberCache = new SubscriberCache(monitorCache, messageFactory, topicDispatcher);
        }


        public RouterBuilder WithTransport<TTransport, TSender, TReceiver>()
            where TTransport : ITransport<TSender, TReceiver>
            where TSender : ISender
            where TReceiver : IReceiver
        {
            var transport = Activator.CreateInstance<TTransport>();
            senderCache.AddFactory(transport.SenderFactory);
            receiverCache.AddFactory(transport.ReceiverFactory);
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


        public RouterBuilder WithHandler<TRequest, TResponse>(IRequestHandler<TRequest, TResponse> handler)
            where TRequest : class
            where TResponse : class
        {
            requestDispatcher.Register(handler);
            return this;
        }


        public RouterBuilder WithHandler<TRequest, TResponse>(RequestHandlerDelegate<TRequest, TResponse> handler)
            where TRequest : class
            where TResponse : class
        {
            requestDispatcher.Register(handler);
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
