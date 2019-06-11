using System;

using NetMQ;

using Pigeon.Packages;
using Pigeon.Serialization;

namespace Pigeon.NetMQ
{
    /// <summary>
    /// Creates and extracts <see cref="NetMQMessage"/>s
    /// </summary>
    public class NetMQMessageFactory : INetMQMessageFactory
    {
        private readonly ISerializer serializer;
        private readonly IPackageFactory packageFactory;


        /// <summary>
        /// Initializes a new instance of <see cref="NetMQMessageFactory"/>
        /// </summary>
        /// <param name="serializer">A serializer that will convert data into a binary format for transmission</param>
        /// <param name="packageFactory">Wraps objects in a packages</param>
        public NetMQMessageFactory(ISerializer serializer, IPackageFactory packageFactory)
        {
            this.serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            this.packageFactory = packageFactory ?? throw new ArgumentNullException(nameof(packageFactory));
        }


        /// <summary>
        /// Creates a <see cref="NetMQMessage"/> wrapping a topic event
        /// </summary>
        /// <param name="topicEvent">Topic event to be wrapped in a <see cref="NetMQMessage"/></param>
        /// <returns><see cref="NetMQMessage"/> wrapping a topic event</returns>
        public NetMQMessage CreateTopicMessage(object topicEvent)
        {
            var package = packageFactory.Pack(topicEvent);
            var message = new NetMQMessage(2);
            message.Append(topicEvent.GetType().FullName);
            message.Append(serializer.Serialize(package));
            return message;
        }


        /// <summary>
        /// Extracts a topic event from the <see cref="NetMQMessage"/>
        /// </summary>
        /// <param name="message"><see cref="NetMQMessage"/> wrapping a topic evnet</param>
        /// <returns>Topic event contained within the <see cref="NetMQMessage"/></returns>
        public object ExtractTopic(NetMQMessage message)
        {
            var package = serializer.Deserialize<Package>(message[1].ToByteArray());
            return packageFactory.Unpack(package);
        }


        /// <summary>
        /// Checks to see whether the <see cref="NetMQMessage"/> topic is valid
        /// </summary>
        /// <param name="topicMessage"></param>
        /// <returns></returns>
        public bool IsValidTopicMessage(NetMQMessage topicMessage)
        {
            return null != topicMessage 
                && topicMessage.FrameCount == 2
                && !topicMessage[0].IsEmpty
                && !topicMessage[1].IsEmpty;
        }


        /// <summary>
        /// Creates a <see cref="NetMQMessage"/> wapping a request object
        /// </summary>
        /// <param name="request">Request object to be wrapped in a <see cref="NetMQMessage"/></param>
        /// <param name="requestId">An <see cref="int"/> identifier for matching asynchronous requests and responses</param>
        /// <returns><see cref="NetMQMessage"/> wrapping the request object</returns>
        public NetMQMessage CreateRequestMessage(object request, int requestId)
        {
            var package = packageFactory.Pack(request);
            var header = new SerializationHeader(new ProtocolVersion { Major = 1, Minor = 0 }, SerializationDescriptor.DotNet.Name);

            var data = serializer.Serialize(package, header.ToBytesCount());
            header.ToBytes(data);

            var message = new NetMQMessage(4);
            message.AppendEmptyFrame();
            message.Append(requestId);
            message.AppendEmptyFrame();
            message.Append(data);
            return message;
        }

        
        /// <summary>
        /// Extracts a request from the <see cref="NetMQMessage"/>
        /// </summary>
        /// <param name="message"><see cref="NetMQMessage"/> wrapping a request object</param>
        /// <returns>Request object contained withing the <see cref="NetMQMessage"/>, address of remote sender, and request identifier</returns>
        public (object request, byte[] address, int requestId) ExtractRequest(NetMQMessage message)
        {
            var address = message[0].ToByteArray();
            var requestId = message[2].ConvertToInt32();

            var data = message[4].ToByteArray();
            var (header, byteCount) = SerializationHeader.FromBytes(data);
            var obj = serializer.Deserialize(data, byteCount);

            if (!(obj is Package))
                throw new InvalidOperationException("Unable to deserialize");
            
            var request = packageFactory.Unpack(obj as Package);
            return (request, address, requestId);
        }


        /// <summary>
        /// Checks to see whether the <see cref="NetMQMessage"/> request is valid
        /// </summary>
        /// <param name="requestMessage"><see cref="NetMQMessage"/> request to check for validity</param>
        /// <returns>True if the request <see cref="NetMQMessage"/> is valid; false otherwise</returns>
        public bool IsValidRequestMessage(NetMQMessage requestMessage)
        {
            return null != requestMessage
                && requestMessage.FrameCount == 5
                && !requestMessage[0].IsEmpty
                && !requestMessage[2].IsEmpty
                && !requestMessage[4].IsEmpty;
        }


        /// <summary>
        /// Creates a <see cref="NetMQMessage"/> wrapping a response object
        /// </summary>
        /// <param name="response">Response object to be wrapped in a <see cref="NetMQMessage"/></param>
        /// <param name="address">Address of the remote</param>
        /// <param name="requestId">An <see cref="int"/> identifier for matching asynchronous requests and responses</param>
        /// <returns><see cref="NetMQMessage"/> wrapping the response object</returns>
        public NetMQMessage CreateResponseMessage(object response, byte[] address, int requestId)
        {
            var package = packageFactory.Pack(response);
            var header = new SerializationHeader(new ProtocolVersion { Major = 1, Minor = 0 }, SerializationDescriptor.DotNet.Name);

            var data = serializer.Serialize(package, header.ToBytesCount());
            header.ToBytes(data);

            var message = new NetMQMessage(5);
            message.Append(address);
            message.AppendEmptyFrame();
            message.Append(requestId);
            message.AppendEmptyFrame();
            message.Append(data);
            return message;
        }


        /// <summary>
        /// Extracts a response from the <see cref="NetMQMessage"/>
        /// </summary>
        /// <param name="message"><see cref="NetMQMessage"/> wrapping a response object</param>
        /// <returns>Request identifier, response object contained within the <see cref="NetMQMessage"/></returns>
        public (int requestId, object response) ExtractResponse(NetMQMessage message)
        {
            var requestId = message[1].ConvertToInt32();
            var data = message[3].ToByteArray();
            var (header, byteCount) = SerializationHeader.FromBytes(data);
            var obj = serializer.Deserialize(data, byteCount);

            if (!(obj is Package))
                throw new InvalidOperationException("Unable to deserialize response");

            var response = packageFactory.Unpack(obj as Package);
            return (requestId, response);
        }


        /// <summary>
        /// Checks to see whether the <see cref="NetMQMessage"/> request is valid
        /// </summary>
        /// <param name="responseMessage"><see cref="NetMQMessage"/> response to check for validity</param>
        /// <returns>True if the response <see cref="NetMQMessage"/> is valid; false otherwise</returns>
        public bool IsValidResponseMessage(NetMQMessage responseMessage)
        {
            return null != responseMessage
                && responseMessage.FrameCount == 4
                && !responseMessage[1].IsEmpty
                && !responseMessage[3].IsEmpty;
        }
    }
}
