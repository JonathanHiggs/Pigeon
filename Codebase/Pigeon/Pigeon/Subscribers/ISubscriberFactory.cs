using System;

using Pigeon.Addresses;
using Pigeon.Monitors;

namespace Pigeon.Subscribers
{
    /// <summary>
    /// Factory for creating <see cref="ISubscriber"/>s
    /// </summary>
    public interface ISubscriberFactory
    {
        /// <summary>
        /// Gets the transport specific implementation type of <see cref="ISubscriber"/>s this factory creates
        /// </summary>
        Type SubscriberType { get; }


        /// <summary>
        /// Gets the <see cref="IMonitor"/> associated with the <see cref="ISubscriberFactory"/>s <see cref="ISubscriber"/>s
        /// </summary>
        IMonitor SubscriberMonitor { get; }


        /// <summary>
        /// Creates a new instance of a <see cref="ISubscriber"/> connected to the supplied <see cref="IAddress"/> and
        /// monitored by the factories <see cref="IMonitor"/>
        /// </summary>
        /// <param name="address"><see cref="IAddress"/> of the remote publishing <see cref="IConnection"/></param>
        /// <returns><see cref="ISubscriber"/> connected to the <see cref="IAddress"/></returns>
        ISubscriber CreateSubscriber(IAddress address);
    }


    /// <summary>
    /// Type specified factory for creating <see cref="ISubscriber"/>s
    /// </summary>
    /// <typeparam name="TSubscriber">Transport specific implementation of <see cref="TSubscriber"/>s the factory creates</typeparam>
    public interface ISubscriberFactory<TSubscriber> : ISubscriberFactory where TSubscriber : ISubscriber
    { }
}
