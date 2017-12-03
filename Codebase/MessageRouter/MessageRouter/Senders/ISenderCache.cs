using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Senders
{
    public interface ISenderCache
    {
        IReadOnlyCollection<ISenderFactory> Factories { get; }

        ISender SenderFor<TRequest>();
    }
}
