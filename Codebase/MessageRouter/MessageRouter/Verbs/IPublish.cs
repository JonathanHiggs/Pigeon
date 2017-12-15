using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageRouter.Subscribers;

namespace MessageRouter.Verbs
{
    public interface IPublish
    {
        /// <summary>
        /// Distributes a message to any and all connected <see cref="ISubscriber"/>s
        /// </summary>
        /// <typeparam name="TTopic">The topic type of the message to publish</typeparam>
        /// <param name="message">The topic message to distribute</param>
        void Publish<TTopic>(TTopic message)
            where TTopic : class;
    }
}
