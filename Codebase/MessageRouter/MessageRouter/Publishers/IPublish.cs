using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageRouter.Subscribers;

namespace MessageRouter.Publishers
{
    public interface IPublish
    {
        /// <summary>
        /// Distributes a message to any and all connected <see cref="ISubscriber"/>s
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="message">The published message to distribute</param>
        void Publish<TMessage>(TMessage message)
            where TMessage : class;
    }
}
