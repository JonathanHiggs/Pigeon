using System;
using System.ComponentModel;

namespace ExampleContracts
{
    [Serializable]
    [ImmutableObject(true)]
    public class Disconect
    {
        public Disconect(int userId, string userName)
        {
            UserId = userId;
            UserName = userName;
        }


        public int UserId { get; }
        public string UserName { get; }
    }
}
