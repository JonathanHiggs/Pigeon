using System;
using System.Collections.Generic;
using System.Text;

namespace Pigeon
{
    /// <summary>
    /// Encapsulates a unique identifier and other information to identify and understamd the position
    /// of a node in a network
    /// </summary>
    [Serializable]
    public class NodeIdentity
    {
        /// <summary>
        /// Gets a unique identifier for the node
        /// </summary>
        public Guid Id { get; private set; }


        /// <summary>
        /// Gets a human readable name for the node
        /// </summary>
        public string Name { get; private set; }


        /// <summary>
        /// Host machine for the node
        /// </summary>
        public string Host { get; private set; }


        /// <summary>
        /// Initializes a new instance of <see cref="NodeIdentity"/>
        /// </summary>
        /// <param name="name">Name of the node</param>
        public NodeIdentity(string name)
            : this(Guid.NewGuid(), name, Environment.MachineName)
        { }


        /// <summary>
        /// Initializes a new instance of <see cref="NodeIdentity"/>
        /// </summary>
        /// <param name="id">Unique identifier of the node</param>
        /// <param name="name">Name of the node</param>
        /// <param name="host">Host machine name of the node</param>
        public NodeIdentity(Guid id, string name, string host)
        {
            Id = id;
            Name = name;
            Host = host;
        }
    }
}
