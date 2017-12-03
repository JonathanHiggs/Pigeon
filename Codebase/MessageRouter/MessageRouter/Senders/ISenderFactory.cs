﻿using MessageRouter.Addresses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Senders
{
    /// <summary>
    /// Factory for <see cref="ISender"/>s and <see cref="ISenderMonitor"/>s
    /// </summary>
    public interface ISenderFactory
    {
        /// <summary>
        /// Creates a new instance of an <see cref="ISender"/> connected to the supplied <see cref="IAddress"/>
        /// </summary>
        /// <param name="address">Address of the remote the sender will connect to</param>
        /// <returns>Sender connected to the remote address</returns>
        ISender CreateSender(IAddress address);
    }


    public interface ISenderFactory<TSender> : ISenderFactory where TSender : ISender
    {
        /// <summary>
        /// Creates a new instance of an <see cref="TSender"/> connected to the supplied <see cref="IAddress"/>
        /// </summary>
        /// <param name="address">Address of the remote the sender will connect to</param>
        /// <returns>Sender connected to the remote address</returns>
        TSender Create(IAddress address);
    }
}
