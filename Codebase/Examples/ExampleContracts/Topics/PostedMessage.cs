using System;
using System.ComponentModel;

using ExampleContracts.Models;

using Pigeon.Annotations;

namespace ExampleContracts.Topics
{
    [Serializable]
    [ImmutableObject(true)]
    [Topic]
    public class PostedMessage
    {
        public PostedMessage(Message message) =>
            Message = message;


        public Message Message { get; }
    }
}
