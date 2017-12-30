using System;

using Pigeon.Addresses;
using Pigeon.Publishers;
using Pigeon.Subscribers;

namespace Pigeon.Routing
{
    /// <summary>
    /// Attaches a <see cref="ISubscriber"/> type to a remote <see cref="IAddress"/> for runtime transport resolution
    /// </summary>
    public struct SubscriberRouting
    {
        /// <summary>
        /// Transport specific type of the local <see cref="ISubscriber"/>
        /// </summary>
        public readonly Type SubscriberType;


        /// <summary>
        /// Remote <see cref="IAddress"/> for the <see cref="ISubscriber"/>
        /// </summary>
        public readonly IAddress Address;


        /// <summary>
        /// Initializes a new instance of <see cref="SubscriberRouting"/>
        /// </summary>
        /// <param name="subscriberType">Transport specific type of the local <see cref="ISubscriber"/></param>
        /// <param name="address"><see cref="IAddress"/> for the remote <see cref="IPublisher"/></param>
        private SubscriberRouting(Type subscriberType, IAddress address)
        {
            SubscriberType = subscriberType ?? throw new ArgumentNullException(nameof(subscriberType));
            Address = address ?? throw new ArgumentNullException(nameof(address));
        }


        /// <summary>
        /// Initializes a new instance of <see cref="SubscriberRouting"/>
        /// </summary>
        /// <typeparam name="TSubscriber">Transport specific type of the local <see cref="ISubscriber"/></typeparam>
        /// <param name="address"><see cref="IAddress"/> for the remote <see cref="IPublisher"/></param>
        /// <returns></returns>
        public static SubscriberRouting For<TSubscriber>(IAddress address)
            where TSubscriber : ISubscriber
        {
            return new SubscriberRouting(typeof(TSubscriber), address);
        }


        /// <summary>
        /// Converts the <see cref="SubscriberRouting"/> to a string representation
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{SubscriberType.Name} <- {Address.ToString()}";
        }
    }
}
