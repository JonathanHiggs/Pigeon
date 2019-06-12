using System;

namespace ExampleServer
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime ConnectedTimestamp { get; set; }
        public DateTime DisconnectedTimestamp { get; set; }
    }
}
