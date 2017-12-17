using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageRouter.Addresses;
using MessageRouter.Packages;
using MessageRouter.Serialization;
using MessageRouter.Subscribers;
using NetMQ;
using NetMQ.Sockets;

namespace MessageRouter.NetMQ.Subscribers
{
    public class NetMQSubscriber : INetMQSubscriber, IDisposable
    {
        private readonly List<IAddress> addresses = new List<IAddress>();
        private readonly ISerializer<byte[]> serializer;
        private SubscriberSocket socket;
        private bool isConnected = false;


        public ISocketPollable PollableSocket => socket;


        public IEnumerable<IAddress> Addresses => addresses;


        public event TopicEventHandler TopicMessageReceived;


        public NetMQSubscriber(SubscriberSocket socket, ISerializer<byte[]> serializer)
        {
            this.socket = socket ?? throw new ArgumentNullException(nameof(socket));
            this.serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));

            socket.ReceiveReady += OnMessageReceived;
        }


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


        public void ConnectAll()
        {
            foreach (var address in addresses)
                socket.Connect(address.ToString());
        }


        public void DisconnectAll()
        {
            foreach (var address in addresses)
                socket.Disconnect(address.ToString());
        }


        public void RemoveAddress(IAddress address)
        {
            if (null == address || !addresses.Contains(address))
                return;

            if (isConnected)
                socket.Disconnect(address.ToString());

            addresses.Remove(address);
        }


        public void RemoveAllAddresses()
        {
            if (isConnected)
                foreach (var address in addresses)
                    socket.Disconnect(address.ToString());

            addresses.Clear();
        }


        public void Subscribe<TTopic>()
        {
            var topicName = typeof(TTopic).FullName;
            socket.Subscribe(topicName);
        }


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


        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
