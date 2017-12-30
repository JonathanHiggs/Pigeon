using System;
using System.Collections.Generic;

using Pigeon.Addresses;
using Pigeon.Diagnostics;
using Pigeon.Receivers;
using Pigeon.Senders;

namespace Pigeon.Routing
{
    /// <summary>
    /// Maps request message types to a <see cref="SenderRouting"/> for runtime construction and resolution of
    /// <see cref="ISender"/>s from config-time setup
    /// </summary>
    public class RequestRouter : IRequestRouter
    {
        private readonly Dictionary<Type, SenderRouting> routingTable = new Dictionary<Type, SenderRouting>();


        /// <summary>
        /// Gets the routing table of request message type to <see cref="SenderRouting"/>
        /// </summary>
        public IReadOnlyDictionary<Type, SenderRouting> RoutingTable => routingTable;


        /// <summary>
        /// Adds to the routing table
        /// </summary>
        /// <typeparam name="TRequest">Request message type</typeparam>
        /// <typeparam name="TSender">Transport specific <see cref="ISender"/> type</typeparam>
        /// <param name="address"><see cref="IAddress"/> of the remote <see cref="IReceiver"/> for this <see cref="ISender"/></param>
        public void AddRequestRouting<TRequest, TSender>(IAddress address)
            where TSender : ISender
        {
            if (null == address)
                throw new ArgumentNullException(nameof(address));

            var requestType = typeof(TRequest);
            var newRouting = SenderRouting.For<TSender>(address);

            if (routingTable.TryGetValue(requestType, out var existingRouting))
            {
                if (newRouting.Address.Equals(existingRouting.Address))
                    return;
                else
                    throw new RoutingAlreadyRegisteredException<SenderRouting>(newRouting, existingRouting);
            }

            routingTable.Add(requestType, newRouting);
        }


        /// <summary>
        /// TryGets a <see cref="SenderRouting"/> for the request message type
        /// </summary>
        /// <typeparam name="TRequest">Request message type</typeparam>
        /// <param name="routing">Outs a matching <see cref="SenderRouting"/> for the request message type if the 
        /// <see cref="RequestRouter"/> has one added</param>
        /// <returns>True if the <see cref="RequestRouter"/> has a <see cref="SenderRouting"/> for the request type;
        /// otherwise, false</returns>
        public bool RoutingFor<TRequest>(out SenderRouting senderMapping) => routingTable.TryGetValue(typeof(TRequest), out senderMapping);
    }
}
