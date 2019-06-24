using System;
using System.ComponentModel;

using ExampleContracts.Models;
using ExampleContracts.Responses;

using Newtonsoft.Json;

using Pigeon.Annotations;

namespace ExampleContracts.Requests
{
    [Serializable]
    [ImmutableObject(true)]
    [Request(ResponseType = typeof(Response<Message>))]
    public class PostMessage
    {
        [JsonConstructor]
        public PostMessage(Message message) =>
            Message = message;


        public Message Message { get; }
    }
}
