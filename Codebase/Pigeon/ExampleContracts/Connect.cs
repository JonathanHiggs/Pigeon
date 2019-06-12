using System;
using System.ComponentModel;

namespace ExampleContracts
{
    [Serializable]
    [ImmutableObject(true)]
    public class Connect
    {
        public Connect(string userName) => UserName = userName;


        public string UserName { get; }
    }
}
