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

namespace MessageRouter.NetMQ
{
    /// <summary>
    /// NetMQ implementation of <see cref="IAsyncSender"/> that wraps a <see cref="DealerSocket"/> that connects to remotes
    /// and sends <see cref="Message"/> both synchronously and asynchronously
    /// </summary>
    public class NetMQSender : ISender, INetMQSender
    {
        private readonly List<IAddress> addresses = new List<IAddress>();
        private readonly ISerializer<byte[]> binarySerializer;
        private readonly AsyncSocket socket;


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
        public ISocketPollable PollableSocket => socket.PollableSocket;


        /// <summary>
        /// Initializes a new instance of a NetMQAsyncSender
        /// </summary>
        /// <param name="socket">Inner socket</param>
        /// <param name="binarySerializer">Binary serializer</param>
        public NetMQSender(AsyncSocket socket, ISerializer<byte[]> binarySerializer)
        {
            this.socket = socket ?? throw new ArgumentNullException(nameof(socket));
            this.binarySerializer = binarySerializer ?? throw new ArgumentNullException(nameof(binarySerializer));
        }


        /// <summary>
        /// Initializes a connection to a remote for the given <see cref="IAddress"/>
        /// </summary>
        /// <param name="address">Address for a remote</param>
        public void Connect(IAddress address)
        {
            if (!addresses.Contains(address))
            {
                socket.Connect(address);
                addresses.Add(address);
            }
        }


        /// <summary>
        /// Disconects the sender from the remote for the given <see cref="IAddress"/>
        /// </summary>
        /// <param name="address">Address for a remote</param>
        public void Disconnect(IAddress address)
        {
            if (addresses.Contains(address))
            {
                socket.Disconnect(address);
                addresses.Remove(address);
            }
        }


        /// <summary>
        /// Sends a request <see cref="Message"/> to the connected remote<see cref="IReeceiver"/> and returns the response <see cref="Message"/>
        /// </summary>
        /// <param name="message">Request message</param>
        /// <returns>Response message</returns>
        public Message SendAndReceive(Message message)
        {
            var requestTask = SendAndReceiveAsync(message);
            requestTask.Wait();
            return requestTask.Result;
        }


        /// <summary>
        /// Asynchronously sends a <see cref="Message"/> to the connected remote <see cref="IReceiver"/> and returns the reponse <see cref="Message"/>
        /// Default 1 hour timeout
        /// </summary>
        /// <param name="message">Request message</param>
        /// <returns>Response message</returns>
        public async Task<Message> SendAndReceiveAsync(Message request)
        {
            return await SendAndReceiveAsync(request, TimeSpan.FromHours(1));
        }


        /// <summary>
        /// Asynchronously sends a <see cref="Message"/> to the connected remote <see cref="IReceiver"/> and returns the reponse <see cref="Message"/>
        /// </summary>
        /// <param name="message">Request message</param>
        /// <returns>Response message</returns>
        public async Task<Message> SendAndReceiveAsync(Message request, TimeSpan timeout)
        {
            var message = new NetMQMessage();
            message.AppendEmptyFrame();
            message.Append(binarySerializer.Serialize<Message>(request));

            var responseMessage = await socket.SendAndReceive(message, timeout.TotalMilliseconds);

            return binarySerializer.Deserialize<Message>(responseMessage[1].ToByteArray());
        }
    }
}
