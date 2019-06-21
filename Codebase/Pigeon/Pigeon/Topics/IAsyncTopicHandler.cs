using System.Threading.Tasks;

namespace Pigeon.Topics
{
    /// <summary>
    /// Interface for async handling a specific type of subscription event that gets registered to a
    /// <see cref="ITopicDispatcher"/> and is resolved at runtime when a new event is received from
    /// a remote <see cref="IPublisher"/> source
    /// </summary>
    /// <typeparam name="TTpoic">Type of the topic message</typeparam>
    public interface IAsyncTopicHandler<TTopic>
    {
        /// <summary>
        /// Performs any necessary actions when receiving a topic message
        /// </summary>
        /// <param name="message">Topic message</param>
        Task Handle(TTopic message);
    }
}
