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
        /// <param name="messageFactory">Factory for creating <see cref="NetMQMessage"/>s</param>
        public NetMQPublisher(PublisherSocket socket, IMessageFactory messageFactory)
            : base(socket, messageFactory)
        {
            this.socket = socket ?? throw new ArgumentNullException(nameof(socket));
        }


        /// <summary>
        /// Transmits the events to all connected <see cref="ISubscriber"/>s
        /// </summary>
        /// <param name="topicEvent">Topic event to be sent to all remote <see cref="Subscribers.ISubscriber"/>s</param>
        public void Publish(object package)
        {
            socket.SendMultipartMessage(messageFactory.CreateTopicMessage(package));
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
    }
}
