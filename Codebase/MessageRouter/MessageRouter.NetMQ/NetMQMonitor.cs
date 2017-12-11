using System;
using System.Collections.Generic;
using MessageRouter.NetMQ.Receivers;
using MessageRouter.NetMQ.Senders;
using MessageRouter.Receivers;
using MessageRouter.Senders;

using NetMQ;

namespace MessageRouter.NetMQ
{
    /// <summary>
    /// Actively manages <see cref="INetMQSender"/>s and <see cref="INetMQReceiver"/>s, monitoring for asynchronously received
    /// <see cref="NetMQMessage"/>s from remote connections
    /// </summary>
    public class NetMQMonitor : ISenderMonitor<INetMQSender>, IReceiverMonitor<INetMQReceiver>
    {
        private readonly INetMQPoller poller;
        private readonly HashSet<INetMQReceiver> receivers = new HashSet<INetMQReceiver>();
        private readonly HashSet<INetMQSender> senders = new HashSet<INetMQSender>();
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
        /// Adds a <see cref="INetMQReceiver"/> to the internal cache of monitored receivers
        /// </summary>
        public void AddReceiver(INetMQReceiver receiver)
        {
            if (null == receiver)
                return;

            poller.Add(receiver.PollableSocket);
            receivers.Add(receiver);

            lock(lockObj)
            {
                if (running)
                    receiver.BindAll();
            }
        }


        /// <summary>
        /// Adds a <see cref="INetMQSender"/> to the internal cache of monitored senders
        /// </summary>
        public void AddSender(INetMQSender sender)
        {
            if (null == sender)
                return;

            poller.Add(sender.PollableSocket);
            senders.Add(sender);

            lock(lockObj)
            {
                if (running)
                    sender.ConnectAll();
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
