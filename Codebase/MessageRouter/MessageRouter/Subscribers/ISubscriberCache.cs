using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Subscribers
{
    /// <summary>
    /// Manages the state and lifecycle of <see cref="ISubscriber"/>s
    /// </summary>
    public interface ISubscriberCache
    {
        /// <summary>
        /// Gets a readonly collection of all registered <see cref="ISubscriberFactory"/>s for creating <see cref="ISubscriber"/>s at runtime
        /// </summary>
        IReadOnlyCollection<ISubscriberFactory> SubscriberFactories { get; }
    }
}
