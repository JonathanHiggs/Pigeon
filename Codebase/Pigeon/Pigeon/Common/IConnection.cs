using System.Collections.Generic;

using Pigeon.Addresses;

namespace Pigeon.Common
{
    // ToDo: Remove Add/Remove address
    public interface IConnection
    {
        /// <summary>
        /// Gets an enumerable of <see cref="IAddress"/> that the receiver is listening to
        /// </summary>
        IEnumerable<IAddress> Addresses { get; }


        /// <summary>
        /// Gets a bool that returns true when the <see cref="IConnection"/> is connected; otherwise false
        /// </summary>
        bool IsConnected { get; }

        
        /// <summary>
        /// Adds the <see cref="IAddress"/> to the set of adresses the <see cref="IConnection"/> will listen to
        /// for incoming <see cref="Package"/>s
        /// </summary>
        /// <param name="address"><see cref="IAddress"/> to add</param>
        void AddAddress(IAddress address);


        /// <summary>
        /// Removes the <see cref="IAddress"/> from the set of addresses the <see cref="IConnection"/> will listen to
        /// for incoming <see cref="Packages.Package"/>s
        /// </summary>
        /// <param name="address"><see cref="IAddress"/> to remove</param>
        void RemoveAddress(IAddress address);


        /// <summary>
        /// Removes all <see cref="IAddress"/> from the set of addresses the <see cref="IConnection"/> will listen to
        /// for incoming <see cref="Packages.Package"/>s
        /// </summary>
        void RemoveAllAddresses();


        /// <summary>
        /// Initializes the connections to all added <see cref="IAddress"/>es
        /// </summary>
        void InitializeConnection();


        /// <summary>
        /// Terminates the connection to all added <see cref="IAddress"/>es
        /// </summary>
        void TerminateConnection();
    }
}
