using System;
using System.Runtime.Serialization;

using Pigeon.Diagnostics;
using Pigeon.Senders;

namespace Pigeon.Routing
{
    /// <summary>
    /// Exception that is thrown by a <see cref="ISenderCache"/> when it cannot resolve am <see cref="ISender"/>
    /// for a request
    /// </summary>
    [Serializable]
    public class MissingSenderRouting : PigeonException
    {
        /// <summary>
        /// Initializes a new instance of <see cref="MissingSenderRouting"/>
        /// </summary>
        /// <param name="requestType">Type of request made</param>
        public MissingSenderRouting(Type requestType)
            : this(requestType, $"No sender routing found for {requestType.FullName}")
        { }


        /// <summary>
        /// Initializes a new instance of <see cref="MissingSenderRouting"/>
        /// </summary>
        /// <param name="requestType">Type of request made</param>
        /// <param name="message">Message that describes the exception</param>
        public MissingSenderRouting(Type requestType, string message) 
            : base(message)
        {
            RequestType = requestType;
        }


        /// <summary>
        /// Initializes a new instance of <see cref="MissingSenderRouting"/>
        /// </summary>
        /// <param name="requestType">Type of request made</param>
        /// <param name="message">Message that describes the exception</param>
        /// <param name="inner">Inner exception</param>
        public MissingSenderRouting(Type requestType, string message, Exception inner) 
            : base(message, inner)
        {
            RequestType = requestType;
        }


        /// <summary>
        /// Initializes a new instance of <see cref="MissingSenderRouting"/> from serialized data
        /// </summary>
        /// <param name="info">Holds the serialized object data about the exception</param>
        /// <param name="context">Contains contextual information about the source or destination</param>
        protected MissingSenderRouting(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        { }


        /// <summary>
        /// Gets and sets the request type
        /// </summary>
        public Type RequestType { get; set; }
    }
}
