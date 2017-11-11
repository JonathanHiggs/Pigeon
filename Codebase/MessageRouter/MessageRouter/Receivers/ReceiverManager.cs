using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageRouter.Addresses;

namespace MessageRouter.Receivers
{
    /// <summary>
    /// Manages the state of Receivers and can be used to combine several Receivers to create a remote that
    /// can receiver requests from multiple sources
    /// </summary>
    public class ReceiverManager : IReceiverManager
    {
        private IReceiver receiver;

        
        /// <summary>
        /// Initializes a ReceiverManager
        /// </summary>
        /// <param name="receiver">IReceiver endpoint</param>
        public ReceiverManager(IReceiver receiver)
        {
            this.receiver = receiver ?? throw new ArgumentNullException(nameof(receiver));
        }


        /// <summary>
        /// Synchronously retrieves a RequestTask incoming from a remote
        /// </summary>
        /// <returns>Combination of the request <see cref="Message"/> and a response Action</returns>
        public RequestTask Receive()
        {
            return receiver.Receive();
        }
    }
}
