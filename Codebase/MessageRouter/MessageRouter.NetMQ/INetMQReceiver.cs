using MessageRouter.Receivers;
using NetMQ;

namespace MessageRouter.NetMQ
{
    public interface INetMQReceiver : IReceiver
    {
        ISocketPollable PollableSocket { get; }
    }
}