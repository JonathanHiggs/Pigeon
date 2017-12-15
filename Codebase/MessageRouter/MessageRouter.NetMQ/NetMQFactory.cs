using System;

using MessageRouter.Addresses;
using MessageRouter.Messages;
using MessageRouter.NetMQ.Publishers;
using MessageRouter.NetMQ.Receivers;
using MessageRouter.NetMQ.Senders;
using MessageRouter.NetMQ.Subscribers;
using MessageRouter.Receivers;
using MessageRouter.Senders;
using MessageRouter.Serialization;
using MessageRouter.Transport;
using NetMQ.Sockets;

namespace MessageRouter.NetMQ
{
    /// <summary>
    /// Combined factory for NetMQ specific implementations of <see cref="INetMQSender"/>s and <see cref="INetMQReceiver"/>s
    /// </summary>
    public class NetMQFactory : TransportFactory<INetMQSender, INetMQReceiver, INetMQPublisher, INetMQSubscriber>
    {
        private readonly ISerializer<byte[]> serializer;


        /// <summary>
        /// Initializes a new instance of <see cref="NetMQFactory"/>
        /// </summary>
        /// <param name="monitor">Monitor that all NetMQ transports will be added to</param>
        /// <param name="serializer"><see cref="ISerializer{TData}"/> that converts <see cref="Message"/> to binary for sending over the wire</param>
        public NetMQFactory(INetMQMonitor monitor, ISerializer<byte[]> serializer)
            : base(monitor, monitor, null, null)
        {
            if (null == monitor)
                throw new ArgumentNullException(nameof(monitor));

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


        /// <summary>
        /// Creates a new instance of a <see cref="INetMQPublisher"/> bound to the supplied <see cref="IAddress"/>
        /// </summary>
        /// <param name="address"><see cref="IAddress"/> publisher binds to</param>
        /// <returns><see cref="INetMQPublisher"/> bound to the <see cref="IAddress"/></returns>
        protected override INetMQPublisher CreateNewPublisher(IAddress address)
        {
            throw new NotSupportedException();
        }


        /// <summary>
        /// Creates a new instance of a <see cref="INetMQSubscriber"/> connected to the supplied <see cref="IAddress"/>
        /// </summary>
        /// <param name="address"><see cref="IAddress"/> of the remote publishing endpoint</param>
        /// <returns><see cref="INetMQSubscriber"/> connected to the <see cref="IAddress"/></returns>
        protected override INetMQSubscriber CreateNewSubscriber(IAddress address)
        {
            throw new NotSupportedException();
        }
    }
}
