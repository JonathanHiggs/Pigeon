using System;
using System.Collections.Generic;
using System.ComponentModel;

using ExampleContracts.Models;

namespace ExampleContracts.Responses
{
    [Serializable]
    [ImmutableObject(true)]
    public class ConnectedUserList
    {
        public ConnectedUserList(List<User> users)
        {
            Users = users;
        }


        public List<User> Users { get; set; }
    }
}
