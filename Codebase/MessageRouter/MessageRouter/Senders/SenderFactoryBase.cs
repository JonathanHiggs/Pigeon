using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageRouter.Addresses;

namespace MessageRouter.Senders
{
    public abstract class SenderFactoryBase<TSender> : ISenderFactory
        where TSender : ISender
    {
        private readonly ISenderMonitor<TSender> senderMonitor;


        public SenderFactoryBase(ISenderMonitor<TSender> senderMonitor)
        {
            this.senderMonitor = senderMonitor ?? throw new ArgumentNullException(nameof(senderMonitor));
        }


        public ISender CreateSender(IAddress address)
        {
            if (null == address)
                throw new ArgumentNullException($"Non-null address needed to create a sender");

            return CreateAndAddToMonitor(address);
        }


        private TSender CreateAndAddToMonitor(IAddress address)
        {
            var sender = Create(address);
            senderMonitor.AddSender(sender);
            return sender;
        }


        protected abstract TSender Create(IAddress address);
    }
}
