using MessageRouter.Receivers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageRouter.Addresses;
using MessageRouter.Serialization;
using NetMQ.Sockets;

namespace MessageRouter.NetMQ
{
    public class NetMQReceiverFactory : IReceiverFactory
    {
        private readonly ISerializer<byte[]> binarySerializer = new BinarySerializer();
        

        public IReceiver Create(IAddress address)
        {
            var socket = new RouterSocket();
            var receiver = new NetMQReceiver(socket, binarySerializer);

            receiver.Bind(address);

            return receiver;
        }
    }
}
