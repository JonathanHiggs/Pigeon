using System.Collections.Generic;

using Pigeon.Addresses;
using Pigeon.Common;
using Pigeon.Verbs;

namespace Pigeon.Publishers
{
    /// <summary>
    /// Manages the life-cycle of <see cref="IPublisher"/>s
    /// </summary>
    public interface IPublisherCache : IPublish, ICache<IPublisher>
    {
        /// <summary>
        /// Gets a read-only collection of <see cref="IPublisherFactory"/>s for creating <see cref="IPublisher"/>s at configuration time
        /// </summary>
        IReadOnlyCollection<IPublisherFactory> PublisherFactories { get; }


        /// <summary>
        /// Adds a <see cref="IPublisherFactory{TPublisher}"/> to the cache for configuration time creation of <see cref="IPublisher"/>s
        /// </summary>
        /// <param name="factory">Factory used to create <see cref="IPublisher"/>s at configuration time</param>
        void AddFactory(IPublisherFactory factory);


        /// <summary>
        /// Creates and adds a <see cref="IPublisher"/> to the cache that binds and distributes <see cref="Package"/>s to <see cref="ISubscriber"/>s
        /// </summary>
        /// <typeparam name="TPublisher">Transport specific implementation of <see cref="IPublisher"/> to create</typeparam>
        /// <param name="address">The <see cref="IAddress"/> to bind to on which <see cref="ISubscriber"/>s can connect to receive updates</param>
        void AddPublisher<TPublisher>(IAddress address) where TPublisher : IPublisher;
    }
}
