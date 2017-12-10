﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageRouter.Addresses;
using MessageRouter.Messages;
using MessageRouter.Monitors;
using MessageRouter.Receivers;
using MessageRouter.Routing;
using MessageRouter.Senders;
using MessageRouter.Server;

namespace MessageRouter.Fluent
{
    public class RouterBuilder
    {
        private string name;

        private readonly RequestRouter requestRouter;
        private readonly MessageFactory messageFactory;
        private readonly RequestDispatcher requestDispatcher;

        private readonly SenderCache senderCache;
        private readonly ReceiverCache receiverCache;
        private readonly MonitorCache monitorCache;


        public RouterBuilder()
        {
            requestRouter = new RequestRouter();
            messageFactory = new MessageFactory();
            requestDispatcher = new RequestDispatcher();
            monitorCache = new MonitorCache();

            senderCache = new SenderCache(requestRouter, monitorCache, messageFactory);
            receiverCache = new ReceiverCache(monitorCache, messageFactory, requestDispatcher);
        }


        public RouterBuilder WithName(string name)
        {
            this.name = name;
            return this;
        }


        public RouterBuilder WithTransport<TSender, TReceiver>(ITransport<TSender, TReceiver> transport)
            where TSender : ISender
            where TReceiver : IReceiver
        {
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


        public Router Build()
        {
            return new Router(name, senderCache, monitorCache, receiverCache);
        }
    }
}