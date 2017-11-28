using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageRouter.Addresses;
using MessageRouter.Diagnostics;

namespace MessageRouter.Senders
{
    // ToDo: This should be MessageRouter
    public abstract class SenderMonitor : ISenderMonitor
    {
        private readonly ISenderFactory senderFactory;
        private readonly Dictionary<Type, IAddress> routingTable = new Dictionary<Type, IAddress>();
        private readonly Dictionary<IAddress, ISender> senderMapping = new Dictionary<IAddress, ISender>();


        public SenderMonitor(ISenderFactory senderFactory)
        {
            this.senderFactory = senderFactory ?? throw new ArgumentNullException(nameof(senderFactory));
        }


        public void AddRequestMapping<TRequest>(IAddress address)
        {
            foreach (var kv in routingTable)
                if (kv.Key.IsAssignableFrom(typeof(TRequest)))
                    if (kv.Value != address)
                        throw new RequestAlreadyMappedException(typeof(TRequest), address, kv.Key, kv.Value);
                    else
                        return;

            routingTable.Add(typeof(TRequest), address);
        }


        public ISender SenderFor<TRequest>()
        {
            var address = GetAddressForRequest(typeof(TRequest));

            if (!senderMapping.TryGetValue(address, out var sender))
            {
                // Create sender
                sender = senderFactory.Create(address);
                senderMapping[address] = sender;
            }

            return sender;
        }


        public abstract void Start();


        public abstract void Stop();


        private IAddress GetAddressForRequest(Type requestType)
        {
            foreach (var kv in routingTable)
                if (kv.Key.IsAssignableFrom(requestType))
                    return kv.Value;

            throw new RequestNotMappedException(requestType);
        }
    }
}
