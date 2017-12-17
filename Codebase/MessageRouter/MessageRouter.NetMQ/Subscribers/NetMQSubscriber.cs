using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageRouter.Addresses;
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
    public class NetMQSubscriber : INetMQSubscriber, IDisposable
    {
        private readonly List<IAddress> addresses = new List<IAddress>();
        private readonly ISerializer<byte[]> serializer;
        private SubscriberSocket socket;
        private bool isConnected = false;


        /// <summary>
        /// The NetMQ socket that a <see cref="NetMQPoller"/> will actively monitor for incoming requests
        /// </summary>
        public ISocketPollable PollableSocket => socket;


        /// <summary>
        /// Gets an eumerable of the <see cref="IAddress"/> for remotes that the sender is connected to
        /// </summary>
        public IEnumerable<IAddress> Addresses => addresses;


        /// <summary>
        /// Raised when an incoming message is received
        /// </summary>
        public event TopicEventHandler TopicMessageReceived;


        /// <summary>
        /// Initializes a new instance of <see cref="NetMQSubscriber"/>
        /// </summary>
        /// <param name="socket">Inner <see cref="SubscriberSocket"/> that receives data from remotes</param>
        /// <param name="serializer">A serializer that will convert published data from binary</param>
        public NetMQSubscriber(SubscriberSocket socket, ISerializer<byte[]> serializer)
        {
            this.socket = socket ?? throw new ArgumentNullException(nameof(socket));
            this.serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));

            socket.ReceiveReady += OnMessageReceived;
        }


        /// <summary>
        /// Adds the <see cref="IAddress"/> to the set of endpoints the <see cref="ISubscriber"/> will listen to
        /// for incoming <see cref="Package"/>s
        /// </summary>
        /// <param name="address"><see cref="IAddress"/> to add</param>
        public void AddAddress(IAddress address)
        {
            if (null == address)
                throw new ArgumentNullException(nameof(address));

            if (addresses.Contains(address))
                return;

            addresses.Add(address);

            if (isConnected)
                socket.Connect(address.ToString());
        }


        /// <summary>
        /// Initializes the connections to all added addresses
        /// </summary>
        public void ConnectAll()
        {
            foreach (var address in addresses)
                socket.Connect(address.ToString());
        }


        /// <summary>
        /// Terminates the connection to all added addresses
        /// </summary>
        public void DisconnectAll()
        {
            foreach (var address in addresses)
                socket.Disconnect(address.ToString());
        }


        /// <summary>
        /// Removes the <see cref="IAddress"/> from the set of endpoints the <see cref="ISubscriber"/> will listen to
        /// for incoming <see cref="Package"/>s
        /// </summary>
        /// <param name="address"><see cref="IAddress"/> to remove</param>
        public void RemoveAddress(IAddress address)
        {
            if (null == address || !addresses.Contains(address))
                return;

            if (isConnected)
                socket.Disconnect(address.ToString());

            addresses.Remove(address);
        }


        /// <summary>
        /// Removes all <see cref="IAddress"/> from the set of endpoints the <see cref="ISubscriber"/> will listen to
        /// for incoming <see cref="Package"/>s
        /// </summary>
        public void RemoveAllAddresses()
        {
            if (isConnected)
                foreach (var address in addresses)
                    socket.Disconnect(address.ToString());

            addresses.Clear();
        }


        /// <summary>
        /// Initializes a subscription to the topic message stream from a remote <see cref="IPublisher"/>
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


        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls


        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    DisconnectAll();
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
