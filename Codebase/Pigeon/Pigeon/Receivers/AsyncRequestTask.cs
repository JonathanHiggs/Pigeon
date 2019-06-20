using System;
using System.Threading.Tasks;

namespace Pigeon.Receivers
{
    public struct AsyncRequestTask
    {
        /// <summary>
        /// Stores a readonly reference to an incoming request
        /// </summary>
        public readonly object Request;


        /// <summary>
        /// Stores a readonly reference to an action that will send a response
        /// </summary>
        public readonly Action<Task<object>> ResponseHandler;


        /// <summary>
        /// Initializes a new instance of a RequestTask composed of the supplied request object and handler
        /// </summary>
        /// <param name="request">The incoming request</param>
        /// <param name="responseHandler">Action to return a response</param>
        public AsyncRequestTask(object request, Action<Task<object>> responseHandler)
        {
            Request = request ?? throw new ArgumentNullException(nameof(request));
            ResponseHandler = responseHandler ?? throw new ArgumentNullException(nameof(responseHandler));
        }
    }
}
