using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageRouter.Addresses;
using MessageRouter.Senders;

namespace MessageRouter.Routing
{
    /// <summary>
    /// Maps request types to a <see cref="SenderRouting"/>
    /// </summary>
    public class Router : IRouter
    {
        private readonly Dictionary<Type, SenderRouting> routingTable = new Dictionary<Type, SenderRouting>();


        /// <summary>
        /// Gets the routing table
        /// </summary>
        public IReadOnlyDictionary<Type, SenderRouting> RoutingTable => routingTable;


        /// <summary>
        /// Adds a <see cref="SenderRouting"/> to the routing table
        /// </summary>
        /// <typeparam name="TRequest">Request type</typeparam>
        /// <typeparam name="TSender"><see cref="ISender"/> type</typeparam>
        /// <param name="address">Remote address</param>
        public void AddSenderRouting<TRequest, TSender>(IAddress address)
            where TRequest : class
            where TSender : ISender
        {
            if (null == address)
                throw new ArgumentNullException(nameof(address));

            var requestType = typeof(TRequest);
            var newRouting = SenderRouting.For<TSender>(address);

            if (routingTable.ContainsKey(requestType))
                throw new RoutingAlreadyRegisteredException(newRouting, routingTable[requestType]);

            routingTable.Add(requestType, newRouting);
        }


        /// <summary>
        /// TryGets a routing for a request type
        /// </summary>
        /// <typeparam name="TRequest">Request type</typeparam>
        /// <param name="senderMapping">Matching <see cref="SenderRouting"/></param>
        /// <returns>true if the <see cref="IRouter"/> has a <see cref="SenderRouting"/> for the request type; otherwise, false</returns>
        public bool RoutingFor<TRequest>(out SenderRouting senderMapping)
        {
            return routingTable.TryGetValue(typeof(TRequest), out senderMapping);
        }
    }
}
