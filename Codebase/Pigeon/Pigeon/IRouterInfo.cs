using System;

namespace Pigeon
{
    /// <summary>
    /// Interface for retrieving <see cref="IRouter{TRouterInfo}"/> state information
    /// </summary>
    public interface IRouterInfo
    {
        /// <summary>
        /// Get a flag that is true if the <see cref="IRouter{TRouterInfo}"/> is running, and false otherwise
        /// </summary>
        bool Running { get; }


        /// <summary>
        /// Gets a <see cref="string"/> that represents a human readable name of the node
        /// </summary>
        string Name { get; }


        /// <summary>
        /// Gets a <see cref="DateTime"/> of the last time the <see cref="IRouter{TRouterInfo}"/> started
        /// </summary>
        DateTime? StartedTimestamp { get; }


        /// <summary>
        /// Gets a <see cref="DateTime"/> of the last time the <see cref="IRouter{TRouterInfo}"/> stopped
        /// </summary>
        DateTime? StoppedTimestamp { get; }
    }
}
