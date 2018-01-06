using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pigeon.Requests;

namespace Pigeon.Fluent.Handlers
{
    class HandlerSetup : IHandlerSetup
    {
        private IDIRequestDispatcher dispatcher;

        public HandlerSetup(IDIRequestDispatcher dispatcher)
        {
            this.dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
        }


        public IHandlerSetup WithAsyncRequestHandler<TRequest, TResponse>(AsyncRequestHandlerDelegate<TRequest, TResponse> handler)
            where TRequest : class
            where TResponse : class
        {
            dispatcher.RegisterAsync(handler);
            return this;
        }


        public IHandlerSetup WithRequestHandler<TRequest, TResponse, THandler>()
            where TRequest : class
            where TResponse : class
            where THandler : IRequestHandler<TRequest, TResponse>
        {
            dispatcher.Register<TRequest, TResponse, THandler>();
            return this;
        }


        public IHandlerSetup WithRequestHandler<TRequest, TResponse>(IRequestHandler<TRequest, TResponse> handler)
            where TRequest : class
            where TResponse : class
        {
            dispatcher.Register(handler);
            return this;
        }


        public IHandlerSetup WithRequestHandler<TRequest, TResponse>(RequestHandlerDelegate<TRequest, TResponse> handler)
            where TRequest : class
            where TResponse : class
        {
            dispatcher.Register(handler);
            return this;
        }
    }
}
