using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Server
{
    /// <summary>
    /// Delegate for handling responses to requests
    /// </summary>
    /// <param name="request">Request object</param>
    /// <returns>Response object</returns>
    internal delegate object RequestHandlerFunction(object request);
}
