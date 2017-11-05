using MessageRouter.Receivers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.NetMQ
{
    public class NetMQReceiverManager : IReceiverManager
    {
        private readonly NetMQReceiver receiver;


        public NetMQReceiverManager(NetMQReceiver receiver)
        {
            this.receiver = receiver ?? throw new ArgumentNullException(nameof(receiver));
        }


        public RequestTask Receive()
        {
            return receiver.Receive();
        }
    }
}
