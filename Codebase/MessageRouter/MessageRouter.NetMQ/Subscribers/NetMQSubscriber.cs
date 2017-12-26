using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageRouter.Addresses;
using MessageRouter.NetMQ.Common;
using MessageRouter.Packages;
using MessageRouter.Publishers;
using MessageRouter.Serialization;
using MessageRouter.Subscribers;
using NetMQ;
using NetMQ.Sockets;

namespace MessageRouter.NetMQ.Subscribers
{
    /// <summary>
    /// NetMQ implementation of <see cref="ISubscriber"/> that wraps a <see cref="SubscriberSocket"/> that connects to 
    /// remote <see cref="INetMQSubscriber"/>s to receive published <see cref="Package"/>es
    /// </summary>
    public class NetMQSubscriber : NetMQConnection, INetMQSubscriber, IDisposable
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
        public NetMQSubscriber(SubscriberSocket socket, IMessageFactory messageFactory, TopicEventHandler handler)
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
            // Move handling request off NetMQPoller thread and onto TaskPool as soon as possible
            Task.Run(() =>
            {
                NetMQMessage message = default(NetMQMessage);

                if (!socket.TryReceiveMultipartMessage(ref message, 2))
                    return;
                
                var package = messageFactory.ExtractTopicPackage(message);
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


        protected virtual void Dispose(bool disposing)
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
