using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageRouter.Messages;
using MessageRouter.Addresses;

namespace MessageRouter.Receivers
{
    /// <summary>
    /// Interface encapsulates a connection that is able to bind to an <see cref="IAddress"/> to receive and synchronously reply 
    /// to incoming messages from remote <see cref="ISender"/>
    /// </summary>
    public interface IReceiver
    {
        /// <summary>
        /// Gets an enumerable of <see cref="IAddress"/> that the receiver is listening to
        /// </summary>
        IEnumerable<IAddress> Addresses { get; }


        /// <summary>
        /// Starts the receiver listening for incoming messages on the specified <see cref="IAddress"/>
        /// </summary>
        /// <param name="address">Address to bind to</param>
        void Bind(IAddress address);


        /// <summary>
        /// Stops the receiver listening for incoming messages on the specified <see cref="IAddress"/>
        /// </summary>
        /// <param name="address">Bound address to unbind</param>
        void Unbind(IAddress address);


        void UnbindAll();


        /// <summary>
        /// Synchronously retrieves a <see cref="RequestTask"/> from a connected <see cref="ISender"/>
        /// </summary>
        /// <returns>Combination of the request <see cref="Message"/> and a response Action</returns>
        RequestTask Receive();
    }
}
