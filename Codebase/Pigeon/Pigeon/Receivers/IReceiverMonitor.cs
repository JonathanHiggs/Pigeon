using Pigeon.Monitors;

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
        /// Adds a <see cref="TReceiver"/> to the internal cache of monitored <see cref="IReceiver"/>s
        /// </summary>
        /// <param name="receiver"><see cref="TReceiver"/> to add to the cache of monitored <see cref="IReceiver"/>s</param>
        void AddReceiver(TReceiver receiver);
    }
}
