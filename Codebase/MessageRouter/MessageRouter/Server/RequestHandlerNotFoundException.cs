using MessageRouter.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Server
{
    /// <summary>
    /// Exception that is throw when a <see cref="IRequestDispatcher"/> is unable to resolve a handler for the request type
    /// </summary>
    [Serializable]
    public class RequestHandlerNotFoundException : MessageRouterException
    {
        private readonly Type requestType;

        
        /// <summary>
        /// Get the type of request for which a handler was not found
        /// </summary>
        public Type RequestType => requestType;


        /// <summary>
        /// Initializes a new instance of a <see cref="RequestHandlerNotFoundException"/>
        /// </summary>
        /// <param name="requestType">Request type</param>
        public RequestHandlerNotFoundException(Type requestType)
            : this(requestType, $"No request mapped for {requestType.Name}", null)
        { }

        
        /// <summary>
        /// Initializes a new instance of a <see cref="RequestHandlerNotFoundException"/>
        /// </summary>
        /// <param name="requestType">Request type</param>
        /// <param name="message">Message that describes the exception</param>
        public RequestHandlerNotFoundException(Type requestType, string message) 
            : this(requestType, message, null)
        { }


        /// <summary>
        /// Initializes a new instance of a <see cref="RequestHandlerNotFoundException"/>
        /// </summary>
        /// <param name="requestType">Request type</param>
        /// <param name="message">Message that describes the exception</param>
        /// <param name="inner">Inner exception</param>
        public RequestHandlerNotFoundException(Type requestType, string message, Exception inner) 
            : base(message, inner)
        {
            this.requestType = requestType;
        }


        protected RequestHandlerNotFoundException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
