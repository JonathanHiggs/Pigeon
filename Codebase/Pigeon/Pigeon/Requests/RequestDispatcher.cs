using System;
using System.Collections.Generic;
using System.Reflection;

using Pigeon.Annotations;
using Pigeon.Diagnostics;
using Pigeon.Receivers;

namespace Pigeon.Requests
{
    /// <summary>
    /// Stores and resolves registered handlers for an incoming request to invoke to prepare responses and respond to
    /// </summary>
    public class RequestDispatcher : IRequestDispatcher
    {
        protected readonly Dictionary<Type, RequestHandlerFunction> requestHandlers = new Dictionary<Type, RequestHandlerFunction>();


        /// <summary>
        /// Dispatches a request and returns the result to the client through the receiver
        /// </summary>
        /// <param name="receiver"><see cref="IReceiver"/> that the request was sent to</param>
        /// <param name="requestTask">Combines all details needed to handle the incoming request</param>
        public void Handle(IReceiver receiver, ref RequestTask requestTask)
        {
            var requestType = requestTask.Request.GetType();
            if (!requestHandlers.TryGetValue(requestType, out var handler))
                throw new RequestHandlerNotFoundException(requestType);

            var response = handler(requestTask.Request);
            requestTask.ResponseHandler(response);
        }


        /// <summary>
        /// Registers an <see cref="IRequestHandler{TRequest, TResponse}"/>
        /// </summary>
        /// <typeparam name="TRequest">Type of request message</typeparam>
        /// <typeparam name="TResponse">Type of response message</typeparam>
        /// <param name="handler">Request handler instance</param>
        public void Register<TRequest, TResponse>(IRequestHandler<TRequest, TResponse> handler)
        {
            ValidateTypes<TRequest, TResponse>();
            requestHandlers.Add(
                typeof(TRequest), 
                request => handler.Handle((TRequest)request));
        }


        /// <summary>
        /// Registers an <see cref="IAsyncRequestHandler{TRequest, TResponse}"/>
        /// </summary>
        /// <typeparam name="TRequest">Type of request message</typeparam>
        /// <typeparam name="TResponse">Type of response message</typeparam>
        /// <param name="handler">Request handler instance</param>
        public void Register<TRequest, TResponse>(IAsyncRequestHandler<TRequest, TResponse> handler)
        {
            ValidateTypes<TRequest, TResponse>();
            requestHandlers.Add(
                typeof(TRequest), 
                request => 
                    handler
                        .Handle((TRequest)request)
                        .GetAwaiter()
                        .GetResult());
        }


        /// <summary>
        /// Registers a <see cref="RequestHandlerDelegate{TRequest, TResponse}"/>
        /// </summary>
        /// <typeparam name="TRequest">Type of request object</typeparam>
        /// <typeparam name="TResponse">Type of response object</typeparam>
        /// <param name="handler">Request handler instance</param>
        public void Register<TRequest, TResponse>(RequestHandlerDelegate<TRequest, TResponse> handler)
        {
            ValidateTypes<TRequest, TResponse>();
            requestHandlers.Add(typeof(TRequest), request => handler((TRequest)request));
        }


        /// <summary>
        /// Registers an <see cref="AsyncRequestHandlerDelegate{TRequest, TResponse}"/>
        /// </summary>
        /// <typeparam name="TRequest">Type of request object</typeparam>
        /// <typeparam name="TResponse">Type of response object</typeparam>
        /// <param name="handler">Request handler instance</param>
        public void RegisterAsync<TRequest, TResponse>(AsyncRequestHandlerDelegate<TRequest, TResponse> handler)
        {
            ValidateTypes<TRequest, TResponse>();

            requestHandlers.Add(
                typeof(TRequest), 
                request => 
                    handler((TRequest)request)
                        .GetAwaiter()
                        .GetResult());
        }


        /// <summary>
        /// Performs checks on the registering types
        /// </summary>
        /// <typeparam name="TRequest">Type of request message</typeparam>
        /// <typeparam name="TResponse">Type of response message</typeparam>
        protected void ValidateTypes<TRequest, TResponse>()
        {
            if (typeof(TRequest).GetCustomAttribute<SerializableAttribute>() is null)
                throw new UnserializableTypeException(typeof(TRequest));

            if (typeof(TResponse).GetCustomAttribute<SerializableAttribute>() is null)
                throw new UnserializableTypeException(typeof(TResponse));

            var requestAttribute = typeof(TRequest).GetCustomAttribute<RequestAttribute>();
            if (requestAttribute is null)
                throw new MissingAttributeException(typeof(TRequest), typeof(RequestAttribute));

            if (requestAttribute.ResponseType != typeof(TResponse))
                throw new MismatchingResponseTypeException(typeof(TRequest), typeof(TResponse));
        }
    }
}
