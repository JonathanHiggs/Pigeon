using Pigeon.Senders;

namespace Pigeon.Monitors
{
    /// <summary>
    /// A runtime cache of <see cref="IMonitor"/>s that different transport providers require to maintain
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
        /// Adds a new <see cref="IMonitor"/> to the cache
        /// </summary>
        /// <param name="monitor"><see cref="IMonitor"/> to add to cache</param>
        void AddMonitor(IMonitor monitor);
    }
}
