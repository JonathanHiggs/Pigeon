using System.IO;

using Pigeon.Serialization;

namespace Pigeon.Protocol
{
    /// <summary>
    /// Root for protocol messages inheritance hierarchy
    /// </summary>
    public abstract class ProtocolMessage : IWriteable
    {
        /// <summary>
        /// Returns a binary serialized representation of the object instance
        /// </summary>
        /// <returns></returns>
        public byte[] WriteTo()
        {
            using (var stream = new MemoryStream())
            {
                WriteTo(stream);
                return stream.ToArray();
            }
        }


        /// <summary>
        /// Writes a binary serialized representation of the object instance to the supplied stream
        /// </summary>
        /// <param name="stream"><see cref="Stream"/> into which a binary serialized representation of the object instance will be written</param>
        public void WriteTo(Stream stream)
        {
            using (var writer = new BinaryWriter(stream))
                WriteTo(writer);
        }


        /// <summary>
        /// Creates a binary serialized representation of the object instance
        /// /// </summary>
        /// <param name="writer"><see cref="BinaryWriter"/> used to create a serialized instance of the object instance</param>
        public virtual void WriteTo(BinaryWriter writer) =>
            writer.Write(GetProtocolName());


        /// <summary>
        /// Returns the protocol identifier name
        /// </summary>
        /// <returns></returns>
        protected abstract string GetProtocolName();
    }
}