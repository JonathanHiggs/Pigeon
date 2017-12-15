using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageRouter.Addresses;
using MessageRouter.Messages;
using MessageRouter.Monitors;
using MessageRouter.Publishers;
using MessageRouter.Topics;

namespace MessageRouter.Subscribers
{
    /// <summary>
    /// Manages the state and lifecycle of <see cref="ISubscriber"/>s
    /// </summary>
    public class SubscriberCache : ISubscriberCache
    {
        private readonly IMonitorCache monitorCache;
        private readonly IMessageFactory messageFactory;
        private readonly ITopicDispatcher dispatcher;
        private readonly Dictionary<IAddress, ISubscriber> subscribers = new Dictionary<IAddress, ISubscriber>();
        private readonly Dictionary<Type, ISubscriberFactory> factories = new Dictionary<Type, ISubscriberFactory>();


        /// <summary>
        /// Gets a readonly collection of all registered <see cref="ISubscriberFactory"/>s for creating <see cref="ISubscriber"/>s at runtime
        /// </summary>
        public IReadOnlyCollection<ISubscriberFactory> SubscriberFactories => factories.Values;


        /// <summary>
        /// Initializes a new instance of <see cref="SubscriberCache"/>
        /// </summary>
        /// <param name="monitorCache">Stores <see cref="IMonitor"/>s that actively manage <see cref="ISubscriber"/>s</param>
        /// <param name="messageFactory">Creates and extracts <see cref="Message"/>s that are received from remote <see cref="IPublisher"/>s</param>
        /// <param name="dispatcher">Maps request messages to registered handlers for processing</param>
        public SubscriberCache(IMonitorCache monitorCache, IMessageFactory messageFactory, ITopicDispatcher dispatcher)
        {
            this.monitorCache = monitorCache ?? throw new ArgumentNullException(nameof(monitorCache));
            this.messageFactory = messageFactory ?? throw new ArgumentNullException(nameof(messageFactory));
            this.dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
        }


        /// <summary>
        /// Processes an incoming <see cref="Message"/> extracting the wrapped subscription event message 
        /// </summary>
        /// <param name="message">Incoming wrapped subscription event message</param>
        public void HandleSubscriptionEvent(Message message)
        {
            var subscriptionMessage = messageFactory.ExtractMessage(message);
            dispatcher.Handle(subscriptionMessage);
        }


        /// <summary>
        /// Adds a <see cref="ISubscriberFactory{TSubscriber}"/> to the set of factories for runtime creation of <see cref="ISubscriber"/>s
        /// </summary>
        /// <typeparam name="TSubscriber">Transport specific implementation of <see cref="ISubscriber"/></typeparam>
        /// <param name="factory">Factory that will be used to create <see cref="ISubscriber"/>s when 
        /// <see cref="AddSubscriber{TSubscriber}(IAddress)"/> is called</param>
        public void AddFactory<TSubscriber>(ISubscriberFactory<TSubscriber> factory) where TSubscriber : ISubscriber
        {
            if (null == factory)
                throw new ArgumentNullException(nameof(factory));

            if (factories.ContainsKey(factory.SubscriberType))
                return;

            factories.Add(factory.SubscriberType, factory);
            monitorCache.AddMonitor(factory.SubscriberMonitor);
        }


        /// <summary>
        /// Creates and adds a <see cref="ISubscriber"/> to the cache that connects and received subscription event messages published at the 
        /// <see cref="IAddress"/> remote endpoint, using a matching <see cref="ISubscriberFactory{TSubscriber}"/> registered with
        /// <see cref="AddFactory{TSubscriber}(ISubscriberFactory{TSubscriber})"/>
        /// </summary>
        /// <typeparam name="TSubscriber">Transport specific implementation of <see cref="ISubscriber"/></typeparam>
        /// <param name="address">The <see cref="IAddress"/> to which the <see cref="ISubscriber"/> connects</param>
        public void AddSubscriber<TSubscriber>(IAddress address) where TSubscriber : ISubscriber
        {
            if (null == address)
                throw new ArgumentNullException(nameof(address));

            if (subscribers.ContainsKey(address))
                throw new InvalidOperationException($"Cache already contains a {typeof(TSubscriber).Name} for {address.ToString()}");

            var factory = factories[typeof(TSubscriber)];
            var subscriber = factory.CreateSubscriber(address);
            subscriber.TopicMessageReceived += (s, e) => Task.Run(() => HandleSubscriptionEvent(e));
            subscribers.Add(address, subscriber);
        }
    }
}
