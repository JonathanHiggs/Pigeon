using System;
using System.Collections.Generic;

using Pigeon.Addresses;
using Pigeon.Diagnostics;
using Pigeon.Publishers;
using Pigeon.Subscribers;

namespace Pigeon.Routing
{
    /// <summary>
    /// Maps topic message types to a <see cref="SubscriberRouting"/> for runtime construction and resolution of
    /// <see cref="ISubscriber"/>s from config-time setup
    /// </summary>
    public class TopicRouter : ITopicRouter
    {
        private readonly Dictionary<Type, SubscriberRouting> routingTable = new Dictionary<Type, SubscriberRouting>();


        /// <summary>
        /// Gets the routing table of topic message type to <see cref="SubscriberRouting"/>
        /// </summary>
        public IReadOnlyDictionary<Type, SubscriberRouting> RoutingTable => routingTable;


        /// <summary>
        /// Adds to the routing table
        /// </summary>
        /// <typeparam name="TTopic">Topic message type</typeparam>
        /// <typeparam name="TSubscriber">Transport specific <see cref="ISubscriber"/> type</typeparam>
        /// <param name="address"><see cref="IAddress"/> of the remote <see cref="IPublisher"/> for this
        /// <see cref="ISubscriber"/></param>
        public void AddTopicRouting<TTopic, TSubscriber>(IAddress address)
            where TSubscriber : ISubscriber
        {
            if (address is null)
                throw new ArgumentNullException(nameof(address));

            var topicType = typeof(TTopic);
            var newRouting = SubscriberRouting.For<TSubscriber>(address);

            if (routingTable.TryGetValue(topicType, out var existingRouting))
            {
                if (newRouting.Address.Equals(existingRouting.Address))
                    return;
                else
                    throw new RoutingAlreadyRegisteredException<SubscriberRouting>(newRouting, existingRouting);
            }

            routingTable.Add(topicType, newRouting);
        }


        /// <summary>
        /// TryGets a <see cref="SubscriberRouting"/> from the topic message type
        /// </summary>
        /// <typeparam name="TTopic">Topic message type</typeparam>
        /// <param name="routing">Outs a matching <see cref="SubscriberRouting"/> for the topic message type if the
        /// <see cref="TopicRouter"/> has one added</param>
        /// <returns>True if the <see cref="TopicRouter"/> has a <see cref="SubscriberRouting"/> for the topic message
        /// type; otherwise false</returns>
        public bool RoutingFor<TTopic>(out SubscriberRouting routing)
        {
            return routingTable.TryGetValue(typeof(TTopic), out routing);
        }
    }
}
