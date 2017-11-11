using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Diagnostics
{
    /// <summary>
    /// Represents an error when a SenderManager is unable to resolve a sender for request type
    /// </summary>
    [Serializable]
    public class SenderNotRegisteredException : MessageRouterException
    {
        private readonly Type requestType;
        

        /// <summary>
        /// Gets the Type of the request
        /// </summary>
        public Type RequestType => requestType;
        
        
        /// <summary>
        /// Initializes a new SenderNotRegisteredException
        /// </summary>
        /// <param name="requestType">Request type</param>
        public SenderNotRegisteredException(Type requestType)
            : this(requestType, $"No sender registered for {requestType.Name}")
        { }


        /// <summary>
        /// Initializes a new SenderNotRegisteredException
        /// </summary>
        /// <param name="requestType">Request type</param>
        /// <param name="message">Exception message</param>
        public SenderNotRegisteredException(Type requestType, string message) 
            : this(requestType, message, null)
        { }


        /// <summary>
        /// Initializes a new SenderNotRegisteredException
        /// </summary>
        /// <param name="requestType">Request type</param>
        /// <param name="message">Exception message</param>
        /// <param name="inner">Inner exception</param>
        public SenderNotRegisteredException(Type requestType, string message, Exception inner) 
            : base(message, inner)
        {
            this.requestType = requestType ?? throw new ArgumentNullException(nameof(requestType));
        }


        /// <summary>
        /// Initializes a new SenderNotRegisteredException
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected SenderNotRegisteredException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
