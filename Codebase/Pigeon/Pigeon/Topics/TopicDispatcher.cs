using System;
using System.Collections.Generic;
using System.Reflection;

using Pigeon.Annotations;
using Pigeon.Diagnostics;
using Pigeon.Subscribers;

namespace Pigeon.Topics
{
    /// <summary>
    /// Finds an appropriate handler from the registered handlers to invoke when a topic message is received
    /// received
    /// </summary>
    public class TopicDispatcher : ITopicDispatcher
    {
        protected readonly Dictionary<Type, TopicHandlerFunction> handlers = new Dictionary<Type, TopicHandlerFunction>();


        /// <summary>
        /// Finds and invokes a registered handler for the topic message
        /// </summary>
        /// <param name="subscriber"><see cref="ISubscriber"/> that received the topic message</param>
        /// <param name="message">Topic message</param>
        public void Handle(ISubscriber subscriber, object message)
        {
            if (message is null)
                return;

            var eventType = message.GetType();
            if (!handlers.TryGetValue(eventType, out var handler))
                return; // ToDo: add some logging?

            handler(message);
        }


        /// <summary>
        /// Registeres a <see cref="ITopicHandler{TEvent}"/>
        /// </summary>
        /// <typeparam name="TTopic">Type of the topic message</typeparam>
        /// <param name="handler">Topic message handler</param>
        public void Register<TTopic>(ITopicHandler<TTopic> handler)
        {
            Validate<TTopic>();
            handlers.Add(
                typeof(TTopic), 
                eventMessage => handler.Handle((TTopic)eventMessage));
        }


        /// <summary>
        /// Registeres a <see cref="ITopicHandler{TEvent}"/>
        /// </summary>
        /// <typeparam name="TTopic">Type of the topic message</typeparam>
        /// <param name="handler">Topic message handler</param>
        public void Register<TTopic>(IAsyncTopicHandler<TTopic> handler)
        {
            Validate<TTopic>();
            handlers.Add(
                typeof(TTopic), 
                eventMessage => handler.Handle((TTopic)eventMessage));
        }


        /// <summary>
        /// Registers a <see cref="TopicHandlerDelegate{TEvent}"/>
        /// </summary>
        /// <typeparam name="TTopic">Type of the topic message</typeparam>
        /// <param name="handler">Topic message handler</param>
        public void Register<TTopic>(TopicHandlerDelegate<TTopic> handler)
        {
            Validate<TTopic>();

            // Note: don't need to GetAwaiter().GetResult() since there is no return
            handlers.Add(
                typeof(TTopic), 
                eventMessage => handler((TTopic)eventMessage));
        }


        /// <summary>
        /// Registers a <see cref="TopicHandlerDelegate{TEvent}"/>
        /// </summary>
        /// <typeparam name="TTopic">Type of the topic message</typeparam>
        /// <param name="handler">Topic message handler</param>
        public void Register<TTopic>(AsyncTopicHandlerDelegate<TTopic> handler)
        {
            Validate<TTopic>();

            // Note: don't need to GetAwaiter().GetResult() since there is no return
            handlers.Add(
                typeof(TTopic), 
                eventMessage => handler((TTopic)eventMessage));
        }


        /// <summary>
        /// Performs correctness checks on the registering type
        /// </summary>
        /// <typeparam name="TTopic"></typeparam>
        protected void Validate<TTopic>()
        {
            if (typeof(TTopic).GetCustomAttribute<SerializableAttribute>() is null)
                throw new UnserializableTypeException(typeof(TTopic));

            if (typeof(TTopic).GetCustomAttribute<TopicAttribute>() is null)
                throw new MissingAttributeException(typeof(TTopic), typeof(TopicAttribute));
        }
    }
}
