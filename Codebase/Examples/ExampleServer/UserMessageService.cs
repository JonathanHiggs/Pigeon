using System;
using System.Collections.Generic;
using System.Linq;

using ExampleContracts.Requests;
using ExampleContracts.Responses;
using ExampleContracts.Topics;

using ExampleServer.Models;

using Pigeon;
using Pigeon.Requests;

using DTO = ExampleContracts.Models;

namespace ExampleServer
{
    /// <summary>
    /// Does all the work of the server, tracks users and messages
    /// </summary>
    public class UserMessageService :
        IRequestHandler<UserConnecting, Response<DTO.User>>,
        IRequestHandler<UserDisconecting, Response<DTO.User>>,
        IRequestHandler<PostMessage, Response<DTO.Message>>,
        IRequestHandler<ConnectedUsers, ConnectedUsersList>
    {
        private readonly IRouter<IRouterInfo> router;
        private int id = 1;

        private HashSet<User> users { get; } = new HashSet<User>(User.Comparer.Default);
        private List<Message> messages { get; } = new List<Message>();


        private readonly object usersLock = new object();
        private readonly object messagesLock = new object();


        /// <summary>
        /// Initializes a new instance of <see cref="UserMessageService"/>
        /// </summary>
        /// <param name="router"></param>
        public UserMessageService(IRouter<IRouterInfo> router)
        {
            this.router = router ?? throw new ArgumentNullException(nameof(router));
        }


        /// <summary>
        /// Takes provisional user details, completes and returns to the caller
        /// </summary>
        /// <param name="requestUser"></param>
        /// <returns></returns>
        public User Connect(User requestUser)
        {
            lock (usersLock)
            {
                var user = users.SingleOrDefault(u => u.Name == requestUser.Name);

                if (!(user is null))
                {
                    if (user.IsConnected)
                        throw new InvalidOperationException("User name already taken");

                    // Reconnecting user
                    user.ConnectedTimestamp = DateTime.UtcNow;
                    user.DisconnectedTimestamp = null;

                    Console.WriteLine($"User reconnected: {requestUser.Name} ({requestUser.Id})");

                    router.Publish(new UserConnected(user.ToDTO()));

                    return user;
                }
                
                // New user
                requestUser.Id = id++;
                requestUser.ConnectedTimestamp = DateTime.UtcNow;
                requestUser.DisconnectedTimestamp = null;

                users.Add(requestUser);

                Console.WriteLine($"User connected: {requestUser.Name} ({requestUser.Id})");

                router.Publish(new UserConnected(requestUser.ToDTO()));

                return requestUser;
            }
        }


        /// <summary>
        /// Sets the supplied used to disconnected
        /// </summary>
        /// <param name="requestUser"></param>
        public void Disconnect(User requestUser)
        {
            lock (usersLock)
            {
                var user = users.SingleOrDefault(u => u.Id == requestUser.Id);

                if (user is null)
                    throw new InvalidOperationException($"User not found: {requestUser.Name} ({requestUser.Id})");

                if (!user.IsConnected)
                    throw new InvalidOperationException($"User has already disconnected");

                user.DisconnectedTimestamp = DateTime.UtcNow;

                Console.WriteLine($"User disconnected: {user.Name} ({user.Id})");

                router.Publish(new UserDisconnected(user.ToDTO()));
            }
        }


        /// <summary>
        /// Adds a user message
        /// </summary>
        /// <param name="postedMessage"></param>
        public void AddMessage(Message postedMessage)
        {
            lock (messagesLock)
            {
                if (!users.TryGetValue(postedMessage.User, out var user))
                    throw new InvalidOperationException($"User {postedMessage.User.Name} ({postedMessage.User.Id}) not found");
                
                if (!user.IsConnected)
                    throw new InvalidOperationException($"User {postedMessage.User.Name} ({postedMessage.User.Id}) is not connected");

                messages.Add(postedMessage);

                Console.WriteLine($"User message: {user.Name} ({user.Id}) - {postedMessage.Content}");

                router.Publish(new PostedMessage(postedMessage.ToDTO()));
            }
        }


        #region Handlers

        // Handlers do the DTP <> Domain conversions

        public Response<DTO.User> Handle(UserConnecting request)
        {
            var user = Connect(User.FromDTO(request.User));
            return Response<DTO.User>.Ok(user.ToDTO());
        }


        public Response<DTO.User> Handle(UserDisconecting request)
        {
            Disconnect(User.FromDTO(request.User));
            return Response<DTO.User>.Ok(request.User);
        }


        public Response<DTO.Message> Handle(PostMessage request)
        {
            AddMessage(Message.FromDTO(request.Message));
            return Response<DTO.Message>.Ok(request.Message);
        }


        public ConnectedUsersList Handle(ConnectedUsers request)
        {
            return new ConnectedUsersList(
                users
                    .Select(u => u.ToDTO())
                    .ToList());
        }

        #endregion


        #region Helper methods

        public void Reset()
        {
            foreach (var user in users)
                router.Publish(new UserDisconnected(user.ToDTO()));

            users.Clear();
            messages.Clear();
        }

        #endregion
    }
}
