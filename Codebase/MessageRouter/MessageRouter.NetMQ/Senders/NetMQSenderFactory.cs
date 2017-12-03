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
    /// Factory for NetMQ <see cref="ISender"/>s and <see cref="IAsyncSender"/>s
    /// </summary>
    public class NetMQSenderFactory : SenderFactoryBase<NetMQSender>
    {
        private ISerializer<byte[]> serializer;


        public NetMQSenderFactory(NetMQSenderMonitor monitor, ISerializer<byte[]> serializer)
            : base(monitor)
        {
            this.serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }


        public override NetMQSender Create(IAddress address)
        {
            var dealerSocket = new DealerSocket();
            var asyncSocket = new AsyncSocket(dealerSocket);
            var sender = new NetMQSender(asyncSocket, serializer);

            sender.AddAddress(address);

            SenderMonitor.AddSender(sender);

            return sender;
        }
    }
}
