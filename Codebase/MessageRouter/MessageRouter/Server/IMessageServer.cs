using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MessageRouter.Server
{
    public interface IMessageServer<TServerInfo>
    {
        /// <summary>
        /// Gets the current state of the server
        /// </summary>
        TServerInfo ServerInfo { get; }


        /// <summary>
        /// Synchronously starts the server running
        /// </summary>
        void Run();


        /// <summary>
        /// Synchronously starts the server running with a <see cref="CancellationToken"/> to stop
        /// </summary>
        /// <param name="cancellationToken"></param>
        void Run(CancellationToken cancellationToken);
    }
}
