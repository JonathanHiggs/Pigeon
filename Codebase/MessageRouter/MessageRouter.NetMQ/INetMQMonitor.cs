using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageRouter.NetMQ.Receivers;
using MessageRouter.NetMQ.Senders;
using MessageRouter.Receivers;
using MessageRouter.Senders;

namespace MessageRouter.NetMQ
{
    public interface INetMQMonitor : ISenderMonitor<INetMQSender>, IReceiverMonitor<INetMQReceiver>
    {
    }
}
