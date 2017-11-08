using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageRouter.Addresses;

namespace MessageRouter.Receivers
{
    public class ReceiverManager : IReceiverManager
    {
        private IReceiver receiver;

        
        public ReceiverManager(IReceiver receiver)
        {
            this.receiver = receiver ?? throw new ArgumentNullException(nameof(receiver));
        }


        public RequestTask Receive()
        {
            return receiver.Receive();
        }
    }
}
