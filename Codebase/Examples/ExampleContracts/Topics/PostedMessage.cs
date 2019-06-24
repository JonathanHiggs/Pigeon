using System;
using System.ComponentModel;

using ExampleContracts.Models;

using Newtonsoft.Json;

using Pigeon.Annotations;

namespace ExampleContracts.Topics
{
    [Serializable]
    [ImmutableObject(true)]
    [Topic]
    public class PostedMessage
    {
        [JsonConstructor]
        public PostedMessage(Message message) =>
            Message = message;


        public Message Message { get; }
    }
}
