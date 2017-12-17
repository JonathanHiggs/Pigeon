using MessageRouter.Addresses;
using MessageRouter.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Diagnostics
{
    /// <summary>
    /// Exception that is thrown when a overriding routing is added to a router
    /// </summary>
    [Serializable]
    public class RoutingAlreadyRegisteredException<T> : MessageRouterException
    {
        private readonly T overridingRouting;
        private readonly T existingRouting;


        /// <summary>
        /// Gets the overriding routing that was attempted to be registered
        /// </summary>
        public T OverridingRouting => overridingRouting;


        /// <summary>
        /// Gets the pre-existing routing that was already registered
        /// </summary>
        public T ExistingRouting => existingRouting;

        
        /// <summary>
        /// Initializes a new instance of <see cref="RoutingAlreadyRegisteredException"/>
        /// </summary>
        /// <param name="overridingRouting">New routing that was attempted to be registered</param>
        /// <param name="existingRouting">Pre-existing routing that was already registered</param>
        public RoutingAlreadyRegisteredException(T overridingRouting, T existingRouting)
            : this(overridingRouting, existingRouting, $"New mapping {overridingRouting.ToString()} already registered with {existingRouting.ToString()}", null)
        { }


        /// <summary>
        /// Initializes a new instance of <see cref="RoutingAlreadyRegisteredException"/>
        /// </summary>
        /// <param name="overridingRouting">New routing that was attempted to be registered</param>
        /// <param name="existingRouting">Pre-existing routing that was already registered</param>
        /// <param name="message">Message that describes the exception</param>
        public RoutingAlreadyRegisteredException(T overridingRouting, T existingRouting, string message) 
            : this(overridingRouting, existingRouting, message, null)
        { }


        /// <summary>
        /// Initializes a new instance of <see cref="RoutingAlreadyRegisteredException"/>
        /// </summary>
        /// <param name="overridingRouting">New routing that was attempted to be registered</param>
        /// <param name="existingRouting">Pre-existing routing that was already registered</param>
        /// <param name="message">Message that describes the exception</param>
        /// <param name="inner">Inner exception</param>
        public RoutingAlreadyRegisteredException(T overridingRouting, T existingRouting, string message, Exception inner) 
            : base(message, inner)
        {
            this.overridingRouting = overridingRouting;
            this.existingRouting = existingRouting;
        }


        protected RoutingAlreadyRegisteredException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
