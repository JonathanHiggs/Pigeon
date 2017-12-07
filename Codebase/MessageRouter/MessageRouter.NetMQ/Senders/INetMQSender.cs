using MessageRouter.Addresses;
using MessageRouter.Receivers;
using MessageRouter.Senders;
using NetMQ;

namespace MessageRouter.NetMQ.Senders
{
    /// <summary>
    /// Interface that encapsulates a NetMQ <see cref="ISender"/> that is able to connect to a remote
    /// <see cref="IAddress"/> to send and receive response messages from a remote <see cref="IReceiver"/>
    /// </summary>
    public interface INetMQSender : ISender
    {
        /// <summary>
        /// The NetMQ socket that a <see cref="NetMQPoller"/> will actively monitor for incoming requests
        /// </summary>
        ISocketPollable PollableSocket { get; }
    }
}