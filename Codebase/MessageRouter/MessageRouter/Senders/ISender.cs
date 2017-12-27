using MessageRouter.Addresses;
using MessageRouter.Common;
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
    public interface ISender : IConnection
    {
        /// <summary>
        /// Asynchronously dispatches a request along the transport to the connected remote 
        /// <see cref="IReceiver"/> and returns a task that will complete when a response is returned from the
        /// remote or when a default timeout elapses
        /// </summary>
        /// <param name="request">Request to send to the remote</param>
        /// <returns>A task that will complete successfully when a responce is received or that will fail once the timeout elapses</returns>
        Task<object> SendAndReceive(object request);


        /// <summary>
        /// Asynchronously dispatches a request along the transport to the connected remote 
        /// <see cref="IReceiver"/> and returns a task that will complete when a response is returned from the
        /// remote or when the timeout elapses
        /// </summary>
        /// <param name="request">Request to send to the remote</param>
        /// <param name="timeout"><see cref="TimeSpan"/> after which the returned <see cref="Task{Message}"/> will throw an error if no response has been received</param>
        /// <returns>A task that will complete successfully when a responce is received or that will fail once the timeout elapses</returns>
        Task<object> SendAndReceive(object request, TimeSpan timeout);
    }
}
