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
        private readonly ISerializer<byte[]> binarySerializer = new BinarySerializer();
        private NetMQSenderMonitor monitor;


        public NetMQSenderFactory(NetMQSenderMonitor monitor)
        {
            this.monitor = monitor ?? throw new ArgumentNullException(nameof(monitor));
        }

        public override SenderMonitorBase<NetMQSender> CreateMonitor()
        {
            return monitor;
        }

        public override NetMQSender CreateSender(IAddress address)
        {
            var dealerSocket = new DealerSocket();
            var asyncSocket = new AsyncSocket(dealerSocket);
            var sender = new NetMQSender(asyncSocket, binarySerializer);
            
            sender.Connect(address);

            return sender;
        }
    }
}
