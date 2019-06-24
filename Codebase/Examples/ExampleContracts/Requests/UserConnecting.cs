using System;
using System.ComponentModel;

using ExampleContracts.Models;
using ExampleContracts.Responses;

using Newtonsoft.Json;

using Pigeon.Annotations;

namespace ExampleContracts.Requests
{
    [Serializable]
    [ImmutableObject(true)]
    [Request(ResponseType = typeof(Response<User>))]
    public class UserConnecting
    {
        [JsonConstructor]
        public UserConnecting(User user) =>
            User = user;


        public User User { get; }
    }
}
