using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageRouter.Addresses;
using MessageRouter.Monitors;
using MessageRouter.Receivers;
using MessageRouter.Senders;

namespace MessageRouter
{
    public abstract class EndPointFactory<TSender, TReceiver> : IEndPointFactory<TSender, TReceiver>
        where TSender : ISender
        where TReceiver : IReceiver
    {
        private readonly ISenderMonitor<TSender> senderMonitor;
        private readonly IReceiverMonitor<TReceiver> receiverMonitor;


        public IMonitor SenderMonitor => senderMonitor;


        public Type SenderType => typeof(TSender);


        public Type ReceiverType => typeof(TReceiver);


        public IMonitor ReceiverMonitor => receiverMonitor;


        public EndPointFactory(ISenderMonitor<TSender> senderMonitor, IReceiverMonitor<TReceiver> receiverMonitor)
        {
            this.senderMonitor = senderMonitor ?? throw new ArgumentNullException(nameof(senderMonitor));
            this.receiverMonitor = receiverMonitor ?? throw new ArgumentNullException(nameof(receiverMonitor));
        }


        public IReceiver CreateReceiver(IAddress address)
        {
            return CreateAndAddReceiver(address);
        }


        public ISender CreateSender(IAddress address)
        {
            return CreateAndAddSender(address);
        }


        protected abstract TSender CreateNewSender(IAddress address);
        protected abstract TReceiver CreateNewReceiver(IAddress address);


        private TSender CreateAndAddSender(IAddress address)
        {
            var sender = CreateNewSender(address);
            senderMonitor.AddSender(sender);
            return sender;
        }


        private TReceiver CreateAndAddReceiver(IAddress address)
        {
            var receiver = CreateNewReceiver(address);
            receiverMonitor.AddReceiver(receiver);
            return receiver;
        }
    }
}
