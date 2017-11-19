using MessageRouter.Senders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageRouter.Addresses;
using NetMQ;
using MessageRouter.Diagnostics;

namespace MessageRouter.NetMQ
{
    public class NetMQSenderManager : ISenderManager
    {
        private readonly NetMQSenderFactory senderFactory;
        private readonly NetMQPoller poller;
        private readonly Dictionary<IAddress, IAsyncSender> asyncSenders = new Dictionary<IAddress, IAsyncSender>();
        private readonly Dictionary<Type, IAsyncSender> asyncRoutingTable = new Dictionary<Type, IAsyncSender>();
        private readonly object lockObj = new object();


        public NetMQSenderManager(NetMQSenderFactory senderFactory, NetMQPoller poller)
        {
            this.senderFactory = senderFactory ?? throw new ArgumentNullException(nameof(senderFactory));
            this.poller = poller ?? throw new ArgumentNullException(nameof(poller));
        }


        public void Add<TRequest>(IAddress address)
        {
            throw new NotImplementedException();
        }

        public void AddAsync<TRequest>(IAddress address)
        {
            lock (lockObj)
            {
                foreach (var kv in asyncRoutingTable)
                    if (kv.Key.IsAssignableFrom(typeof(TRequest)))
                        throw new SenderAlreadyRegisteredException(kv.Value, typeof(TRequest));

                var sender = GetOrCreateAsync(address);

                asyncRoutingTable.Add(typeof(TRequest), sender);
            }
        }

        public IAsyncSender AsyncSenderFor<TRequest>()
        {
            foreach (var kv in asyncRoutingTable)
                if (kv.Key.IsAssignableFrom(typeof(TRequest)))
                    return kv.Value;

            throw new SenderNotRegisteredException(typeof(TRequest));
        }

        public ISender SenderFor<TRequest>()
        {
            throw new NotImplementedException();
        }

        private IAsyncSender GetOrCreateAsync(IAddress address)
        {
            if (asyncSenders.ContainsKey(address))
                return asyncSenders[address];
            
            var sender = senderFactory.CreateAsync(address) as NetMQAsyncSender;

            // Move this to factory
            poller.Add(sender.PollableSocket);

            asyncSenders[address] = sender;

            return sender;
        }
    }
}
