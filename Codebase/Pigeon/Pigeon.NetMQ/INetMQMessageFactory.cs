using Pigeon.Packages;
using NetMQ;

namespace Pigeon.NetMQ
{
    /// <summary>
    /// Creates and extracts <see cref="NetMQMessage"/>s
    /// </summary>
    public interface INetMQMessageFactory
    {
        /// <summary>
        /// Creates a <see cref="NetMQMessage"/> wrapping a topic event
        /// </summary>
        /// <param name="topicEvent">Topic event to be wrapped in a <see cref="NetMQMessage"/></param>
        /// <returns><see cref="NetMQMessage"/> wrapping a topic event</returns>
        NetMQMessage CreateTopicMessage(object topicEvent);


        /// <summary>
        /// Extracts a topic event from the <see cref="NetMQMessage"/>
        /// </summary>
        /// <param name="message"><see cref="NetMQMessage"/> wrapping a topic evnet</param>
        /// <returns>Topic event contained within the <see cref="NetMQMessage"/></returns>
        object ExtractTopic(NetMQMessage message);


        /// <summary>
        /// Checks to see whether the <see cref="NetMQMessage"/> topic is valid
        /// </summary>
        /// <param name="topicMessage"></param>
        /// <returns></returns>
        bool IsValidTopicMessage(NetMQMessage topicMessage);


        /// <summary>
        /// Creates a <see cref="NetMQMessage"/> wapping a request object
        /// </summary>
        /// <param name="request">Request object to be wrapped in a <see cref="NetMQMessage"/></param>
        /// <param name="requestId">An <see cref="int"/> identifier for matching asynchronous requests and responses</param>
        /// <returns><see cref="NetMQMessage"/> wrapping the request object</returns>
        NetMQMessage CreateRequestMessage(object request, int requestId);


        /// <summary>
        /// Extracts a request from the <see cref="NetMQMessage"/>
        /// </summary>
        /// <param name="message"><see cref="NetMQMessage"/> wrapping a request object</param>
        /// <returns>Request object contained withing the <see cref="NetMQMessage"/>, address of remote sender, and request identifier</returns>
        (object request, byte[] address, int requestId) ExtractRequest(NetMQMessage message);


        /// <summary>
        /// Checks to see whether the <see cref="NetMQMessage"/> request is valid
        /// </summary>
        /// <param name="requestMessage"><see cref="NetMQMessage"/> request to check for validity</param>
        /// <returns>True if the request <see cref="NetMQMessage"/> is valid; false otherwise</returns>
        bool IsValidRequestMessage(NetMQMessage requestMessage);


        /// <summary>
        /// Creates a <see cref="NetMQMessage"/> wrapping a response object
        /// </summary>
        /// <param name="response">Response object to be wrapped in a <see cref="NetMQMessage"/></param>
        /// <param name="address">Address of the remote</param>
        /// <param name="requestId">An <see cref="int"/> identifier for matching asynchronous requests and responses</param>
        /// <returns><see cref="NetMQMessage"/> wrapping the response object</returns>
        NetMQMessage CreateResponseMessage(object response, byte[] address, int requestId);


        /// <summary>
        /// Extracts a response from the <see cref="NetMQMessage"/>
        /// </summary>
        /// <param name="message"><see cref="NetMQMessage"/> wrapping a response object</param>
        /// <returns>Request identifier, response object contained within the <see cref="NetMQMessage"/></returns>
        (int requestId, object response) ExtractResponse(NetMQMessage message);


        /// <summary>
        /// Checks to see whether the <see cref="NetMQMessage"/> request is valid
        /// </summary>
        /// <param name="responseMessage"><see cref="NetMQMessage"/> response to check for validity</param>
        /// <returns>True if the response <see cref="NetMQMessage"/> is valid; false otherwise</returns>
        bool IsValidResponseMessage(NetMQMessage responseMessage);
    }
}