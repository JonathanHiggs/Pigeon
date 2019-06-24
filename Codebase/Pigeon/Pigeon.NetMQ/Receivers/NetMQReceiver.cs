using System;
using System.Threading.Tasks;

using NetMQ;
using NetMQ.Sockets;

using Pigeon.Addresses;
using Pigeon.NetMQ.Common;
using Pigeon.Packages;
using Pigeon.Receivers;
using Pigeon.Requests;

namespace Pigeon.NetMQ.Receivers
{
    /// <summary>
    /// Implementation of <see cref="IReceiver"/> that wraps a NetMQ <see cref="RouterSocket"/>. Encapsulates a connection
    /// that is able to bind to an <see cref="IAddress"/> to receive and respond to incoming <see cref="Package"/>es from
    /// connected remote <see cref="INetMQReceiver"/>s
    /// </summary>
    public sealed class NetMQReceiver : NetMQConnection, INetMQReceiver
    {
        private readonly IRequestDispatcher requestDispatcher;
        private RouterSocket socket;


        /// <summary>
        /// Initializes a new instance of <see cref="NetMQReceiver"/>
        /// </summary>
        /// <param name="socket">Inner NetMQ <see cref="RouterSocket"/></param>
        /// <param name="messageFactory">Factory for creating <see cref="NetMQMessage"/>s</param>
        /// <param name="requestDispatcher"><see cref="IRequestDispatcher"/> that will route incoming messages</param>
        public NetMQReceiver(RouterSocket socket, INetMQMessageFactory messageFactory, IRequestDispatcher requestDispatcher)
            : base(socket, messageFactory)
        {
            this.socket = socket ?? throw new ArgumentNullException(nameof(socket));
            this.requestDispatcher = requestDispatcher ?? throw new ArgumentNullException(nameof(requestDispatcher));

            socket.ReceiveReady += OnRequestReceived;
        }


        /// <summary>
        /// Add <see cref="IAddress"/> to the socket
        /// </summary>
        /// <param name="address"><see cref="IAddress"/> to be added</param>
        public override void SocketAdd(IAddress address)
        {
            if (disposedValue)
                throw new InvalidOperationException("NetMQReceiver has been disposed");

            socket.Bind(address.ToString());
        }


        /// <summary>
        /// Remote <see cref="IAddress"/> from the socket
        /// </summary>
        /// <param name="address"><see cref="IAddress"/> to be removed</param>
        public override void SocketRemove(IAddress address)
        {
            if (disposedValue)
                throw new InvalidOperationException("NetMQReceiver has been disposed");

            socket.Unbind(address.ToString());
        }


        /// <summary>
        /// Called by <see cref="NetMQPoller"/> when there is an incoming message
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnRequestReceived(object sender, NetMQSocketEventArgs e)
        {
            NetMQMessage requestMessage = null;
            if (!socket.TryReceiveMultipartMessage(ref requestMessage, 5))
                return;

            // Move handling request off NetMQPoller thread and onto TaskPool as soon as possible
            Task.Run(() =>
            {
                if (!messageFactory.IsValidRequestMessage(requestMessage))
                    return;

                var (request, address, requestId, serializerName) = messageFactory.ExtractRequest(requestMessage);

                void SendResponse(object response) {
                    var message = messageFactory.CreateResponseMessage(response, address, requestId, serializerName);
                    socket.SendMultipartMessage(message);
                }

                var requestTask = new RequestTask(this, request, SendResponse, SendResponse);

                requestDispatcher.Handle(ref requestTask);
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
                        socket.ReceiveReady -= OnRequestReceived;
                        socket.Dispose();
                        socket = null;
                    }
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
