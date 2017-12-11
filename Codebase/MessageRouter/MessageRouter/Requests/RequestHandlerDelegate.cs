using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Requests
{
    /// <summary>
    /// Type safe delegaste for handling responses to requests
    /// </summary>
    /// <typeparam name="TRequest">Type of request object</typeparam>
    /// <typeparam name="TResponse">Type of response object</typeparam>
    /// <param name="request">Request object</param>
    /// <returns>Response object</returns>
    public delegate TResponse RequestHandlerDelegate<TRequest, TResponse>(TRequest request);
}
