using System;
using System.Runtime.Serialization;

namespace Pigeon.Serialization
{
    /// <summary>
    /// Exception that is thrown when <see cref="ISerializerCache"/> is not able to find a matching
    /// <see cref="ISerializer"/>
    /// </summary>
    [Serializable]
    public class MissingSerializerException : Exception
    {
        /// <summary>
        /// Gets the # representing the expected <see cref="ISerializer"/>
        /// </summary>
        public string Name { get; }


        /// <summary>
        /// Initializes a new instance of <see cref="MissingSerializerException"/>
        /// </summary>
        /// <param name="name">Name of the expected <see cref="ISerializer"/></param>
        public MissingSerializerException(string name)
            : this(name, $"Serializer {name} not found", null)
        { }

        
        /// <summary>
        /// Initializes a new instance of <see cref="MissingSerializerException"/>
        /// </summary>
        /// <param name="name">Name of the expected <see cref="ISerializer"/></param>
        /// <param name="message">Message that describes the exception</param>
        public MissingSerializerException(string name, string message) 
            : this(name, message, null) 
        { }


        /// <summary>
        /// Initializes a new instance of <see cref="MissingSerializerException"/>
        /// </summary>
        /// <param name="name">Name of the expected <see cref="ISerializer"/></param>
        /// <param name="message">Message that describes the exception</param>
        /// <param name="inner">Inner exception</param>
        public MissingSerializerException(string name, string message, Exception inner) 
            : base(message, inner)
        {
            Name = name;
        }


        /// <summary>
        /// Initializes a new instance of <see cref="MissingSerializerException"/> from serializes data. #meta
        /// </summary>
        /// <param name="info">Holds the serialized object data about the exception</param>
        /// <param name="context">Contains contextual information about the source or destination</param>
        protected MissingSerializerException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        { }
    }
}
