using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Verbs
{
    public interface ISubscribe
    {
        IDisposable Subscribe<TTopic>();

        void Unsubscribe<TTopic>();
    }
}
