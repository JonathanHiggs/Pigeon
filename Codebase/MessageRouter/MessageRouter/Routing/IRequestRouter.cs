﻿using System;
using System.Collections.Generic;

using MessageRouter.Addresses;
using MessageRouter.Senders;

namespace MessageRouter.Routing
{
    /// <summary>
    /// Maps request types to a <see cref="SenderRouting"/> for runtime construction and resolution of <see cref="ISender"/>s
    /// from config-time setups
    /// </summary>
    public interface IRequestRouter
    {
        /// <summary>
        /// Gets the routing table of request type to <see cref="SenderRouting"/>
        /// </summary>
        IReadOnlyDictionary<Type, SenderRouting> RoutingTable { get; }


        /// <summary>
        /// TryGets a <see cref="SenderRouting"/> for the request message type
        /// </summary>
        /// <typeparam name="TRequest">Request message type</typeparam>
        /// <param name="routing">Outs a matching <see cref="SenderRouting"/> for the request message type if the 
        /// <see cref="IRequestRouter"/> has one added</param>
        /// <returns>True if the <see cref="IRequestRouter"/> has a <see cref="SenderRouting"/> for the request type; otherwise, false</returns>
        bool RoutingFor<TRequest>(out SenderRouting routing);


        /// <summary>
        /// Adds to the routing table
        /// </summary>
        /// <typeparam name="TRequest">Request message type</typeparam>
        /// <typeparam name="TSender">Transport specific <see cref="ISender"/> type</typeparam>
        /// <param name="address"><see cref="IAddress"/> of the remote <see cref="IReceiver"/> for this <see cref="ISender"/></param>
        void AddRequestRouting<TRequest, TSender>(IAddress address) where TSender : ISender;
    }
}
