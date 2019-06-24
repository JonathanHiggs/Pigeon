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
    public class UserDisconnected
    {
        [JsonConstructor]
        public UserDisconnected(User user) =>
            User = user;


        public User User { get; }
    }
}
