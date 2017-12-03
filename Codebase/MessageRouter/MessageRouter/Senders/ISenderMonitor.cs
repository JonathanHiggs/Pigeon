using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Senders
{
    public interface ISenderMonitor
    {
        void StartSenders();
        void StopSenders();
    }


    public interface ISenderMonitor<TSender> : ISenderMonitor where TSender : ISender
    {
        void AddSender(TSender sender);
    }
}
