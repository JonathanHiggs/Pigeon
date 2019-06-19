﻿using System;
using System.Runtime.Serialization;

using Pigeon.Senders;

namespace Pigeon.Routing
{
    /// <summary>
    /// Exception that is thrown by a <see cref="ISenderCache"/> when it cannot resolve am <see cref="ISender"/>
    /// for a request
    /// </summary>
    [Serializable]
    public class MissingSenderRouting : Exception
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
        /// <param name="requestType">Tyoe of request made</param>
        /// <param name="message">Message that describes the exception</param>
        /// <param name="inner">Inner exception</param>
        public MissingSenderRouting(Type requestType, string message, Exception inner) 
            : base(message, inner)
        {
            RequestType = requestType;
        }


        protected MissingSenderRouting(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        { }


        public Type RequestType { get; set; }
    }
}
