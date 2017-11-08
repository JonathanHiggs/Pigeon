using MessageRouter.Addresses;
using MessageRouter.Senders;
using MessageRouter.Serialization;
using NetMQ.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.NetMQ
{
    public class NetMQSenderFactory : ISenderFactory
    {
        private readonly ISerializer<byte[]> binarySerializer = new BinarySerializer();


        public ISender Create(IAddress address)
        {
            var socket = new DealerSocket();
            var sender = new NetMQSender(socket, binarySerializer);
            sender.Connect(address);
            return sender;
        }
    }
}
