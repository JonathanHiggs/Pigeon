using Pigeon.Addresses;
using Pigeon.NetMQ.Common;
using Pigeon.Publishers;
using Pigeon.Subscribers;

namespace Pigeon.NetMQ.Subscribers
{
    /// <summary>
    /// Interface encapsulates a connection that is able to connect to <see cref="IAddress"/>es to receive <see cref="Packages.Package"/>
    /// from <see cref="IPublisher"/>s
    /// </summary>
    public interface INetMQSubscriber : ISubscriber, INetMQConnection
    { }
}
