using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageRouter.Publishers;

namespace MessageRouter.Topics
{
    /// <summary>
    /// Interface for handling a specific type of subscription event that gets registered to a
    /// <see cref="ITopicDispatcher"/> and is resolved at runtime when a new event is received from
    /// a remote <see cref="IPublisher"/> source
    /// </summary>
    /// <typeparam name="TTpoic">Type of the topic message</typeparam>
    public interface ITopicHandler<TTpoic>
    {
        /// <summary>
        /// Performs any necessary actions when receiving a topic message
        /// </summary>
        /// <param name="message">Topic message</param>
        void Handle(TTpoic message);
    }
}
