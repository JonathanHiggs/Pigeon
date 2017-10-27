using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Server
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
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
