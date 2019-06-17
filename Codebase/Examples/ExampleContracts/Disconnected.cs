using System;
using System.ComponentModel;

namespace ExampleContracts
{
    [Serializable]
    [ImmutableObject(true)]
    public class Disconnected
    {
        public Disconnected(bool success, string reason = "")
        {
            Success = success;
            Reason = reason;
        }

        public bool Success { get; }
        public string Reason { get; }
    }
}
