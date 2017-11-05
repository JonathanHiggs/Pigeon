using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Messages
{
    /// <summary>
    /// Interface defines an abstract factory to control the creation and extraction of data from <see cref="Message"/>s
    /// </summary>
    public interface IMessageFactory
    {
        /// <summary>
        /// Wraps the supplied request object in a <see cref="Message"/>
        /// </summary>
        /// <typeparam name="TRequest">Type of the request object</typeparam>
        /// <param name="request">Request object</param>
        /// <returns>Serializable Message wrapping the request object</returns>
        Message CreateRequest<TRequest>(TRequest request) where TRequest : class;


        /// <summary>
        /// Wraps the supplied response object in a <see cref="Message"/>
        /// </summary>
        /// <typeparam name="TResponse">Type of the response object</typeparam>
        /// <param name="response">Response object</param>
        /// <returns>Serializable Message wrapping the response object</returns>
        Message CreateResponse<TResponse>(TResponse response) where TResponse : class;


        /// <summary>
        /// Extracts a request object from the supplied <see cref="Message"/>. An exception will be throw if the message response is an unexpected type
        /// </summary>
        /// <param name="requestMessage">Request Message wrapper</param>
        /// <returns>Request object</returns>
        object ExtractRequest(Message requestMessage);


        /// <summary>
        /// Extracts a responce object from the supplied <see cref="Message"/>. An exception will be throw if the message response is an unexpected type
        /// </summary>
        /// <typeparam name="TResponse">Type of the response object</typeparam>
        /// <param name="responseMessage">Response Message wrapper</param>
        /// <returns>Response object</returns>
        TResponse ExtractResponse<TResponse>(Message responseMessage) where TResponse : class;
    }
}
