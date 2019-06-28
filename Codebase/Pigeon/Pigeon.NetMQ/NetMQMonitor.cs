using System;
using System.Collections.Generic;

using NetMQ;

using Pigeon.NetMQ.Common;
using Pigeon.NetMQ.Publishers;
using Pigeon.NetMQ.Receivers;
using Pigeon.NetMQ.Senders;
using Pigeon.NetMQ.Subscribers;
using Pigeon.Publishers;
using Pigeon.Receivers;
using Pigeon.Senders;
using Pigeon.Subscribers;

namespace Pigeon.NetMQ
{
    /// <summary>
    /// Actively manages <see cref="INetMQSender"/>s and <see cref="INetMQReceiver"/>s, monitoring for asynchronously received
    /// <see cref="NetMQMessage"/>s from remote connections
    /// </summary>
    public class NetMQMonitor : INetMQMonitor
    {
        private readonly INetMQPoller poller;
        private readonly HashSet<INetMQSender> senders = new HashSet<INetMQSender>();
        private readonly HashSet<INetMQReceiver> receivers = new HashSet<INetMQReceiver>();
        private readonly HashSet<INetMQPublisher> publishers = new HashSet<INetMQPublisher>();
        private readonly HashSet<INetMQSubscriber> subscribers = new HashSet<INetMQSubscriber>();
        private readonly object lockObj = new object();

        private bool running = false;


        /// <summary>
        /// Initializes a new instance of <see cref="NetMQMonitor"/>
        /// </summary>
        /// <param name="poller">The <see cref="INetMQPoller"/> polls the sender and receiver connections for incoming messages</param>
        public NetMQMonitor(INetMQPoller poller)
        {
            this.poller = poller ?? throw new ArgumentNullException(nameof(poller));
        }


        /// <summary>
        /// Adds a <see cref="INetMQSender"/> to the internal cache of monitored <see cref="ISender"/>s
        /// </summary>
        /// <param name="sender"><see cref="INetMQSender"/> to add to the monitored cache of <see cref="ISender"/>s</param>
        public void AddSender(INetMQSender sender) => 
            Add(sender, senders);


        /// <summary>
        /// Adds a <see cref="INetMQReceiver"/> to the internal cache of monitored <see cref="IReceiver"/>s
        /// </summary>
        /// <param name="receiver"><see cref="INetMQReceiver"/> to add to the cache of monitored <see cref="IReceiver"/>s</param>
        public void AddReceiver(INetMQReceiver receiver) => 
            Add(receiver, receivers);


        /// <summary>
        /// Adds a <see cref="INetMQPublisher"/> to the internal cache of monitored <see cref="IPublisher"/>s
        /// </summary>
        /// <param name="publisher"><see cref="INetMQPublisher"/> to add to the cache of monitored <see cref="IPublisher"/>s</param>
        public void AddPublisher(INetMQPublisher publisher) => 
            Add(publisher, publishers);


        /// <summary>
        /// Adds a <see cref="INetMQSubscriber"/> to the internal cache of monitored <see cref="ISubscriber"/>s
        /// </summary>
        /// <param name="subscriber"><see cref="INetMQSubscriber"/> to add to the cache of monitored <see cref="ISubscriber"/>s</param>
        public void AddSubscriber(INetMQSubscriber subscriber) => 
            Add(subscriber, subscribers);


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
                poller.Dispose();

                foreach (var sender in senders)
                    sender.TerminateConnection();

                foreach (var receiver in receivers)
                    receiver.TerminateConnection();

                foreach (var publisher in publishers)
                    publisher.TerminateConnection();

                foreach (var subscriber in subscribers)
                    subscriber.TerminateConnection();

                NetMQConfig.Cleanup(false);

                running = false;
            }
        }


        private void Add<TConnection>(TConnection connection, HashSet<TConnection> connectionSet)
            where TConnection : class, INetMQConnection
        {
            if (connection is null)
                return;

            poller.Add(connection.PollableSocket);
            connectionSet.Add(connection);

            lock(lockObj)
            {
                if (running)
                    connection.InitializeConnection();
            }
        }
    }
}
