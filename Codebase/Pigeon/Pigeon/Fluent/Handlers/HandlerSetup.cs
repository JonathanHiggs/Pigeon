using System;

using Pigeon.Requests;
using Pigeon.Topics;

namespace Pigeon.Fluent.Handlers
{
    public class HandlerSetup : IHandlerSetup
    {
        private IDIRequestDispatcher requestDispatcher;
        private IDITopicDispatcher topicDispatcher;

        public HandlerSetup(IDIRequestDispatcher requestDispatcher, IDITopicDispatcher topicDispatcher)
        {
            this.requestDispatcher = requestDispatcher ?? throw new ArgumentNullException(nameof(requestDispatcher));
            this.topicDispatcher = topicDispatcher ?? throw new ArgumentNullException(nameof(topicDispatcher));
        }


        public IHandlerSetup WithRequestHandler<TRequest, TResponse, THandler>()
            where TRequest : class
            where TResponse : class
            where THandler : IRequestHandler<TRequest, TResponse>
        {
            requestDispatcher.Register<TRequest, TResponse, THandler>();
            return this;
        }


        public IHandlerSetup WithAsyncRequestHandler<TRequest, TResponse, THandler>()
            where TRequest : class
            where TResponse : class
            where THandler : IAsyncRequestHandler<TRequest, TResponse>
        {
            requestDispatcher.RegisterAsync<TRequest, TResponse, THandler>();
            return this;
        }


        public IHandlerSetup WithRequestHandler<TRequest, TResponse>(IRequestHandler<TRequest, TResponse> handler)
            where TRequest : class
            where TResponse : class
        {
            requestDispatcher.Register(handler);
            return this;
        }


        public IHandlerSetup WithRequestHandler<TRequest, TResponse>(IAsyncRequestHandler<TRequest, TResponse> handler)
            where TRequest : class
            where TResponse : class
        {
            requestDispatcher.Register(handler);
            return this;
        }


        public IHandlerSetup WithRequestHandler<TRequest, TResponse>(RequestHandlerDelegate<TRequest, TResponse> handler)
            where TRequest : class
            where TResponse : class
        {
            requestDispatcher.Register(handler);
            return this;
        }


        public IHandlerSetup WithAsyncRequestHandler<TRequest, TResponse>(AsyncRequestHandlerDelegate<TRequest, TResponse> handler)
            where TRequest : class
            where TResponse : class
        {
            requestDispatcher.RegisterAsync(handler);
            return this;
        }


        public IHandlerSetup WithTopicHandler<TTopic, THandler>()
            where TTopic : class
            where THandler : ITopicHandler<TTopic>
        {
            topicDispatcher.Register<TTopic, THandler>();
            return this;
        }

        public IHandlerSetup WithAsyncTopicHandler<TTopic, THandler>()
            where TTopic : class
            where THandler : IAsyncTopicHandler<TTopic>
        {
            topicDispatcher.RegisterAsync<TTopic, THandler>();
            return this;
        }


        public IHandlerSetup WithTopicHandler<TTopic, THandler>(ITopicHandler<TTopic> handler) where TTopic : class
        {
            topicDispatcher.Register(handler);
            return this;
        }


        public IHandlerSetup WithTopicHandler<TTopic, THandler>(IAsyncTopicHandler<TTopic> handler) where TTopic : class
        {
            topicDispatcher.Register(handler);
            return this;
        }


        public IHandlerSetup WithTopicHandler<TTopic>(TopicHandlerDelegate<TTopic> handler) where TTopic : class
        {
            topicDispatcher.Register(handler);
            return this;
        }


        public IHandlerSetup WithTopicHandler<TTopic>(AsyncTopicHandlerDelegate<TTopic> handler) where TTopic : class
        {
            topicDispatcher.Register(handler);
            return this;
        }
    }
}
