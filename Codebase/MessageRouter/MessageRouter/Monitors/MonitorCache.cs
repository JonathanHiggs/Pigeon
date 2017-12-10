using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageRouter.Senders;

namespace MessageRouter.Monitors
{
    /// <summary>
    /// A runtime cache of <see cref="ISenderMonitor"/>s that different transport providers require to maintain
    /// <see cref="ISender"/>s
    /// </summary>
    public class MonitorCache : IMonitorCache
    {
        private readonly HashSet<IMonitor> monitors = new HashSet<IMonitor>();
        private bool running = false;
        private object lockObj = new object();


        /// <summary>
        /// Starts all monitors
        /// </summary>
        public void StartAllMonitors()
        {
            lock (lockObj)
            {
                if (running)
                    return;

                foreach (var monitor in monitors)
                    monitor.StartSenders();

                running = true;
            }
        }


        /// <summary>
        /// Stops all monitors
        /// </summary>
        public void StopAllMonitors()
        {
            lock (lockObj)
            {
                if (!running)
                    return;

                foreach (var monitor in monitors)
                    monitor.StopSenders();

                running = false;
            }
        }


        /// <summary>
        /// Adds a new <see cref="ISenderMonitor"/> to the cache
        /// </summary>
        /// <param name="monitor"><see cref="ISenderMonitor"/> to add to cache</param>
        public void AddMonitor(IMonitor monitor)
        {
            lock (lockObj)
            {
                monitors.Add(monitor);

                if (running)
                    monitor.StartSenders();
            }
        }
    }
}
