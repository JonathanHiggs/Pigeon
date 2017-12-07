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
            if (routingTable.ContainsKey(requestType))
                throw new InvalidOperationException($"Router already contains a routing for {requestType.Name}");

            routingTable.Add(requestType, SenderRouting.For<TSender>(address));
        }


        /// <summary>
        /// TryGets a routing for a request type
        /// </summary>
        /// <typeparam name="TRequest">Request type</typeparam>
        /// <param name="senderMapping">Matching <see cref="SenderRouting"/></param>
        /// <returns>Bool indicating whether a <see cref="SenderRouting"/> was found for the request type</returns>
        public bool RoutingFor<TRequest>(out SenderRouting senderMapping)
        {
            return routingTable.TryGetValue(typeof(TRequest), out senderMapping);
        }
    }
}
