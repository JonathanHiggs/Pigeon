using MessageRouter.Addresses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Receivers
{
    /// <summary>
    /// Manages the state of Receivers and can be used to combine several Receivers to create a remote that
    /// can receiver requests from multiple sources
    /// </summary>
    public interface IReceiverManager
    {
        /// <summary>
        /// Synchronously retrieves a RequestTask incoming from a remote
        /// </summary>
        /// <returns>Combination of the request <see cref="Message"/> and a response Action</returns>
        RequestTask Receive();
    }
}
