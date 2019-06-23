namespace Pigeon.Serialization
{
    /// <summary>
    /// Structure representing a version
    /// </summary>
    public readonly struct ProtocolVersion
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ProtocolVersion"/>
        /// </summary>
        /// <param name="major">Major version number</param>
        /// <param name="minor">Minor version number</param>
        public ProtocolVersion(byte major, byte minor)
        {
            Major = major;
            Minor = minor;
        }


        /// <summary>
        /// Gets a byte representing the major version number
        /// </summary>
        public byte Major { get; }


        /// <summary>
        /// Gets a byte representing the minor version number
        /// </summary>
        public byte Minor { get; }
    }
}
