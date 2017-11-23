using MessageRouter.Addresses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Receivers
{
    /// <summary>
    /// Manages the state of <see cref="IReceiver"/>s and can be used to combine several to create a remote that
    /// can accept incoming requests from multiple sources and transports
    /// </summary>
    public interface IReceiverManager
    {
        /// <summary>
        /// Synchronously retrieves a <see cref="RequestTask"/> from a managed <see cref="IReceiver"/>
        /// </summary>
        /// <returns></returns>
        RequestTask Receive();


        /// <summary>
        /// Synchronously tries receiving a <see cref="RequestTask"/> from a managed <see cref="IReceiver"/>
        /// </summary>
        /// <param name="requestTask">RequestTask</param>
        /// <returns></returns>
        bool TryReceive(out RequestTask requestTask);


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
