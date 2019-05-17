using System.Collections.Generic;

using Pigeon.Addresses;
using Pigeon.Common;

namespace Pigeon.Receivers
{
    /// <summary>
    /// Manages the lifecycle of <see cref="IReceiver"/>s
    /// </summary>
    public interface IReceiverCache : ICache<IReceiver>
    {
        /// <summary>
        /// Gets a readonly collection of all registered <see cref="IReceiverFactory"/>s for creating <see cref="IReceiver"/>s at config-time
        /// </summary>
        IReadOnlyCollection<IReceiverFactory> ReceiverFactories { get; }


        /// <summary>
        /// Gets an enumerable of all <see cref="IReceiver"/>s
        /// </summary>
        IEnumerable<IReceiver> Receivers { get; }


        /// <summary>
        /// Adds a <see cref="IReceiverFactory{TReceiver}"/> to the set of factories config-time creation of <see cref="IReceiver"/>s
        /// </summary>
        /// <param name="factory">Factory that will be used to create <see cref="IReceiver"/>s when 
        /// <see cref="AddReceiver{TReceiver}(IAddress)"/> is called</param>
        void AddFactory(IReceiverFactory factory);


        /// <summary>
        /// Creates and adds a <see cref="IReceiver"/> to the cache that binds and accepts requests sent to the <see cref="IAddress"/> 
        /// using a matching <see cref="IReceiverFactory{TReceiver}"/> registered with <see cref="AddFactory{TReceiver}(IReceiverFactory{TReceiver})"/>
        /// </summary>
        /// <typeparam name="TReceiver">Transport specific implementation of <see cref="IReceiver"/> to create</typeparam>
        /// <param name="address">The <see cref="IAddress"/> to which the <see cref="IReceiver"/> binds</param>
        void AddReceiver<TReceiver>(IAddress address) where TReceiver : IReceiver;
    }
}
