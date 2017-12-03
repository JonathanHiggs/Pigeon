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
    public class NetMQSenderMonitor : INetMQSenderMonitor
    {
        private readonly INetMQPoller poller;
        private readonly HashSet<INetMQSender> senders = new HashSet<INetMQSender>();
        private bool isRunning = false;
        private readonly object lockObj = new object();


        public NetMQSenderMonitor(INetMQPoller poller)
        {
            this.poller = poller ?? throw new ArgumentNullException(nameof(poller));
        }


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
