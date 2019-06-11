using System;

using Pigeon.Requests;
using Pigeon.Topics;

namespace Pigeon.Fluent.Handlers
{
    public class HandlerSetup : IHandlerSetup
    {
        private IDIRequestDispatcher requestdispatcher;
        private IDITopicDispatcher topicDispatcher;

        public HandlerSetup(IDIRequestDispatcher requestDispatcher, IDITopicDispatcher topicDispatcher)
        {
            this.requestdispatcher = requestDispatcher ?? throw new ArgumentNullException(nameof(requestDispatcher));
            this.topicDispatcher = topicDispatcher ?? throw new ArgumentNullException(nameof(topicDispatcher));
        }


        public IHandlerSetup WithAsyncRequestHandler<TRequest, TResponse>(AsyncRequestHandlerDelegate<TRequest, TResponse> handler)
            where TRequest : class
            where TResponse : class
        {
            requestdispatcher.RegisterAsync(handler);
            return this;
        }


        public IHandlerSetup WithRequestHandler<TRequest, TResponse, THandler>()
            where TRequest : class
            where TResponse : class
            where THandler : IRequestHandler<TRequest, TResponse>
        {
            requestdispatcher.Register<TRequest, TResponse, THandler>();
            return this;
        }


        public IHandlerSetup WithRequestHandler<TRequest, TResponse>(IRequestHandler<TRequest, TResponse> handler)
            where TRequest : class
            where TResponse : class
        {
            requestdispatcher.Register(handler);
            return this;
        }


        public IHandlerSetup WithRequestHandler<TRequest, TResponse>(RequestHandlerDelegate<TRequest, TResponse> handler)
            where TRequest : class
            where TResponse : class
        {
            requestdispatcher.Register(handler);
            return this;
        }


        public IHandlerSetup WithTopicHandler<TTopic, THandler>()
            where TTopic : class
            where THandler : ITopicHandler<TTopic>
        {
            topicDispatcher.Register<TTopic, THandler>();
            return this;
        }
    }
}
