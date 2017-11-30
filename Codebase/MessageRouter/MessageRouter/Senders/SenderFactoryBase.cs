using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageRouter.Addresses;

namespace MessageRouter.Senders
{
    public abstract class SenderFactoryBase<TSender> : ISenderFactory
        where TSender : ISender
    {
        public ISender GetSender(IAddress address) => CreateSender(address);


        public ISenderMonitor GetMonitor() => CreateMonitor();


        public abstract TSender CreateSender(IAddress address);


        public abstract SenderMonitorBase<TSender> CreateMonitor();
    }
}
