using Pigeon.Addresses;
using Pigeon.Monitors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pigeon.Receivers
{
    /// <summary>
    /// Manages the state of <see cref="TReceiver"/>s and can be used to combine several to create a remote that
    /// can accept incoming requests from multiple remote sources
    /// </summary>
    /// <typeparam name="TReceiver">Transport specific implementation of <see cref="IReceiver"/></typeparam>
    public interface IReceiverMonitor<TReceiver> : IMonitor where TReceiver : IReceiver
    {
        /// <summary>
        /// Gets a handler delegate for incoming requests
        /// </summary>
        RequestTaskHandler RequestHandler { get; }


        /// <summary>
        /// Adds a <see cref="TReceiver"/> to the internal cache of monitored <see cref="IReceiver"/>s
        /// </summary>
        /// <param name="receiver"><see cref="TReceiver"/> to add to the cache of monitored <see cref="IReceiver"/>s</param>
        void AddReceiver(TReceiver receiver);
    }
}
