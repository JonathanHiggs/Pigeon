using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pigeon
{
    /// <summary>
    /// Interface for retrieving <see cref="IRouter{TRouterInfo}"/> state information
    /// </summary>
    public class RouterInfo : IRouterInfo
    {
        /// <summary>
        /// Get a flag that is true if the <see cref="IRouter{TRouterInfo}"/> is running, and false otherwise
        /// </summary>
        public bool Running { get; set; }


        /// <summary>
        /// Gets a <see cref="string"/> that represents a human readable name of the node
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        /// Gets a <see cref="DateTime"/> of the last time the <see cref="IRouter{TRouterInfo}"/> started
        /// </summary>
        public DateTime? StartedTimestamp { get; set; }


        /// <summary>
        /// Gets a <see cref="DateTime"/> of the last time the <see cref="IRouter{TRouterInfo}"/> stopped
        /// </summary>
        public DateTime? StoppedTimestamp { get; set; }
    }
}
