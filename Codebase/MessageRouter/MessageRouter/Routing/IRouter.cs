using MessageRouter.Addresses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Routing
{
    /// <summary>
    /// Maps request types to a <see cref="SenderRouting"/>
    /// </summary>
    public interface IRouter
    {
        /// <summary>
        /// Gets the routing table
        /// </summary>
        IReadOnlyDictionary<Type, SenderRouting> RoutingTable { get; }


        /// <summary>
        /// TryGets a routing for a request type
        /// </summary>
        /// <typeparam name="TRequest">Request type</typeparam>
        /// <param name="senderMapping">Matching <see cref="SenderRouting"/></param>
        /// <returns>Bool indicating whether a <see cref="SenderRouting"/> was found for the request type</returns>
        bool RoutingFor<TRequest>(out SenderRouting senderMapping);
    }
}
