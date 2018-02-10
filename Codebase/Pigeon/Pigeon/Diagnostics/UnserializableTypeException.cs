﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pigeon.Diagnostics;

namespace Pigeon.Diagnostics
{
    /// <summary>
    /// Exception that is thrown when trying to register a type that is not serializable
    /// </summary>
    [Serializable]
    public class UnserializableTypeException : PigeonException
    {
        private readonly Type type;


        /// <summary>
        /// Gets the type that is unserilizable
        /// </summary>
        public Type Type => type;


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
            this.type = type;
        }


        protected UnserializableTypeException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}