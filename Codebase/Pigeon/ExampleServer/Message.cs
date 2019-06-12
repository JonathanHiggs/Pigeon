using System;

namespace ExampleServer
{
    public class Message
    {
        public User User { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
