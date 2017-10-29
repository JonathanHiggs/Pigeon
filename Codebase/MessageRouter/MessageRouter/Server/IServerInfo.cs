using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Server
{
    /// <summary>
    /// Interface for storing <see cref="IMessageServer{TServerInfo}"/> state information
    /// </summary>
    public interface IServerInfo
    {
        /// <summary>
        /// Gets a <see cref="bool"/> indicating whether the server is accepting requests from remotes
        /// </summary>
        bool Running { get; }


        /// <summary>
        /// Gets a <see cref="string"/> containing the name of the server
        /// </summary>
        string Name { get; }


        /// <summary>
        /// Gets a <see cref="DateTime"/> of the server start up timestamp 
        /// </summary>
        DateTime? StartUpTimeStamp { get; }
    }
}
