using System;
using System.IO;

using Pigeon.Serialization;

using BasePigeonMessage = Pigeon.Protocol.Pigeon.PigeonMessage;

namespace Pigeon.Protocol.Pigeon.v1_0
{
    /// <summary>
    /// Base class for pigeon protocol v1.0 messages
    /// </summary>
    public abstract class PigeonMessage : BasePigeonMessage
    {
        /// <summary>
        /// Gets the protocol version number
        /// </summary>
        public static readonly ProtocolVersion ProtocolVersion = new ProtocolVersion(1, 0);


        /// <summary>
        /// Creates a binary serialized representation of the object instance
        /// /// </summary>
        /// <param name="writer"><see cref="BinaryWriter"/> used to create a serialized instance of the object instance</param>
        public override void WriteTo(BinaryWriter writer)
        {
            base.WriteTo(writer);
            writer.Write(GetMessageType());
        }


        /// <summary>
        /// Initializes a new instance of <see cref="PigeonMessage"/> from serialized data
        /// </summary>
        /// <param name="reader">Data reader to de-serialize</param>
        /// <returns></returns>
        public static new PigeonMessage ReadFrom(BinaryReader reader)
        {
            var messageType = reader.ReadString();

            switch (messageType)
            {
                case PigeonRequestMessage.MessageType:
                    return PigeonRequestMessage.ReadFrom(reader);

                case PigeonResponseMessage._MessageType:
                    return PigeonResponseMessage.ReadFrom(reader);

                case PigeonTopicMessage.MessageType:
                    return PigeonTopicMessage.ReadFrom(reader);

                default:
                    throw new InvalidOperationException();
            }
        }


        /// <summary>
        /// Returns the protocol version
        /// </summary>
        /// <returns></returns>
        protected override ProtocolVersion GetProtocolVersion() => ProtocolVersion;


        /// <summary>
        /// Returns the message type identifier
        /// </summary>
        /// <returns></returns>
        protected abstract string GetMessageType();
    }
}
