using System;
using System.Collections.Generic;
using System.Linq;

using ExampleContracts;

using Pigeon;
using Pigeon.Requests;

namespace ExampleServer
{
    public class Server : IRequestHandler<Connect, Connected>, IRequestHandler<Disconect, Disconnected>, IRequestHandler<ExampleContracts.Message, MessagePosted>
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


        public Connected Handle(Connect request)
        {
            lock (usersLock)
            {
                if (users.Any(u => u.Name == request.UserName))
                    return Connected.Failure("Username taken");

                var user = new User { Id = id++, Name = request.UserName, ConnectedTimestamp = DateTime.UtcNow };

                users.Add(user);

                Console.WriteLine($"User connected: {user.Name} ({user.Id})");
                router.Publish(new UserConnected(user.Id, user.Name, user.ConnectedTimestamp));

                return Connected.Ok(user.Id);
            }
        }


        public Disconnected Handle(Disconect request)
        {
            lock (usersLock)
            {
                var user = users.SingleOrDefault(u => u.Id == request.UserId);

                if (user is null)
                    return new Disconnected(false, $"User not found: {request.UserName} ({request.UserId})");

                user.DisconnectedTimestamp = DateTime.UtcNow;

                Console.WriteLine($"User disconnected: {user.Name} ({user.Id})");
                router.Publish(new UserDisconnected(user.Id, user.Name, user.DisconnectedTimestamp));

                return new Disconnected(true);
            }
        }


        public MessagePosted Handle(ExampleContracts.Message request)
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
