using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageRouter.Addresses;
using MessageRouter.Common;
using NetMQ;

namespace MessageRouter.NetMQ.Common
{
    /// <summary>
    /// Common interface for all NetMQ implementations of <see cref="MessageRouter.Common.IConnection"/>
    /// </summary>
    public interface INetMQConnection : IConnection
    {
        /// <summary>
        /// The NetMQ socket that a <see cref="NetMQPoller"/> will actively monitor for incoming requests
        /// </summary>
        ISocketPollable PollableSocket { get; }
    }
}
