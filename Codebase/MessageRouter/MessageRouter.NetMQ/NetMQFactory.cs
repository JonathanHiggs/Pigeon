using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageRouter.Addresses;
using MessageRouter.Monitors;
using MessageRouter.NetMQ.Receivers;
using MessageRouter.NetMQ.Senders;
using MessageRouter.Receivers;
using MessageRouter.Senders;
using MessageRouter.Serialization;
using NetMQ.Sockets;

namespace MessageRouter.NetMQ
{
    public class NetMQFactory : EndPointFactory<INetMQSender, INetMQReceiver>
    {
        private readonly ISerializer<byte[]> serializer;


        public NetMQFactory(ISenderMonitor<INetMQSender> senderMonitor, IReceiverMonitor<INetMQReceiver> receiverMonitor, ISerializer<byte[]> serializer)
            : base(senderMonitor, receiverMonitor)
        {
            this.serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }


        protected override INetMQReceiver CreateNewReceiver(IAddress address)
        {
            var socket = new RouterSocket();
            var receiver = new NetMQReceiver(socket, serializer);

            receiver.AddAddress(address);

            return receiver;
        }

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
