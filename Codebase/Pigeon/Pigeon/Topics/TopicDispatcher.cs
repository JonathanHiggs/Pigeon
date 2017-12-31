using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pigeon.Topics
{
    /// <summary>
    /// Finds an appropriate handler from the registered handlers to invoke when a topic message is received
    /// received
    /// </summary>
    public class TopicDispatcher : ITopicDispatcher
    {
        private readonly Dictionary<Type, TopicHandlerFunction> handlers = new Dictionary<Type, TopicHandlerFunction>();


        /// <summary>
        /// Finds and invokes a registered handler for the topic message
        /// </summary>
        /// <param name="message">Topic message</param>
        public Task Handle(object message)
        {
            if (null == message)
                return Task.CompletedTask;

            var eventType = message.GetType();
            if (!handlers.TryGetValue(eventType, out var handler))
                return Task.CompletedTask;

            return handler(message);
        }


        /// <summary>
        /// Registeres a <see cref="ITopicHandler{TEvent}"/>
        /// </summary>
        /// <typeparam name="TTopic">Type of the topic message</typeparam>
        /// <param name="handler">Topic message handler</param>
        /// <returns>Returns the same <see cref="ITopicDispatcher"/> for fluent construction</returns>
        public ITopicDispatcher Register<TTopic>(ITopicHandler<TTopic> handler)
        {
            handlers.Add(typeof(TTopic), eventMessage => Task.Run(() => handler.Handle((TTopic)eventMessage)));
            return this;
        }


        /// <summary>
        /// Registers a <see cref="TopicHandlerDelegate{TEvent}"/>
        /// </summary>
        /// <typeparam name="TTopic">Type of the topic message</typeparam>
        /// <param name="handler">Topic message handler</param>
        /// <returns>Returns the same <see cref="ITopicDispatcher"/> for fluent construction</returns>
        public ITopicDispatcher Register<TTopic>(TopicHandlerDelegate<TTopic> handler)
        {
            handlers.Add(typeof(TTopic), eventMessage => Task.Run(() => handler((TTopic)eventMessage)));
            return this;
        }


        /// <summary>
        /// Registers a <see cref="TopicHandlerDelegate{TEvent}"/>
        /// </summary>
        /// <typeparam name="TTopic">Type of the topic message</typeparam>
        /// <param name="handler">Topic message handler</param>
        /// <returns>Returns the same <see cref="ITopicDispatcher"/> for fluent construction</returns>
        public ITopicDispatcher RegisterAsync<TTopic>(AsyncTopicHandlerDelegate<TTopic> handler)
        {
            handlers.Add(typeof(TTopic), eventMessage => handler((TTopic)eventMessage));
            return this;
        }
    }
}
