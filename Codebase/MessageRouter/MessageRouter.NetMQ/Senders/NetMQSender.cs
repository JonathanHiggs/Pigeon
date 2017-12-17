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
using MessageRouter.NetMQ.Common;

namespace MessageRouter.NetMQ.Senders
{
    /// <summary>
    /// NetMQ implementation of <see cref="ISender"/> that wraps a <see cref="DealerSocket"/> that connects to 
    /// remote <see cref="INetMQReceiver"/>s and send and receive <see cref="Package"/>es
    /// </summary>
    public class NetMQSender : NetMQConnection, INetMQSender
    {
        private readonly IAsyncSocket socket;
        

        /// <summary>
        /// Initializes a new instance of a <see cref="NetMQSender"/>
        /// </summary>
        /// <param name="socket">Inner <see cref="IAsyncSocket"/> that sends data to remotes</param>
        /// <param name="serializer">A serializer that will convert request and response data to a binary format to be sent to a remote</param>
        public NetMQSender(IAsyncSocket socket, ISerializer<byte[]> serializer)
            : base(socket, serializer)
        {
            this.socket = socket ?? throw new ArgumentNullException(nameof(socket));
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

            var responseMessage = await socket.SendAndReceive(message, timeout);

            return serializer.Deserialize<Package>(responseMessage[1].ToByteArray());
        }


        public override void SocketAdd(IAddress address)
        {
            socket.Connect(address.ToString());
        }


        public override void SocketRemove(IAddress address)
        {
            socket.Disconnect(address.ToString());
        }
    }
}
