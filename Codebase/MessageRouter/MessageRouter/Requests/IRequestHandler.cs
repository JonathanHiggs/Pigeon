using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Requests
{
    /// <summary>
    /// Interface for defining the handling of the specified request type that
    /// gets registered to an <see cref="IRequestDispatcher"/>
    /// </summary>
    /// <typeparam name="TRequest">Type of request object</typeparam>
    /// <typeparam name="TResponse">Type of response object</typeparam>
    public interface IRequestHandler<TRequest, TResponse>
    {
        /// <summary>
        /// Dispatches the execution of a request object to return the response object
        /// </summary>
        /// <param name="request">Request object</param>
        /// <returns>Response object</returns>
        TResponse Handle(TRequest request);
    }
}
