using Pigeon.Addresses;
using Pigeon.Monitors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pigeon.Senders
{
    /// <summary>
    /// Factory for <see cref="ISender"/>s
    /// </summary>
    public interface ISenderFactory
    {
        /// <summary>
        /// Gets the <see cref="IMonitor"/> associated with the <see cref="ISenderFactory"/> <see cref="ISender"/>s
        /// </summary>
        IMonitor SenderMonitor { get; }


        /// <summary>
        /// Gets the type of <see cref="ISender"/>s that this factory creates
        /// </summary>
        Type SenderType { get; }


        /// <summary>
        /// Constructs a new instance of an <see cref="ISender"/> connected to the supplied <see cref="IAddress"/>
        /// </summary>
        /// <param name="address">Address of the remote the sender will connect to</param>
        /// <returns>Sender connected to the remote address</returns>
        ISender CreateSender(IAddress address);
    }


    /// <summary>
    /// Factory for creating <see cref="TSender"/>s
    /// </summary>
    /// <typeparam name="TSender">Type of <see cref="ISender"/>s the factors constructs</typeparam>
    public interface ISenderFactory<TSender> : ISenderFactory where TSender : ISender
    { }
}
