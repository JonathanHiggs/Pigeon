using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Publishers
{
    /// <summary>
    /// Manages the lifecycle of <see cref="IPublisher"/>s
    /// </summary>
    public interface IPublisherCache
    {
        /// <summary>
        /// Gets a readonly collection of <see cref="IPublisherFactory"/>s for creating <see cref="IPublisher"/>s at config-time
        /// </summary>
        IReadOnlyCollection<IPublisherFactory> PublisherFactories { get; }
    }
}
