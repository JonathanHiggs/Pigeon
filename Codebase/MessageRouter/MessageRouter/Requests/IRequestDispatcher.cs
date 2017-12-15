using System;
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
    }
}
