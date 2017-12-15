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
    public interface IRequestRouter
    {
        /// <summary>
        /// Gets the routing table
        /// </summary>
        IReadOnlyDictionary<Type, SenderRouting> RoutingTable { get; }


        /// <summary>
        /// TryGets a routing for a request type
        /// </summary>
        /// <typeparam name="TRequest">Request type</typeparam>
        /// <param name="routing">Matching <see cref="SenderRouting"/></param>
        /// <returns>true if the <see cref="IRequestRouter"/> has a <see cref="SenderRouting"/> for the request type; otherwise, false</returns>
        bool RoutingFor<TRequest>(out SenderRouting routing);
    }
}
