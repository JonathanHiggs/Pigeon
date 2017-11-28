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
    public class NetMQSenderFactory : ISenderFactory
    {
        private readonly NetMQPoller poller;
        private readonly ISerializer<byte[]> binarySerializer = new BinarySerializer();


        public NetMQSenderFactory(NetMQPoller poller)
        {
            this.poller = poller ?? throw new ArgumentNullException(nameof(poller));
        }


        /// <summary>
        /// Creates a new instance of an <see cref="ISender"/> connected to the supplied <see cref="IAddress"/>
        /// </summary>
        /// <param name="address">Address of the remote the sender will connect to</param>
        /// <returns>Sender connected to the remote address</returns>
        public ISender Create(IAddress address)
        {
            var dealerSocket = new DealerSocket();
            var asyncSocket = new AsyncSocket(dealerSocket);
            var sender = new NetMQSender(asyncSocket, binarySerializer);

            poller.Add(dealerSocket);
            sender.Connect(address);

            return sender;
        }
    }
}
