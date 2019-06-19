using System;
using System.Runtime.Serialization;

namespace Pigeon.Diagnostics
{
    /// <summary>
    /// Exception that is thrown when a contract class is used without the required attribute annotations
    /// </summary>
    [Serializable]
    public class MissingAttributeException : Exception
    {
        /// <summary>
        /// Initializes a new instance of <see cref="MissingAttributeException"/>
        /// </summary>
        /// <param name="contractType">Type of contract class that is missing an attribute</param>
        /// <param name="attributeType">Type of expected attribute class</param>
        public MissingAttributeException(Type contractType, Type attributeType)
            : this(contractType, attributeType, $"{contractType.FullName} is missing {attributeType.Name}")
        { }


        /// <summary>
        /// Initializes a new instance of <see cref="MissingAttributeException"/>
        /// </summary>
        /// <param name="contractType">Type of contract class that is missing an attribute</param>
        /// <param name="attributeType">Type of expected attribute class</param>
        /// <param name="message">Message that describes the exception</param>
        public MissingAttributeException(Type contractType, Type attributeType, string message) 
            : this(contractType, attributeType, message, null)
        { }


        /// <summary>
        /// Initializes a new instance of <see cref="MissingAttributeException"/>
        /// </summary>
        /// <param name="contractType">Type of contract class that is missing an attribute</param>
        /// <param name="attributeType">Type of expected attribute class</param>
        /// <param name="message">Message that describes the exception</param>
        /// <param name="inner">Inner exception</param>
        public MissingAttributeException(Type contractType, Type attributeType, string message, Exception inner) 
            : base(message, inner)
        {
            ContractType = contractType;
            AttributeType = attributeType;
        }


        protected MissingAttributeException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        { }


        /// <summary>
        /// Gets the type of attribute class expected
        /// </summary>
        public Type AttributeType { get; }


        /// <summary>
        /// Gets the type of contract class that is missing an attribute
        /// </summary>
        public Type ContractType { get; }
    }
}
