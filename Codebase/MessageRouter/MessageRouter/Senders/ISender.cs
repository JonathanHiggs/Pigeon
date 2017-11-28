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
    public interface ISender
    {
        /// <summary>
        /// Gets an eumerable of the <see cref="IAddress"/> for remotes that the sender is connected to
        /// </summary>
        IEnumerable<IAddress> Addresses { get; }


        /// <summary>
        /// Gets the type of the message <see cref="ISerializer<>"/> the sender uses
        /// </summary>
        Type SerializerType { get; }


        /// <summary>
        /// Initializes a connection to a remote for the given <see cref="IAddress"/>
        /// </summary>
        /// <param name="address">Address for a remote</param>
        void Connect(IAddress address);


        /// <summary>
        /// Disconects the sender from the remote for the given <see cref="IAddress"/>
        /// </summary>
        /// <param name="address">Address for a remote</param>
        void Disconnect(IAddress address);


        /// <summary>
        /// Asynchronously sends a <see cref="Message"/> to the connected remote <see cref="IReceiver"/> and returns the reponse <see cref="Message"/>
        /// Default 1 hour timeout
        /// </summary>
        /// <param name="message">Request message</param>
        /// <returns>Response message</returns>
        Task<Message> SendAndReceive(Message message);


        /// <summary>
        /// Asynchronously sends a <see cref="Message"/> to the connected remote <see cref="IReceiver"/> and returns the reponse <see cref="Message"/>
        /// </summary>
        /// <param name="message">Request message</param>
        /// <param name="timeout">Time to wait without a response before throwing an exception</param>
        /// <returns>Response message</returns>
        Task<Message> SendAndReceive(Message message, TimeSpan timeout);
    }
}
