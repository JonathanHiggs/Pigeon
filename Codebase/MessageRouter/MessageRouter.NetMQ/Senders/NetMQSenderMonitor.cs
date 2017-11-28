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
    public class NetMQSenderMonitor : SenderMonitor
    {
        private readonly INetMQPoller poller;

        public NetMQSenderMonitor(NetMQSenderFactory senderFactory, INetMQPoller poller)
            : base(senderFactory)
        {
            this.poller = poller ?? throw new ArgumentNullException(nameof(poller));
        }


        public override void Start()
        {
            poller.RunAsync();
        }


        public override void Stop()
        {
            poller.StopAsync();
        }
    }
}
