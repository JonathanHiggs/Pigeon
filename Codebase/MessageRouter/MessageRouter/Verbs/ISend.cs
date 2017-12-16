using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageRouter.Receivers;

namespace MessageRouter.Verbs
{
    /// <summary>
    /// Common verb interface that defines how a node is able to send a request message to a remote
    /// </summary>
    public interface ISend
    {
        /// <summary>
        /// Dispatches a request asynchronously through an internally resolved <see cref="ISender"/> to a remote
        /// <see cref="IReceiver"/> with a default timeout of one hour
        /// </summary>
        /// <typeparam name="TRequest">Request message type</typeparam>
        /// <typeparam name="TResponse">Expected response message type</typeparam>
        /// <param name="message">Request message</param>
        /// <returns>Response message</returns>
        Task<TResponse> Send<TRequest, TResponse>(TRequest message)
            where TRequest : class
            where TResponse : class;


        /// <summary>
        /// Dispatches a request asynchronously through an internally resolved <see cref="ISender"/> to a remote
        /// <see cref="IReceiver"/>
        /// </summary>
        /// <typeparam name="TRequest">Request message type</typeparam>
        /// <typeparam name="TResponse">Expected response message type</typeparam>
        /// <param name="message">Request message</param>
        /// <param name="timeout">Time to wait for a response before throwing an exception</param>
        /// <returns>Response message</returns>
        Task<TResponse> Send<TRequest, TResponse>(TRequest message, TimeSpan timeout)
            where TRequest : class
            where TResponse : class;
    }
}
