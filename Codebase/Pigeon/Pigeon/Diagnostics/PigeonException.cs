using System;

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


        protected PigeonException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
