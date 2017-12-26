using MessageRouter.NetMQ.Publishers;
using MessageRouter.NetMQ.Receivers;
using MessageRouter.NetMQ.Senders;
using MessageRouter.NetMQ.Subscribers;
using MessageRouter.Transport;

namespace MessageRouter.NetMQ
{
    /// <summary>
    /// Combined factory interface for NetMQ specific implementations of <see cref="Common.INetMQConnection"/>s
    /// </summary>
    public interface INetMQFactory : ITransportFactory<INetMQSender, INetMQReceiver, INetMQPublisher, INetMQSubscriber>
    {}
}
