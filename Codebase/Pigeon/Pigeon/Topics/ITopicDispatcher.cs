using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pigeon.Topics
{
    /// <summary>
    /// Takes an incoming topic message and resolves a handler to process it
    /// </summary>
    public interface ITopicDispatcher
    {
        /// <summary>
        /// Dispatches the handling of a published message
        /// </summary>
        /// <param name="message">The message published by a remote source to be handled</param>
        Task Handle(object message);


        /// <summary>
        /// Registeres a <see cref="ITopicHandler{TEvent}"/>
        /// </summary>
        /// <typeparam name="TTopic">Type of the topic message</typeparam>
        /// <param name="handler">Topic message handler</param>
        void Register<TTopic>(ITopicHandler<TTopic> handler);


        /// <summary>
        /// Registers a <see cref="TopicHandlerDelegate{TEvent}"/>
        /// </summary>
        /// <typeparam name="TTopic">Type of the topic message</typeparam>
        /// <param name="handler">Topic message handler</param>
        void Register<TTopic>(TopicHandlerDelegate<TTopic> handler);


        /// <summary>
        /// Registers a <see cref="TopicHandlerDelegate{TEvent}"/>
        /// </summary>
        /// <typeparam name="TTopic">Type of the topic message</typeparam>
        /// <param name="handler">Topic message handler</param>
        void RegisterAsync<TTopic>(AsyncTopicHandlerDelegate<TTopic> handler);
    }
}
