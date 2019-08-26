using System;

namespace Pigeon.Protocol
{
    /// <summary>
    /// Attribute used to tag a protocol root class with and identifying name
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ProtocolNameAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ProtocolNameAttribute"/>
        /// </summary>
        /// <param name="name">Protocol identifying name</param>
        public ProtocolNameAttribute(string name)
        {
            Name = name;
        }


        /// <summary>
        /// Gets a name that identifies a protocol
        /// </summary>
        public string Name { get; }
    }
}
