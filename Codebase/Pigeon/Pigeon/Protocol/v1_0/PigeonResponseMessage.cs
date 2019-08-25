using System;
using System.IO;

using Pigeon.Serialization;
using Pigeon.Utils;

namespace Pigeon.Protocol.v1_0
{
    /// <summary>
    /// Represents a pigeon protocol response message
    /// </summary>
    public class PigeonResponseMessage : PigeonMessage
    {
        /// <summary>
        /// Initializes a new instance of <see cref="PigeonResponseMessage"/>
        /// </summary>
        /// <param name="conversationId">GUID used to identify the conversation</param>
        /// <param name="status">Status code for the response</param>
        /// <param name="responseType">Type of the response message</param>
        /// <param name="serialization">Serialization used for the request</param>
        /// <param name="data">Serializes response message data</param>
        public PigeonResponseMessage(
            Guid conversationId,
            ResponseStatus status,
            Type responseType,
            SerializationDescriptor serialization,
            byte[] data
        )
        {
            ConversationId = conversationId;
            Status = status;
            ResponseType = responseType;
            Serialization = serialization;
            Data = data;
        }


        /// <summary>
        /// Gets the GUID identifying the conversation between two peers 
        /// </summary>
        public Guid ConversationId { get; }


        /// <summary>
        /// Gets the message type
        /// </summary>
        public override MessageType MessageType => MessageType.Response;


        /// <summary>
        /// Gets the response status
        /// </summary>
        public ResponseStatus Status { get; }


        /// <summary>
        /// Gets the <see cref="Type"/> of the response message
        /// </summary>
        public Type ResponseType { get; }


        /// <summary>
        /// Gets a descriptor of the serialization used to encode the request message
        /// </summary>
        public SerializationDescriptor Serialization { get; }


        /// <summary>
        /// Gets the serialized request message data
        /// </summary>
        public byte[] Data { get; }


        public override void WriteTo(BinaryWriter writer)
        {
            base.WriteTo(writer);
            writer.Write(ConversationId);
            writer.Write(Status);
            writer.Write(ResponseType);
            writer.Write(Serialization);
            writer.Write(Data.Length);
            writer.Write(Data);
        }


        public static new PigeonResponseMessage ReadFrom(BinaryReader reader) =>
            new PigeonResponseMessage(
                reader.ReadGuid(),
                reader.ReadResponseStatus(),
                reader.ReadType(),
                reader.ReadSerializationDescriptor(),
                reader.ReadBytes(reader.ReadInt32()));
    }
}
