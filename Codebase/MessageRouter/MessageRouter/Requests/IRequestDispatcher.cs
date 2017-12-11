using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Requests
{
    /// <summary>
    /// Interface defines the surface for routing the executing of requests
    /// to handle and prepare responses
    /// </summary>
    public interface IRequestDispatcher
    {
        /// <summary>
        /// Dispatching the handling of a Message and returns the response
        /// </summary>
        /// <param name="requestObject">Request object</param>
        /// <returns>Response object</returns>
        object Handle(object requestObject);
    }
}
