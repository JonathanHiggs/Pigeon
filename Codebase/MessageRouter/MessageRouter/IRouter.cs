using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageRouter.Receivers;
using MessageRouter.Senders;

namespace MessageRouter
{
    /// <summary>
    /// <see cref="IRouter{TRouterInfo}"/> represents a complete abstraction of the transport layer to all other
    /// parts of the architecture
    /// </summary>
    /// <typeparam name="TRouterInfo"></typeparam>
    public interface IRouter<TRouterInfo> where TRouterInfo : IRouterInfo
    {
        /// <summary>
        /// Gets a <see cref="TRouterInfo"/> to access state information of the <see cref="IRouter{TRouterInfo}"/>
        /// </summary>
        TRouterInfo Info { get; }


        /// <summary>
        /// Dispatches a request asynchronously through an internally resolved <see cref="ISender"/> to a remote
        /// <see cref="IReceiver"/> with a default timeout of one hour
        /// </summary>
        /// <typeparam name="TRequest">Request type</typeparam>
        /// <typeparam name="TResponse">Expected response type</typeparam>
        /// <param name="request">Request object</param>
        /// <returns>Response object</returns>
        Task<TResponse> Send<TRequest, TResponse>(TRequest request)
            where TRequest : class
            where TResponse : class;


        /// <summary>
        /// Dispatches a request asynchronously through an internally resolved <see cref="ISender"/> to a remote
        /// <see cref="IReceiver"/>
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
        /// Starts all internal active transports running
        /// </summary>
        void Start();


        /// <summary>
        /// Stops all internal active transports running
        /// </summary>
        void Stop();
    }
}
