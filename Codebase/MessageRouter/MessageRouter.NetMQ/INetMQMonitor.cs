using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageRouter.NetMQ.Publishers;
using MessageRouter.NetMQ.Receivers;
using MessageRouter.NetMQ.Senders;
using MessageRouter.NetMQ.Subscribers;
using MessageRouter.Publishers;
using MessageRouter.Receivers;
using MessageRouter.Senders;
using MessageRouter.Subscribers;

namespace MessageRouter.NetMQ
{
    public interface INetMQMonitor : ISenderMonitor<INetMQSender>, IReceiverMonitor<INetMQReceiver>, IPublisherMonitor<INetMQPublisher>, ISubscriberMonitor<INetMQSubscriber>
    {
    }
}
