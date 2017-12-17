using MessageRouter.Addresses;
using MessageRouter.Packages;
using MessageRouter.Receivers;
using MessageRouter.Serialization;
using MessageRouter.Transport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Senders
{
    /// <summary>
    /// Interface encapsulates a connection to a remote that is able to be sent and receive <see cref="Package"/>s
    /// </summary>
    public interface ISender : IEndPoint
    {
        /// <summary>
        /// Gets the type of the message <see cref="ISerializer<>"/> the sender uses
        /// </summary>
        Type SerializerType { get; }

        
        /// <summary>
        /// Adds an <see cref="IAddress"/> to the collection of endpoints the <see cref="ISender"/> connects to
        /// </summary>
        /// <param name="address">Address of the remote</param>
        void AddAddress(IAddress address);

        
        /// <summary>
        /// Removes an <see cref="IAddress"/> from the collection of endpoints the <see cref="ISender"/> connects to
        /// </summary>
        /// <param name="address"></param>
        void RemoveAddress(IAddress address);


        /// <summary>
        /// Initializes the connections to all added addresses
        /// </summary>
        void ConnectAll();


        /// <summary>
        /// Terminates the connections to all added addresses
        /// </summary>
        void DisconnectAll();


        /// <summary>
        /// Asynchronously dispatches a <see cref="Package"/> along the transport to the connected remote 
        /// <see cref="IReceiver"/> and returns a task that will complete when a response is returned from the
        /// remote or when a default timeout elapses
        /// </summary>
        /// <param name="package"><see cref="Package"/> to send to the remote</param>
        /// <returns>A task that will complete successfully when a responce is received or that will fail once the timeout elapses</returns>
        Task<Package> SendAndReceive(Package package);


        /// <summary>
        /// Asynchronously dispatches a <see cref="Package"/> along the transport to the connected remote 
        /// <see cref="IReceiver"/> and returns a task that will complete when a response is returned from the
        /// remote or when the timeout elapses
        /// </summary>
        /// <param name="package"><see cref="Package"/> to send to the remote</param>
        /// <param name="timeout"><see cref="TimeSpan"/> after which the returned <see cref="Task{Message}"/> will throw an error if no response has been received</param>
        /// <returns>A task that will complete successfully when a responce is received or that will fail once the timeout elapses</returns>
        Task<Package> SendAndReceive(Package package, TimeSpan timeout);
    }
}
