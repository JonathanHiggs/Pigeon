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

        
        /// <summary>
        /// Raised when an incoming message is received
        /// </summary>
        public event TopicEventHandler TopicMessageReceived;


        /// <summary>
        /// Initializes a new instance of <see cref="NetMQSubscriber"/>
        /// </summary>
        /// <param name="socket">Inner <see cref="SubscriberSocket"/> that receives data from remotes</param>
        /// <param name="serializer">A serializer that will convert published data from binary</param>
        public NetMQSubscriber(SubscriberSocket socket, ISerializer serializer)
            : base(socket, serializer)
        {
            this.socket = socket ?? throw new ArgumentNullException(nameof(socket));
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
            var topicMessage = socket.ReceiveMultipartMessage();
            var packageData = topicMessage[1].ToByteArray();
            var package = serializer.Deserialize<Package>(packageData);
            TopicMessageReceived?.Invoke(this, package);
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
