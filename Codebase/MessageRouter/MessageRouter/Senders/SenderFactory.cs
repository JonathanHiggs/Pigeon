using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageRouter.Addresses;
using MessageRouter.Monitors;

namespace MessageRouter.Senders
{
    /// <summary>
    /// Abstract <see cref="ISenderFactory"/> base class that has methods common to all transports already implemented
    /// </summary>
    /// <typeparam name="TSender">The type of <see cref="ISender"/> that this factory constructs</typeparam>
    public abstract class SenderFactory<TSender> : ISenderFactory<TSender>
        where TSender : ISender
    {
        private readonly ISenderMonitor<TSender> senderMonitor;


        /// <summary>
        /// Initializes a new instance of <see cref="SenderFactory{TSender}"/>
        /// </summary>
        /// <param name="senderMonitor">The <see cref="ISenderMonitor{TSender}"/> that this factory will add newly created <see cref="TSender"/>s to for active monitoring</param>
        public SenderFactory(ISenderMonitor<TSender> senderMonitor)
        {
            this.senderMonitor = senderMonitor ?? throw new ArgumentNullException(nameof(senderMonitor));
        }


        /// <summary>
        /// Gets the <see cref="ISenderMonitor"/> associated with <see cref="TSender"/>s
        /// </summary>
        public IMonitor SenderMonitor => senderMonitor;


        /// <summary>
        /// Creates a new instance of an <see cref="ISender"/> connected to the supplied <see cref="IAddress"/>
        /// </summary>
        /// <param name="address">Address of the remote the sender will connect to</param>
        /// <returns>Sender connected to the remote address</returns>
        public ISender CreateSender(IAddress address)
        {
            if (null == address)
                throw new ArgumentNullException($"Non-null address needed to create a sender");

            return CreateAndAddToMonitor(address);
        }


        /// <summary>
        /// Creates a new instance of a <see cref="TSender"/>
        /// </summary>
        /// <param name="address"><see cref="IAddress"/> the <see cref="TSender"/> will connect to</param>
        /// <returns>A newly created <see cref="TSender"/> connected to the given address</returns>
        protected abstract TSender Create(IAddress address);


        private TSender CreateAndAddToMonitor(IAddress address)
        {
            var sender = Create(address);
            senderMonitor.AddSender(sender);
            return sender;
        }
    }
}
