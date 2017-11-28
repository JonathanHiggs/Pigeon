using MessageRouter.Senders;
using NetMQ;

namespace MessageRouter.NetMQ
{
    public interface INetMQSender : ISender
    {
        ISocketPollable PollableSocket { get; }
    }
}