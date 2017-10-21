using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter
{
    /// <summary>
    /// Interface encapsulates a connection to a remote that is able to be sent synchronous requests
    /// </summary>
    public interface ISender
    {
        /// <summary>
        /// Gets an eumerable of the <see cref="IAddress"/> for remotes that the sender is connected to
        /// </summary>
        IEnumerable<IAddress> Addresses { get; }


        /// <summary>
        /// Gets the type of the message serializer the sender uses
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
        /// Sends a request <see cref="Message"/> to the connected remote and returns the response <see cref="Message"/>
        /// </summary>
        /// <param name="message">Request message</param>
        /// <returns>Response message</returns>
        Message Send(Message message);
    }
}
