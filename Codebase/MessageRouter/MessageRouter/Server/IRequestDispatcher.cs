using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Server
{
    public interface IRequestDispatcher
    {
        object Handle(object requestObject);
    }
}
