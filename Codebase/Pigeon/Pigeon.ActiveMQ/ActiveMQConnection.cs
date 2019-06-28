using System;
using System.Collections.Generic;

using Apache.NMS;

using Pigeon.Addresses;

namespace Pigeon.ActiveMQ
{
    public class ActiveMQConnection : IDisposable
    {
        private NMSConnectionFactory connectionFactory;
        private IConnection connection;
        protected ISession session;
        protected ActiveMQAddress address;


        /// <summary>
        /// Gets an enumerable of <see cref="IAddress"/> that the receiver is listening to
        /// </summary>
        public IEnumerable<IAddress> Addresses => new[] { address };


        /// <summary>
        /// Gets a bool that returns true when the <see cref="NetMQConnection"/> is connected; otherwise false
        /// </summary>
        public bool IsConnected { get; private set; } = false;


        /// <summary>
        /// Adds the <see cref="IAddress"/> to the set of adresses the <see cref="IConnection"/> will listen to
        /// for incoming <see cref="Package"/>s
        /// </summary>
        /// <param name="address"><see cref="IAddress"/> to add</param>
        public void AddAddress(IAddress address)
        {
            if (!(address is null))
                throw new InvalidOperationException($"{GetType().Name} may only have one address");

            if (!(address is ActiveMQAddress activeAddress))
                throw new InvalidOperationException($"{nameof(ActiveMQAddress)} is required for {GetType().Name}");

            address = activeAddress;

            if (IsConnected)
            {
                connectionFactory = new NMSConnectionFactory(address.ToString());
                connection = connectionFactory.CreateConnection();
                connection.Start();
                session = connection.CreateSession(AcknowledgementMode.AutoAcknowledge);
            }
        }


        /// <summary>
        /// Initializes the connections to all added <see cref="IAddress"/>es
        /// </summary>
        public void InitializeConnection()
        {
            if (IsConnected)
                return;

            if (address is null)
                throw new InvalidOperationException($"{GetType().Name} requires a remote address before initializing the connection");

            connectionFactory = new NMSConnectionFactory(address.ToString());
            connection = connectionFactory.CreateConnection();
            connection.Start();
            session = connection.CreateSession(AcknowledgementMode.AutoAcknowledge);

            IsConnected = true;
        }


        /// <summary>
        /// Removes the <see cref="IAddress"/> from the set of addresses the <see cref="IConnection"/> will listen to
        /// for incoming <see cref="Packages.Package"/>s
        /// </summary>
        /// <param name="address"><see cref="IAddress"/> to remove</param>
        public void RemoveAddress(IAddress address)
        {
            TerminateConnection();

            this.address = null;
        }


        /// <summary>
        /// Removes all <see cref="IAddress"/> from the set of addresses the <see cref="IConnection"/> will listen to
        /// for incoming <see cref="Packages.Package"/>s
        /// </summary>
        public void RemoveAllAddresses() =>
            RemoveAddress(address);


        /// <summary>
        /// Terminates the connection to all added <see cref="IAddress"/>es
        /// </summary>
        public void TerminateConnection()
        {
            if (!IsConnected)
                return;

            session.Close();
            session.Dispose();
            connection.Stop();
            connection.Dispose();

            session = null;
            connection = null;
            connectionFactory = null;
        }


        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls


        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    TerminateConnection();
                }

                disposedValue = true;
            }
        }


        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}
