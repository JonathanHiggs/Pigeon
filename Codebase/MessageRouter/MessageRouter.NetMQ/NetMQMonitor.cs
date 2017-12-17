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

namespace MessageRouter.NetMQ
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
        private bool running = false;
        private object lockObj = new object();


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
        public void AddSender(INetMQSender sender) => Add(sender, senders, s => s.ConnectAll());


        /// <summary>
        /// Adds a <see cref="INetMQReceiver"/> to the internal cache of monitored <see cref="IReceiver"/>s
        /// </summary>
        /// <param name="receiver"><see cref="INetMQReceiver"/> to add to the cache of monitored <see cref="IReceiver"/>s</param>
        public void AddReceiver(INetMQReceiver receiver) => Add(receiver, receivers, r => r.BindAll());


        /// <summary>
        /// Adds a <see cref="INetMQPublisher"/> to the internal cache of monitored <see cref="IPublisher"/>s
        /// </summary>
        /// <param name="publisher"><see cref="INetMQPublisher"/> to add to the cache of monitored <see cref="IPublisher"/>s</param>
        public void AddPublisher(INetMQPublisher publisher) => Add(publisher, publishers, p => p.BindAll());


        /// <summary>
        /// Adds a <see cref="INetMQSubscriber"/> to the internal cache of monitored <see cref="ISubscriber"/>s
        /// </summary>
        /// <param name="subscriber"><see cref="INetMQSubscriber"/> to add to the cache of monitored <see cref="ISubscriber"/>s</param>
        public void AddSubscriber(INetMQSubscriber subscriber) => Add(subscriber, subscribers, s => s.ConnectAll());


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
                    sender.ConnectAll();

                foreach (var receiver in receivers)
                    receiver.BindAll();

                foreach (var publisher in publishers)
                    publisher.BindAll();

                foreach (var subscriber in subscribers)
                    subscriber.ConnectAll();

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
                    sender.DisconnectAll();

                foreach (var receiver in receivers)
                    receiver.UnbindAll();

                foreach (var publisher in publishers)
                    publisher.UnbindAll();

                foreach (var subscriber in subscribers)
                    subscriber.DisconnectAll();

                running = false;
            }
        }


        private void Add<TEndPoint>(TEndPoint endPoint, HashSet<TEndPoint> set, Action<TEndPoint> runningAction)
            where TEndPoint : INetMQEndPoint
        {
            if (null == endPoint)
                return;

            poller.Add(endPoint.PollableSocket);
            set.Add(endPoint);

            lock(lockObj)
            {
                if (running)
                    runningAction(endPoint);
            }
        }
    }
}
