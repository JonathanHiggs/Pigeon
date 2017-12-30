using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pigeon.NetMQ.Common;
using Pigeon.Packages;
using Pigeon.Publishers;
using Pigeon.Subscribers;
using NetMQ;

namespace Pigeon.NetMQ.Publishers
{
    /// <summary>
    /// Interface encapsulates a NetMQ connection that is able to publish <see cref="Package"/>s to <see cref="ISubscriber"/>s
    /// </summary>
    public interface INetMQPublisher : IPublisher, INetMQConnection
    { }
}
