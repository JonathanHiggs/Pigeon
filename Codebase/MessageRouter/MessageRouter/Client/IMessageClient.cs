using MessageRouter.Senders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Client
{
    public interface IMessageClient
    {
        /// <summary>
        /// Dispatches a request asynchronously to a remote routed by the <see cref="ISenderMonitor"/>
        /// Default timeout of one hour
        /// </summary>
        /// <typeparam name="TRequest">Request type</typeparam>
        /// <typeparam name="TResponse">Expected response type</typeparam>
        /// <param name="request">Request object</param>
        /// <returns>Response object</returns>
        Task<TResponse> Send<TRequest, TResponse>(TRequest request)
            where TRequest : class
            where TResponse : class;


        /// <summary>
        /// Dispatches a request asynchronously to a remote routed by the <see cref="ISenderMonitor"/>
        /// </summary>
        /// <typeparam name="TRequest">Request type</typeparam>
        /// <typeparam name="TResponse">Expected response type</typeparam>
        /// <param name="request">Request object</param>
        /// <param name="timeout">Time to wait for a response before throwing an exception</param>
        /// <returns>Response object</returns>
        Task<TResponse> Send<TRequest, TResponse>(TRequest request, TimeSpan timeout)
            where TRequest : class
            where TResponse : class;


        /// <summary>
        /// Starts the <see cref="Client"/> running
        /// </summary>
        void Start();


        /// <summary>
        /// Stops the <see cref="Client"/> running and disconnects <see cref="ISender"/>
        /// </summary>
        void Stop();
    }
}
