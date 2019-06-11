using Pigeon.Monitors;

namespace Pigeon.Senders
{
    /// <summary>
    /// Manages the state of <see cref="TSender"/>s that connect to different remotes
    /// </summary>
    /// <typeparam name="TSender">The type of senders that this can monitor</typeparam>
    public interface ISenderMonitor<TSender> : IMonitor where TSender : ISender
    {
        /// <summary>
        /// Adds a <see cref="TSender"/> to the internal cache of monitored <see cref="ISender"/>s
        /// </summary>
        /// <param name="sender"><see cref="TSender"/> to add to the monitored cache of <see cref="ISender"/>s</param>
        void AddSender(TSender sender);
    }
}
