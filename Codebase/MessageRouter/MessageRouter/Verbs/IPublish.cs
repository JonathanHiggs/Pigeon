using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageRouter.Subscribers;

namespace MessageRouter.Verbs
{
    /// <summary>
    /// Common verb interface that defines now a node is able to publish topic messages to remotes
    /// </summary>
    public interface IPublish
    {
        /// <summary>
        /// Distributes a message to any and all connected <see cref="ISubscriber"/>s
        /// </summary>
        /// <typeparam name="TTopic">The topic type of the message to publish</typeparam>
        /// <param name="topicEvent">The topic message to distribute</param>
        void Publish<TTopic>(TTopic topicEvent)
            where TTopic : class;
    }
}
