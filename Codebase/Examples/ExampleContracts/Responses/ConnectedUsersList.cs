using System;
using System.Collections.Generic;
using System.ComponentModel;

using ExampleContracts.Models;

using Newtonsoft.Json;

namespace ExampleContracts.Responses
{
    [Serializable]
    [ImmutableObject(true)]
    public class ConnectedUsersList
    {
        [JsonConstructor]
        public ConnectedUsersList(List<User> users)
        {
            Users = users;
        }


        public List<User> Users { get; set; }
    }
}
