using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageRouter.Addresses;
using MessageRouter.Messages;

namespace MessageRouter.Subscribers
{
    /// <summary>
    /// Interface encapsulates a connection that is able to connect to <see cref="IAddress"/>es to receive messages
    /// from <see cref="IPublisher"/>s
    /// </summary>
    public interface ISubscriber : IEndPoint
    {
        /// <summary>
        /// Raised when an incoming message is received
        /// </summary>
        event TopicEventHandler TopicMessageReceived;


        /// <summary>
        /// Adds the <see cref="IAddress"/> to the set of endpoints the <see cref="ISubscriber"/> will listen to
        /// for incoming <see cref="Message"/>s
        /// </summary>
        /// <param name="address"></param>
        void AddAddress(IAddress address);


        /// <summary>
        /// Removes the <see cref="IAddress"/> from the set of endpoints the <see cref="ISubscriber"/> will listen to
        /// for incoming <see cref="Message"/>s
        /// </summary>
        /// <param name="address"></param>
        void RemoveAddress(IAddress address);


        /// <summary>
        /// Removes all <see cref="IAddress"/> from the set of endpoints the <see cref="ISubscriber"/> will listen to
        /// for incoming <see cref="Message"/>s
        /// </summary>
        void RemoveAllAddresses();


        /// <summary>
        /// Initializes the connections to all added addresses
        /// </summary>
        void ConnectAll();


        /// <summary>
        /// Terminates the connection to all added addresses
        /// </summary>
        void DisconnectAll();
    }
}
