using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageRouter.Monitors;

namespace MessageRouter.Publishers
{
    /// <summary>
    /// Manages the state of <see cref="TPublisher"/>s
    /// </summary>
    /// <typeparam name="TPublisher">Transport specific implementation of <see cref="IPublisher"/></typeparam>
    public interface IPublisherMonitor<TPublisher> : IMonitor where TPublisher : IPublisher
    {
        /// <summary>
        /// Adds a <see cref="TPublisher"/> to the internal cache of monitored <see cref="IPublisher"/>s
        /// </summary>
        /// <param name="publisher"><see cref="TPublisher"/> to add to the cache of monitored <see cref="IPublisher"/>s</param>
        void AddPublisher(TPublisher publisher);
    }
}
