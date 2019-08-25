using System;
using System.Collections.Generic;

using NetMQ;

using Pigeon.Addresses;

namespace Pigeon.NetMQ.Common
{
    /// <summary>
    /// Common implementation for all NetMQ <see cref="INetMQConnection"/>s
    /// </summary>
    public abstract class NetMQConnection : INetMQConnection, IDisposable
    {
        protected readonly INetMQMessageFactory messageFactory;

        protected bool disposedValue = false;
        private readonly HashSet<IAddress> addresses = new HashSet<IAddress>();


        /// <summary>
        /// Gets an enumerable of <see cref="IAddress"/> that the receiver is listening to
        /// </summary>
        public IEnumerable<IAddress> Addresses => addresses;


        /// <summary>
        /// Gets a <see cref="bool"/> that returns true when the <see cref="NetMQConnection"/> is connected; otherwise false
        /// </summary>
        public bool IsConnected { get; private set; } = false;


        /// <summary>
        /// The NetMQ socket that a <see cref="NetMQPoller"/> will actively monitor for incoming requests
        /// </summary>
        public ISocketPollable PollableSocket { get; private set; }


        /// <summary>
        /// Initializes a new instance of <see cref="NetMQConnection"/>
        /// </summary>
        /// <param name="pollableSocket">Inner socket connection that is monitored by <see cref="INetMQPoller"/></param>
        /// <param name="messageFactory">Factory for creating <see cref="NetMQMessage"/>s</param>
        public NetMQConnection(ISocketPollable pollableSocket, INetMQMessageFactory messageFactory)
        {
            this.PollableSocket = pollableSocket ?? throw new ArgumentNullException(nameof(pollableSocket));
            this.messageFactory = messageFactory ?? throw new ArgumentNullException(nameof(messageFactory));
        }


        /// <summary>
        /// Adds the <see cref="IAddress"/> to the set of addresses the <see cref="NetMQConnection"/> will listen to
        /// for incoming <see cref="Packages.Package"/>s
        /// </summary>
        /// <param name="address"><see cref="IAddress"/> to add</param>
        public void AddAddress(IAddress address)
        {
            if (address is null)
                throw new ArgumentNullException(nameof(address));

            if (disposedValue)
                throw new InvalidOperationException("NetMQConnection is disposed");

            if (addresses.Contains(address))
                return;

            addresses.Add(address);

            if (IsConnected)
                SocketAdd(address);
        }


        /// <summary>
        /// Removes the <see cref="IAddress"/> from the set of addresses the <see cref="NetMQConnection"/> will listen to
        /// for incoming <see cref="Packages.Package"/>s
        /// </summary>
        /// <param name="address"><see cref="IAddress"/> to remove</param>
        public void RemoveAddress(IAddress address)
        {
            if (address is null || !addresses.Contains(address))
                return;

            if (IsConnected)
                SocketRemove(address);

            addresses.Remove(address);

            if (addresses.Count == 0)
                IsConnected = false;
        }


        /// <summary>
        /// Removes all <see cref="IAddress"/> from the set of addresses the <see cref="NetMQConnection"/> will listen to
        /// for incoming <see cref="Packages.Package"/>s
        /// </summary>
        public void RemoveAllAddresses()
        {
            if (IsConnected)
                foreach (var address in addresses)
                    SocketRemove(address);

            addresses.Clear();

            IsConnected = false;
        }


        /// <summary>
        /// Initializes the connections to all added <see cref="IAddress"/>
        /// </summary>
        public void InitializeConnection()
        {
            if (IsConnected)
                return;

            foreach (var address in addresses)
                SocketAdd(address);

            IsConnected = true;
        }


        /// <summary>
        /// Terminates the connection to all added <see cref="IAddress"/>
        /// </summary>
        public void TerminateConnection()
        {
            if (!IsConnected)
                return;

            foreach (var address in addresses)
                SocketRemove(address);
            
            IsConnected = false;
        }


        /// <summary>
        /// Add <see cref="IAddress"/> to the socket
        /// </summary>
        /// <param name="address"><see cref="IAddress"/> to be added</param>
        public abstract void SocketAdd(IAddress address);


        /// <summary>
        /// Remote <see cref="IAddress"/> from the socket
        /// </summary>
        /// <param name="address"><see cref="IAddress"/> to be removed</param>
        public abstract void SocketRemove(IAddress address);


        /// <summary>
        /// Cleans up resources
        /// </summary>
        public abstract void Dispose();
    }
}
