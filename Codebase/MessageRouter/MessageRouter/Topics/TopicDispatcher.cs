using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Topics
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
        public void Handle(object message)
        {
            if (null == message)
                return;

            var eventType = message.GetType();
            if (!handlers.TryGetValue(eventType, out var handler))
                return;

            handler(message);
        }


        /// <summary>
        /// Registeres a <see cref="ITopicHandler{TEvent}"/>
        /// </summary>
        /// <typeparam name="TTopic">Type of the topic message</typeparam>
        /// <param name="handler">Topic message handler</param>
        /// <returns>Returns the same <see cref="TopicDispatcher"/> for fluent construction</returns>
        public TopicDispatcher Register<TTopic>(ITopicHandler<TTopic> handler)
        {
            handlers.Add(typeof(TTopic), eventMessage => handler.Handle((TTopic)eventMessage));
            return this;
        }


        /// <summary>
        /// Registers a <see cref="TopicHandler{TEvent}"/>
        /// </summary>
        /// <typeparam name="TTopic">Type of the topic message</typeparam>
        /// <param name="handler">Topic message handler</param>
        /// <returns>Returns the same <see cref="TopicDispatcher"/> for fluent construction</returns>
        public TopicDispatcher Register<TTopic>(TopicHandler<TTopic> handler)
        {
            handlers.Add(typeof(TTopic), eventMessage => handler((TTopic)eventMessage));
            return this;
        }


        /// <summary>
        /// Initializes a new instance of <see cref="TopicDispatcher"/> for fluent construction
        /// </summary>
        /// <returns>An empty <see cref="TopicDispatcher"/></returns>
        public static TopicDispatcher Create()
        {
            return new TopicDispatcher();
        }
    }
}
