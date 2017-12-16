using MessageRouter.Addresses;
using MessageRouter.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Routing
{
    /// <summary>
    /// Exception that is thrown when a duplicate routing is added to a router
    /// </summary>
    [Serializable]
    public class RoutingAlreadyRegisteredException<T> : MessageRouterException
    {
        private readonly T duplicateRouting;
        private readonly T existingRouting;


        /// <summary>
        /// Gets the duplicate routing that was attempted to be registered
        /// </summary>
        public T DuplicateRouting => duplicateRouting;


        /// <summary>
        /// Gets the pre-existing routing that was already registered
        /// </summary>
        public T ExistingRouting => existingRouting;

        
        /// <summary>
        /// Initializes a new instance of <see cref="RoutingAlreadyRegisteredException"/>
        /// </summary>
        /// <param name="duplicateRouting">New routing that was attempted to be registered</param>
        /// <param name="existingRouting">Pre-existing routing that was already registered</param>
        public RoutingAlreadyRegisteredException(T duplicateRouting, T existingRouting)
            : this(duplicateRouting, existingRouting, $"New mapping {duplicateRouting.ToString()} already registered with {existingRouting.ToString()}", null)
        { }


        /// <summary>
        /// Initializes a new instance of <see cref="RoutingAlreadyRegisteredException"/>
        /// </summary>
        /// <param name="duplicateRouting">New routing that was attempted to be registered</param>
        /// <param name="existingRouting">Pre-existing routing that was already registered</param>
        /// <param name="message">Message that describes the exception</param>
        public RoutingAlreadyRegisteredException(T duplicateRouting, T existingRouting, string message) 
            : this(duplicateRouting, existingRouting, message, null)
        { }


        /// <summary>
        /// Initializes a new instance of <see cref="RoutingAlreadyRegisteredException"/>
        /// </summary>
        /// <param name="duplicateRouting">New routing that was attempted to be registered</param>
        /// <param name="existingRouting">Pre-existing routing that was already registered</param>
        /// <param name="message">Message that describes the exception</param>
        /// <param name="inner">Inner exception</param>
        public RoutingAlreadyRegisteredException(T duplicateRouting, T existingRouting, string message, Exception inner) 
            : base(message, inner)
        {
            this.duplicateRouting = duplicateRouting;
            this.existingRouting = existingRouting;
        }


        protected RoutingAlreadyRegisteredException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
