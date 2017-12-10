using MessageRouter.Addresses;
using MessageRouter.Messages;
using MessageRouter.Receivers;
using MessageRouter.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Senders
{
    /// <summary>
    /// Interface encapsulates a connection to a remote that is able to be sent and receive <see cref="Message"/>s
    /// </summary>
    public interface ISender : IEndPoint
    {
        /// <summary>
        /// Gets the type of the message <see cref="ISerializer<>"/> the sender uses
        /// </summary>
        Type SerializerType { get; }

        
        /// <summary>
        /// Adds an address to the collection of endpoints the <see cref="ISender"/> connects to
        /// </summary>
        /// <param name="address">Address of the remote</param>
        void AddAddress(IAddress address);

        
        /// <summary>
        /// Removes an address from the collection of endpoints the <see cref="ISender"/> connects to
        /// </summary>
        /// <param name="address"></param>
        void RemoveAddress(IAddress address);


        /// <summary>
        /// Initializes the connection to all added addresses
        /// </summary>
        void ConnectAll();


        /// <summary>
        /// Terminates the connection to all added addresses
        /// </summary>
        void DisconnectAll();


        /// <summary>
        /// Asynchronously dispatches a <see cref="Message"/> along the transport to the connected remote 
        /// <see cref="IReceiver"/> and returns a task that will complete when a response is returned from the
        /// remote or when a default timeout elapses
        /// </summary>
        /// <param name="message"><see cref="Message"/> to send to the remote</param>
        /// <returns>A task that will complete successfully when a responce is received or that will fail once the timeout elapses</returns>
        Task<Message> SendAndReceive(Message message);


        /// <summary>
        /// Asynchronously dispatches a <see cref="Message"/> along the transport to the connected remote 
        /// <see cref="IReceiver"/> and returns a task that will complete when a response is returned from the
        /// remote or when the timeout elapses
        /// </summary>
        /// <param name="message"><see cref="Message"/> to send to the remote</param>
        /// <param name="timeout"><see cref="TimeSpan"/> after which the returned <see cref="Task{Message}"/> will throw an error if no response has been received</param>
        /// <returns>A task that will complete successfully when a responce is received or that will fail once the timeout elapses</returns>
        Task<Message> SendAndReceive(Message message, TimeSpan timeout);
    }
}
