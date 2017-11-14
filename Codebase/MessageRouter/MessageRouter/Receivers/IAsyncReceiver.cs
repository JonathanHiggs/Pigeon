using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Receivers
{
    /// <summary>
    /// Extends the <see cref="IReceiver"/> interface an async incoming message event
    /// </summary>
    public interface IAsyncReceiver : IReceiver
    {
        /// <summary>
        /// Raised when an incoming message is received
        /// </summary>
        event RequestTaskDelegate RequestReceived;
    }
}
