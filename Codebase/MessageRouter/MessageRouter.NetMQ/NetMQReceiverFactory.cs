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
    /// <summary>
    /// Factory for NetMQ <see cref="IReceiver"/>s and <see cref="IAsyncReceiver"/>s
    /// </summary>
    public class NetMQReceiverFactory : IReceiverFactory
    {
        private readonly ISerializer<byte[]> binarySerializer = new BinarySerializer();


        /// <summary>
        /// Creates a new instance of a <see cref="IReceiver"/> bound to the supplied <see cref="IAddress"/>
        /// </summary>
        /// <param name="address">Address of local bound endpoint</param>
        /// <returns>Receiver bound to the address</returns>
        public IReceiver Create(IAddress address)
        {
            var socket = new RouterSocket();
            var receiver = new NetMQReceiver(socket, binarySerializer);

            receiver.Add(address);

            return receiver;
        }


        /// <summary>
        /// Creates a new instance of a <see cref="IAsyncReceiver"/> bound to the supplied <see cref="IAddress"/>
        /// </summary>
        /// <param name="address">Address of local bound enpoint</param>
        /// <returns>AsyncReceiver bound to the address</returns>
        public IAsyncReceiver CreateAsync(IAddress address)
        {
            var socket = new RouterSocket();
            var receiver = new NetMQAsyncReceiver(socket, binarySerializer);

            receiver.Add(address);

            return receiver;
        }
    }
}
