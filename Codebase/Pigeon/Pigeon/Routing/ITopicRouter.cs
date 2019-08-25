using System;
using System.Collections.Generic;

using Pigeon.Addresses;
using Pigeon.Subscribers;

namespace Pigeon.Routing
{
    /// <summary>
    /// Maps topic message types to a <see cref="SubscriberRouting"/> for runtime construction and resolution of
    /// <see cref="ISubscriber"/>s from configuration time set-up
    /// </summary>
    public interface ITopicRouter
    {
        /// <summary>
        /// Gets the routing table of topic message type to <see cref="SubscriberRouting"/>
        /// </summary>
        IReadOnlyDictionary<Type, SubscriberRouting> RoutingTable { get; }


        /// <summary>
        /// TryGets a <see cref="SubscriberRouting"/> from the topic message type
        /// </summary>
        /// <typeparam name="TTopic">Topic message type</typeparam>
        /// <param name="routing">Outs a matching <see cref="SubscriberRouting"/> for the topic message type if the
        /// <see cref="ITopicRouter"/> has one added</param>
        /// <returns>True if the <see cref="ITopicRouter"/> has a <see cref="SubscriberRouting"/> for the topic message
        /// type; otherwise false</returns>
        bool RoutingFor<TTopic>(out SubscriberRouting routing);


        /// <summary>
        /// Adds to the routing table
        /// </summary>
        /// <typeparam name="TTopic">Topic message type</typeparam>
        /// <typeparam name="TSubscriber">Transport specific <see cref="ISubscriber"/> type</typeparam>
        /// <param name="address"><see cref="IAddress"/> of the remote <see cref="IPublisher"/> for this
        /// <see cref="ISubscriber"/></param>
        void AddTopicRouting<TTopic, TSubscriber>(IAddress address) where TSubscriber : ISubscriber;
    }
}
