using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageRouter.Receivers;

namespace MessageRouter.Verbs
{
    public interface ISend
    {
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
    }
}
