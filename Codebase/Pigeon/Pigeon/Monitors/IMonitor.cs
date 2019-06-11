namespace Pigeon.Monitors
{
    /// <summary>
    /// Performs any active management for transports
    /// </summary>
    public interface IMonitor
    {
        /// <summary>
        /// Starts active monitoring of transports
        /// </summary>
        void StartMonitoring();


        /// <summary>
        /// Stops active monitoring of transports
        /// </summary>
        void StopMonitoring();
    }
}
