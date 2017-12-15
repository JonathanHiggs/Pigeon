using System;
using System.Collections.Generic;
using MessageRouter.NetMQ.Publishers;
using MessageRouter.NetMQ.Receivers;
using MessageRouter.NetMQ.Senders;
using MessageRouter.NetMQ.Subscribers;
using MessageRouter.Receivers;
using MessageRouter.Senders;

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
        /// Adds a <see cref="INetMQSender"/> to the internal cache of monitored senders
        /// </summary>
        public void AddSender(INetMQSender sender) => Add(sender, senders, s => s.ConnectAll());


        /// <summary>
        /// Adds a <see cref="INetMQReceiver"/> to the internal cache of monitored receivers
        /// </summary>
        public void AddReceiver(INetMQReceiver receiver) => Add(receiver, receivers, r => r.BindAll());


        public void AddPublisher(INetMQPublisher publisher) => Add(publisher, publishers, p => p.BindAll());


        public void AddSubscriber(INetMQSubscriber subscriber) => Add(subscriber, subscribers, s => s.ConnectAll());


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

                running = false;
            }
        }
    }
}
