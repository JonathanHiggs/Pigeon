using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Routing
{
    public interface ITopicRouter
    {
        IReadOnlyDictionary<Type, SubscriberRouting> RoutingTable { get; }


        bool RoutingFor<TTopic>(out SubscriberRouting routing);
    }
}
