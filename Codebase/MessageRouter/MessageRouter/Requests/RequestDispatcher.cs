using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Requests
{
    /// <summary>
    /// Finds an appropriate handler for an incoming request to invoke to prepare responses and respond to
    /// </summary>
    public class RequestDispatcher : IRequestDispatcher
    {
        private readonly Dictionary<Type, RequestHandlerFunction> requestHandlers = new Dictionary<Type, RequestHandlerFunction>();


        /// <summary>
        /// Finds and invokes a registered handler for the reuqest to prepare a response
        /// </summary>
        /// <param name="request">Request message</param>
        /// <returns>Response message</returns>
        public object Handle(object request)
        {
            if (null == request)
                throw new ArgumentNullException(nameof(request));

            var requestType = request.GetType();
            if (!requestHandlers.TryGetValue(requestType, out var handler))
                throw new RequestHandlerNotFoundException(requestType);

            return handler(request);
        }


        /// <summary>
        /// Registers an <see cref="IRequestHandler{TRequest, TResponse}"/>
        /// </summary>
        /// <typeparam name="TRequest">Type of request message</typeparam>
        /// <typeparam name="TResponse">Type of response message</typeparam>
        /// <param name="handler">Request handler instance</param>
        /// <returns>Returns the same <see cref="RequestDispatcher"/> for fluent construction</returns>
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
        /// <returns>An empty <see cref="RequestDispatcher"/></returns>
        public static RequestDispatcher Create()
        {
            return new RequestDispatcher();
        }
    }
}
