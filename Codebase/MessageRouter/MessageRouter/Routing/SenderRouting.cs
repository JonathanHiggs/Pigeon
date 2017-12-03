using MessageRouter.Addresses;
using MessageRouter.Senders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Routing
{
    public struct SenderRouting
    {
        public Type SenderType;
        public IAddress Address;

        private SenderRouting(Type senderType, IAddress address)
        {
            SenderType = senderType;
            Address = address;
        }

        public static SenderRouting For<TSender>(IAddress address)
            where TSender : ISender
        {
            return new SenderRouting(typeof(TSender), address);
        }
    }
}
