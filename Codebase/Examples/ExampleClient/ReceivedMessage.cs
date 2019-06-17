using System;

namespace ExampleClient
{
    public class ReceivedMessage
    {
        public ReceivedMessage(int userId, string userName, string content, DateTime timestamp)
        {
            UserId = userId;
            UserName = userName;
            Content = content;
            Timestamp = timestamp;
        }


        public int UserId { get; }
        public string UserName { get; }
        public string Content { get; }
        public DateTime Timestamp { get; }
    }
}