using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Receivers
{
    /// <summary>
    /// Interface 
    /// </summary>
    public interface IAsyncReceiverManager
    {
        event RequestTaskDelegate RequestReceived;


        /// <summary>
        /// Asynchronously starts the receiver manager running
        /// </summary>
        void Start();


        /// <summary>
        /// Stops the asynchronous receiver manager
        /// </summary>
        void Stop();
    }
}
