using System;

using Pigeon.Addresses;
using Pigeon.Monitors;

namespace Pigeon.Receivers
{
    /// <summary>
    /// Factory for creating <see cref="IReceiver"/>s at configuration time
    /// </summary>
    public interface IReceiverFactory
    {
        /// <summary>
        /// Gets the transport specific implementation type of <see cref="IReceiver"/>s that this factory creates
        /// </summary>
        Type ReceiverType { get; }


        /// <summary>
        /// Gets the <see cref="IMonitor"/> associated with the <see cref="IReceiverFactory"/>s <see cref="IReceiver"/>
        /// </summary>
        IMonitor ReceiverMonitor { get; }


        /// <summary>
        /// Creates a new instance of <see cref="IReceiver"/> bound to the supplied <see cref="IAddress"/> and
        /// monitored by the factories <see cref="IMonitor"/>
        /// </summary>
        /// <param name="address"><see cref="IAddress"/> of local bound <see cref="IConnection"/></param>
        /// <param name="requestTaskHandler"><see cref="RequestTaskHandler"/> delegate that is called when by the 
        /// <see cref="IReceiver"/> when an incoming message is received</param>
        /// <returns><see cref="IReceiver"/> bound to the <see cref="IAddress"/></returns>
        IReceiver CreateReceiver(IAddress address);
    }


    /// <summary>
    /// Type specified factory for creating  <see cref="TReceiver"/>s at configuration time
    /// </summary>
    /// <typeparam name="TReceiver">Transport specific implementation of <see cref="IReceiver"/>s the factory creates</typeparam>
    public interface IReceiverFactory<TReceiver> : IReceiverFactory where TReceiver : IReceiver
    { }
}
