using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pigeon.Requests
{
    /// <summary>
    /// Interface for defining the handling of the specified request type that
    /// gets registered to an <see cref="IRequestDispatcher"/>
    /// </summary>
    /// <typeparam name="TRequest">Type of request message</typeparam>
    /// <typeparam name="TResponse">Type of response message</typeparam>
    public interface IRequestHandler<TRequest, TResponse>
    {
        /// <summary>
        /// Dispatches the execution of a request object to return the response object
        /// </summary>
        /// <param name="request">Request message</param>
        /// <returns>Response message</returns>
        TResponse Handle(TRequest request);
    }
}
