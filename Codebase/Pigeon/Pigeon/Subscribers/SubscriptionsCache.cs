using System;
using System.Collections.Generic;

using Pigeon.Publishers;

namespace Pigeon.Subscribers
{
    /// <summary>
    /// Maintains a set of added subscriptions performing the necessary clean up upon unsubscription
    /// </summary>
    public class SubscriptionsCache : ISubscriptionsCache
    {
        // ToDo: change this to a HashSet
        private readonly Dictionary<Key, Subscription> subscriptions = new Dictionary<Key, Subscription>();


        /// <summary>
        /// Initializes and stores a subscription to the topic message stream from a remote <see cref="IPublisher"/>
        /// </summary>
        /// <typeparam name="TTopic">The type of the published topic message</typeparam>
        /// <param name="subscriber"></param>
        /// <param name="subject">Topic subject identifier</param>
        /// <returns>A representation of the subscription, the dispose method can be used to terminate the subscription</returns>
        public Subscription Add<TTopic>(ISubscriber subscriber, string subject)
        {
            var key = new Key(subscriber, typeof(TTopic), subject);

            if (subscriptions.TryGetValue(key, out var subscription))
                return subscription;

            subscription = new Subscription(subscriber, typeof(TTopic), subject, () =>
            {
                subscriber.Unsubscribe<TTopic>();
                subscriptions.Remove(key);
            });

            subscriptions.Add(key, subscription);

            return subscription;
        }


        /// <summary>
        /// Terminates a stored subscription to the topic message stream
        /// </summary>
        /// <typeparam name="TTopic">The type of the published topic message</typeparam>
        /// <param name="subscriber"><see cref="ISubscriber"/> for which to terminate the topic subscription</param>
        /// <param name="subject">Topic subject identifier</param>
        public void Remove<TTopic>(ISubscriber subscriber, string subject)
        {
            var key = new Key(subscriber, typeof(TTopic), subject);

            if (!subscriptions.TryGetValue(key, out var subscription))
                return;

            subscription.Dispose();
        }


        /// <summary>
        /// Key for the subscription cache
        /// </summary>
        private readonly struct Key
        {
            public readonly ISubscriber Subscriber;
            public readonly Type TopicType;
            public readonly string Subject;

            public Key(ISubscriber subscriber, Type topicType, string subject)
            {
                Subscriber = subscriber ?? throw new ArgumentNullException(nameof(subscriber));
                TopicType = topicType ?? throw new ArgumentNullException(nameof(topicType));
                Subject = subject;
            }
        }
    }
}
