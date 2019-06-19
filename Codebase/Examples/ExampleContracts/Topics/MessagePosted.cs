using System;
using System.ComponentModel;

namespace ExampleContracts.Topics
{
    [Serializable]
    [ImmutableObject(true)]
    public class MessagePosted
    {
        private MessagePosted(int messageId, bool success, string reason)
        {
            MessageId = messageId;
            Success = success;
            Reason = reason;
        }


        public static MessagePosted Ok(int messageId) =>
            new MessagePosted(messageId, true, string.Empty);


        public static MessagePosted Failure(int messageId, string reason) =>
            new MessagePosted(messageId, false, reason);


        public int MessageId { get; }
        public bool Success { get; }
        public string Reason { get; }
    }
}
