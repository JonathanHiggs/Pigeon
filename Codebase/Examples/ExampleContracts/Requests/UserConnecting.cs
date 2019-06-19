using System;
using System.ComponentModel;

using ExampleContracts.Responses;

using Pigeon.Annotations;

namespace ExampleContracts.Requests
{
    [Serializable]
    [ImmutableObject(true)]
    [Request(ResponseType = typeof(UserConnect))]
    public class UserConnecting
    {
        public UserConnecting(string userName) => UserName = userName;


        public string UserName { get; }
    }
}
