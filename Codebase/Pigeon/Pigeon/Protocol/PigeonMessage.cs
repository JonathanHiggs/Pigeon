using System;
using System.IO;

using Pigeon.Serialization;

namespace Pigeon.Protocol
{
    public abstract class PigeonMessage : IWriteable
    {
        /// <summary>
        /// Gets the protocol version number
        /// </summary>
        public abstract ProtocolVersion ProtocolVersion { get; }


        public virtual void WriteTo(BinaryWriter writer)
        {
            writer.Write(ProtocolVersion);
        }


        public static PigeonMessage ReadFrom(BinaryReader reader)
        {
            var version = reader.ReadProtocolVersion();

            if (version == v1_0.PigeonMessage.Version)
                return v1_0.PigeonMessage.ReadFrom(reader);

            throw new InvalidOperationException();
        }
    }
}
