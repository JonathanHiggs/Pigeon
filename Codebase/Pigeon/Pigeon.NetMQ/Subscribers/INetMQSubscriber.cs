using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pigeon.Addresses;
using Pigeon.NetMQ.Common;
using Pigeon.Publishers;
using Pigeon.Subscribers;
using NetMQ;

namespace Pigeon.NetMQ.Subscribers
{
    /// <summary>
    /// Interface encapsulates a connection that is able to connect to <see cref="IAddress"/>es to receive <see cref="Packages.Package"/>
    /// from <see cref="IPublisher"/>s
    /// </summary>
    public interface INetMQSubscriber : ISubscriber, INetMQConnection
    { }
}
