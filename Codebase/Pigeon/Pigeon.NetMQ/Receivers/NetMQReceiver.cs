using System;
using System.Threading.Tasks;

using NetMQ;
using NetMQ.Sockets;

using Pigeon.Addresses;
using Pigeon.NetMQ.Common;
using Pigeon.Packages;
using Pigeon.Receivers;

namespace Pigeon.NetMQ.Receivers
{
    /// <summary>
    /// Implementation of <see cref="IReceiver"/> that wraps a NetMQ <see cref="RouterSocket"/>. Encapsulates a connection
    /// that is able to bind to an <see cref="IAddress"/> to receive and respond to incoming <see cref="Package"/>es from
    /// connected remote <see cref="INetMQReceiver"/>s
    /// </summary>
    public sealed class NetMQReceiver : NetMQConnection, INetMQReceiver, IDisposable
    {
        private RouterSocket socket;


        /// <summary>
        /// Gets the <see cref="RequestTaskHandler"/> delegate the <see cref="IReceiver"/> calls when
        /// an incoming message is received
        /// </summary>
        public RequestTaskHandler Handler { get; private set; }


        /// <summary>
        /// Initializes a new instance of <see cref="NetMQReceiver"/>
        /// </summary>
        /// <param name="socket">Inner NetMQ <see cref="RouterSocket"/></param>
        /// <param name="messageFactory">Factory for creating <see cref="NetMQMessage"/>s</param>
        /// <param name="handler"><see cref="RequestTaskHandler"/> is called when an incoming message is received</param>
        public NetMQReceiver(RouterSocket socket, INetMQMessageFactory messageFactory, RequestTaskHandler handler)
            : base(socket, messageFactory)
        {
            this.socket = socket ?? throw new ArgumentNullException(nameof(socket));
            this.Handler = handler ?? throw new ArgumentNullException(nameof(handler));

            socket.ReceiveReady += OnRequestReceived;
        }


        /// <summary>
        /// Add <see cref="IAddress"/> to the socket
        /// </summary>
        /// <param name="address"><see cref="IAddress"/> to be added</param>
        public override void SocketAdd(IAddress address)
        {
            socket.Bind(address.ToString());
        }


        /// <summary>
        /// Remote <see cref="IAddress"/> from the socket
        /// </summary>
        /// <param name="address"><see cref="IAddress"/> to be removed</param>
        public override void SocketRemove(IAddress address)
        {
            socket.Unbind(address.ToString());
        }


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

                // So this is some pretty cool shit, yo
                var (request, address, requestId) = messageFactory.ExtractRequest(requestMessage);

                var requestTask = new RequestTask(request, (response) =>
                {
                    var message = messageFactory.CreateResponseMessage(response, address, requestId);
                    socket.SendMultipartMessage(message);
                });

                Handler(this, ref requestTask);
            });
        }
        
        
        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls


        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    TerminateConnection();
                    socket.ReceiveReady -= OnRequestReceived;
                    socket.Dispose();
                    socket = null;
                    Handler = null;
                }
                
                disposedValue = true;
            }
        }

        
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
