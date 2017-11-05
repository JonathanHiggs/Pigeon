﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Messages
{
    /// <summary>
    /// Default implementation of IMessageFactory
    /// Wraps requests and responses in DataMessages
    /// </summary>
    public class MessageFactory : IMessageFactory
    {
        /// <summary>
        /// Wraps the supplied request object in a <see cref="Message"/>
        /// </summary>
        /// <typeparam name="TRequest">Type of the request object</typeparam>
        /// <param name="request">Request object</param>
        /// <returns>Serializable Message wrapping the request object</returns>
        public Message CreateRequest<TRequest>(TRequest request) where TRequest : class
        {
            return Create<TRequest>(request);
        }


        /// <summary>
        /// Wraps the supplied response object in a <see cref="Message"/>
        /// </summary>
        /// <typeparam name="TResponse">Type of the response object</typeparam>
        /// <param name="response">Response object</param>
        /// <returns>Serializable Message wrapping the response object</returns>
        public Message CreateResponse<TResponse>(TResponse response) where TResponse : class
        {
            return Create<TResponse>(response);
        }


        /// <summary>
        /// Extracts a request object from the supplied <see cref="Message"/>. An exception will be throw if the message response is an unexpected type
        /// </summary>
        /// <param name="requestMessage">Request Message wrapper</param>
        /// <returns>Request object</returns>
        public object ExtractRequest(Message requestMessage)
        {
            return requestMessage.Body;
        }


        /// <summary>
        /// Extracts a responce object from the supplied <see cref="Message"/>. An exception will be throw if the message response is an unexpected type
        /// </summary>
        /// <typeparam name="TResponse">Type of the response object</typeparam>
        /// <param name="responseMessage">Response Message wrapper</param>
        /// <returns>Response object</returns>
        public TResponse ExtractResponse<TResponse>(Message responseMessage) where TResponse : class
        {
            if (typeof(TResponse).IsAssignableFrom(responseMessage.GetType()) && typeof(TResponse) != typeof(object))
                return responseMessage as TResponse;
            else if (responseMessage is DataMessage<TResponse>)
                return (responseMessage as DataMessage<TResponse>).Data;
            else
                throw new InvalidCastException($"Unable to extract type {typeof(TResponse).Name} from message");
        }


        private Message Create<T>(T data)
            where T : class
        {
            if (typeof(Message).IsAssignableFrom(data.GetType()))
                return data as Message;
            return new DataMessage<T>(new GuidMessageId(), data);
        }
    }
}
