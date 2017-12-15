using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageRouter.Addresses;
using MessageRouter.Subscribers;

namespace MessageRouter.Routing
{
    public struct SubscriberRouting
    {
        public readonly Type SubscriberType;


        public readonly IAddress Address;


        private SubscriberRouting(Type subscriberType, IAddress address)
        {
            SubscriberType = subscriberType ?? throw new ArgumentNullException(nameof(subscriberType));
            Address = address ?? throw new ArgumentNullException(nameof(address));
        }


        public static SubscriberRouting For<TSubscriber>(IAddress address)
            where TSubscriber : ISubscriber
        {
            return new SubscriberRouting(typeof(TSubscriber), address);
        }


        public override string ToString()
        {
            return $"{SubscriberType.Name} <- {Address.ToString()}";
        }
    }
}
