using System;
using System.ComponentModel;

namespace ExampleContracts.Models
{
    [Serializable]
    [ImmutableObject(true)]
    public class User
    {
        public User(int userId, string userName, DateTime connectedTimestamp)
        {
            UserId = userId;
            UserName = userName;
            ConnectedTimestamp = connectedTimestamp;
        }


        public User(int userId, string userName, DateTime connectedTimestamp, DateTime? disconnectedTimestamp)
        {
            UserId = userId;
            UserName = userName;
            ConnectedTimestamp = connectedTimestamp;
            DisconnectedTimestamp = disconnectedTimestamp;
        }


        public int UserId { get; }
        public string UserName { get; }
        public DateTime ConnectedTimestamp { get; }
        public DateTime? DisconnectedTimestamp { get; } = null;
    }
}
