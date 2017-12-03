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
    /// Factory for creating <see cref="INetMQSender"/>s
    /// </summary>
    public class NetMQSenderFactory : SenderFactoryBase<INetMQSender>
    {
        private ISerializer<byte[]> serializer;


        public NetMQSenderFactory(INetMQSenderMonitor monitor, ISerializer<byte[]> serializer)
            : base(monitor)
        {
            this.serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }


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
