using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageRouter.Messages;
using MessageRouter.Publishers;
using MessageRouter.Subscribers;
using NetMQ;

namespace MessageRouter.NetMQ.Publishers
{
    /// <summary>
    /// Interface encapsulates a NetMQ connection that is able to publish <see cref="Message"/>s to <see cref="ISubscriber"/>s
    /// </summary>
    public interface INetMQPublisher : IPublisher
    {
        /// <summary>
        /// The NetMQ socket that a <see cref="NetMQPoller"/> will actively monitor for incoming requests
        /// </summary>
        ISocketPollable PollableSocket { get; }
    }
}
