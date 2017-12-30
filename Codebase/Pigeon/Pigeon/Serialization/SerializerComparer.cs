using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pigeon.Serialization
{
    /// <summary>
    /// Compares <see cref="ISerializer"/>s for equality
    /// </summary>
    public class SerializerComparer : IEqualityComparer<ISerializer>
    {
        /// <summary>
        /// Determines whether the specified <see cref="ISerializer"/>s are equal
        /// </summary>
        /// <param name="x">The first <see cref="ISerializer"/> to compare</param>
        /// <param name="y">The second <see cref="ISerializer"/> to compare</param>
        /// <returns>true if the specified objects are equal; otherwise, false</returns>
        public bool Equals(ISerializer x, ISerializer y)
        {
            return x.Descriptor == y.Descriptor;
        }


        /// <summary>
        /// Returns a hash code for the specified <see cref="ISerializer"/>
        /// </summary>
        /// <param name="obj">The <see cref="ISerializer"/> for which a hash code is to be returned</param>
        /// <returns>A hash code for the specified <see cref="ISerializer"/></returns>
        public int GetHashCode(ISerializer obj)
        {
            return obj.Descriptor.GetHashCode();
        }
    }
}
