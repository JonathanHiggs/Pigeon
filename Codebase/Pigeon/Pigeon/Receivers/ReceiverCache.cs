using System;
using System.Collections.Generic;

using Pigeon.Addresses;
using Pigeon.Diagnostics;
using Pigeon.Monitors;

namespace Pigeon.Receivers
{
    /// <summary>
    /// Manages the lifecycle of <see cref="IReceiver"/>s
    /// </summary>
    public class ReceiverCache : IReceiverCache
    {
        private readonly IMonitorCache monitorCache;
        private readonly Dictionary<IAddress, IReceiver> receivers = new Dictionary<IAddress, IReceiver>();
        private readonly Dictionary<Type, IReceiverFactory> factories = new Dictionary<Type, IReceiverFactory>();


        /// <summary>
        /// Gets a readonly collection of <see cref="IReceiverFactory"/>s for creating <see cref="IReceiver"/>s at runtime
        /// </summary>
        public IReadOnlyCollection<IReceiverFactory> ReceiverFactories => factories.Values;

        
        /// <summary>
        /// Initializes a new instance of <see cref="ReceiverCache"/>
        /// </summary>
        /// <param name="monitorCache">Stores <see cref="IMonitor"/>s that actively manage <see cref="IReceiver"/>s</param>
        public ReceiverCache(IMonitorCache monitorCache)
        {
            this.monitorCache = monitorCache ?? throw new ArgumentNullException(nameof(monitorCache));
        }


        /// <summary>
        /// Adds a <see cref="IReceiverFactory{TReceiver}"/> to the set of factories config-time creation of <see cref="IReceiver"/>s
        /// </summary>
        /// <param name="factory">Factory that will be used to create <see cref="IReceiver"/>s when 
        /// <see cref="AddReceiver{TReceiver}(IAddress)"/> is called</param>
        public void AddFactory(IReceiverFactory factory)
        {
            if (factory is null)
                throw new ArgumentNullException(nameof(factory));

            if (factories.ContainsKey(factory.ReceiverType))
                return;

            factories.Add(factory.ReceiverType, factory);
            monitorCache.AddMonitor(factory.ReceiverMonitor);
        }


        /// <summary>
        /// Creates and adds a <see cref="IReceiver"/> to the cache that binds and accepts requests sent to the <see cref="IAddress"/> 
        /// using a matching <see cref="IReceiverFactory{TReceiver}"/> registered with <see cref="AddFactory{TReceiver}(IReceiverFactory{TReceiver})"/>
        /// </summary>
        /// <typeparam name="TReceiver">Transport specific implementation of <see cref="IReceiver"/> to create</typeparam>
        /// <param name="address">The <see cref="IAddress"/> to which the <see cref="IReceiver"/> binds</param>
        public void AddReceiver<TReceiver>(IAddress address) where TReceiver : IReceiver
        {
            if (address is null)
                throw new ArgumentNullException(nameof(address));

            if (receivers.ContainsKey(address))
                throw new InvalidOperationException($"Cache already contains a {typeof(TReceiver).Name} for {address.ToString()}");

            if (!factories.TryGetValue(typeof(TReceiver), out var factory))
                throw MissingFactoryException.For<TReceiver, ReceiverCache>();

            var receiver = factory.CreateReceiver(address);
            receivers.Add(address, receiver);
        }
    }
}
