using System;
using System.IO;

using Pigeon.Serialization;
using Pigeon.Utils;

namespace Pigeon.Protocol.Pigeon.v1_0
{
    /// <summary>
    /// Represents a pigeon protocol topic message
    /// </summary>
    public class PigeonTopicMessage : PigeonMessage
    {
        /// <summary>
        /// Message type identifier
        /// </summary>
        public const string MessageType = "TOPIC";


        public PigeonTopicMessage(
            string topicName,
            Type topicBodyType,
            SerializationDescriptor serialization,
            byte[] data)
        {
            TopicName = topicName;
            TopicBodyType = topicBodyType;
            Serialization = serialization;
            Data = data;
        }

        public override string GetMessageType() => "";


        public string TopicName { get; }


        public Type TopicBodyType { get; }


        public SerializationDescriptor Serialization { get; }


        public byte[] Data { get; }


        /// <summary>
        /// Creates a binary serialized representation of the object instance
        /// /// </summary>
        /// <param name="writer"><see cref="BinaryWriter"/> used to create a serialized instance of the object instance</param>
        public override void WriteTo(BinaryWriter writer)
        {
            base.WriteTo(writer);
            writer.Write(TopicName);
            writer.Write(TopicBodyType);
            writer.Write(Serialization);
            writer.Write(Data.Length);
            writer.Write(Data);
        }


        /// <summary>
        /// Initializes a new instance of <see cref="PigeonTopicMessage"/> from serialized data
        /// </summary>
        /// <param name="reader">Data reader to de-serialize</param>
        /// <returns></returns>
        public static new PigeonTopicMessage ReadFrom(BinaryReader reader) =>
            new PigeonTopicMessage(
                reader.ReadString(),
                reader.ReadType(),
                reader.ReadSerializationDescriptor(),
                reader.ReadBytes(reader.ReadInt32()));
    }
}
