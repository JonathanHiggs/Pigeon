using MessageRouter.Senders;
using MessageRouter.NetMQ.Receivers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageRouter.Addresses;
using MessageRouter.Packages;
using MessageRouter.Serialization;
using NetMQ;
using NetMQ.Sockets;

namespace MessageRouter.NetMQ.Senders
{
    /// <summary>
    /// NetMQ implementation of <see cref="ISender"/> that wraps a <see cref="DealerSocket"/> that connects to 
    /// remote <see cref="INetMQReceiver"/>s and send and receive <see cref="Package"/>es
    /// </summary>
    public class NetMQSender : INetMQSender
    {
        private readonly List<IAddress> addresses = new List<IAddress>();
        private readonly ISerializer<byte[]> serializer;
        private readonly IAsyncSocket asyncSocket;


        /// <summary>
        /// Gets an eumerable of the <see cref="IAddress"/> for remotes that the sender is connected to
        /// </summary>
        public IEnumerable<IAddress> Addresses => addresses;


        /// <summary>
        /// Gets the inner pollable socket
        /// </summary>
        public ISocketPollable PollableSocket => asyncSocket.PollableSocket;


        /// <summary>
        /// Initializes a new instance of a <see cref="NetMQSender"/>
        /// </summary>
        /// <param name="asyncSocket">Inner <see cref="IAsyncSocket"/> that sends data to remotes</param>
        /// <param name="serializer">A serializer that will convert request and response data to a binary format to be sent to a remote</param>
        public NetMQSender(IAsyncSocket asyncSocket, ISerializer<byte[]> serializer)
        {
            this.asyncSocket = asyncSocket ?? throw new ArgumentNullException(nameof(asyncSocket));
            this.serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }


        /// <summary>
        /// Adds an address to the collection of endpoints the <see cref="NetMQSender"/> connects to
        /// </summary>
        /// <param name="address">Address of the remote</param>
        public void AddAddress(IAddress address)
        {
            if (null == address)
                throw new ArgumentNullException(nameof(address));

            if (addresses.Contains(address))
                return;

            addresses.Add(address);
        }


        /// <summary>
        /// Removes an address from the collection of endpoints the <see cref="NetMQSender"/> connects to
        /// </summary>
        /// <param name="address"></param>
        public void RemoveAddress(IAddress address)
        {
            if (null == address || !addresses.Contains(address))
                return;

            addresses.Remove(address);
        }


        /// <summary>
        /// Initializes the connection to all added addresses
        /// </summary>
        public void ConnectAll()
        {
            foreach (var address in addresses)
                asyncSocket.Connect(address);
        }


        /// <summary>
        /// Terminates the connection to all added addresses
        /// </summary>
        public void DisconnectAll()
        {
            foreach (var address in addresses)
                asyncSocket.Disconnect(address);
        }


        /// <summary>
        /// Asynchronously dispatches a <see cref="Package"/> along the transport to the connected remote 
        /// <see cref="NetMQReceiver"/> and returns a task that will complete when a response is returned from the
        /// remote or when the one hour default timeout elapses
        /// </summary>
        /// <param name="message"><see cref="Package"/> to send to the remote</param>
        /// <returns>A task that will complete successfully when a responce is received or that will fail once the timeout elapses</returns>
        public async Task<Package> SendAndReceive(Package request)
        {
            return await SendAndReceive(request, TimeSpan.FromHours(1));
        }


        /// <summary>
        /// Asynchronously dispatches a <see cref="Package"/> along the transport to the connected remote 
        /// <see cref="NetMQReceiver"/> and returns a task that will complete when a response is returned from the
        /// remote or when the timeout elapses
        /// </summary>
        /// <param name="request"><see cref="Package"/> to send to the remote</param>
        /// <param name="timeout"><see cref="TimeSpan"/> after which the returned <see cref="Task{Message}"/> will throw an error if no response has been received</param>
        /// <returns>A task that will complete successfully when a responce is received or that will fail once the timeout elapses</returns>
        public async Task<Package> SendAndReceive(Package request, TimeSpan timeout)
        {
            var message = new NetMQMessage();
            message.AppendEmptyFrame();
            message.Append(serializer.Serialize(request));

            var responseMessage = await asyncSocket.SendAndReceive(message, timeout);

            return serializer.Deserialize<Package>(responseMessage[1].ToByteArray());
        }
    }
}
