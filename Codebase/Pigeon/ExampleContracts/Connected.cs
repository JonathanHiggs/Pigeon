using System;
using System.ComponentModel;

namespace ExampleContracts
{
    [Serializable]
    [ImmutableObject(true)]
    public class Connected
    {
        private Connected(int userId, bool success, string reason)
        {
            UserId = userId;
            Success = success;
            Reason = reason;
        }

        public static Connected Ok(int userId) =>
            new Connected(userId, true, string.Empty);

        public static Connected Failure(string reason) =>
            new Connected(-1, false, reason);

        public int UserId { get; }
        public bool Success { get; }
        public string Reason { get; }
    }
}
