﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Requests
{
    /// <summary>
    /// Takes incoming request messages and resolved a registered <see cref="IRequestHandler{TRequest, TResponse}"/>
    /// prepare a response message
    /// </summary>
    public interface IRequestDispatcher
    {
        /// <summary>
        /// Dispatching the handling of a message and returns the response
        /// </summary>
        /// <param name="request">Request message</param>
        /// <returns>Response to the request</returns>
        object Handle(object request);


        /// <summary>
        /// Registers an <see cref="IRequestHandler{TRequest, TResponse}"/>
        /// </summary>
        /// <typeparam name="TRequest">Type of request message</typeparam>
        /// <typeparam name="TResponse">Type of response message</typeparam>
        /// <param name="handler">Request handler instance</param>
        /// <returns>Returns the same <see cref="RequestDispatcher"/> for fluent construction</returns>
        RequestDispatcher Register<TRequest, TResponse>(IRequestHandler<TRequest, TResponse> handler);


        /// <summary>
        /// Registers a <see cref="RequestHandlerDelegate{TRequest, TResponse}"/>
        /// </summary>
        /// <typeparam name="TRequest">Type of request object</typeparam>
        /// <typeparam name="TResponse">Type of response object</typeparam>
        /// <param name="handler">Request handler instance</param>
        /// <returns>Returns the same RequestDispatcher instance for fluent construction</returns>
        RequestDispatcher Register<TRequest, TResponse>(RequestHandlerDelegate<TRequest, TResponse> handler);
    }
}
