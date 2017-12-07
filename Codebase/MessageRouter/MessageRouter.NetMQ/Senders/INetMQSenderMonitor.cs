using MessageRouter.Senders;

namespace MessageRouter.NetMQ.Senders
{
    /// <summary>
    /// Performs active management for <see cref="INetMQSender"/>s
    /// </summary>
    public interface INetMQSenderMonitor : ISenderMonitor<INetMQSender>
    { }
}