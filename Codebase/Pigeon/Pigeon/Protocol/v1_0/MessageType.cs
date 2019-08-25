using System.IO;

namespace Pigeon.Protocol.v1_0
{
    public enum MessageType
    {
        Request = 0,
        Response = 1,
        Topic = 2
    }


    public static class MessageTypeExtensions
    {
        public static void Write(this BinaryWriter writer, MessageType messageType)
            => writer.Write((int)messageType);
        
        public static MessageType ReadMessageType(this BinaryReader reader)
            => (MessageType)reader.ReadInt32();
    }
}
