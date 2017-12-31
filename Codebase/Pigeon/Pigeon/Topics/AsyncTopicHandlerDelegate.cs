using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pigeon.Topics
{
    /// <summary>
    /// Represents the method to handle <see cref="Pigeon.Subscribers.ISubscriber.Handler"/> events
    /// </summary>
    /// <typeparam name="TTopic">Type of the topic message that is handled</typeparam>
    /// <param name="message">The topic message object that was published</param>
    public delegate Task AsyncTopicHandlerDelegate<TTopic>(TTopic message);
}
