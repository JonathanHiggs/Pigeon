using System;
using System.ComponentModel;

using ExampleContracts.Models;
using ExampleContracts.Responses;

using Pigeon.Annotations;

namespace ExampleContracts.Requests
{
    [Serializable]
    [ImmutableObject(true)]
    [Request(ResponseType = typeof(Response<Message>))]
    public class PostMessage
    {
        public PostMessage(Message message) =>
            Message = message;


        public Message Message { get; }
    }
}
