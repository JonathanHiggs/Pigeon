using System;
using System.IO;

using Pigeon.Serialization;
using Pigeon.Utils;

using BasePigeonMessage = Pigeon.Protocol.PigeonMessage;

namespace Pigeon.Protocol.v1_0
{
    /// <summary>
    /// Base class for pigeon protocol v1.0 messages
    /// </summary>
    public abstract class PigeonMessage : BasePigeonMessage
    {
        public static readonly ProtocolVersion Version = new ProtocolVersion(1, 0);

        
        /// <summary>
        /// Gets the protocol version number
        /// </summary>
        public override ProtocolVersion ProtocolVersion => Version;


        /// <summary>
        /// Gets the message type
        /// </summary>
        public abstract MessageType MessageType { get; }


        public override void WriteTo(BinaryWriter writer)
        {
            base.WriteTo(writer);
            writer.Write(MessageType);
        }


        public static new PigeonMessage ReadFrom(BinaryReader reader)
        {
            var messageType = reader.ReadMessageType();

            switch (messageType)
            {
                case MessageType.Request:
                    return PigeonRequestMessage.ReadFrom(reader);

                case MessageType.Response:
                    return PigeonResponseMessage.ReadFrom(reader);

                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
