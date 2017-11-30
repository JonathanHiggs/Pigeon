using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Senders
{
    public abstract class SenderMonitorBase<TSender> : ISenderMonitor
        where TSender : ISender
    {
        public void Add(ISender sender)
        {
            AddSender((TSender)sender);
        }

        public abstract void AddSender(TSender sender);

        public abstract void Start();

        public abstract void Stop();
    }
}
