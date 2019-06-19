using System;
using System.ComponentModel;

using ExampleContracts.Responses;

using Pigeon.Annotations;

namespace ExampleContracts.Requests
{
    [Serializable]
    [ImmutableObject(true)]
    [Request(ResponseType = typeof(UserDisconnect))]
    public class UserDisconecting
    {
        public UserDisconecting(int userId, string userName)
        {
            UserId = userId;
            UserName = userName;
        }


        public int UserId { get; }
        public string UserName { get; }
    }
}
