using System;

namespace ExampleServer.Models
{
    public class Message
    {
        public User User { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }


        public ExampleContracts.Models.Message ToDTO() =>
            new ExampleContracts.Models.Message(User.ToDTO(), Content, Timestamp);


        public static Message FromDTO(ExampleContracts.Models.Message message) =>
            new Message
            {
                User = User.FromDTO(message.User),
                Content = message.Content,
                Timestamp = message.Timestamp
            };
    }
}
