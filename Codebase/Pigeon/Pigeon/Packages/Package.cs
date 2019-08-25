using System;
using System.ComponentModel;

namespace Pigeon.Packages
{
    /// <summary>
    /// Base class for the messaging package protocol
    /// </summary>
    [Serializable]
    [ImmutableObject(true)]
    public abstract class Package
    {
        /// <summary>
        /// Stores a read-only reference to a <see cref="IPackageId"/> for the <see cref="Package"/>
        /// </summary>
        public readonly IPackageId Id;


        /// <summary>
        /// Gets the packed message body
        /// </summary>
        public abstract object Body { get; }


        /// <summary>
        /// Initializes a new instance of <see cref="Package"/>
        /// </summary>
        /// <param name="id">Package identifier</param>
        public Package(IPackageId id)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
        }
    }
}
