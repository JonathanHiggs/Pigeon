using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Server
{
    /// <summary>
    /// Data class for storing <see cref="IMessageServer{TServerInfo}"/> state information
    /// </summary>
    public class ServerInfo : IServerInfo
    {
        /// <summary>
        /// Gets and sets a <see cref="bool"/> indicating whether the server is accepting requests from remotes
        /// </summary>
        public bool Running { get; set; }


        /// <summary>
        /// Gets and sets a <see cref="string"/> containing the name of the server
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        /// Gets and sets a <see cref="DateTime"/> of the server start up timestamp
        /// </summary>
        public DateTime? StartUpTimeStamp { get; set; }
    }
}
