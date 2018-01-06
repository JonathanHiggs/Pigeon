using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pigeon.Addresses;

namespace Pigeon.Fluent
{
    public interface ITransportSetup
    {
        ISenderSetup WithSender(IAddress address);
        ISubscriberSetup WithSubscriber(IAddress address);
    }
}
