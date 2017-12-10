using MessageRouter.Addresses;
using MessageRouter.Monitors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Receivers
{
    /// <summary>
    /// Manages the state of <see cref="IReceiver"/>s and can be used to combine several to create a remote that
    /// can accept incoming requests from multiple remote sources
    /// </summary>
    public interface IReceiverMonitor<TReceiver> : IMonitor where TReceiver : IReceiver
    {
        /// <summary>
        /// Adds a <see cref="TReceiver"/> to the internal cache of monitored receivers
        /// </summary>
        /// <param name="receiver"><see cref="TReceiver"/> to add to the monitored cache of receivers</param>
        void AddReceiver(TReceiver receiver);
    }
}
