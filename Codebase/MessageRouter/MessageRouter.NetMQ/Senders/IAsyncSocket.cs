using System.Threading.Tasks;
using MessageRouter.Addresses;
using NetMQ;

namespace MessageRouter.NetMQ.Senders
{
    public interface IAsyncSocket
    {
        ISocketPollable PollableSocket { get; }

        void Connect(IAddress address);
        void Disconnect(IAddress address);
        Task<NetMQMessage> SendAndReceive(NetMQMessage message, double timeout = 0);
    }
}