using System;
using System.Collections.Generic;
using System.Linq;

using ExampleContracts.Requests;
using ExampleContracts.Responses;
using ExampleContracts.Topics;

using Pigeon;
using Pigeon.Requests;

namespace ExampleServer
{
    public class Server : IRequestHandler<UserConnecting, UserConnect>, IRequestHandler<UserDisconecting, UserDisconnect>, IRequestHandler<ExampleContracts.Models.Message, MessagePosted>
    {
        private readonly IRouter<IRouterInfo> router;

        private readonly List<User> users = new List<User>();
        private readonly List<Message> messages = new List<Message>();

        private int id = 1;

        private readonly object usersLock = new object();
        private readonly object messagesLock = new object();


        public Server(IRouter<IRouterInfo> router)
        {
            this.router = router ?? throw new ArgumentNullException(nameof(router));
        }


        public UserConnect Handle(UserConnecting request)
        {
            lock (usersLock)
            {
                if (users.Any(u => u.Name == request.UserName))
                    return ExampleContracts.Responses.UserConnect.Failure("Username taken");

                var user = new User { Id = id++, Name = request.UserName, ConnectedTimestamp = DateTime.UtcNow };

                users.Add(user);

                Console.WriteLine($"User connected: {user.Name} ({user.Id})");
                router.Publish(new UserConnected(user.Id, user.Name, user.ConnectedTimestamp));

                return UserConnect.Ok(user.Id);
            }
        }


        public UserDisconnect Handle(UserDisconecting request)
        {
            lock (usersLock)
            {
                var user = users.SingleOrDefault(u => u.Id == request.UserId);

                if (user is null)
                    return new UserDisconnect(false, $"User not found: {request.UserName} ({request.UserId})");

                user.DisconnectedTimestamp = DateTime.UtcNow;

                Console.WriteLine($"User disconnected: {user.Name} ({user.Id})");
                router.Publish(new UserDisconnected(user.Id, user.Name, user.DisconnectedTimestamp));

                return new UserDisconnect(true);
            }
        }


        public MessagePosted Handle(ExampleContracts.Models.Message request)
        {
            lock (messagesLock)
            {
                var user = users.SingleOrDefault(u => u.Id == request.UserId);

                if (user is null)
                    return MessagePosted.Failure(request.MessageId, $"User {request.UserName} ({request.UserId}) not found");

                var message = new Message { User = user, Content = request.Content, Timestamp = request.Timestamp };

                messages.Add(message);

                Console.WriteLine($"User message: {user.Name} ({user.Id}) - {message.Content}");
                router.Publish(request);

                return MessagePosted.Ok(request.MessageId);
            }
        }
    }
}
