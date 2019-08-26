using System.IO;

namespace Pigeon.Serialization
{
    /// <summary>
    /// Exposes the ability to serialize the object to some binary format
    /// </summary>
    public interface IWriteable
    {
        /// <summary>
        /// Returns a binary serialized representation of the object instance
        /// </summary>
        /// <returns></returns>
        byte[] WriteTo();


        /// <summary>
        /// Writes a binary serialized representation of the object instance to the supplied stream
        /// </summary>
        /// <param name="stream"><see cref="Stream"/> into which a binary serialized representation of the object instance will be written</param>
        void WriteTo(Stream stream);


        /// <summary>
        /// Creates a binary serialized representation of the object instance
        /// /// </summary>
        /// <param name="writer"><see cref="BinaryWriter"/> used to create a serialized instance of the object instance</param>
        void WriteTo(BinaryWriter writer);
    }
}
