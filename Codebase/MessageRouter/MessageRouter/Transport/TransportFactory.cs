using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageRouter.Addresses;
using MessageRouter.Monitors;
using MessageRouter.Receivers;
using MessageRouter.Senders;

namespace MessageRouter.Transport
{
    /// <summary>
    /// Abstract implementation of <see cref="ITransportFactory{TSender, TReceiver}"/> with common methods implemented for convienence
    /// </summary>
    /// <typeparam name="TSender">The implementation of <see cref="ISender"/> this factory creates</typeparam>
    /// <typeparam name="TReceiver">The implementation of <see cref="IReceiver"/> this factory creates</typeparam>
    public abstract class TransportFactory<TSender, TReceiver> : ITransportFactory<TSender, TReceiver>
        where TSender : ISender
        where TReceiver : IReceiver
    {
        private readonly ISenderMonitor<TSender> senderMonitor;
        private readonly IReceiverMonitor<TReceiver> receiverMonitor;


        /// <summary>
        /// Gets the <see cref="IMonitor"/> for <see cref="TSender"/>s
        /// </summary>
        public IMonitor SenderMonitor => senderMonitor;


        /// <summary>
        /// Gets the type of <see cref="ISender"/>s this factory creates
        /// </summary>
        public Type SenderType => typeof(TSender);


        /// <summary>
        /// Gets the <see cref="IMonitor"/> for <see cref="TReceiver"/>s
        /// </summary>
        public IMonitor ReceiverMonitor => receiverMonitor;


        /// <summary>
        /// Gets the type of <see cref="IReceiver"/>s this factory creates
        /// </summary>
        public Type ReceiverType => typeof(TReceiver);


        /// <summary>
        /// Initializes a new instance of <see cref="TransportFactory{TSender, TReceiver}"/>
        /// </summary>
        /// <param name="senderMonitor">Monitor that <see cref="TSender"/>s will be added to</param>
        /// <param name="receiverMonitor">Monitor that <see cref="TReceiver"/>s will be added to</param>
        public TransportFactory(ISenderMonitor<TSender> senderMonitor, IReceiverMonitor<TReceiver> receiverMonitor)
        {
            this.senderMonitor = senderMonitor ?? throw new ArgumentNullException(nameof(senderMonitor));
            this.receiverMonitor = receiverMonitor ?? throw new ArgumentNullException(nameof(receiverMonitor));
        }


        /// <summary>
        /// Creates a new instance of a <see cref="IReceiver"/> bound to the supplied <see cref="IAddress"/>
        /// </summary>
        /// <param name="address">Address of local bound endpoint</param>
        /// <returns>Receiver bound to the address</returns>
        public IReceiver CreateReceiver(IAddress address)
        {
            return CreateAndAddReceiver(address);
        }


        /// <summary>
        /// Constructs a new instance of an <see cref="ISender"/> connected to the supplied <see cref="IAddress"/>
        /// </summary>
        /// <param name="address">Address of the remote the sender will connect to</param>
        /// <returns>Sender connected to the remote address</returns>
        public ISender CreateSender(IAddress address)
        {
            return CreateAndAddSender(address);
        }


        /// <summary>
        /// Constructs a new instance of an <see cref="TSender"/> connected to the supplied <see cref="IAddress"/>
        /// </summary>
        /// <param name="address">Address of the remote the sender will connect to</param>
        /// <returns>Sender connected to the remote address</returns>
        protected abstract TSender CreateNewSender(IAddress address);


        /// <summary>
        /// Creates a new instance of a <see cref="TReceiver"/> bound to the supplied <see cref="IAddress"/>
        /// </summary>
        /// <param name="address">Address of local bound endpoint</param>
        /// <returns>Receiver bound to the address</returns>
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
