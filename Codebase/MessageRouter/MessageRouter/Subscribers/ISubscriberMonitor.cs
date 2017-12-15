using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageRouter.Monitors;

namespace MessageRouter.Subscribers
{
    /// <summary>
    /// Manages the state of <see cref="TSubscriber"/>s
    /// </summary>
    /// <typeparam name="TSubscriber"></typeparam>
    public interface ISubscriberMonitor<TSubscriber> : IMonitor where TSubscriber : ISubscriber
    {
        /// <summary>
        /// Adds a <see cref="TSubscriber"/> to the internal cache of monitored <see cref="ISubscriber"/>s
        /// </summary>
        /// <param name="subscriber"><see cref="TSubscriber"/> to add to the cache of monitored <see cref="ISubscriber"/>s</param>
        void AddSubscriber(TSubscriber subscriber);
    }
}
