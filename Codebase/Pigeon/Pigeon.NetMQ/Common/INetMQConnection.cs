using NetMQ;

using Pigeon.Common;

namespace Pigeon.NetMQ.Common
{
    /// <summary>
    /// Common interface for all NetMQ implementations of <see cref="Pigeon.Common.IConnection"/>
    /// </summary>
    public interface INetMQConnection : IConnection
    {
        /// <summary>
        /// The NetMQ socket that a <see cref="NetMQPoller"/> will actively monitor for incoming requests
        /// </summary>
        ISocketPollable PollableSocket { get; }
    }
}
