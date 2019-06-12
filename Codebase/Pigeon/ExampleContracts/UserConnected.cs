using System;
using System.ComponentModel;

namespace ExampleContracts
{
    [Serializable]
    [ImmutableObject(true)]
    public class UserConnected
    {
        public UserConnected(int id, string userName, DateTime timestamp)
        {
            UserId = id;
            UserName = userName;
            Timestamp = timestamp;
        }


        public int UserId { get; }
        public string UserName { get; }
        public DateTime Timestamp { get; }
    }
}
