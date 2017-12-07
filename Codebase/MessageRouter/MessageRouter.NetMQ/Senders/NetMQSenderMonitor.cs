using MessageRouter.Senders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageRouter.Addresses;
using NetMQ;
using MessageRouter.Diagnostics;

namespace MessageRouter.NetMQ.Senders
{
    /// <summary>
    /// Performs active management for <see cref="INetMQSender"/>s by attaching them to a <see cref="INetMQPoller"/>
    /// that polls for response <see cref="NetMQMessage"/> returning asynchronously
    /// </summary>
    public class NetMQSenderMonitor : INetMQSenderMonitor
    {
        private readonly INetMQPoller poller;
        private readonly HashSet<INetMQSender> senders = new HashSet<INetMQSender>();
        private bool isRunning = false;
        private readonly object lockObj = new object();


        /// <summary>
        /// Initializes a new instance of <see cref="NetMQSenderMonitor"/>
        /// </summary>
        /// <param name="poller"><see cref="INetMQPoller"/> that actively polls for response <see cref="NetMQMessage"/> returning asynchronously</param>
        public NetMQSenderMonitor(INetMQPoller poller)
        {
            this.poller = poller ?? throw new ArgumentNullException(nameof(poller));
        }


        /// <summary>
        /// Adds a <see cref="INetMQSender"/> to the internal cache of monitored senders
        /// </summary>
        /// <param name="sender"><see cref="INetMQSender"/> to add to the monitored cache of senders</param>
        public void AddSender(INetMQSender sender)
        {
            if (null == sender)
                throw new ArgumentNullException("Sender cannot be null");

            poller.Add(sender.PollableSocket);
            senders.Add(sender);

            lock (lockObj)
            {
                if (isRunning)
                    sender.ConnectAll();
            }
        }


        /// <summary>
        /// Starts active polling <see cref="INetMQSender"/>s transports
        /// </summary>
        public void StartSenders()
        {
            lock (lockObj)
            {
                if (isRunning)
                    return;

                foreach (var sender in senders)
                    sender.ConnectAll();

                poller.RunAsync();

                isRunning = true;
            }
        }


        /// <summary>
        /// Stops active polling <see cref="INetMQSender"/>s transports
        /// </summary>
        public void StopSenders()
        {
            lock (lockObj)
            {
                if (!isRunning)
                    return;

                foreach (var sender in senders)
                    sender.DisconnectAll();

                poller.StopAsync();

                isRunning = false;
            }
        }
    }
}
