using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageRouter.Receivers;
using MessageRouter.Senders;

namespace MessageRouter.Transport
{
    /// <summary>
    /// Combined factory for <see cref="ISender"/>s and <see cref="IReceiver"/>s
    /// </summary>
    /// <typeparam name="TSender">The implementation of <see cref="ISender"/> this factory creates</typeparam>
    /// <typeparam name="TReceiver">The implementation of <see cref="IReceiver"/> this factory creates</typeparam>
    public interface ITransportFactory<TSender, TReceiver> : ISenderFactory<TSender>, IReceiverFactory<TReceiver>
        where TSender : ISender
        where TReceiver : IReceiver
    {
    }
}
