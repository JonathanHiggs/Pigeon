using MessageRouter.Senders;

namespace MessageRouter.NetMQ.Senders
{
    public interface INetMQSenderMonitor : ISenderMonitor<INetMQSender>
    { }
}