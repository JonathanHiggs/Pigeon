﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Pigeon.Diagnostics;

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
        public void Register<TTopic>(ITopicHandler<TTopic> handler)
        {
            Validate<TTopic>();
            handlers.Add(typeof(TTopic), eventMessage => Task.Run(() => handler.Handle((TTopic)eventMessage)));
        }


        /// <summary>
        /// Registers a <see cref="TopicHandlerDelegate{TEvent}"/>
        /// </summary>
        /// <typeparam name="TTopic">Type of the topic message</typeparam>
        /// <param name="handler">Topic message handler</param>
        public void Register<TTopic>(TopicHandlerDelegate<TTopic> handler)
        {
            Validate<TTopic>();
            handlers.Add(typeof(TTopic), eventMessage => Task.Run(() => handler((TTopic)eventMessage)));
        }


        /// <summary>
        /// Registers a <see cref="TopicHandlerDelegate{TEvent}"/>
        /// </summary>
        /// <typeparam name="TTopic">Type of the topic message</typeparam>
        /// <param name="handler">Topic message handler</param>
        public void RegisterAsync<TTopic>(AsyncTopicHandlerDelegate<TTopic> handler)
        {
            Validate<TTopic>();
            handlers.Add(typeof(TTopic), eventMessage => handler((TTopic)eventMessage));
        }


        protected void Validate<TTopic>()
        {
            if (null == typeof(TTopic).GetCustomAttribute<SerializableAttribute>())
                throw new UnserializableTypeException(typeof(TTopic));
        }
    }
}