using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MessageRouter.Server
{
    /// <summary>
    /// Interface controls the active server process that accepts and responces to incoming request asynchronously
    /// </summary>
    /// <typeparam name="TServerInfo">Type of <see cref="IServerInfo"/> data object that exposes data information</typeparam>
    public interface IAsyncMessageServer<TServerInfo> where TServerInfo : IServerInfo
    {
        /// <summary>
        /// Gets the current state of the server
        /// </summary>
        TServerInfo ServerInfo { get; }


        /// <summary>
        /// Asynchronously starts the server running
        /// </summary>
        void Start();


        /// <summary>
        /// Stops the server running
        /// </summary>
        void Stop();
    }
}
