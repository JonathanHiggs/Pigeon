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
    public interface IReceiver : IEndPoint
    {
        /// <summary>
        /// Gets a bool status flag indicating whether the receiver is bound to its <see cref="IAddress"/> and is listening to incoming messages
        /// </summary>
        bool IsBound { get; }


        /// <summary>
        /// Raised when an incoming message is received
        /// </summary>
        event RequestTaskHandler RequestReceived;


        /// <summary>
        /// Adds the <see cref="IAddress"/> the set of endpoints the <see cref="IReceiver"/> will listening to incoming <see cref="Message"/>s on
        /// </summary>
        /// <param name="address"></param>
        void AddAddress(IAddress address);


        /// <summary>
        /// Removes all <see cref="IAddress"/>es the <see cref="IReceiver"/> will listen for incoming <see cref="Message"/>s on
        /// </summary>
        void RemoveAllAddresses();


        /// <summary>
        /// Removes an <see cref="IAddress"/> the <see cref="IReceiver"/> will listen for incoming <see cref="Message"/>s on
        /// </summary>
        /// <param name="address"></param>
        void RemoveAddress(IAddress address);

        
        /// <summary>
        /// Starts the <see cref="IReceiver"/> listening for incoming <see cref="Message"/>s  on all added <see cref="IAddress"/>es
        /// </summary>
        void BindAll();


        /// <summary>
        /// Stops the <see cref="IReceiver"/> listening for incoming <see cref="Message"/>s on all added <see cref="IAddress"/>es
        /// </summary>
        void UnbindAll();
    }
}
