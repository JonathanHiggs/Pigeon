using Pigeon.Common;
using Pigeon.Subscribers;

namespace Pigeon.Publishers
{
    /// <summary>
    /// Interface encapsulates a connection that is able to publish events to <see cref="ISubscriber"/>s
    /// </summary>
    public interface IPublisher : IConnection
    {
        /// <summary>
        /// Transmits the events to all connected <see cref="ISubscriber"/>s
        /// </summary>
        /// <param name="topicEvent">Topic event to be sent to all remote <see cref="Subscribers.ISubscriber"/>s</param>
        void Publish(object topicEvent);
    }
}
