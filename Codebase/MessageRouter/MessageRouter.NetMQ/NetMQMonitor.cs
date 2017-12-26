using System;
using System.Collections.Generic;
using MessageRouter.NetMQ.Publishers;
using MessageRouter.NetMQ.Receivers;
using MessageRouter.NetMQ.Senders;
using MessageRouter.NetMQ.Subscribers;
using MessageRouter.Receivers;
using MessageRouter.Senders;
using MessageRouter.Publishers;
using MessageRouter.Subscribers;

using NetMQ;
using MessageRouter.NetMQ.Common;
using MessageRouter.Requests;
using MessageRouter.Packages;

namespace MessageRouter.NetMQ
{
    /// <summary>
    /// Actively manages <see cref="INetMQSender"/>s and <see cref="INetMQReceiver"/>s, monitoring for asynchronously received
    /// <see cref="NetMQMessage"/>s from remote connections
    /// </summary>
    public class NetMQMonitor : INetMQMonitor
    {
        private readonly INetMQPoller poller;
        private readonly IPackageFactory packageFactory;
        private readonly IRequestDispatcher requestDispatcher;
        private readonly HashSet<INetMQSender> senders = new HashSet<INetMQSender>();
        private readonly HashSet<INetMQReceiver> receivers = new HashSet<INetMQReceiver>();
        private readonly HashSet<INetMQPublisher> publishers = new HashSet<INetMQPublisher>();
        private readonly HashSet<INetMQSubscriber> subscribers = new HashSet<INetMQSubscriber>();
        private bool running = false;
        private object lockObj = new object();


        /// <summary>
        /// Gets a handler delegate for incoming requests
        /// </summary>
        public RequestTaskHandler RequestHandler => HandleRequest;


        /// <summary>
        /// Initializes a new instance of <see cref="NetMQMonitor"/>
        /// </summary>
        /// <param name="poller">The <see cref="INetMQPoller"/> polls the sender and receiver connections for incoming messages</param>
        /// <param name="requestDispatcher">Delegates handling of incoming requests to registered handlers</param>
        /// <param name="packageFactory">Creates and extracts <see cref="Package"/>s that are received from remote connections</param>
        public NetMQMonitor(INetMQPoller poller, IRequestDispatcher requestDispatcher, IPackageFactory packageFactory)
        {
            this.poller = poller ?? throw new ArgumentNullException(nameof(poller));
            this.requestDispatcher = requestDispatcher ?? throw new ArgumentNullException(nameof(requestDispatcher));
            this.packageFactory = packageFactory ?? throw new ArgumentNullException(nameof(packageFactory));
        }


        /// <summary>
        /// Adds a <see cref="INetMQSender"/> to the internal cache of monitored <see cref="ISender"/>s
        /// </summary>
        /// <param name="sender"><see cref="INetMQSender"/> to add to the monitored cache of <see cref="ISender"/>s</param>
        public void AddSender(INetMQSender sender) => Add(sender, senders, s => s.InitializeConnection());


        /// <summary>
        /// Adds a <see cref="INetMQReceiver"/> to the internal cache of monitored <see cref="IReceiver"/>s
        /// </summary>
        /// <param name="receiver"><see cref="INetMQReceiver"/> to add to the cache of monitored <see cref="IReceiver"/>s</param>
        public void AddReceiver(INetMQReceiver receiver) => Add(receiver, receivers, r => r.InitializeConnection());


        /// <summary>
        /// Adds a <see cref="INetMQPublisher"/> to the internal cache of monitored <see cref="IPublisher"/>s
        /// </summary>
        /// <param name="publisher"><see cref="INetMQPublisher"/> to add to the cache of monitored <see cref="IPublisher"/>s</param>
        public void AddPublisher(INetMQPublisher publisher) => Add(publisher, publishers, p => p.InitializeConnection());


        /// <summary>
        /// Adds a <see cref="INetMQSubscriber"/> to the internal cache of monitored <see cref="ISubscriber"/>s
        /// </summary>
        /// <param name="subscriber"><see cref="INetMQSubscriber"/> to add to the cache of monitored <see cref="ISubscriber"/>s</param>
        public void AddSubscriber(INetMQSubscriber subscriber) => Add(subscriber, subscribers, s => s.InitializeConnection());


        /// <summary>
        /// Starts active monitoring of transports
        /// </summary>
        public void StartMonitoring()
        {
            lock(lockObj)
            {
                if (running)
                    return;

                poller.RunAsync();

                foreach (var sender in senders)
                    sender.InitializeConnection();

                foreach (var receiver in receivers)
                    receiver.InitializeConnection();

                foreach (var publisher in publishers)
                    publisher.InitializeConnection();

                foreach (var subscriber in subscribers)
                    subscriber.InitializeConnection();

                running = true;
            }
        }


        /// <summary>
        /// Stops active monitoring of transports
        /// </summary>
        public void StopMonitoring()
        {
            lock(lockObj)
            {
                if (!running)
                    return;

                poller.StopAsync();

                foreach (var sender in senders)
                    sender.TerminateConnection();

                foreach (var receiver in receivers)
                    receiver.TerminateConnection();

                foreach (var publisher in publishers)
                    publisher.TerminateConnection();

                foreach (var subscriber in subscribers)
                    subscriber.TerminateConnection();

                running = false;
            }
        }


        private void Add<TConnection>(TConnection connection, HashSet<TConnection> connectionSet, Action<TConnection> runningAction)
            where TConnection : INetMQConnection
        {
            if (null == connection)
                return;

            poller.Add(connection.PollableSocket);
            connectionSet.Add(connection);

            lock(lockObj)
            {
                if (running)
                    runningAction(connection);
            }
        }

        
        private void HandleRequest(IReceiver receiver, RequestTask requestTask)
        {
            var requestObject = packageFactory.Unpack(requestTask.Request);
            var responseObject = requestDispatcher.Handle(requestObject);
            var responseMessage = packageFactory.Pack(responseObject);
            requestTask.ResponseHandler(responseMessage);
        }
    }
}
