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
    public class MonitorCache : IMonitorCache
    {
        private readonly HashSet<ISenderMonitor> monitors = new HashSet<ISenderMonitor>();


        /// <summary>
        /// Starts all monitors
        /// </summary>
        public void StartAllMonitors()
        {
            foreach (var monitor in monitors)
                monitor.StartSenders();
        }


        /// <summary>
        /// Stops all monitors
        /// </summary>
        public void StopAllMonitors()
        {
            foreach (var monitor in monitors)
                monitor.StopSenders();
        }


        /// <summary>
        /// Adds a new <see cref="ISenderMonitor"/> to the cache
        /// </summary>
        /// <param name="monitor"></param>
        public void AddMonitor(ISenderMonitor monitor)
        {
            monitors.Add(monitor);
        }
    }
}
