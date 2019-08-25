using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Pigeon.Addresses;
using Pigeon.Diagnostics;
using Pigeon.Monitors;
using Pigeon.Receivers;
using Pigeon.Routing;

namespace Pigeon.Senders
{
    /// <summary>
    /// Manages the resolution and life-cycle of <see cref="ISender"/>s
    /// </summary>
    public class SenderCache : ISenderCache
    {
        private readonly IRequestRouter requestRouter;
        private readonly IMonitorCache monitorCache;
        private readonly Dictionary<SenderRouting, ISender> senders = new Dictionary<SenderRouting, ISender>();
        private readonly Dictionary<Type, ISenderFactory> factories = new Dictionary<Type, ISenderFactory>();


        /// <summary>
        /// Gets a read-only collection of <see cref="ISenderFactory"/>s for creating <see cref="ISender"/>s at runtime
        /// </summary>
        public IReadOnlyCollection<ISenderFactory> Factories => factories.Values;


        /// <summary>
        /// Initializes a new instance of <see cref="SenderCache"/>
        /// </summary>
        /// <param name="requestRouter">Router to manage resolving request types to <see cref="SenderRouting"/>s</param>
        /// <param name="monitorCache">Stores <see cref="IMonitor"/>s that actively manage <see cref="ISender"/></param>
        public SenderCache(IRequestRouter requestRouter, IMonitorCache monitorCache)
        {
            this.requestRouter = requestRouter ?? throw new ArgumentNullException(nameof(requestRouter));
            this.monitorCache = monitorCache ?? throw new ArgumentNullException(nameof(monitorCache));
        }


        /// <summary>
        /// Retrieves a <see cref="ISender"/> for the request type to connected to a remote <see cref="IAddress"/> registered in the <see cref="IRequestRouter"/>
        /// </summary>
        /// <typeparam name="TRequest">Request type</typeparam>
        /// <returns>Matching <see cref="ISender"/> for the given request type</returns>
        public ISender SenderFor<TRequest>()
        {
            if (!requestRouter.RoutingFor<TRequest>(out var senderRouting))
                throw new MissingSenderRouting(typeof(TRequest));

            if (!senders.TryGetValue(senderRouting, out var sender))
            {
                if (!factories.TryGetValue(senderRouting.SenderType, out var factory))
                    throw new MissingFactoryException(senderRouting.SenderType, typeof(SenderCache));

                sender = factory.CreateSender(senderRouting.Address);  // SenderRouting.Address is not-null
                senders.Add(senderRouting, sender);
            }

            return sender;
        }


        /// <summary>
        /// Adds a <see cref="ISenderFactory{TSender}"/> to the registered factories
        /// </summary>
        /// <param name="factory">Factory used to create <see cref="ISender"/>s at when required to send first message to remote <see cref="IReceiver"/></param>
        public void AddFactory(ISenderFactory factory)
        {
            if (factory is null)
                throw new ArgumentNullException(nameof(factory));

            if (factories.ContainsKey(factory.SenderType))
                return;

            factories.Add(factory.SenderType, factory);
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
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            var sender = SenderFor<TRequest>();
            var response = await sender.SendAndReceive(request, timeout);
            return (TResponse)response;
        }
    }
}
