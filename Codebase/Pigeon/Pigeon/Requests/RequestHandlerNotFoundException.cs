using System;
using System.Runtime.Serialization;
using Pigeon.Diagnostics;

namespace Pigeon.Requests
{
    /// <summary>
    /// Exception that is throw when a <see cref="IRequestDispatcher"/> is unable to resolve a handler for the request type
    /// </summary>
    [Serializable]
    public class RequestHandlerNotFoundException : PigeonException
    {
        /// <summary>
        /// Get the type of request for which a handler was not found
        /// </summary>
        public Type RequestType { get; private set; }


        /// <summary>
        /// Initializes a new instance of <see cref="RequestHandlerNotFoundException"/>
        /// </summary>
        /// <param name="requestType">Request type</param>
        public RequestHandlerNotFoundException(Type requestType)
            : this(requestType, $"No request mapped for {requestType.Name}", null)
        { }

        
        /// <summary>
        /// Initializes a new instance of <see cref="RequestHandlerNotFoundException"/>
        /// </summary>
        /// <param name="requestType">Request type</param>
        /// <param name="message">Message that describes the exception</param>
        public RequestHandlerNotFoundException(Type requestType, string message) 
            : this(requestType, message, null)
        { }


        /// <summary>
        /// Initializes a new instance of <see cref="RequestHandlerNotFoundException"/>
        /// </summary>
        /// <param name="requestType">Request type</param>
        /// <param name="message">Message that describes the exception</param>
        /// <param name="inner">Inner exception</param>
        public RequestHandlerNotFoundException(Type requestType, string message, Exception inner) 
            : base(message, inner)
        {
            this.RequestType = requestType;
        }


        /// <summary>
        /// Initializes a new instance of <see cref="RequestHandlerNotFoundException"/> from serialized data
        /// </summary>
        /// <param name="info">Holds the serialized object data about the exception</param>
        /// <param name="context">Contains contextual information about the source or destination</param>
        protected RequestHandlerNotFoundException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        { }
    }
}
