using System;
using System.ComponentModel;

namespace ExampleContracts.Models
{
    [Serializable]
    [ImmutableObject(true)]
    public class User
    {
        public User(int id, string name, DateTime? connectedTimestamp)
        {
            Id = id;
            Name = name;
            ConnectedTimestamp = connectedTimestamp;
        }


        public User(int id, string name, DateTime? connectedTimestamp, DateTime? disconnectedTimestamp)
        {
            Id = id;
            Name = name;
            ConnectedTimestamp = connectedTimestamp;
            DisconnectedTimestamp = disconnectedTimestamp;
        }


        public int Id { get; }
        public string Name { get; }
        public DateTime? ConnectedTimestamp { get; }
        public DateTime? DisconnectedTimestamp { get; } = null;
    }
}
