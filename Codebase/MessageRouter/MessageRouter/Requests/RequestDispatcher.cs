using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Requests
{
    /// <summary>
    /// Prepares responses to requests by routing to a registered handler
    /// </summary>
    public class RequestDispatcher : IRequestDispatcher
    {
        private readonly Dictionary<Type, RequestHandlerFunction> requestHandlers = new Dictionary<Type, RequestHandlerFunction>();


        /// <summary>
        /// Dispatches the handling of a request to a registered handler
        /// </summary>
        /// <param name="requestObject">Request object</param>
        /// <returns>Response object</returns>
        public object Handle(object requestObject)
        {
            if (null == requestObject)
                throw new ArgumentNullException(nameof(requestObject));

            var requestType = requestObject.GetType();
            if (!requestHandlers.ContainsKey(requestType))
                throw new RequestHandlerNotFoundException(requestType);

            return requestHandlers[requestType](requestObject);
        }


        /// <summary>
        /// Registers an <see cref="IRequestHandler{TRequest, TResponse}"/>
        /// </summary>
        /// <typeparam name="TRequest">Type of request object</typeparam>
        /// <typeparam name="TResponse">Type of response object</typeparam>
        /// <param name="handler">Request handler instance</param>
        /// <returns>Returns he same RequestDispatcher instance for fluent construction</returns>
        public RequestDispatcher Register<TRequest, TResponse>(IRequestHandler<TRequest, TResponse> handler)
        {
            requestHandlers.Add(typeof(TRequest), request => handler.Handle((TRequest)request));
            return this;
        }


        /// <summary>
        /// Registers a <see cref="RequestHandlerDelegate{TRequest, TResponse}"/>
        /// </summary>
        /// <typeparam name="TRequest">Type of request object</typeparam>
        /// <typeparam name="TResponse">Type of response object</typeparam>
        /// <param name="handler">Request handler instance</param>
        /// <returns>Returns the same RequestDispatcher instance for fluent construction</returns>
        public RequestDispatcher Register<TRequest, TResponse>(RequestHandlerDelegate<TRequest, TResponse> handler)
        {
            requestHandlers.Add(typeof(TRequest), request => handler((TRequest)request));
            return this;
        }


        /// <summary>
        /// Initializes a new instance of a <see cref="RequestDispatcher"/> used for fluent construction
        /// </summary>
        /// <returns>Empty <see cref="RequestDispatcher"/></returns>
        public static RequestDispatcher Create()
        {
            return new RequestDispatcher();
        }
    }
}
