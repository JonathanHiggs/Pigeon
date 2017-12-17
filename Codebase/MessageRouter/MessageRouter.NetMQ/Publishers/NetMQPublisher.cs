using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageRouter.Addresses;
using MessageRouter.NetMQ.Common;
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
    public class NetMQPublisher : NetMQConnection, INetMQPublisher
    {
        private readonly PublisherSocket socket;


        /// <summary>
        /// Initializes a new instance of <see cref="NetMQPublisher"/>
        /// </summary>
        /// <param name="socket">Inner <see cref="PublisherSocket"/> that sends data to remotes</param>
        /// <param name="serializer">A serializer that will convert request and response data to a binary format to be sent to a remote</param>
        public NetMQPublisher(PublisherSocket socket, ISerializer<byte[]> serializer)
            : base(socket, serializer)
        {
            this.socket = socket ?? throw new ArgumentNullException(nameof(socket));
        }


        /// <summary>
        /// Transmits the <see cref="Package"/> to all connected <see cref="Subscribers.INetMQSubscriber"/>s
        /// </summary>
        /// <param name="package"><see cref="Package"/> to be sent to all remote <see cref="Subscribers.INetMQSubscriber"/>s</param>
        public void Publish(Package package)
        {
            var topicName = package.Body.GetType().FullName;
            var data = serializer.Serialize(package);

            socket.SendMoreFrame(topicName).SendFrame(data);
        }


        public override void SocketAdd(IAddress address)
        {
            socket.Bind(address.ToString());
        }


        public override void SocketRemove(IAddress address)
        {
            socket.Unbind(address.ToString());
        }
    }
}
