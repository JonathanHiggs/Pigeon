using Pigeon.Receivers;

namespace Pigeon.Requests
{
    /// <summary>
    /// Takes incoming request messages and resolved a registered <see cref="IRequestHandler{TRequest, TResponse}"/>
    /// prepare a response message
    /// </summary>
    public interface IRequestDispatcher
    {
        /// <summary>
        /// Dispatches a request and returns the result to the client through the receiver
        /// </summary>
        /// <param name="requestTask">Combines all details needed to handle the incoming request</param>
        void Handle(ref RequestTask requestTask);


        /// <summary>
        /// Registers an <see cref="IRequestHandler{TRequest, TResponse}"/>
        /// </summary>
        /// <typeparam name="TRequest">Type of request message</typeparam>
        /// <typeparam name="TResponse">Type of response message</typeparam>
        /// <param name="handler">Request handler instance</param>
        void Register<TRequest, TResponse>(IRequestHandler<TRequest, TResponse> handler);


        /// <summary>
        /// Registers an <see cref="IAsyncRequestHandler{TRequest, TResponse}"/>
        /// </summary>
        /// <typeparam name="TRequest">Type of request message</typeparam>
        /// <typeparam name="TResponse">Type of response message</typeparam>
        /// <param name="handler">Request handler instance</param>
        void Register<TRequest, TResponse>(IAsyncRequestHandler<TRequest, TResponse> handler);


        /// <summary>
        /// Registers a <see cref="RequestHandlerDelegate{TRequest, TResponse}"/>
        /// </summary>
        /// <typeparam name="TRequest">Type of request object</typeparam>
        /// <typeparam name="TResponse">Type of response object</typeparam>
        /// <param name="handler">Request handler instance</param>
        void Register<TRequest, TResponse>(RequestHandlerDelegate<TRequest, TResponse> handler);


        /// <summary>
        /// Registers an <see cref="AsyncRequestHandlerDelegate{TRequest, TResponse}"/>
        /// </summary>
        /// <typeparam name="TRequest">Type of request object</typeparam>
        /// <typeparam name="TResponse">Type of response object</typeparam>
        /// <param name="handler">Request handler instance</param>
        void RegisterAsync<TRequest, TResponse>(AsyncRequestHandlerDelegate<TRequest, TResponse> handler);
    }
}
