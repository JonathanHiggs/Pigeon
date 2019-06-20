using System;
using System.ComponentModel;

namespace ExampleContracts.Models
{
    [Serializable]
    [ImmutableObject(true)]
    public class Message
    {
        public Message(User user, string content, DateTime timestamp)
        {
            User = user;
            Content = content;
            Timestamp = timestamp;
        }


        public User User { get; }
        public string Content { get; }
        public DateTime Timestamp { get; }
    }
}
