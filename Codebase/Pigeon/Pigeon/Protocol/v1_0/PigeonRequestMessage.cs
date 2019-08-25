using System;
using System.IO;

using Pigeon.Serialization;
using Pigeon.Utils;

namespace Pigeon.Protocol.v1_0
{
    /// <summary>
    /// Represents a pigeon protocol request message
    /// </summary>
    public class PigeonRequestMessage : PigeonMessage
    {
        /// <summary>
        /// Initializes a new instance of <see cref="PigeonRequestMessage"/>
        /// </summary>
        /// <param name="conversationId">GUID used to identify the conversation</param>
        /// <param name="requestType">Type of the request object</param>
        /// <param name="expectedResponseType">Expected type of the response</param>
        /// <param name="serialization">Serialization used for the request</param>
        /// <param name="data">Serialized request message data</param>
        public PigeonRequestMessage(
            Guid conversationId,
            Type requestType,
            Type expectedResponseType,
            SerializationDescriptor serialization,
            byte[] data
        )
        {
            ConversationId = conversationId;
            RequestType = requestType;
            ExpectedResponseType = expectedResponseType;
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
        public override MessageType MessageType => MessageType.Request;


        /// <summary>
        /// Gets the <see cref="Type"/> of the request
        /// </summary>
        public Type RequestType { get; }


        /// <summary>
        /// Gets the expected <see cref="Type"/> of the response
        /// </summary>
        public Type ExpectedResponseType { get; }


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
            writer.Write(RequestType);
            writer.Write(ExpectedResponseType);
            writer.Write(Serialization);
            writer.Write(Data.Length);
            writer.Write(Data);
        }


        public static new PigeonRequestMessage ReadFrom(BinaryReader reader) =>
            new PigeonRequestMessage(
                reader.ReadGuid(),
                reader.ReadType(),
                reader.ReadType(),
                reader.ReadSerializationDescriptor(),
                reader.ReadBytes(reader.ReadInt32()));
    }
}