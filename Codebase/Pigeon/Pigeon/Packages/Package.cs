using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pigeon.Packages
{
    /// <summary>
    /// Base class for the messaging pacakge protocol
    /// </summary>
    [Serializable]
    [ImmutableObject(true)]
    public abstract class Package
    {
        /// <summary>
        /// Stores a readonly reference to a <see cref="IPackageId"/> for the <see cref="Package"/>
        /// </summary>
        public readonly IPackageId Id;


        /// <summary>
        /// Gets the packed message body
        /// </summary>
        public abstract object Body { get; }


        /// <summary>
        /// Initializes a new instance of a <see cref="Package"/>
        /// </summary>
        /// <param name="id">Package indentifier</param>
        public Package(IPackageId id)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
        }
    }
}
