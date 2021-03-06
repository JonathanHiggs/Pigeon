﻿using System.Collections.Generic;

using Pigeon.Common;
using Pigeon.Verbs;

namespace Pigeon.Subscribers
{
    /// <summary>
    /// Manages the state and life-cycle of <see cref="ISubscriber"/>s
    /// </summary>
    public interface ISubscriberCache : ISubscribe, ICache<ISubscriber>
    {
        /// <summary>
        /// Gets a read-only collection of all registered <see cref="ISubscriberFactory"/>s for creating <see cref="ISubscriber"/>s at runtime
        /// </summary>
        IReadOnlyCollection<ISubscriberFactory> SubscriberFactories { get; }


        /// <summary>
        /// Adds a <see cref="ISubscriberFactory{TSubscriber}"/> to the set of factories for runtime creation of <see cref="ISubscriber"/>s
        /// </summary>
        /// <param name="factory">Factory that will be used to create <see cref="ISubscriber"/>s when 
        /// <see cref="AddSubscriber{TSubscriber}(IAddress)"/> is called</param>
        void AddFactory(ISubscriberFactory factory);
    }
}
