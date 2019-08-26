using System.IO;

namespace Pigeon.Serialization
{
    /// <summary>
    /// Data structure representing a version
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


        public static bool operator ==(ProtocolVersion a, ProtocolVersion b) =>
            a.Major == b.Major && a.Minor == b.Minor;


        public static bool operator !=(ProtocolVersion a, ProtocolVersion b) =>
            !(a == b);


        public override bool Equals(object obj)
        {
            return !(obj is null) && obj is ProtocolVersion other && this == other;
        }


        public override int GetHashCode()
        {
            var hashCode = 317314336;
            hashCode = hashCode * -1521134295 + Major.GetHashCode();
            hashCode = hashCode * -1521134295 + Minor.GetHashCode();
            return hashCode;
        }


        public override string ToString() =>
            $"v{Major}.{Minor}";
    }


    public static class ProtocolVersionExtensions
    {
        public static void Write(this BinaryWriter writer, ProtocolVersion protocolVersion)
        {
            writer.Write(protocolVersion.Major);
            writer.Write(protocolVersion.Minor);
        }


        public static ProtocolVersion ReadProtocolVersion(this BinaryReader reader) =>
            new ProtocolVersion(
                reader.ReadByte(),
                reader.ReadByte());
    }
}