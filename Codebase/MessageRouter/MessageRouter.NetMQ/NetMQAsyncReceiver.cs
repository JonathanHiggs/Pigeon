using MessageRouter.Receivers;
using MessageRouter.Serialization;
using NetMQ;
using NetMQ.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageRouter.Addresses;

namespace MessageRouter.NetMQ
{
    /// <summary>
    /// Implementation of <see cref="IAsyncReceiver"/> that wraps a NetMQ <see cref="RouterSocket"/>. Encapsulates a connection
    /// that is able to bind to a <see cref="IAddress"/> to receiver both synchronously and asynchronously to incoming messages
    /// from connected remote <see cref="ISender"/>s
    /// </summary>
    public class NetMQAsyncReceiver : NetMQReceiver, IAsyncReceiver
    {
        /// <summary>
        /// Raised when an incoming message is received
        /// </summary>
        public event RequestTaskDelegate RequestReceived;


        /// <summary>
        /// Gets the inner pollable socket
        /// </summary>
        public ISocketPollable PollableSocket => routerSocket;


        /// <summary>
        /// Initializes a new instance of a NetMQAsyncReceiver
        /// </summary>
        /// <param name="routerSocket">Inner NetMQ RouterSocket</param>
        /// <param name="binarySerializer">Binary serializer</param>
        public NetMQAsyncReceiver(RouterSocket routerSocket, ISerializer<byte[]> binarySerializer)
            : base(routerSocket, binarySerializer)
        {
            routerSocket.ReceiveReady += OnRequestReceived;
        }


        private void OnRequestReceived(object sender, NetMQSocketEventArgs e)
        {
            RequestReceived?.Invoke(this, Receive());
        }
    }
}
