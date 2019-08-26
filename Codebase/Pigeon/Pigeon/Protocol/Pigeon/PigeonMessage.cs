using System;
using System.IO;

using Pigeon.Serialization;

namespace Pigeon.Protocol.Pigeon
{
    /// <summary>
    /// Root object for Pigeon message protocol
    /// </summary>
    [ProtocolName(ProtocolName)]
    public abstract class PigeonMessage : ProtocolMessage
    {
        /// <summary>
        /// Protocol identifier
        /// </summary>
        public const string ProtocolName = "PIGEON";
        

        /// <summary>
        /// Creates a binary serialized representation of the object instance
        /// /// </summary>
        /// <param name="writer"><see cref="BinaryWriter"/> used to create a serialized instance of the object instance</param>
        public override void WriteTo(BinaryWriter writer)
        {
            base.WriteTo(writer);
            writer.Write(GetProtocolVersion());
        }


        /// <summary>
        /// Initializes a new instance of <see cref="PigeonMessage"/> from serialized data
        /// </summary>
        /// <param name="reader">Data reader to de-serialize</param>
        /// <returns></returns>
        public static PigeonMessage ReadFrom(BinaryReader reader)
        {
            var version = reader.ReadProtocolVersion();

            if (version == v1_0.PigeonMessage.ProtocolVersion)
                return v1_0.PigeonMessage.ReadFrom(reader);

            throw new InvalidOperationException();
        }


        /// <summary>
        /// Returns the protocol identifier name
        /// </summary>
        /// <returns></returns>
        protected override string GetProtocolName()
            => ProtocolName;


        /// <summary>
        /// Returns the protocol version
        /// </summary>
        /// <returns></returns>
        protected abstract ProtocolVersion GetProtocolVersion();
    }
}
