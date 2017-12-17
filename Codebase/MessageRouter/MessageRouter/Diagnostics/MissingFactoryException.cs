using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageRouter.Common;
using MessageRouter.Transport;

namespace MessageRouter.Diagnostics
{
    /// <summary>
    /// Exception that is thrown when an <see cref="IEndPoint"/> cache does not have the necessary factory registered to
    /// create the requested <see cref="IEndPoint"/>
    /// </summary>
    [Serializable]
    public class MissingFactoryException : MessageRouterException
    {
        /// <summary>
        /// Gets the type of the <see cref="IEndPoint"/> that was unable to be created
        /// </summary>
        public readonly Type EndPointType;


        /// <summary>
        /// Gets the type of the <see cref="IEndPoint"/> cache that was attempting to create it
        /// </summary>
        public readonly Type CacheType;


        /// <summary>
        /// Initializes a new instance of <see cref="MissingFactoryException"/>
        /// </summary>
        /// <param name="endPointType">Type of the <see cref="IEndPoint"/></param>
        /// <param name="cacheType">Type of the cache trying to create the <see cref="IEndPoint"/></param>
        public MissingFactoryException(Type endPointType, Type cacheType)
            : this(endPointType, cacheType, $"{cacheType.Name} was unable to create a {endPointType.Name} because it didn't have the required factory")
        { }


        /// <summary>
        /// Initializes a new instance of <see cref="MissingFactoryException"/>
        /// </summary>
        /// <param name="endPointType">Type of the <see cref="IEndPoint"/></param>
        /// <param name="cacheType">Type of the cache trying to create the <see cref="IEndPoint"/></param>
        /// <param name="message">Message that describes the exception</param>
        public MissingFactoryException(Type endPointType, Type cacheType, string message) 
            : this(endPointType, cacheType, message, null)
        { }


        /// <summary>
        /// Initializes a new instance of <see cref="MissingFactoryException"/>
        /// </summary>
        /// <param name="endPointType">Type of the <see cref="IEndPoint"/></param>
        /// <param name="cacheType">Type of the cache trying to create the <see cref="IEndPoint"/></param>
        /// <param name="message">Message that describes the exception</param>
        /// <param name="inner">Inner exception</param>
        public MissingFactoryException(Type endPointType, Type cacheType, string message, Exception inner) 
            : base(message, inner)
        {
            EndPointType = endPointType;
            CacheType = cacheType;
        }


        public static MissingFactoryException For<TEndPoint, TCache>()
            where TEndPoint : IEndPoint
            where TCache : ICache
        {
            return new MissingFactoryException(typeof(TEndPoint), typeof(TCache));
        }


        protected MissingFactoryException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
