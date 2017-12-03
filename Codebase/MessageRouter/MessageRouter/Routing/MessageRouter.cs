using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageRouter.Addresses;
using MessageRouter.Senders;

namespace MessageRouter.Routing
{
    public class MessageRouter : IMessageRouter
    {
        private readonly Dictionary<Type, SenderRouting> routingTable = new Dictionary<Type, SenderRouting>();
        

        public void AddSenderRouting<TRequest, TSender>(IAddress address)
            where TRequest : class
            where TSender : ISender
        {
            routingTable.Add(typeof(TRequest), SenderRouting.For<TSender>(address));
        }


        public bool RoutingFor<TRequest>(out SenderRouting senderMapping)
        {
            return routingTable.TryGetValue(typeof(TRequest), out senderMapping);
        }
    }
}
