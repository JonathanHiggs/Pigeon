using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageRouter.Addresses;
using NetMQ;

namespace MessageRouter.NetMQ
{
    /// <summary>
    /// Common interface for all NetMQ implementations of <see cref="Common.IEndPoint"/>
    /// </summary>
    public interface INetMQEndPoint
    {
        /// <summary>
        /// The NetMQ socket that a <see cref="NetMQPoller"/> will actively monitor for incoming requests
        /// </summary>
        ISocketPollable PollableSocket { get; }


        /// <summary>
        /// Gets an eumerable of the <see cref="IAddress"/> for remotes that the sender is connected to
        /// </summary>
        IEnumerable<IAddress> Addresses { get; }
    }
}
