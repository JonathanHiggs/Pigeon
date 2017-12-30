using Pigeon.Senders;
using Pigeon.NetMQ.Receivers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pigeon.Addresses;
using Pigeon.Packages;
using Pigeon.Serialization;
using NetMQ;
using NetMQ.Sockets;
using Pigeon.NetMQ.Common;
using Pigeon.Utils;

namespace Pigeon.NetMQ.Senders
{
    /// <summary>
    /// NetMQ implementation of <see cref="ISender"/> that wraps a <see cref="DealerSocket"/> that connects to 
    /// remote <see cref="INetMQReceiver"/>s and send and receive <see cref="Package"/>es
    /// </summary>
    public class NetMQSender : NetMQConnection, INetMQSender
    {
        private readonly DealerSocket socket;
        private readonly RemoteTaskManager<object, int> taskManager = new RemoteTaskManager<object, int>(1, id => id++);


        /// <summary>
        /// Initializes a new instance of a <see cref="NetMQSender"/>
        /// </summary>
        /// <param name="socket">Inner <see cref="DealerSocket"/> that sends data to remotes</param>
        /// <param name="messageFactory">Factory for creating <see cref="NetMQMessage"/>s</param>
        public NetMQSender(DealerSocket socket, INetMQMessageFactory messageFactory)
            : base(socket, messageFactory)
        {
            this.socket = socket ?? throw new ArgumentNullException(nameof(socket));
            socket.ReceiveReady += PendingMessage;
        }


        /// <summary>
        /// Asynchronously dispatches a request along the transport to the connected remote 
        /// <see cref="NetMQReceiver"/> and returns a task that will complete when a response is returned from the
        /// remote or when the one hour default timeout elapses
        /// </summary>
        /// <param name="request">Request to send to the remote</param>
        /// <returns>A task that will complete successfully when a responce is received or that will fail once the timeout elapses</returns>
        public async Task<object> SendAndReceive(object request)
        {
            return await SendAndReceive(request, TimeSpan.FromHours(1));
        }


        /// <summary>
        /// Asynchronously dispatches a request along the transport to the connected remote 
        /// <see cref="NetMQReceiver"/> and returns a task that will complete when a response is returned from the
        /// remote or when the timeout elapses
        /// </summary>
        /// <param name="request">Request to send to the remote</param>
        /// <param name="timeout"><see cref="TimeSpan"/> after which the returned <see cref="Task{Message}"/> will throw an error if no response has been received</param>
        /// <returns>A task that will complete successfully when a responce is received or that will fail once the timeout elapses</returns>
        public Task<object> SendAndReceive(object request, TimeSpan timeout)
        {
            return taskManager.StartRemoteTask(requestId =>
            {
                var message = messageFactory.CreateRequestMessage(request, requestId);
                socket.SendMultipartMessage(message);
            }, timeout);
        }


        /// <summary>
        /// Add <see cref="IAddress"/> to the socket
        /// </summary>
        /// <param name="address"><see cref="IAddress"/> to be added</param>
        public override void SocketAdd(IAddress address)
        {
            socket.Connect(address.ToString());
        }


        /// <summary>
        /// Remote <see cref="IAddress"/> from the socket
        /// </summary>
        /// <param name="address"><see cref="IAddress"/> to be removed</param>
        public override void SocketRemove(IAddress address)
        {
            socket.Disconnect(address.ToString());
        }


        private void PendingMessage(object sender, NetMQSocketEventArgs e)
        {
            NetMQMessage responseMessage = null;
            if (!socket.TryReceiveMultipartMessage(ref responseMessage, 4))
                return;
            
            Task.Run(() =>
            {
                if (!messageFactory.IsValidResponseMessage(responseMessage))
                    return;

                var (requestId, response) = messageFactory.ExtractResponse(responseMessage);
                taskManager.CompleteTask(requestId, response);
            });
        }
    }
}
