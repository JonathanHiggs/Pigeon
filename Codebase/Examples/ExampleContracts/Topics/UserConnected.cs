using System;
using System.ComponentModel;

using ExampleContracts.Models;

using Newtonsoft.Json;

using Pigeon.Annotations;

namespace ExampleContracts.Topics
{
    [Serializable]
    [ImmutableObject(true)]
    [Topic]
    public class UserConnected
    {
        [JsonConstructor]
        public UserConnected(User user) =>
            User = user;


        public User User { get; }
    }
}
