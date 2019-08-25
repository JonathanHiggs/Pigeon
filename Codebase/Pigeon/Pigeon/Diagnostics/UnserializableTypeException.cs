using System;
using System.Runtime.Serialization;

namespace Pigeon.Diagnostics
{
    /// <summary>
    /// Exception that is thrown when trying to register a type that is not serializable
    /// </summary>
    [Serializable]
    public class UnserializableTypeException : PigeonException
    {
        /// <summary>
        /// Gets the type that is unserializable
        /// </summary>
        public Type Type { get; private set; }


        /// <summary>
        /// Initializes a new instance of <see cref="UnserializableTypeException"/>
        /// </summary>
        /// <param name="type">Type that is unserializable</param>
        public UnserializableTypeException(Type type)
            : this(type, $"{type.Name} is not serializable")
        { }


        /// <summary>
        /// Initializes a new instance of <see cref="UnserializableTypeException"/>
        /// </summary>
        /// <param name="type">Type that is unserializable</param>
        /// <param name="message">Message that describes the exception</param>
        public UnserializableTypeException(Type type, string message) 
            : this(type, message, null)
        { }


        /// <summary>
        /// Initializes a new instance of <see cref="UnserializableTypeException"/>
        /// </summary>
        /// <param name="type">Type that is unserializable</param>
        /// <param name="message">Message that describes the exception</param>
        /// <param name="inner">Inner exception</param>
        public UnserializableTypeException(Type type, string message, Exception inner) 
            : base(message, inner)
        {
            this.Type = type;
        }


        /// <summary>
        /// Initializes a new instance of <see cref="UnserializableTypeException"/> from serialized data
        /// </summary>
        /// <param name="info">Holds the serialized object data about the exception</param>
        /// <param name="context">Contains contextual information about the source or destination</param>
        protected UnserializableTypeException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        { }
    }
}
