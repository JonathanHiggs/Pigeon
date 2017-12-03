using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageRouter.Addresses;

namespace MessageRouter.Senders
{
    public abstract class SenderFactoryBase<TSender> : ISenderFactory<TSender>
        where TSender : ISender
    {
        private readonly ISenderMonitor<TSender> senderMonitor;


        public ISenderMonitor<TSender> SenderMonitor => senderMonitor;


        public SenderFactoryBase(ISenderMonitor<TSender> senderMonitor)
        {
            this.senderMonitor = senderMonitor ?? throw new ArgumentNullException(nameof(senderMonitor));
        }


        public ISender CreateSender(IAddress address)
        {
            return CreateAndAddToMonitor(address);
        }


        private TSender CreateAndAddToMonitor(IAddress address)
        {
            var sender = Create(address);
            senderMonitor.AddSender(sender);
            return sender;
        }


        public abstract TSender Create(IAddress address);
    }
}
