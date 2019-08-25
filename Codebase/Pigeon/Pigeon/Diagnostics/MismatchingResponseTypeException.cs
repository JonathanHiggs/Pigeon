using System;
using System.Reflection;
using System.Runtime.Serialization;

using Pigeon.Annotations;

namespace Pigeon.Diagnostics
{
    /// <summary>
    /// Exception that is thrown when an annotated request contract is used with a response contract that does not match its annotated response class
    /// </summary>
    [Serializable]
    public class MismatchingResponseTypeException : PigeonException
    {
        /// <summary>
        /// Initializes a new instance of <see cref="MismatchingResponseTypeException"/>
        /// </summary>
        /// <param name="requestType">Type of the request class</param>
        /// <param name="responseType">Type of the response class</param>
        public MismatchingResponseTypeException(Type requestType, Type responseType)
            : this(requestType, responseType, $"Request {requestType.FullName} and response {responseType.FullName} do not match with the annotated response type")
        { }


        /// <summary>
        /// Initializes a new instance of <see cref="MismatchingResponseTypeException"/>
        /// </summary>
        /// <param name="requestType">Type of the request class</param>
        /// <param name="responseType">Type of the response class</param>
        /// <param name="message">Message that describes the exception</param>
        public MismatchingResponseTypeException(Type requestType, Type responseType, string message) 
            : this(requestType, responseType, message, null)
        { }


        /// <summary>
        /// Initializes a new instance of <see cref="MismatchingResponseTypeException"/>
        /// </summary>
        /// <param name="requestType">Type of the request class</param>
        /// <param name="responseType">Type of the response class</param>
        /// <param name="message">Message that describes the exception</param>
        /// <param name="inner">Inner exception</param>
        public MismatchingResponseTypeException(Type requestType, Type responseType, string message, Exception inner) 
            : base(message, inner)
        {
            RequestType = requestType;
            ResponseType = responseType;
        }


        /// <summary>
        /// Initialized a new instance of <see cref="MismatchingResponseTypeException"/> with serialized data
        /// </summary>
        /// <param name="info">Holds the serialized object data about the exception</param>
        /// <param name="context">Contains contextual information about the source or destination</param>
        protected MismatchingResponseTypeException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        { }


        /// <summary>
        /// Gets the type of request contract
        /// </summary>
        public Type RequestType { get; }


        /// <summary>
        /// Gets the type of the response contract
        /// </summary>
        public Type ResponseType { get; }


        /// <summary>
        /// Gets the expected response type
        /// </summary>
        public Type ExpectedResponseType => RequestType.GetCustomAttribute<RequestAttribute>()?.ResponseType ?? null;
    }
}
