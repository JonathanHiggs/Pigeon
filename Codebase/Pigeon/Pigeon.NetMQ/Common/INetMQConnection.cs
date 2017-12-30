using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pigeon.Addresses;
using Pigeon.Common;
using NetMQ;

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
