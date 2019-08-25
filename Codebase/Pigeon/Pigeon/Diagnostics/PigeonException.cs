using System;
using System.Runtime.Serialization;

namespace Pigeon.Diagnostics
{
    /// <summary>
    /// Base class for all Pigeon runtime exceptions
    /// </summary>
    [Serializable]
    public abstract class PigeonException : Exception
    {
        /// <summary>
        /// Initializes a new instance of <see cref="PigeonException"/>
        /// </summary>
        public PigeonException() { }


        /// <summary>
        /// Initializes a new instance of <see cref="PigeonException"/>
        /// </summary>
        /// <param name="message">Message that describes the exception</param>
        public PigeonException(string message) : base(message) { }


        /// <summary>
        /// Initializes a new instance of <see cref="PigeonException"/>
        /// </summary>
        /// <param name="message">Message that describes the exception</param>
        /// <param name="inner">Inner exception</param>
        public PigeonException(string message, Exception inner) : base(message, inner) { }


        /// <summary>
        /// Initializes a new instance of <see cref="PigeonException"/> with serialized data
        /// </summary>
        /// <param name="info">Holds the serialized object data about the exception</param>
        /// <param name="context">Contains contextual information about the source or destination</param>
        protected PigeonException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        { }
    }
}
