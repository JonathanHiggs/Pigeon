using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageRouter.Packages;
using MessageRouter.Publishers;
using MessageRouter.Subscribers;
using NetMQ;

namespace MessageRouter.NetMQ.Publishers
{
    /// <summary>
    /// Interface encapsulates a NetMQ connection that is able to publish <see cref="Package"/>s to <see cref="ISubscriber"/>s
    /// </summary>
    public interface INetMQPublisher : IPublisher, INetMQEndPoint
    { }
}
