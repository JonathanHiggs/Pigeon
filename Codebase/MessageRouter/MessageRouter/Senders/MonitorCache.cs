using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Senders
{
    public class MonitorCache : IMonitorCache
    {
        private readonly HashSet<ISenderMonitor> monitors = new HashSet<ISenderMonitor>();


        public void StartAllMonitors()
        {
            foreach (var monitor in monitors)
                monitor.StartSenders();
        }


        public void StopAllMonitors()
        {
            foreach (var monitor in monitors)
                monitor.StopSenders();
        }


        public void AddMonitor(ISenderMonitor monitor)
        {
            monitors.Add(monitor);
        }
    }
}
