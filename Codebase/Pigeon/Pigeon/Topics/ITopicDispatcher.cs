using Pigeon.Subscribers;

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
        /// <param name="subscriber"><see cref="ISubscriber"/> that received the topic message</param>
        /// <param name="message">The message published by a remote source to be handled</param>
        void Handle(ISubscriber subscriber, object message);


        /// <summary>
        /// Registeres a <see cref="ITopicHandler{TEvent}"/>
        /// </summary>
        /// <typeparam name="TTopic">Type of topic message</typeparam>
        /// <param name="handler">Topic message handler</param>
        void Register<TTopic>(ITopicHandler<TTopic> handler);


        /// <summary>
        /// Registers a <see cref="IAsyncTopicHandler{TTopic}"/>
        /// </summary>
        /// <typeparam name="TTopic">Type of topic message</typeparam>
        /// <param name="handler">Topic message handler</param>
        void Register<TTopic>(IAsyncTopicHandler<TTopic> handler);


        /// <summary>
        /// Registers a <see cref="TopicHandlerDelegate{TEvent}"/>
        /// </summary>
        /// <typeparam name="TTopic">Type of topic message</typeparam>
        /// <param name="handler">Topic message handler</param>
        void Register<TTopic>(TopicHandlerDelegate<TTopic> handler);


        /// <summary>
        /// Registers a <see cref="TopicHandlerDelegate{TEvent}"/>
        /// </summary>
        /// <typeparam name="TTopic">Type of topic message</typeparam>
        /// <param name="handler">Topic message handler</param>
        void Register<TTopic>(AsyncTopicHandlerDelegate<TTopic> handler);
    }
}
