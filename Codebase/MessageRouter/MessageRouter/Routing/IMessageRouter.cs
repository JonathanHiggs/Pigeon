using MessageRouter.Addresses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Routing
{
    public interface IMessageRouter
    {
        bool RoutingFor<TRequest>(out SenderRouting senderMapping);
    }
}
