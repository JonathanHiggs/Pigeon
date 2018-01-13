using System;

namespace Pigeon.Diagnostics
{
    /// <summary>
    /// Exception that is thrown when a type is set for future resolution from an <see cref="IContainer"/>
    /// without a suitable mapping registration
    /// </summary>
    [Serializable]
    public class NotRegisteredException : PigeonException
    {
        private Type type;


        /// <summary>
        /// Gets the type that would have been requested and failed to resolve
        /// </summary>
        public Type Type => type;


        /// <summary>
        /// Initializes a new instance of <see cref="NotRegisteredException"/>
        /// </summary>
        /// <param name="type">Type that would fail to resolce</param>
        public NotRegisteredException(Type type)
            : this(type, $"{type.Name} is not registered in IContainer")
        { }


        /// <summary>
        /// Initializes a new instance of <see cref="NotRegisteredException"/>
        /// </summary>
        /// <param name="type">Type that would fail to resolce</param>
        /// <param name="message">Message that describes the exception</param>
        public NotRegisteredException(Type type, string message) 
            : this(type, message, null)
        { }


        /// <summary>
        /// Initializes a new instance of <see cref="NotRegisteredException"/>
        /// </summary>
        /// <param name="type">Type that would fail to resolce</param>
        /// <param name="message">Message that describes the exception</param>
        /// <param name="inner">Inner exception</param>
        public NotRegisteredException(Type type, string message, Exception inner) 
            : base(message, inner)
        {
            this.type = type;
        }


        protected NotRegisteredException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
