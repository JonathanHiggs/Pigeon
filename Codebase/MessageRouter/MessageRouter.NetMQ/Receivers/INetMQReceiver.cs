using MessageRouter.Addresses;
using MessageRouter.Receivers;
using MessageRouter.Senders;

using NetMQ;

namespace MessageRouter.NetMQ.Receivers
{
    /// <summary>
    /// Interface that encapsulates a NetMQ <see cref="IReceiver"/> that is able to bind to <see cref="IAddress"/>es 
    /// to receive and reply to incoming messages from remote <see cref="ISender"/>
    /// </summary>
    public interface INetMQReceiver : IReceiver
    {
        /// <summary>
        /// The NetMQ socket that a <see cref="NetMQPoller"/> will actively monitor for incoming requests
        /// </summary>
        ISocketPollable PollableSocket { get; }
    }
}