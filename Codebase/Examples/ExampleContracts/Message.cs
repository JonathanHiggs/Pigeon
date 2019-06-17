using System;
using System.ComponentModel;

namespace ExampleContracts
{
    [Serializable]
    [ImmutableObject(true)]
    public class Message
    {
        public Message(int userId, string userName, int messageId, string content, DateTime timestamp)
        {
            UserId = userId;
            UserName = userName;
            MessageId = messageId;
            Content = content;
            Timestamp = timestamp;
        }


        public int UserId { get; }
        public string UserName { get; }
        public int MessageId { get; }
        public string Content { get; }
        public DateTime Timestamp { get; }
    }
}
