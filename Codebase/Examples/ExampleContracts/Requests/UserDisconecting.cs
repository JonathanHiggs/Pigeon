using System;
using System.ComponentModel;

namespace ExampleContracts.Requests
{
    [Serializable]
    [ImmutableObject(true)]
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
