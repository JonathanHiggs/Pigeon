using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageRouter.Packages;
using MessageRouter.Addresses;
using MessageRouter.Transport;
using MessageRouter.Common;

namespace MessageRouter.Receivers
{
    /// <summary>
    /// Interface encapsulates a connection that is able to bind to <see cref="IAddress"/>es to receive and reply 
    /// to incoming messages from remote <see cref="ISender"/>
    /// </summary>
    public interface IReceiver : IConnection
    {
        /// <summary>
        /// Raised when an incoming message is received
        /// </summary>
        event RequestTaskHandler RequestReceived;
    }
}
