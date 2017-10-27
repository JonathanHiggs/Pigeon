using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Server
{
    public class ServerInfo : IServerInfo
    {
        public bool Running { get; set; }
        public string Name { get; set; }
        public DateTime? StartUpTimeStamp { get; set; }
    }
}
