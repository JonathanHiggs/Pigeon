using System;
using System.Runtime.Serialization;

namespace Pigeon.Diagnostics
{
    /// <summary>
    /// Exception that is thrown when a type is set for future resolution from an <see cref="IContainer"/>
    /// without a suitable mapping registration
    /// </summary>
    [Serializable]
    public class NotRegisteredException : PigeonException
    {
        /// <summary>
        /// Gets the type that would have been requested and failed to resolve
        /// </summary>
        public Type Type { get; private set; }


        /// <summary>
        /// Initializes a new instance of <see cref="NotRegisteredException"/>
        /// </summary>
        /// <param name="type">Type that would fail to resolve</param>
        public NotRegisteredException(Type type)
            : this(type, $"{type.Name} is not registered in IContainer")
        { }


        /// <summary>
        /// Initializes a new instance of <see cref="NotRegisteredException"/>
        /// </summary>
        /// <param name="type">Type that would fail to resolve</param>
        /// <param name="message">Message that describes the exception</param>
        public NotRegisteredException(Type type, string message) 
            : this(type, message, null)
        { }


        /// <summary>
        /// Initializes a new instance of <see cref="NotRegisteredException"/>
        /// </summary>
        /// <param name="type">Type that would fail to resolve</param>
        /// <param name="message">Message that describes the exception</param>
        /// <param name="inner">Inner exception</param>
        public NotRegisteredException(Type type, string message, Exception inner) 
            : base(message, inner)
        {
            Type = type;
        }


        /// <summary>
        /// Initializes a new instance of <see cref="NotRegisteredException"/> from serialized data
        /// </summary>
        /// <param name="info">Holds the serialized object data about the exception</param>
        /// <param name="context">Contains contextual information about the source or destination</param>
        protected NotRegisteredException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        { }
    }
}
