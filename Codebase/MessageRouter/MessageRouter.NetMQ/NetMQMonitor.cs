using System;
using System.Collections.Generic;
using MessageRouter.NetMQ.Receivers;
using MessageRouter.NetMQ.Senders;
using MessageRouter.Receivers;
using MessageRouter.Senders;

using NetMQ;

namespace MessageRouter.NetMQ
{
    public class NetMQMonitor : ISenderMonitor<INetMQSender>, IReceiverMonitor<INetMQReceiver>
    {
        private readonly INetMQPoller poller;
        private readonly HashSet<INetMQReceiver> receivers = new HashSet<INetMQReceiver>();
        private readonly HashSet<INetMQSender> senders = new HashSet<INetMQSender>();
        private bool running = false;
        private object lockObj = new object();


        public NetMQMonitor(INetMQPoller poller)
        {
            this.poller = poller ?? throw new ArgumentNullException(nameof(poller));
        }


        public void AddReceiver(INetMQReceiver receiver)
        {
            poller.Add(receiver.PollableSocket);
            receivers.Add(receiver);

            lock(lockObj)
            {
                if (running)
                    receiver.BindAll();
            }
        }


        public void AddSender(INetMQSender sender)
        {
            poller.Add(sender.PollableSocket);
            senders.Add(sender);

            lock(lockObj)
            {
                if (running)
                    sender.ConnectAll();
            }
        }


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
