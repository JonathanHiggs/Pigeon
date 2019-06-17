using System;

namespace ExampleClient
{
    public class User
    {
        public User(int userId, string userName, DateTime connectedTimestamp)
        {
            UserId = userId;
            UserName = userName;
            ConnectedTimestamp = connectedTimestamp;
        }


        public int UserId { get; }
        public string UserName { get; }
        public DateTime ConnectedTimestamp { get; }
    }
}