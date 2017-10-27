using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Server
{
    public interface IServerInfo
    {
        bool Running { get; }
        string Name { get; }
        DateTime? StartUpTimeStamp { get; }
    }
}
