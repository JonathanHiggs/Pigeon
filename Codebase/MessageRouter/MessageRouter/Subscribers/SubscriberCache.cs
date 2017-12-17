using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageRouter.Addresses;
using MessageRouter.Packages;
using MessageRouter.Monitors;
using MessageRouter.Publishers;
using MessageRouter.Routing;
using MessageRouter.Topics;
using MessageRouter.Diagnostics;

namespace MessageRouter.Subscribers
{
    /// <summary>
    /// Manages the state and lifecycle of <see cref="ISubscriber"/>s
    /// </summary>
    public class SubscriberCache : ISubscriberCache
    {
        private readonly ITopicRouter topicRouter;
        private readonly IMonitorCache monitorCache;
        private readonly IPackageFactory packageFactory;
        private readonly ITopicDispatcher dispatcher;
        private readonly ISubscriptionsCache subscriptions;
        private readonly Dictionary<SubscriberRouting, ISubscriber> subscribers = new Dictionary<SubscriberRouting, ISubscriber>();
        private readonly Dictionary<Type, ISubscriberFactory> factories = new Dictionary<Type, ISubscriberFactory>();


        /// <summary>
        /// Gets a readonly collection of all registered <see cref="ISubscriberFactory"/>s for creating <see cref="ISubscriber"/>s at runtime
        /// </summary>
        public IReadOnlyCollection<ISubscriberFactory> SubscriberFactories => factories.Values;


        /// <summary>
        /// Initializes a new instance of <see cref="SubscriberCache"/>
        /// </summary>
        /// <param name="topicRouter"></param>
        /// <param name="monitorCache">Stores <see cref="IMonitor"/>s that actively manage <see cref="ISubscriber"/>s</param>
        /// <param name="packageFactory">Creates and extracts <see cref="Package"/>s that are received from remote <see cref="IPublisher"/>s</param>
        /// <param name="dispatcher">Maps request messages to registered handlers for processing</param>
        /// <param name="subscriptions"></param>
        public SubscriberCache(ITopicRouter topicRouter, IMonitorCache monitorCache, IPackageFactory packageFactory, ITopicDispatcher dispatcher, ISubscriptionsCache subscriptions)
        {
            this.topicRouter = topicRouter ?? throw new ArgumentNullException(nameof(topicRouter));
            this.monitorCache = monitorCache ?? throw new ArgumentNullException(nameof(monitorCache));
            this.packageFactory = packageFactory ?? throw new ArgumentNullException(nameof(packageFactory));
            this.dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            this.subscriptions = subscriptions ?? throw new ArgumentNullException(nameof(subscriptions));
        }


        /// <summary>
        /// Processes an incoming <see cref="Package"/> extracting the wrapped subscription event message 
        /// </summary>
        /// <param name="package">Incoming wrapped subscription event message</param>
        public void HandleSubscriptionEvent(Package package)
        {
            var subscriptionMessage = packageFactory.Unpack(package);
            dispatcher.Handle(subscriptionMessage);
        }


        /// <summary>
        /// Adds a <see cref="ISubscriberFactory{TSubscriber}"/> to the set of factories for runtime creation of <see cref="ISubscriber"/>s
        /// </summary>
        /// <param name="factory">Factory that will be used to create <see cref="ISubscriber"/>s when 
        /// <see cref="AddSubscriber{TSubscriber}(IAddress)"/> is called</param>
        public void AddFactory(ISubscriberFactory factory)
        {
            if (null == factory)
                throw new ArgumentNullException(nameof(factory));

            if (factories.ContainsKey(factory.SubscriberType))
                return;

            factories.Add(factory.SubscriberType, factory);
            monitorCache.AddMonitor(factory.SubscriberMonitor);
        }


        /// <summary>
        /// Retrieves or creates a <see cref="ISubscriber"/> for the topic type that is connected to a remote 
        /// <see cref="IPublisher"/> chosen by registrations in the <see cref="ITopicRouter"/>
        /// </summary>
        /// <typeparam name="TTopic">Topic type</typeparam>
        /// <returns>Matching <see cref="ISubscriber"/> for the topic type</returns>
        public ISubscriber SubscriberFor<TTopic>()
        {
            if (!topicRouter.RoutingFor<TTopic>(out var routing))
                throw new KeyNotFoundException($"No routing found for {typeof(TTopic).Name}");

            if (!subscribers.TryGetValue(routing, out var subscriber))
            {
                if (!factories.TryGetValue(routing.SubscriberType, out var factory))
                    throw new MissingFactoryException(routing.SubscriberType, typeof(SubscriberCache));

                subscriber = factory.CreateSubscriber(routing.Address);
                subscriber.TopicMessageReceived += (s, e) => Task.Run(() => HandleSubscriptionEvent(e));
                subscribers.Add(routing, subscriber);
            }

            return subscriber;
        }


        #region Verbs.Subscribe
        /// <summary>
        /// Initializes a subscription to the topic message stream from a remote <see cref="IPublisher"/>
        /// </summary>
        /// <typeparam name="TTopic">The type of the published topic message</typeparam>
        /// <returns>A representation of the subscription, the dispose method can be used to terminate the subscription</returns>
        public IDisposable Subscribe<TTopic>()
        {
            var subscriber = SubscriberFor<TTopic>();
            subscriber.Subscribe<TTopic>();

            return subscriptions.Add<TTopic>(subscriber);
        }


        /// <summary>
        /// Terminates a subscription to the topic message stream
        /// </summary>
        /// <typeparam name="TTopic">The type of the published topic message</typeparam>
        public void Unsubscribe<TTopic>()
        {
            var subscriber = SubscriberFor<TTopic>();
            subscriptions.Remove<TTopic>(subscriber);
        }
        #endregion
    }
}
