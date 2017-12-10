using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageRouter.Receivers;
using MessageRouter.Senders;

namespace MessageRouter
{
    public interface IEndPointFactory<TSender, TReceiver> : ISenderFactory<TSender>, IReceiverFactory<TReceiver>
        where TSender : ISender
        where TReceiver : IReceiver
    {
    }
}
