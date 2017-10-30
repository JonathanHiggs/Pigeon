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
        private readonly Dictionary<Type, ISender> routingTable = new Dictionary<Type, ISender>();
        private readonly object lockObj = new object();


        public SenderManager(ISenderFactory senderFactory)
        {
            this.senderFactory = senderFactory ?? throw new ArgumentNullException(nameof(senderFactory));
        }


        public void Add<TRequest>(IAddress address)
        {
            lock (lockObj)
            {
                foreach (var kv in routingTable)
                    if (kv.Key.IsAssignableFrom(typeof(TRequest)))
                        throw new SenderAlreadyRegisteredException<TRequest>(kv.Value);

                var sender = senderFactory.Create(address);

                routingTable.Add(typeof(TRequest), sender);
            }
        }

        public ISender SenderFor<TRequest>()
        {
            foreach (var kv in routingTable)
                if (kv.Key.IsAssignableFrom(typeof(TRequest)))
                    return kv.Value;

            throw new SenderNotRegisteredException<TRequest>();
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
    }
}
