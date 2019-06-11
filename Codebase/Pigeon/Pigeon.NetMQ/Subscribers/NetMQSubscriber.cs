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
    public sealed class NetMQSubscriber : NetMQConnection, INetMQSubscriber, IDisposable
    {
        private SubscriberSocket socket;
        private TopicEventHandler handler;


        /// <summary>
        /// Raised when an incoming message is received
        /// </summary>
        public TopicEventHandler Handler => handler;


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
            this.handler = handler ?? throw new ArgumentNullException(nameof(handler));

            socket.ReceiveReady += OnMessageReceived;
        }
        
        
        /// <summary>
        /// Initializes a subscription to the topic message stream from a remote <see cref="Publishers.INetMQPublisher"/>
        /// </summary>
        /// <typeparam name="TTopic">The type of the published topic message</typeparam>
        public void Subscribe<TTopic>()
        {
            var topicName = typeof(TTopic).FullName;
            socket.Subscribe(topicName);
        }


        /// <summary>
        /// Terminates a subscription to the topic message stream
        /// </summary>
        public void Unsubscribe<TTopic>()
        {
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


        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls


        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    TerminateConnection();
                    socket.ReceiveReady -= OnMessageReceived;
                    socket.Dispose();
                    socket = null;
                    handler = null;
                }
                
                disposedValue = true;
            }
        }


        /// <summary>
        /// Performs cleanup to free any held resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
