using MessageRouter.Receivers;
using NetMQ;

namespace MessageRouter.NetMQ.Receivers
{
    public interface INetMQReceiver : IReceiver
    {
        ISocketPollable PollableSocket { get; }
    }
}