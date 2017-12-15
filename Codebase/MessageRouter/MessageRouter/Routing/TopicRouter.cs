using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageRouter.Addresses;
using MessageRouter.Subscribers;

namespace MessageRouter.Routing
{
    public class TopicRouter : ITopicRouter
    {
        private readonly Dictionary<Type, SubscriberRouting> routingTable = new Dictionary<Type, SubscriberRouting>();


        public IReadOnlyDictionary<Type, SubscriberRouting> RoutingTable => routingTable;


        public void AddSubscriberRouting<TTopic, TSubscriber>(IAddress address)
            where TSubscriber : ISubscriber
        {
            if (null == address)
                throw new ArgumentNullException(nameof(address));

            var topicType = typeof(TTopic);
            var newRouting = SubscriberRouting.For<TSubscriber>(address);

            if (routingTable.ContainsKey(topicType))
                throw new NotImplementedException();

            routingTable.Add(topicType, newRouting);
        }


        public bool RoutingFor<TTopic>(out SubscriberRouting routing)
        {
            return routingTable.TryGetValue(typeof(TTopic), out routing);
        }
    }
}
