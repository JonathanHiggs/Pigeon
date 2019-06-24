using System;

using NetMQ;
using NetMQ.Sockets;

using Pigeon.Addresses;
using Pigeon.NetMQ.Publishers;
using Pigeon.NetMQ.Receivers;
using Pigeon.NetMQ.Senders;
using Pigeon.NetMQ.Subscribers;
using Pigeon.Requests;
using Pigeon.Subscribers;
using Pigeon.Topics;
using Pigeon.Transport;

namespace Pigeon.NetMQ
{
    /// <summary>
    /// Combined factory for NetMQ specific implementations of <see cref="Common.INetMQConnection"/>s
    /// </summary>
    public class NetMQFactory : TransportFactory<INetMQSender, INetMQReceiver, INetMQPublisher, INetMQSubscriber>, INetMQFactory
    {
        private IRequestDispatcher requestDispatcher;
        private ITopicDispatcher topicDispatcher;
        private readonly INetMQMessageFactory messageFactory;
        private readonly INetMQMonitor monitor;


        /// <summary>
        /// Initializes a new instance of <see cref="NetMQFactory"/>
        /// </summary>
        /// <param name="requestDispatcher"><see cref="IRequestDispatcher"/> that will route incoming messages</param>
        /// <param name="topicDispatcher"><see cref="ITopicDispatcher"/> that will route incoming topic messages</param>
        /// <param name="monitor">Monitor that all NetMQ transports will be added to</param>
        /// <param name="messageFactory">Factory for creating <see cref="NetMQMessage"/>s</param>
        public NetMQFactory(IRequestDispatcher requestDispatcher, ITopicDispatcher topicDispatcher, INetMQMonitor monitor, INetMQMessageFactory messageFactory)
            : base(monitor, monitor, monitor, monitor)
        {
            if (monitor is null)
                throw new ArgumentNullException(nameof(monitor));

            this.requestDispatcher = requestDispatcher ?? throw new ArgumentNullException(nameof(requestDispatcher));
            this.topicDispatcher = topicDispatcher ?? throw new ArgumentNullException(nameof(topicDispatcher));
            this.messageFactory = messageFactory ?? throw new ArgumentNullException(nameof(messageFactory));
            this.monitor = monitor ?? throw new ArgumentNullException(nameof(monitor));
        }


        /// <summary>
        /// Creates a new instance of a <see cref="INetMQReceiver"/> bound to the supplied <see cref="IAddress"/>
        /// </summary>
        /// <param name="address">Address of local bound <see cref="Pigeon.Common.IConnection"/></param>
        /// <returns>Receiver bound to the address</returns>
        protected override INetMQReceiver CreateNewReceiver(IAddress address)
        {
            var socket = new RouterSocket();
            var receiver = new NetMQReceiver(socket, messageFactory, requestDispatcher);

            receiver.AddAddress(address);

            return receiver;
        }


        /// <summary>
        /// Constructs a new instance of an <see cref="INetMQSender"/> connected to the supplied <see cref="IAddress"/>
        /// </summary>
        /// <param name="address">Address of the remote the sender will connect to</param>
        /// <returns>Sender connected to the remote address</returns>
        protected override INetMQSender CreateNewSender(IAddress address)
        {
            var socket = new DealerSocket();
            var sender = new NetMQSender(socket, messageFactory);

            sender.AddAddress(address);

            return sender;
        }


        /// <summary>
        /// Creates a new instance of a <see cref="INetMQPublisher"/> bound to the supplied <see cref="IAddress"/>
        /// </summary>
        /// <param name="address"><see cref="IAddress"/> publisher binds to</param>
        /// <returns><see cref="INetMQPublisher"/> bound to the <see cref="IAddress"/></returns>
        protected override INetMQPublisher CreateNewPublisher(IAddress address)
        {
            var socket = new PublisherSocket();
            var publisher = new NetMQPublisher(socket, messageFactory);

            publisher.AddAddress(address);

            return publisher;
        }


        /// <summary>
        /// Creates a new instance of a <see cref="INetMQSubscriber"/> connected to the supplied <see cref="IAddress"/>
        /// </summary>
        /// <param name="address"><see cref="IAddress"/> of the remote publishing <see cref="Pigeon.Common.IConnection"/></param>
        /// <param name="topicEventHandler"><see cref="TopicEventHandler"/> delegate that the <see cref="INetMQSubscriber"/> will call
        /// upon receiving a topic message</param>
        /// <returns><see cref="INetMQSubscriber"/> connected to the <see cref="IAddress"/></returns>
        protected override INetMQSubscriber CreateNewSubscriber(IAddress address)
        {
            var socket = new SubscriberSocket();
            var subscriber = new NetMQSubscriber(socket, messageFactory, topicDispatcher);

            subscriber.AddAddress(address);

            return subscriber;
        }
    }
}
