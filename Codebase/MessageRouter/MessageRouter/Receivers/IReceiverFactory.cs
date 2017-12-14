using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageRouter.Messages;
using MessageRouter.Addresses;
using MessageRouter.Monitors;

namespace MessageRouter.Receivers
{
    /// <summary>
    /// Factory for creating <see cref="IReceiver"/>s at config-time
    /// </summary>
    public interface IReceiverFactory
    {
        /// <summary>
        /// Gets the type of <see cref="IReceiver"/>s that this factory creates
        /// </summary>
        Type ReceiverType { get; }


        /// <summary>
        /// Gets the <see cref="IMonitor"/> associated with the <see cref="IReceiverFactory"/>s <see cref="IReceiver"/>
        /// </summary>
        IMonitor ReceiverMonitor { get; }


        /// <summary>
        /// Creates a new instance of a <see cref="IReceiver"/> bound to the supplied <see cref="IAddress"/>
        /// </summary>
        /// <param name="address">Address of local bound endpoint</param>
        /// <returns>Receiver bound to the address</returns>
        IReceiver CreateReceiver(IAddress address);
    }


    /// <summary>
    /// Factory for createing <see cref="TReceiver"/>s at config-time
    /// </summary>
    /// <typeparam name="TReceiver">Transport specific implementation of <see cref="IReceiver"/>s the factory creates</typeparam>
    public interface IReceiverFactory<TReceiver> : IReceiverFactory where TReceiver : IReceiver
    { }
}
