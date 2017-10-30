using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Senders
{
    /// <summary>
    /// Represents an error in which a sender is attempted to be registered when there is
    /// a preexisting sender also registered for the given request type
    /// </summary>
    /// <typeparam name="TRequest">Type of request object</typeparam>
    [Serializable]
    public class SenderAlreadyRegisteredException<TRequest> : Exception
    {
        private ISender sender;


        /// <summary>
        /// Gets the sender that is registered and blocking a new registration
        /// </summary>
        public ISender Sender => sender;


        /// <summary>
        /// Initializes a new SenderAlreadyRegisteredException
        /// </summary>
        /// <param name="sender">Existing sender</param>
        public SenderAlreadyRegisteredException(ISender sender)
        {
            this.sender = sender ?? throw new ArgumentNullException(nameof(sender));
        }


        protected SenderAlreadyRegisteredException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
