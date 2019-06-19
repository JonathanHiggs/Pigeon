using System;
using System.ComponentModel;

namespace ExampleContracts.Responses
{
    [Serializable]
    [ImmutableObject(true)]
    public class UserConnect
    {
        private UserConnect(int userId, bool success, string reason)
        {
            UserId = userId;
            Success = success;
            Reason = reason;
        }

        public static UserConnect Ok(int userId) =>
            new UserConnect(userId, true, string.Empty);

        public static UserConnect Failure(string reason) =>
            new UserConnect(-1, false, reason);

        public int UserId { get; }
        public bool Success { get; }
        public string Reason { get; }
    }
}
