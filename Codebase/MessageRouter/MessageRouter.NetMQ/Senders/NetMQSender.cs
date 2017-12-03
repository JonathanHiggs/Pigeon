using MessageRouter.Senders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageRouter.Addresses;
using MessageRouter.Messages;
using MessageRouter.Serialization;
using NetMQ;
using NetMQ.Sockets;

namespace MessageRouter.NetMQ.Senders
{
    /// <summary>
    /// NetMQ implementation of <see cref="IAsyncSender"/> that wraps a <see cref="DealerSocket"/> that connects to remotes
    /// and sends <see cref="Message"/> both synchronously and asynchronously
    /// </summary>
    public class NetMQSender : INetMQSender
    {
        private readonly List<IAddress> addresses = new List<IAddress>();
        private readonly ISerializer<byte[]> binarySerializer;
        private readonly IAsyncSocket asyncSocket;


        /// <summary>
        /// Gets an eumerable of the <see cref="IAddress"/> for remotes that the sender is connected to
        /// </summary>
        public IEnumerable<IAddress> Addresses => addresses;


        /// <summary>
        /// Gets the type of the message <see cref="ISerializer<>"/> the sender uses
        /// </summary>
        public Type SerializerType => typeof(ISerializer<byte[]>);


        /// <summary>
        /// Gets the inner pollable socket
        /// </summary>
        public ISocketPollable PollableSocket => asyncSocket.PollableSocket;


        /// <summary>
        /// Initializes a new instance of a NetMQAsyncSender
        /// </summary>
        /// <param name="asyncSocket">Inner socket</param>
        /// <param name="binarySerializer">Binary serializer</param>
        public NetMQSender(IAsyncSocket asyncSocket, ISerializer<byte[]> binarySerializer)
        {
            this.asyncSocket = asyncSocket ?? throw new ArgumentNullException(nameof(asyncSocket));
            this.binarySerializer = binarySerializer ?? throw new ArgumentNullException(nameof(binarySerializer));
        }

        
        public void AddAddress(IAddress address)
        {
            if (!addresses.Contains(address))
                addresses.Add(address);
        }

        
        public void RemoveAddress(IAddress address)
        {
            if (addresses.Contains(address))
                addresses.Remove(address);
        }


        public void ConnectAll()
        {
            foreach (var address in addresses)
                asyncSocket.Connect(address);
        }


        public void DisconnectAll()
        {
            foreach (var address in addresses)
                asyncSocket.Disconnect(address);
        }


        /// <summary>
        /// Asynchronously sends a <see cref="Message"/> to the connected remote <see cref="IReceiver"/> and returns the reponse <see cref="Message"/>
        /// Default 1 hour timeout
        /// </summary>
        /// <param name="message">Request message</param>
        /// <returns>Response message</returns>
        public async Task<Message> SendAndReceive(Message request)
        {
            return await SendAndReceive(request, TimeSpan.FromHours(1));
        }


        /// <summary>
        /// Asynchronously sends a <see cref="Message"/> to the connected remote <see cref="IReceiver"/> and returns the reponse <see cref="Message"/>
        /// </summary>
        /// <param name="message">Request message</param>
        /// <returns>Response message</returns>
        public async Task<Message> SendAndReceive(Message request, TimeSpan timeout)
        {
            var message = new NetMQMessage();
            message.AppendEmptyFrame();
            message.Append(binarySerializer.Serialize(request));

            var responseMessage = await asyncSocket.SendAndReceive(message, timeout.TotalMilliseconds);

            return binarySerializer.Deserialize<Message>(responseMessage[1].ToByteArray());
        }
    }
}
