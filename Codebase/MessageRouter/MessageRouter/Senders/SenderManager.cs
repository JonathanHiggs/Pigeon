using MessageRouter.Addresses;
using MessageRouter.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Senders
{
    /// <summary>
    /// Manages the state of <see cref="ISender"/>s and resolves a sender for a given request object type
    /// </summary>
    public class SenderManager : ISenderManager
    {
        private readonly ISenderFactory senderFactory;
        private readonly Dictionary<IAddress, ISender> senders = new Dictionary<IAddress, ISender>();
        private readonly Dictionary<IAddress, IAsyncSender> asyncSenders = new Dictionary<IAddress, IAsyncSender>();
        private readonly Dictionary<Type, ISender> routingTable = new Dictionary<Type, ISender>();
        private readonly Dictionary<Type, IAsyncSender> asyncRoutingTable = new Dictionary<Type, IAsyncSender>();
        private readonly object lockObj = new object();


        /// <summary>
        /// Initializes a new SenderFactory
        /// </summary>
        /// <param name="senderFactory">SenderFactory dependency</param>
        public SenderManager(ISenderFactory senderFactory)
        {
            this.senderFactory = senderFactory ?? throw new ArgumentNullException(nameof(senderFactory));
        }


        /// <summary>
        /// Registers an <see cref="IAddress"/> as the remote destination for the Request type
        /// </summary>
        /// <typeparam name="TRequest">Type of request object</typeparam>
        /// <param name="address">Address of remote</param>
        public void Add<TRequest>(IAddress address)
        {
            lock (lockObj)
            {
                foreach (var kv in routingTable)
                    if (kv.Key.IsAssignableFrom(typeof(TRequest)))
                        throw new SenderAlreadyRegisteredException(kv.Value, typeof(TRequest));

                var sender = GetOrCreate(address);

                routingTable.Add(typeof(TRequest), sender);
            }
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


        /// <summary>
        /// Resolves a <see cref="ISender"/> for the type of the request with the configured routing
        /// </summary>
        /// <typeparam name="TRequest">Request type</typeparam>
        /// <returns>Sender for the request type</returns>
        public ISender SenderFor<TRequest>()
        {
            foreach (var kv in routingTable)
                if (kv.Key.IsAssignableFrom(typeof(TRequest)))
                    return kv.Value;

            throw new SenderNotRegisteredException(typeof(TRequest));
        }


        public IAsyncSender AsyncSenderFor<TRequest>()
        {
            foreach (var kv in asyncRoutingTable)
                if (kv.Key.IsAssignableFrom(typeof(TRequest)))
                    return kv.Value;

            throw new SenderNotRegisteredException(typeof(TRequest));
        }


        /// <summary>
        /// Initializes a new instance of <see cref="SenderManager"/> for fluent construction
        /// </summary>
        /// <param name="senderFactory"><see cref="ISenderFactory"/> dependency</param>
        /// <returns>SenderManager</returns>
        public static SenderManager Create(ISenderFactory senderFactory)
        {
            return new SenderManager(senderFactory);
        }


        /// <summary>
        /// Adds a <see cref="ISender"/> at the given address resolvable for the request type
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <param name="address"></param>
        /// <returns></returns>
        public SenderManager Route<TRequest>(IAddress address)
        {
            Add<TRequest>(address);
            return this;
        }


        protected ISender GetOrCreate(IAddress address)
        {
            if (senders.ContainsKey(address))
                return senders[address];

            var sender = senderFactory.Create(address);

            senders[address] = sender;

            return sender;
        }


        protected IAsyncSender GetOrCreateAsync(IAddress address)
        {
            if (asyncSenders.ContainsKey(address))
                return asyncSenders[address];

            var sender = senderFactory.CreateAsync(address);

            asyncSenders[address] = sender;

            return sender;
        }
    }
}
