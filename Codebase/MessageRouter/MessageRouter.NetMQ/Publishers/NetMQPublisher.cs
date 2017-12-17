using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageRouter.Addresses;
using MessageRouter.Packages;
using MessageRouter.Serialization;
using NetMQ;
using NetMQ.Sockets;

namespace MessageRouter.NetMQ.Publishers
{
    /// <summary>
    /// NetMQ imlementation of <see cref="MessageRouter.Publishers.IPublisher"/> that wraps a <see cref="PublisherSocket"/>
    /// that connects to remote <see cref="Subscribers.INetMQSubscriber"/>s to publish <see cref="Package"/>s
    /// </summary>
    public class NetMQPublisher : INetMQPublisher
    {
        private readonly Dictionary<IAddress, bool> boundStatusByAddress = new Dictionary<IAddress, bool>();
        private readonly PublisherSocket socket;
        private readonly ISerializer<byte[]> serializer;
        private bool isBound = false;


        /// <summary>
        /// The NetMQ socket that a <see cref="NetMQPoller"/> will actively monitor for incoming requests
        /// </summary>
        public ISocketPollable PollableSocket => socket;


        /// <summary>
        /// Gets an eumerable of the <see cref="IAddress"/> for remotes that the sender is connected to
        /// </summary>
        public IEnumerable<IAddress> Addresses => boundStatusByAddress.Keys;


        /// <summary>
        /// Initializes a new instance of <see cref="NetMQPublisher"/>
        /// </summary>
        /// <param name="socket">Inner <see cref="PublisherSocket"/> that sends data to remotes</param>
        /// <param name="serializer">A serializer that will convert request and response data to a binary format to be sent to a remote</param>
        public NetMQPublisher(PublisherSocket socket, ISerializer<byte[]> serializer)
        {
            this.socket = socket ?? throw new ArgumentNullException(nameof(socket));
            this.serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }


        /// <summary>
        /// Adds an <see cref="IAddress"/> to the collection of endpoints to which the <see cref="IPublisher"/> publishes <see cref="Package"/>s
        /// </summary>
        /// <param name="address"><see cref="IAddress"/> to be added</param>
        public void AddAddress(IAddress address)
        {
            if (boundStatusByAddress.ContainsKey(address))
                return;

            boundStatusByAddress.Add(address, false);

            if (isBound)
            {
                socket.Bind(address.ToString());
                boundStatusByAddress[address] = false;
            }
        }


        /// <summary>
        /// Initializes the bindings to which the <see cref="IPublisher"/> publishes <see cref="Package"/>s allowing <see cref="ISubscribers"/>s to receive them
        /// </summary>
        public void BindAll()
        {
            if (isBound)
                return;

            var addresses = from address in Addresses
                          where !IsAddedAndBound(address)
                          select address;

            foreach (var address in addresses.ToList())
            {
                socket.Bind(address.ToString());
                boundStatusByAddress[address] = true;
            }
        }


        /// <summary>
        /// Transmits the <see cref="Package"/> to all connected <see cref="ISubscriber"/>s
        /// </summary>
        /// <param name="package"><see cref="Package"/> to be sent to all remote <see cref="Subscribers.ISubscriber"/>s</param>
        public void Publish(Package package)
        {
            var topicName = package.Body.GetType().FullName;
            var data = serializer.Serialize(package);

            socket.SendMoreFrame(topicName).SendFrame(data);
        }


        /// <summary>
        /// Removes an <see cref="IAddress"/> from the collection of endpoints to which the <see cref="MessageRouter.Publishers.IPublisher"/> 
        /// publishes <see cref="Package"/>s
        /// </summary>
        /// <param name="address"><see cref="IAddress"/> to be removed</param>
        public void RemoveAddress(IAddress address)
        {
            if (null == address || !boundStatusByAddress.ContainsKey(address))
                return;

            if (isBound && boundStatusByAddress[address])
            {
                socket.Unbind(address.ToString());
                boundStatusByAddress[address] = false;
            }

            boundStatusByAddress.Remove(address);
        }


        /// <summary>
        /// Terminates the bindings to which the <see cref="MessageRouter.Publishers.IPublisher"/> publishes 
        /// <see cref="Package"/>s preventing <see cref="MessageRouter.Subscribers.ISubscriber"/>s from receiving them
        /// </summary>
        public void UnbindAll()
        {
            if (!isBound)
                return;

            var addresses = from address in Addresses
                            where IsAddedAndBound(address)
                            select address;

            foreach (var address in addresses.ToList())
            {
                socket.Unbind(address.ToString());
                boundStatusByAddress[address] = false;
            }
        }


        private bool IsAddedAndBound(IAddress address)
        {
            return boundStatusByAddress.TryGetValue(address, out var bound) && bound;
        }
    }
}
