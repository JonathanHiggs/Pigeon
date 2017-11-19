using MessageRouter.Senders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageRouter.Addresses;
using NetMQ;
using MessageRouter.Diagnostics;

namespace MessageRouter.NetMQ
{
    public class NetMQSenderManager : SenderManager
    {
        private readonly NetMQPoller poller;

        public NetMQSenderManager(NetMQSenderFactory senderFactory, NetMQPoller poller)
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
