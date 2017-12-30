using Pigeon.NetMQ.Publishers;
using Pigeon.NetMQ.Receivers;
using Pigeon.NetMQ.Senders;
using Pigeon.NetMQ.Subscribers;
using Pigeon.Transport;

namespace Pigeon.NetMQ
{
    /// <summary>
    /// Combined factory interface for NetMQ specific implementations of <see cref="Common.INetMQConnection"/>s
    /// </summary>
    public interface INetMQFactory : ITransportFactory<INetMQSender, INetMQReceiver, INetMQPublisher, INetMQSubscriber>
    {}
}
