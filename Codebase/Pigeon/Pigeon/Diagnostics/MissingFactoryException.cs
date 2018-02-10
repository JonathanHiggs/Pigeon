﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pigeon.Common;
using Pigeon.Transport;

namespace Pigeon.Diagnostics
{
    /// <summary>
    /// Exception that is thrown when an <see cref="IConnection"/> cache does not have the necessary factory registered to
    /// create the requested <see cref="IConnection"/>
    /// </summary>
    [Serializable]
    public class MissingFactoryException : PigeonException
    {
        /// <summary>
        /// Gets the type of the <see cref="IConnection"/> that was unable to be created
        /// </summary>
        public readonly Type ConnectionType;


        /// <summary>
        /// Gets the type of the <see cref="IConnection"/> cache that was attempting to create it
        /// </summary>
        public readonly Type CacheType;


        /// <summary>
        /// Initializes a new instance of <see cref="MissingFactoryException"/>
        /// </summary>
        /// <param name="connectionType">Type of the <see cref="IConnection"/></param>
        /// <param name="cacheType">Type of the cache trying to create the <see cref="IConnection"/></param>
        public MissingFactoryException(Type connectionType, Type cacheType)
            : this(connectionType, cacheType, $"{cacheType.Name} was unable to create a {connectionType.Name} because it didn't have the required factory")
        { }


        /// <summary>
        /// Initializes a new instance of <see cref="MissingFactoryException"/>
        /// </summary>
        /// <param name="connectionType">Type of the <see cref="IConnection"/></param>
        /// <param name="cacheType">Type of the cache trying to create the <see cref="IConnection"/></param>
        /// <param name="message">Message that describes the exception</param>
        public MissingFactoryException(Type connectionType, Type cacheType, string message) 
            : this(connectionType, cacheType, message, null)
        { }


        /// <summary>
        /// Initializes a new instance of <see cref="MissingFactoryException"/>
        /// </summary>
        /// <param name="connectionType">Type of the <see cref="IConnection"/></param>
        /// <param name="cacheType">Type of the cache trying to create the <see cref="IConnection"/></param>
        /// <param name="message">Message that describes the exception</param>
        /// <param name="inner">Inner exception</param>
        public MissingFactoryException(Type connectionType, Type cacheType, string message, Exception inner) 
            : base(message, inner)
        {
            ConnectionType = connectionType;
            CacheType = cacheType;
        }


        public static MissingFactoryException For<TConnection, TCache>()
            where TConnection : IConnection
        {
            return new MissingFactoryException(typeof(TConnection), typeof(TCache));
        }


        protected MissingFactoryException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}