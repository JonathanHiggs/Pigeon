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
    /// Interface encapsulates a connection that is able to bind to <see cref="IAddress"/>es to receive and reply 
    /// to incoming messages from remote <see cref="ISender"/>
    /// </summary>
    public interface IReceiver
    {
        /// <summary>
        /// Gets an enumerable of <see cref="IAddress"/> that the receiver is listening to
        /// </summary>
        IEnumerable<IAddress> Addresses { get; }


        /// <summary>
        /// Gets a bool status flag indicating whether the receiver is bound to its <see cref="IAddress"/> and is listening to incoming messages
        /// </summary>
        bool IsBound { get; }


        /// <summary>
        /// Raised when an incoming message is received
        /// </summary>
        event RequestTaskDelegate RequestReceived;


        /// <summary>
        /// Adds an <see cref="IAddress"/> the receiver will listening to incoming <see cref="Message"/>s on
        /// </summary>
        /// <param name="address"></param>
        void Add(IAddress address);


        /// <summary>
        /// Removes all <see cref="IAddress"/>es the receiver will listen for incoming <see cref="Message"/>s on
        /// </summary>
        void RemoveAll();


        /// <summary>
        /// Removes an <see cref="IAddress"/> the receiver will listen for incoming <see cref="Message"/>s on
        /// </summary>
        /// <param name="address"></param>
        void Remove(IAddress address);

        
        /// <summary>
        /// Starts the receiver listening for incoming <see cref="Message"/>s  on all added <see cref="IAddress"/>es
        /// </summary>
        void Bind();


        /// <summary>
        /// Stops the receiver listening for incoming <see cref="Message"/>s on all added <see cref="IAddress"/>es
        /// </summary>
        void UnbindAll();
    }
}
