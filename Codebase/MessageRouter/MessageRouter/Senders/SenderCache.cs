using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using MessageRouter.Addresses;
using MessageRouter.Messages;
using MessageRouter.Monitors;
using MessageRouter.Receivers;
using MessageRouter.Routing;

namespace MessageRouter.Senders
{
    /// <summary>
    /// Manages the resolution and lifecycle of <see cref="ISender"/>s
    /// </summary>
    public class SenderCache : ISenderCache
    {
        private readonly IRequestRouter requestRouter;
        private readonly IMonitorCache monitorCache;
        private readonly IMessageFactory messageFactory;
        private readonly Dictionary<SenderRouting, ISender> senderCache = new Dictionary<SenderRouting, ISender>();
        private readonly Dictionary<Type, ISenderFactory> senderFactories = new Dictionary<Type, ISenderFactory>();


        /// <summary>
        /// Gets a readonly collection of <see cref="ISenderFactory"/>s for creating <see cref="ISender"/>s at runtime
        /// </summary>
        public IReadOnlyCollection<ISenderFactory> Factories => senderFactories.Values;


        /// <summary>
        /// Initializes a new instance of a <see cref="SenderCache"/>
        /// </summary>
        /// <param name="requestRouter">Router to manage resolving request types to <see cref="SenderRouting"/>s</param>
        /// <param name="monitorCache">Stores <see cref="IMonitor"/>s that actively manage <see cref="ISender"/></param>
        /// <param name="messageFactory">Creates and extracts <see cref="Message"/>s that are sent over the wire</param>
        public SenderCache(IRequestRouter requestRouter, IMonitorCache monitorCache, IMessageFactory messageFactory)
        {
            this.requestRouter = requestRouter ?? throw new ArgumentNullException(nameof(requestRouter));
            this.monitorCache = monitorCache ?? throw new ArgumentNullException(nameof(monitorCache));
            this.messageFactory = messageFactory ?? throw new ArgumentNullException(nameof(messageFactory));
        }


        /// <summary>
        /// Retrieves a <see cref="ISender"/> for the request type to connected to a remote <see cref="IAddress"/> registered in the <see cref="IRequestRouter"/>
        /// </summary>
        /// <typeparam name="TRequest">Request type</typeparam>
        /// <returns>Matching <see cref="ISender"/> for the given request type</returns>
        public ISender SenderFor<TRequest>()
        {
            if (!requestRouter.RoutingFor<TRequest>(out var senderRouting))
                throw new KeyNotFoundException($"No mapping found for {typeof(TRequest).Name}");

            if (!senderCache.TryGetValue(senderRouting, out var sender))
            {
                if (!senderFactories.TryGetValue(senderRouting.SenderType, out var factory))
                    throw new KeyNotFoundException($"No SenderFactory found for {senderRouting.SenderType} needed for request type {typeof(TRequest).Name}");

                sender = factory.CreateSender(senderRouting.Address);  // SenderRouting.Address is not-null
                senderCache.Add(senderRouting, sender);
            }

            return sender;
        }


        /// <summary>
        /// Adds a <see cref="ISenderFactory{TSender}"/> to the registered factories
        /// </summary>
        /// <typeparam name="TSender">Transport specific implementation of <see cref="ISender"/></typeparam>
        /// <param name="factory">Factory used to create <see cref="ISender"/>s at when required to send first message to remote <see cref="IReceiver"/></param>
        public void AddFactory<TSender>(ISenderFactory<TSender> factory)
            where TSender : ISender
        {
            if (null == factory)
                throw new ArgumentNullException(nameof(factory));

            if (senderFactories.ContainsKey(factory.SenderType))
                return;

            senderFactories.Add(factory.SenderType, factory);
            monitorCache.AddMonitor(factory.SenderMonitor);
        }


        /// <summary>
        /// Dispatches a request asynchronously through an internally resolved <see cref="ISender"/> to a remote
        /// <see cref="IReceiver"/> with a default timeout of one hour
        /// </summary>
        /// <typeparam name="TRequest">Request type</typeparam>
        /// <typeparam name="TResponse">Expected response type</typeparam>
        /// <param name="request">Request object</param>
        /// <returns>Response object</returns>
        public async Task<TResponse> Send<TRequest, TResponse>(TRequest request)
            where TRequest : class
            where TResponse : class
        {
            return await Send<TRequest, TResponse>(request, TimeSpan.FromHours(1));
        }


        /// <summary>
        /// Dispatches a request asynchronously through an internally resolved <see cref="ISender"/> to a remote
        /// <see cref="IReceiver"/>
        /// </summary>
        /// <typeparam name="TRequest">Request type</typeparam>
        /// <typeparam name="TResponse">Expected response type</typeparam>
        /// <param name="request">Request object</param>
        /// <param name="timeout">Time to wait for a response before throwing an exception</param>
        /// <returns>Response object</returns>
        public async Task<TResponse> Send<TRequest, TResponse>(TRequest request, TimeSpan timeout)
            where TRequest : class
            where TResponse : class
        {
            if (null == request)
                throw new ArgumentNullException(nameof(request));

            var sender = SenderFor<TRequest>();
            var requestMessage = messageFactory.CreateMessage(request);
            var responseMessage = await sender.SendAndReceive(requestMessage, timeout);
            var response = messageFactory.ExtractMessage<TResponse>(responseMessage);
            return response;
        }
    }
}
