using MessageRouter.Packages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Receivers
{
    /// <summary>
    /// Struct to combine an incoming request with a handler to return the response
    /// </summary>
    public struct RequestTask
    {
        /// <summary>
        /// Stores a readonly reference to an incoming request
        /// </summary>
        public readonly object Request;


        /// <summary>
        /// Stores a readonly reference to an action that will send a response
        /// </summary>
        public readonly Action<object> ResponseHandler;
        

        /// <summary>
        /// Initializes a new instance of a RequestTask composed of the supplied request object and handler
        /// </summary>
        /// <param name="request">The incoming request</param>
        /// <param name="responseHandler">Action to return a response</param>
        public RequestTask(object request, Action<object> responseHandler)
        {
            Request = request ?? throw new ArgumentNullException(nameof(request));
            ResponseHandler = responseHandler ?? throw new ArgumentNullException(nameof(responseHandler));
        }
    }
}
