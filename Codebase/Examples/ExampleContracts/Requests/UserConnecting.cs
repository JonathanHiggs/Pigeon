using System;
using System.ComponentModel;

namespace ExampleContracts.Requests
{
    [Serializable]
    [ImmutableObject(true)]
    public class UserConnecting
    {
        public UserConnecting(string userName) => UserName = userName;


        public string UserName { get; }
    }
}
