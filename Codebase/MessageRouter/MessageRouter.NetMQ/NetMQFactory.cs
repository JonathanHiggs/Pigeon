using System;

using MessageRouter.Addresses;
using MessageRouter.Messages;
using MessageRouter.NetMQ.Receivers;
using MessageRouter.NetMQ.Senders;
using MessageRouter.Receivers;
using MessageRouter.Senders;
using MessageRouter.Serialization;

using NetMQ.Sockets;

namespace MessageRouter.NetMQ
{
    /// <summary>
    /// Factory for <see cref="INetMQSender"/>s and <see cref="INetMQReceiver"/>s
    /// </summary>
    public class NetMQFactory : TransportFactory<INetMQSender, INetMQReceiver>
    {
        private readonly ISerializer<byte[]> serializer;


        /// <summary>
        /// Initializes a new instance of <see cref="NetMQFactory"/>
        /// </summary>
        /// <param name="senderMonitor">Monitor that <see cref="INetMQSender"/>s will be added to</param>
        /// <param name="receiverMonitor">Monitor that <see cref="INetMQReceiver"/>s will be added to</param>
        /// <param name="serializer"><see cref="ISerializer{TData}"/> that converts <see cref="Message"/> to binary for sending over the wire</param>
        public NetMQFactory(ISenderMonitor<INetMQSender> senderMonitor, IReceiverMonitor<INetMQReceiver> receiverMonitor, ISerializer<byte[]> serializer)
            : base(senderMonitor, receiverMonitor)
        {
            this.serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }


        /// <summary>
        /// Creates a new instance of a <see cref="INetMQReceiver"/> bound to the supplied <see cref="IAddress"/>
        /// </summary>
        /// <param name="address">Address of local bound endpoint</param>
        /// <returns>Receiver bound to the address</returns>
        protected override INetMQReceiver CreateNewReceiver(IAddress address)
        {
            var socket = new RouterSocket();
            var receiver = new NetMQReceiver(socket, serializer);

            receiver.AddAddress(address);

            return receiver;
        }


        /// <summary>
        /// Constructs a new instance of an <see cref="INetMQSender"/> connected to the supplied <see cref="IAddress"/>
        /// </summary>
        /// <param name="address">Address of the remote the sender will connect to</param>
        /// <returns>Sender connected to the remote address</returns>
        protected override INetMQSender CreateNewSender(IAddress address)
        {
            var dealerSocket = new DealerSocket();
            var asyncSocket = new AsyncSocket(dealerSocket);
            var sender = new NetMQSender(asyncSocket, serializer);

            sender.AddAddress(address);

            return sender;
        }
    }
}
