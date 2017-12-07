using MessageRouter.Addresses;
using MessageRouter.Senders;
using MessageRouter.Serialization;
using NetMQ;
using NetMQ.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.NetMQ.Senders
{
    /// <summary>
    /// Factory for creating <see cref="INetMQSender"/>s that are actively managed by <see cref="INetMQSenderMonitor"/>s
    /// </summary>
    public class NetMQSenderFactory : SenderFactory<INetMQSender>
    {
        private ISerializer<byte[]> serializer;


        /// <summary>
        /// Initializes a new instance of <see cref="NetMQSenderFactory"/>
        /// </summary>
        /// <param name="monitor">A <see cref="INetMQSenderMonitor"/> that will actively monitor and poll the transport for constructed <see cref="INetMQSender"/>s</param>
        /// <param name="serializer">A serializer that will convert request and response messages to a binary format for transport along the wire</param>
        public NetMQSenderFactory(INetMQSenderMonitor monitor, ISerializer<byte[]> serializer)
            : base(monitor)
        {
            this.serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        
        /// <summary>
        /// Creates a new instance of a <see cref="INetMQSender"/>
        /// </summary>
        /// <param name="address"><see cref="IAddress"/> the <see cref="INetMQSender"/> will connect to</param>
        /// <returns>A newly created <see cref="INetMQSender"/> connected to the given address</returns>
        protected override INetMQSender Create(IAddress address)
        {
            var dealerSocket = new DealerSocket();
            var asyncSocket = new AsyncSocket(dealerSocket);
            var sender = new NetMQSender(asyncSocket, serializer);

            sender.AddAddress(address);

            return sender;
        }
    }
}
