using MessageRouter.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Senders
{
    /// <summary>
    /// Manages the resolution and lifecycle of <see cref="ISender"/>s
    /// </summary>
    public class SenderCache : ISenderCache
    {
        private readonly IRouter router;
        private readonly Dictionary<SenderRouting, ISender> senderCache = new Dictionary<SenderRouting, ISender>();
        private readonly Dictionary<Type, ISenderFactory> senderFactories = new Dictionary<Type, ISenderFactory>();


        /// <summary>
        /// Gets a readonly collection of <see cref="ISenderFactory"/>s
        /// </summary>
        public IReadOnlyCollection<ISenderFactory> Factories => senderFactories.Values;


        /// <summary>
        /// Initializes a new instance of a <see cref="SenderCache"/>
        /// </summary>
        /// <param name="router">Router to manage resolving request types to <see cref="SenderRouting"/>s</param>
        public SenderCache(IRouter router)
        {
            this.router = router ?? throw new ArgumentNullException(nameof(router));
        }


        /// <summary>
        /// Retrieves a <see cref="ISender"/> for the request type depending on registered routing
        /// </summary>
        /// <typeparam name="TRequest">Request type</typeparam>
        /// <returns>Matching <see cref="ISender"/> for the given request type</returns>
        public ISender SenderFor<TRequest>()
        {
            if (!router.RoutingFor<TRequest>(out var senderMapping))
                throw new KeyNotFoundException($"No mapping found for {typeof(TRequest).Name}");

            if (!senderCache.TryGetValue(senderMapping, out var sender))
            {
                if (!senderFactories.TryGetValue(senderMapping.SenderType, out var factory))
                    throw new KeyNotFoundException($"No SenderFactory found for {senderMapping.SenderType} needed for request type {typeof(TRequest).Name}");

                sender = factory.CreateSender(senderMapping.Address);
                senderCache.Add(senderMapping, sender);
            }

            return sender;
        }


        /// <summary>
        /// Adds a <see cref="ISenderFactory{TSender}"/> to the registered factories
        /// </summary>
        /// <typeparam name="TSender"></typeparam>
        /// <param name="senderFactory"></param>
        public void AddFactory<TSender>(ISenderFactory<TSender> senderFactory)
            where TSender : ISender
        {
            var senderType = typeof(TSender);

            if (senderFactories.ContainsKey(senderType))
                throw new InvalidOperationException($"SenderFactory already registered for {senderType.Name}");

            senderFactories.Add(senderType, senderFactory);
        }
    }
}
