using MessageRouter.Addresses;
using MessageRouter.NetMQ.Common;
using MessageRouter.Packages;
using MessageRouter.Receivers;
using MessageRouter.Serialization;
using NetMQ;
using NetMQ.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.NetMQ.Receivers
{
    /// <summary>
    /// Implementation of <see cref="IReceiver"/> that wraps a NetMQ <see cref="RouterSocket"/>. Encapsulates a connection
    /// that is able to bind to an <see cref="IAddress"/> to receive and respond to incoming <see cref="Package"/>es from
    /// connected remote <see cref="INetMQReceiver"/>s
    /// </summary>
    public class NetMQReceiver : NetMQConnection, INetMQReceiver, IDisposable
    {
        private RouterSocket socket;
        private RequestTaskHandler handler;


        /// <summary>
        /// Gets the <see cref="RequestTaskHandler"/> delegate the <see cref="IReceiver"/> calls when
        /// an incoming message is received
        /// </summary>
        public RequestTaskHandler Handler => handler;


        /// <summary>
        /// Initializes a new instance of <see cref="NetMQReceiver"/>
        /// </summary>
        /// <param name="socket">Inner NetMQ <see cref="RouterSocket"/></param>
        /// <param name="messageFactory">Factory for creating <see cref="NetMQMessage"/>s</param>
        /// <param name="handler"><see cref="RequestTaskHandler"/> is called when an incoming message is received</param>
        public NetMQReceiver(RouterSocket socket, IMessageFactory messageFactory, RequestTaskHandler handler)
            : base(socket, messageFactory)
        {
            this.socket = socket ?? throw new ArgumentNullException(nameof(socket));
            this.handler = handler ?? throw new ArgumentNullException(nameof(handler));

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
            // Move handling request off NetMQPoller thread and onto TaskPool as soon as possible
            Task.Run(() =>
            {
                NetMQMessage requestMessage = default(NetMQMessage);
                if (!socket.TryReceiveMultipartMessage(ref requestMessage) && !messageFactory.IsValidRequestMessage(requestMessage))
                    return;

                // So this is some pretty cool shit, yo
                var (request, address, requestId) = messageFactory.ExtractRequest(requestMessage);

                var requestTask = new RequestTask(request, (response) =>
                {
                    var message = messageFactory.CreateResponseMessage(response, address, requestId);
                    socket.SendMultipartMessage(message);
                });

                handler(this, requestTask);
            });
        }
        
        
        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls


        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    TerminateConnection();
                    socket.ReceiveReady -= OnRequestReceived;
                    socket.Dispose();
                    socket = null;
                    handler = null;
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
