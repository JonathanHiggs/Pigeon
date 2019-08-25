using System;

using Pigeon.Addresses;
using Pigeon.Monitors;

namespace Pigeon.Publishers
{
    /// <summary>
    /// Factory for creating <see cref="IPublisher"/>s at configuration time
    /// </summary>
    public interface IPublisherFactory
    {
        /// <summary>
        /// Gets the transport specific type of <see cref="IPublisher"/> that this factory creates
        /// </summary>
        Type PublisherType { get; }


        /// <summary>
        /// Gets the <see cref="IMonitor"/> associated with the <see cref="IPublisherFactory"/>s <see cref="IPublisher"/>
        /// </summary>
        IMonitor PublisherMonitor { get; }


        /// <summary>
        /// Creates a new instance of <see cref="IPublisher"/> bound to the supplied <see cref="IAddress"/>
        /// </summary>
        /// <param name="address"><see cref="IAddress"/> publisher binds to</param>
        /// <returns><see cref="IPublisher"/> bound to the <see cref="IAddress"/></returns>
        IPublisher CreatePublisher(IAddress address);
    }


    /// <summary>
    /// Factory for creating <see cref="IPublisher"/>s at configuration time
    /// </summary>
    /// <typeparam name="TPublisher">Transport specific implementation if <see cref="IPublisher"/> the factory creates</typeparam>
    public interface IPublisherFactory<TPublisher> : IPublisherFactory where TPublisher : IPublisher
    { }
}