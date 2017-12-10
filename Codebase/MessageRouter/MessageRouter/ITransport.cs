using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageRouter.Receivers;
using MessageRouter.Senders;

namespace MessageRouter
{
    public interface ITransport<TSender, TReceiver>
        where TSender : ISender
        where TReceiver : IReceiver
    {
        ISenderFactory<TSender> SenderFactory { get; }
        IReceiverFactory<TReceiver> ReceiverFactory { get; }
    }
}
