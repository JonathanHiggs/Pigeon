using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter
{
    /// <summary>
    /// Interface encapsulates a connection that is able to receive and synchronously reply to incoming messages from a remote
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
        /// <param name="address"></param>
        void Unbind(IAddress address);


        /// <summary>
        /// Synchronously retrieves a <see cref="RequestTask"/>
        /// </summary>
        /// <returns></returns>
        RequestTask Receive();
    }
}
