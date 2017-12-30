using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pigeon.Requests
{
    /// <summary>
    /// Type safe delegate for handling responses to requests
    /// </summary>
    /// <typeparam name="TRequest">Type of request object</typeparam>
    /// <typeparam name="TResponse">Type of response object</typeparam>
    /// <param name="request">REquest object</param>
    /// <returns>Task executing the response</returns>
    public delegate Task<TResponse> AsyncRequestHandlerDelegate<TRequest, TResponse>(TRequest request);
}
