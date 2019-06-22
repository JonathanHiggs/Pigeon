using System;
using System.Threading.Tasks;

using NetMQ;
using NetMQ.Sockets;

using Pigeon.Addresses;
using Pigeon.NetMQ.Common;
using Pigeon.Packages;
using Pigeon.Subscribers;

namespace Pigeon.NetMQ.Subscribers
{
    /// <summary>
    /// NetMQ implementation of <see cref="ISubscriber"/> that wraps a <see cref="SubscriberSocket"/> that connects to 
    /// remote <see cref="INetMQSubscriber"/>s to receive published <see cref="Package"/>es
    /// </summary>
    public sealed class NetMQSubscriber : NetMQConnection, INetMQSubscriber
    {
        private SubscriberSocket socket;


        /// <summary>
        /// Raised when an incoming message is received
        /// </summary>
        public TopicEventHandler Handler { get; private set; }


        /// <summary>
        /// Initializes a new instance of <see cref="NetMQSubscriber"/>
        /// </summary>
        /// <param name="socket">Inner <see cref="SubscriberSocket"/> that receives data from remotes</param>
        /// <param name="messageFactory">Factory for creating <see cref="NetMQMessage"/>s</param>
        /// <param name="handler"><see cref="TopicEventHandler"/> delegate that is called when an incoming topic message is received</param>
        public NetMQSubscriber(SubscriberSocket socket, INetMQMessageFactory messageFactory, TopicEventHandler handler)
            : base(socket, messageFactory)
        {
            this.socket = socket ?? throw new ArgumentNullException(nameof(socket));
            this.Handler = handler ?? throw new ArgumentNullException(nameof(handler));

            socket.ReceiveReady += OnMessageReceived;
        }
        
        
        /// <summary>
        /// Initializes a subscription to the topic message stream from a remote <see cref="Publishers.INetMQPublisher"/>
        /// </summary>
        /// <typeparam name="TTopic">The type of the published topic message</typeparam>
        public void Subscribe<TTopic>()
        {
            if (disposedValue)
                throw new InvalidOperationException("NetMQSubscriber has been disposed");

            if (!IsConnected)
                throw new InvalidCastException("NetMQSuscriber is not connected");

            var topicName = typeof(TTopic).FullName;
            socket.Subscribe(topicName);
        }


        /// <summary>
        /// Terminates a subscription to the topic message stream
        /// </summary>
        public void Unsubscribe<TTopic>()
        {
            if (disposedValue)
                throw new InvalidOperationException("NetMQSubscriber has been disposed");

            var topicName = typeof(TTopic).FullName;
            socket.Unsubscribe(topicName);
        }

        
        private void OnMessageReceived(object sender, NetMQSocketEventArgs e)
        {
            NetMQMessage message = null;
            if (!socket.TryReceiveMultipartMessage(ref message, 2))
                return;

            // Move handling request off NetMQPoller thread and onto TaskPool as soon as possible
            Task.Run(() =>
            {
                if (!messageFactory.IsValidTopicMessage(message))
                    return;
                
                var package = messageFactory.ExtractTopic(message);
                Handler(this, package);
            });
        }


        /// <summary>
        /// Add <see cref="IAddress"/> to the socket
        /// </summary>
        /// <param name="address"><see cref="IAddress"/> to be added</param>
        public override void SocketAdd(IAddress address)
        {
            if (disposedValue)
                throw new InvalidOperationException("NetMQSubscriber has been disposed");

            socket.Connect(address.ToString());
        }


        /// <summary>
        /// Remote <see cref="IAddress"/> from the socket
        /// </summary>
        /// <param name="address"><see cref="IAddress"/> to be removed</param>
        public override void SocketRemove(IAddress address)
        {
            if (disposedValue)
                throw new InvalidOperationException("NetMQSubscriber has been disposed");

            socket.Disconnect(address.ToString());
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
                        socket.ReceiveReady -= OnMessageReceived;
                        socket.Dispose();
                        socket = null;
                    }
                    Handler = null;
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
