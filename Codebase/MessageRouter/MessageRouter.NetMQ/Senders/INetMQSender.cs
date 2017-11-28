using MessageRouter.Senders;
using NetMQ;

namespace MessageRouter.NetMQ.Senders
{
    public interface INetMQSender : ISender
    {
        ISocketPollable PollableSocket { get; }
    }
}