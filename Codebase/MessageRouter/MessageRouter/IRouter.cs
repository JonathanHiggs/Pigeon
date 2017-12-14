using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageRouter.Publishers;
using MessageRouter.Receivers;
using MessageRouter.Senders;

namespace MessageRouter
{
    /// <summary>
    /// <see cref="IRouter{TRouterInfo}"/> represents a complete abstraction of the transport layer to all other
    /// parts of the architecture
    /// </summary>
    /// <typeparam name="TRouterInfo"></typeparam>
    public interface IRouter<TRouterInfo> : ISend, IPublish where TRouterInfo : IRouterInfo
    {
        /// <summary>
        /// Gets a <see cref="TRouterInfo"/> to access state information of the <see cref="IRouter{TRouterInfo}"/>
        /// </summary>
        TRouterInfo Info { get; }


        /// <summary>
        /// Starts all internal active transports running
        /// </summary>
        void Start();


        /// <summary>
        /// Stops all internal active transports running
        /// </summary>
        void Stop();
    }
}
