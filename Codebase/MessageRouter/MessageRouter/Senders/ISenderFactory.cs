using MessageRouter.Addresses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Senders
{
    /// <summary>
    /// Factory for <see cref="ISender"/>s that are actively managed by <see cref="ISenderMonitor"/>s
    /// </summary>
    public interface ISenderFactory
    {
        /// <summary>
        /// Gets the <see cref="ISenderMonitor"/> associated with the factories <see cref="ISender"/>s
        /// </summary>
        ISenderMonitor SenderMonitor { get; }


        /// <summary>
        /// Constructs a new instance of an <see cref="ISender"/> connected to the supplied <see cref="IAddress"/>
        /// </summary>
        /// <param name="address">Address of the remote the sender will connect to</param>
        /// <returns>Sender connected to the remote address</returns>
        ISender CreateSender(IAddress address);
    }


    /// <summary>
    /// Factory for creating <see cref="TSender"/> and <see cref="ISenderMonitor{TSender}"/>s
    /// </summary>
    /// <typeparam name="TSender">Type of <see cref="ISender"/> the factors constructs</typeparam>
    public interface ISenderFactory<TSender> : ISenderFactory where TSender : ISender
    { }
}
