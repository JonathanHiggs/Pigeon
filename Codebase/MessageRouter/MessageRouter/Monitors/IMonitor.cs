using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Monitors
{
    /// <summary>
    /// Performs any active management for transports
    /// </summary>
    public interface IMonitor
    {
        /// <summary>
        /// Starts active monitoring of transports
        /// </summary>
        void StartSenders();


        /// <summary>
        /// Stops active monitoring of transports
        /// </summary>
        void StopSenders();
    }
}
