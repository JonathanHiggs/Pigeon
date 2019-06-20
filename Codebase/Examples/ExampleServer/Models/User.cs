using System;
using System.Collections.Generic;

namespace ExampleServer.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? ConnectedTimestamp { get; set; }
        public DateTime? DisconnectedTimestamp { get; set; } = null;
        

        public bool IsConnected => !DisconnectedTimestamp.HasValue;


        public static User FromDTO(ExampleContracts.Models.User user) =>
            new User
            {
                Id = user.Id,
                Name = user.Name,
                ConnectedTimestamp = user.ConnectedTimestamp,
                DisconnectedTimestamp = user.DisconnectedTimestamp
            };


        public ExampleContracts.Models.User ToDTO() =>
            new ExampleContracts.Models.User(Id, Name, ConnectedTimestamp, DisconnectedTimestamp);


        public class Comparer : IEqualityComparer<User>
        {
            public static Comparer Default { get; } = new Comparer();


            public bool Equals(User x, User y) =>
                x.Id == y.Id;


            public int GetHashCode(User obj) =>
                obj.Id;
        }
    }
}
