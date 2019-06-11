using Pigeon.NetMQ.Publishers;
using Pigeon.NetMQ.Receivers;
using Pigeon.NetMQ.Senders;
using Pigeon.NetMQ.Subscribers;
using Pigeon.Publishers;
using Pigeon.Receivers;
using Pigeon.Senders;
using Pigeon.Subscribers;

namespace Pigeon.NetMQ
{
    public interface INetMQMonitor : ISenderMonitor<INetMQSender>, IReceiverMonitor<INetMQReceiver>, IPublisherMonitor<INetMQPublisher>, ISubscriberMonitor<INetMQSubscriber>
    {
    }
}
