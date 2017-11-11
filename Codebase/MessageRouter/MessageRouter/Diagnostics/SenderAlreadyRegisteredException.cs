using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Diagnostics
{
    /// <summary>
    /// Represents an error in which a sender is attempted to be registered when there is
    /// a preexisting sender also registered for the given request type
    /// </summary>
    [Serializable]
    public class SenderAlreadyRegisteredException : MessageRouterException
    {
        private Type requestType;
        private ISender sender;


        /// <summary>
        /// Gets the type of the request
        /// </summary>
        public Type RequestType => requestType;


        /// <summary>
        /// Gets the sender that is registered and blocking a new registration
        /// </summary>
        public ISender Sender => sender;


        /// <summary>
        /// Initializes a new SenderAlreadyRegisteredException
        /// </summary>
        /// <param name="sender">Existing sender</param>
        /// <param name="requestType">Type of request</param>
        public SenderAlreadyRegisteredException(ISender sender, Type requestType)
            : this(sender, requestType, $"Sender is already registered for {requestType.Name}")
        { }


        /// <summary>
        /// Initializes a new SenderAlreadyRegisteredException
        /// </summary>
        /// <param name="sender">Existing sender</param>
        /// <param name="requestType">Type of request</param>
        /// <param name="message">Exception message</param>
        public SenderAlreadyRegisteredException(ISender sender, Type requestType, string message)
            : this(sender, requestType, message, null)
        { }


        /// <summary>
        /// Initializes a new SenderAlreadyRegisteredException
        /// </summary>
        /// <param name="sender">Existing sender</param>
        /// <param name="requestType">Type of request</param>
        /// <param name="message">Exception message</param>
        /// <param name="innerException">Inner Exception</param>
        public SenderAlreadyRegisteredException(ISender sender, Type requestType, string message, Exception innerException)
            : base(message, innerException)
        {
            this.requestType = requestType ?? throw new ArgumentNullException(nameof(requestType));
            this.sender = sender ?? throw new ArgumentNullException(nameof(sender));
        }


        /// <summary>
        /// Initializes a new SenderAlreadyRegisteredException
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected SenderAlreadyRegisteredException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
