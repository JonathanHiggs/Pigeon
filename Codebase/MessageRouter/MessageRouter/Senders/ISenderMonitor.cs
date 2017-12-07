using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Senders
{
    /// <summary>
    /// Performs any active management for <see cref="ISender"/>s
    /// </summary>
    public interface ISenderMonitor
    {
        /// <summary>
        /// Starts active monitoring of <see cref="ISender"/>s transports
        /// </summary>
        void StartSenders();


        /// <summary>
        /// Stops active monitoring of <see cref="ISender"/>s transports
        /// </summary>
        void StopSenders();
    }


    /// <summary>
    /// Manages the state of <see cref="TSender"/>s that connect to different remotes
    /// </summary>
    /// <typeparam name="TSender">The type of senders that this can monitor</typeparam>
    public interface ISenderMonitor<TSender> : ISenderMonitor where TSender : ISender
    {
        /// <summary>
        /// Adds a <see cref="TSender"/> to the internal cache of monitored senders
        /// </summary>
        /// <param name="sender"><see cref="TSender"/> to add to the monitored cache of senders</param>
        void AddSender(TSender sender);
    }
}
