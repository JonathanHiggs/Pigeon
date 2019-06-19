using System;
using System.ComponentModel;

namespace ExampleContracts.Responses
{
    [Serializable]
    [ImmutableObject(true)]
    public class UserDisconnect
    {
        public UserDisconnect(bool success, string reason = "")
        {
            Success = success;
            Reason = reason;
        }

        public bool Success { get; }
        public string Reason { get; }
    }
}
