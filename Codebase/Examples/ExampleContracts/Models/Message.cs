using System;
using System.ComponentModel;

using Newtonsoft.Json;

namespace ExampleContracts.Models
{
    [Serializable]
    [ImmutableObject(true)]
    public class Message
    {
        [JsonConstructor]
        public Message(User user, string content, DateTime timestamp)
        {
            User = user;
            Content = content;
            Timestamp = timestamp;
        }


        public User User { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
