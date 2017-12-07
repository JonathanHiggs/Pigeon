using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Senders
{
    /// <summary>
    /// A runtime cache of <see cref="ISenderMonitor"/>s that different transport providers require to maintain
    /// <see cref="ISender"/>s
    /// </summary>
    public interface IMonitorCache
    {
        /// <summary>
        /// Starts all monitors
        /// </summary>
        void StartAllMonitors();


        /// <summary>
        /// Stops all monitors
        /// </summary>
        void StopAllMonitors();


        /// <summary>
        /// Adds a new <see cref="ISenderMonitor"/> to the cache
        /// </summary>
        /// <param name="monitor"><see cref="ISenderMonitor"/> to add to cache</param>
        void AddMonitor(ISenderMonitor monitor);
    }
}
