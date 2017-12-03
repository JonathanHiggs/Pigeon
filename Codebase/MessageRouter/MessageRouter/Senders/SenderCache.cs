using MessageRouter.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Senders
{
    public class SenderCache : ISenderCache
    {
        private readonly IMessageRouter messageRouter;
        private readonly Dictionary<SenderRouting, ISender> senderCache = new Dictionary<SenderRouting, ISender>();
        private readonly Dictionary<Type, ISenderFactory> senderFactories = new Dictionary<Type, ISenderFactory>();


        public SenderCache(IMessageRouter messageRouter)
        {
            this.messageRouter = messageRouter ?? throw new ArgumentNullException(nameof(messageRouter));
        }


        public ISender SenderFor<TRequest>()
        {
            if (!messageRouter.RoutingFor<TRequest>(out var senderMapping))
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


        public void AddFactory<TSender>(SenderFactoryBase<TSender> senderFactory)
            where TSender : ISender
        {
            var senderType = typeof(TSender);

            if (senderFactories.ContainsKey(senderType))
                throw new InvalidOperationException($"SenderFactory already registered for {senderType.Name}");

            senderFactories.Add(senderType, senderFactory);
        }
    }
}
