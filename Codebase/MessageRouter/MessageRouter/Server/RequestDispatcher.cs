using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Server
{
    internal delegate object RequestHandlerFunction(object request);

    public delegate TResponse RequestHandler<TRequest, TResponse>(TRequest request);


    public class RequestDispatcher : IRequestDispatcher
    {
        private readonly Dictionary<Type, RequestHandlerFunction> requestHandlers = new Dictionary<Type, Server.RequestHandlerFunction>();


        public object Handle(object requestObject)
        {
            if (null == requestObject)
                throw new ArgumentNullException(nameof(requestObject));

            var requestType = requestObject.GetType();
            if (!requestHandlers.ContainsKey(requestType))
                throw new InvalidOperationException($"No handler registered for request type {requestType.Name}");

            return requestHandlers[requestType](requestObject);
        }


        public RequestDispatcher Register<TRequest, TResponse>(IRequestHandler<TRequest, TResponse> handler)
        {
            requestHandlers.Add(typeof(TRequest), request => handler.Handle((TRequest)request));
            return this;
        }


        public RequestDispatcher Register<TRequest, TResponse>(RequestHandler<TRequest, TResponse> handler)
        {
            requestHandlers.Add(typeof(TRequest), request => handler((TRequest)request));
            return this;
        }
    }
}
