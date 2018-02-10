﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pigeon.Addresses;
using Pigeon.Common;
using Pigeon.Packages;
using Pigeon.Publishers;
using Pigeon.Transport;
using Pigeon.Verbs;

namespace Pigeon.Subscribers
{
    /// <summary>
    /// Interface encapsulates a connection that is able to connect to <see cref="IAddress"/>es to receive messages
    /// from <see cref="IPublisher"/>s
    /// </summary>
    public interface ISubscriber : IConnection
    {
        /// <summary>
        /// Gets the <see cref="TopicEventHandler"/> delegate the <see cref="ISubscriber"/> calls when an
        /// incoming topic message is received
        /// </summary>
        TopicEventHandler Handler { get; }


        /// <summary>
        /// Initializes a subscription to the topic message stream from a remote <see cref="IPublisher"/>
        /// </summary>
        /// <typeparam name="TTopic">The type of the published topic message</typeparam>
        void Subscribe<TTopic>();


        /// <summary>
        /// Terminates a subscription to the topic message stream
        /// </summary>
        void Unsubscribe<TTopic>();
    }
}