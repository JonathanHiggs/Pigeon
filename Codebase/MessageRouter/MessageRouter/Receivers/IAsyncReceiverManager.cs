using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Receivers
{
    /// <summary>
    /// Interface manages the state of <see cref="IAsyncReceiver"/>s and can be used to combine several to create a remote
    /// that can accept incoming requests from multiple sources and transports
    /// </summary>
    public interface IAsyncReceiverManager
    {
        /// <summary>
        /// Raised when an incoming message is received
        /// </summary>
        event RequestTaskDelegate RequestReceived;


        /// <summary>
        /// Starts accepting incoming requests
        /// </summary>
        void Start();


        /// <summary>
        /// Stops accepting incoming requests
        /// </summary>
        void Stop();
    }
}
