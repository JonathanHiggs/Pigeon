using System;
using System.Threading.Tasks;

using NetMQ;
using NetMQ.Sockets;

using Pigeon.Addresses;
using Pigeon.NetMQ.Common;
using Pigeon.NetMQ.Receivers;
using Pigeon.Packages;
using Pigeon.Senders;
using Pigeon.Utils;

namespace Pigeon.NetMQ.Senders
{
    /// <summary>
    /// NetMQ implementation of <see cref="ISender"/> that wraps a <see cref="DealerSocket"/> that connects to 
    /// remote <see cref="INetMQReceiver"/>s and send and receive <see cref="Package"/>
    /// </summary>
    public sealed class NetMQSender : NetMQConnection, INetMQSender
    {
        private DealerSocket socket;
        private readonly RemoteTaskManager<object, int> taskManager = new RemoteTaskManager<object, int>(1, id => ++id);


        /// <summary>
        /// Initializes a new instance of <see cref="NetMQSender"/>
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
        /// <returns>A task that will complete successfully when a response is received or that will fail once the timeout elapses</returns>
        public Task<object> SendAndReceive(object request)
        {
            return SendAndReceive(request, TimeSpan.FromHours(1));
        }


        /// <summary>
        /// Asynchronously dispatches a request along the transport to the connected remote 
        /// <see cref="NetMQReceiver"/> and returns a task that will complete when a response is returned from the
        /// remote or when the timeout elapses
        /// </summary>
        /// <param name="request">Request to send to the remote</param>
        /// <param name="timeout"><see cref="TimeSpan"/> after which the returned <see cref="Task{Message}"/> will throw an error if no response has been received</param>
        /// <returns>A task that will complete successfully when a response is received or that will fail once the timeout elapses</returns>
        public Task<object> SendAndReceive(object request, TimeSpan timeout)
        {
            if (disposedValue)
                throw new InvalidOperationException("NetMQSender has been disposed");

            if (!IsConnected)
                throw new InvalidOperationException("NetMQSender is not connected");

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
            if (disposedValue)
                throw new InvalidOperationException("NetMQSender has been disposed");

            socket.Connect(address.ToString());
        }


        /// <summary>
        /// Remote <see cref="IAddress"/> from the socket
        /// </summary>
        /// <param name="address"><see cref="IAddress"/> to be removed</param>
        public override void SocketRemove(IAddress address)
        {
            if (disposedValue)
                throw new InvalidOperationException("NetMQSender has been disposed");

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


        #region IDisposable Support

        /// <summary>
        /// Cleans up resources
        /// </summary>
        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    TerminateConnection();
                    if (!(socket is null))
                    {
                        socket.ReceiveReady -= PendingMessage;
                        socket.Dispose();
                    }
                    socket = null;
                }

                disposedValue = true;
            }
        }


        /// <summary>
        /// Cleans up resources
        /// </summary>
        public override void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}
